using System;
using System.Globalization;
using System.IO;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.UI;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.TypeRules;

namespace DIExample.Services
{
	public class WrappedView : IView, IDisposable
	{
		private bool _disposed;

		public WrappedView(WebFormView baseView, IContainer container)
		{
			BaseView = baseView;
			Container = container;
		}

		public WebFormView BaseView { get; private set; }
		public IContainer Container { get; private set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (Container != null)
				Container.Dispose();

			_disposed = true;
		}

		public void Render(ViewContext viewContext, TextWriter writer)
		{
			if (viewContext == null)
			{
				throw new ArgumentNullException("viewContext");
			}

			object viewInstance = BuildManager.CreateInstanceFromVirtualPath(BaseView.ViewPath, typeof(object));
			if (viewInstance == null)
			{
				throw new InvalidOperationException(
					String.Format(
						CultureInfo.CurrentUICulture,
						"The view found at '{0}' was not created.",
						BaseView.ViewPath));
			}

			////////////////////////////////
			// This is where our code starts
			////////////////////////////////
			var viewType = viewInstance.GetType();
			var isBaseViewPage = viewType.Closes(typeof (ViewPageBase<>));

			Container.Configure(cfg =>
			{
				cfg.For<ViewContext>().Use(viewContext);
				cfg.For<IViewDataContainer>().Use((IViewDataContainer) viewInstance);

				if (isBaseViewPage)
				{
					var modelType = GetModelType(viewType);
					var builderType = typeof (IHtmlBuilder<>).MakeGenericType(modelType);
					var concreteBuilderType = typeof (HtmlBuilder<>).MakeGenericType(modelType);

					cfg.For(builderType).Use(concreteBuilderType);
				}
			});

			Container.BuildUp(viewInstance);
			////////////////////////////////
			// This is where our code ends
			////////////////////////////////

			var viewPage = viewInstance as ViewPage;
			if (viewPage != null)
			{
				RenderViewPage(viewContext, viewPage);
				return;
			}

			ViewUserControl viewUserControl = viewInstance as ViewUserControl;
			if (viewUserControl != null)
			{
				RenderViewUserControl(viewContext, viewUserControl);
				return;
			}

			throw new InvalidOperationException(
				String.Format(
					CultureInfo.CurrentUICulture,
					"The view at '{0}' must derive from ViewPage, ViewPage<TViewData>, ViewUserControl, or ViewUserControl<TViewData>.",
					BaseView.ViewPath));
		}

		private static Type GetModelType(Type viewType)
		{
			if (!viewType.IsGenericType)
				return GetModelType(viewType.BaseType);

			return viewType.GetGenericArguments()[0];
		}

		private void RenderViewPage(ViewContext context, ViewPage page)
		{
			if (!String.IsNullOrEmpty(BaseView.MasterPath))
			{
				page.MasterLocation = BaseView.MasterPath;
			}

			page.ViewData = context.ViewData;

			page.PreLoad += (sender, e) => BuildUpMasterPage(page.Master);

			page.RenderView(context);
		}

		private void BuildUpMasterPage(MasterPage master)
		{
			if (master == null)
				return;

			var masterContainer = Container.GetNestedContainer();

			masterContainer.BuildUp(master);

			BuildUpMasterPage(master.Master);
		}

		private void RenderViewUserControl(ViewContext context, ViewUserControl control)
		{
			if (!String.IsNullOrEmpty(BaseView.MasterPath))
			{
				throw new InvalidOperationException("A master name cannot be specified when the view is a ViewUserControl.");
			}

			control.ViewData = context.ViewData;
			control.RenderView(context);
		}
	}
}