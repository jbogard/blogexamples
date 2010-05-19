using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using StructureMap;

namespace DIExample.Services
{
	public static class HttpContextBaseContainerExtensions
	{
		private static readonly object _nestedContainerKey = new object();

		public static void SetContainer(this HttpContextBase context, IContainer container)
		{
			context.Items[_nestedContainerKey] = container;
		}

		public static IContainer GetContainer(this HttpContextBase context)
		{
			return context.Items[_nestedContainerKey] as IContainer;
		}
	}

	public class StructureMapControllerFactory : DefaultControllerFactory
	{
		private readonly IContainer _container;

		public StructureMapControllerFactory(IContainer container)
		{
			_container = container;
		}


		protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
		{
			if (controllerType == null)
				return null;

			var nestedContainer = _container.GetNestedContainer();

			requestContext.HttpContext.SetContainer(nestedContainer);

			ControllerBase controllerBase = null;

			Func<ControllerContext> ctxtCtor = () => controllerBase == null ? null : controllerBase.ControllerContext;

			nestedContainer.Configure(cfg =>
			                          	{
			                          		cfg.For<RequestContext>().Use(requestContext);
			                          		cfg.For<HttpContextBase>().Use(requestContext.HttpContext);
			                          		cfg.For<Func<ControllerContext>>().Use(ctxtCtor);
			                          	});

			var controller = (IController)nestedContainer.GetInstance(controllerType);

			controllerBase = controller as ControllerBase;

			return controller;
		}

		public override void ReleaseController(IController controller)
		{
			var controllerBase = controller as Controller;

			if (controllerBase != null)
			{
				var httpContextBase = controllerBase.ControllerContext.HttpContext;

				var nestedContainer = httpContextBase.GetContainer();

				if (nestedContainer != null)
					nestedContainer.Dispose();
			}

			base.ReleaseController(controller);
		}
	}
}