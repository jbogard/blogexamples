using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;

namespace DIExample.Services
{
	public interface IActionResult { }

	public class NamedViewResult : IActionResult
	{
		public string ViewName { get; private set; }
		public string MasterName { get; private set; }

		public NamedViewResult(string viewName, string masterName)
		{
			ViewName = viewName;
			MasterName = masterName;
		}
	}

	public class ExplicitViewResult : IActionResult
	{
		public IView View { get; private set; }

		public ExplicitViewResult(IView view)
		{
			View = view;
		}
	}

	public interface IActionResultInvoker<in TActionResult>
		where TActionResult : IActionResult
	{
		void Invoke(TActionResult actionResult);
	}

	public class NamedViewResultInvoker : IActionResultInvoker<NamedViewResult>
	{
		private readonly RouteData _routeData;
		private readonly ViewEngineCollection _viewEngines;

		public NamedViewResultInvoker(RouteData routeData, ViewEngineCollection viewEngines)
		{
			_routeData = routeData;
			_viewEngines = viewEngines;
		}

		public void Invoke(NamedViewResult actionMethodResult)
		{
			// Use action method result
			// and the view engines to render a view
			var viewName = actionMethodResult.ViewName;
			if (String.IsNullOrEmpty(viewName))
			{
				viewName = _routeData.GetRequiredString("action");
			}

			ViewEngineResult result = null;

			var view = FindView(context);

			TextWriter writer = context.HttpContext.Response.Output;
			ViewContext viewContext = new ViewContext(context, View, ViewData, TempData, writer);
			View.Render(viewContext, writer);

			if (result != null)
			{
				result.ViewEngine.ReleaseView(context, View);
			}
		}

		protected ViewEngineResult FindView(ControllerContext context)
		{
			ViewEngineResult result = _viewEngines.FindView(context, ViewName, MasterName);
			if (result.View != null)
			{
				return result;
			}

			// we need to generate an exception containing all the locations we searched
			StringBuilder locationsText = new StringBuilder();
			foreach (string location in result.SearchedLocations)
			{
				locationsText.AppendLine();
				locationsText.Append(location);
			}
			throw new InvalidOperationException(String.Format(CultureInfo.CurrentUICulture,
				MvcResources.Common_ViewNotFound, ViewName, locationsText));
		}

	}
}