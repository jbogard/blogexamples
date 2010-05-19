using System.Web.Mvc;
using StructureMap;

namespace DIExample.Services
{
	public class NestedContainerViewEngine : WebFormViewEngine
	{
		public override ViewEngineResult FindView(
			ControllerContext controllerContext, 
			string viewName, string masterName, bool useCache)
		{
			var result = base.FindView(controllerContext, viewName, masterName, useCache);

			return CreateNestedView(result, controllerContext);
		}

		public override ViewEngineResult FindPartialView(
			ControllerContext controllerContext, 
			string partialViewName, bool useCache)
		{
			var result = base.FindPartialView(controllerContext, partialViewName, useCache);

			return CreateNestedView(result, controllerContext);
		}

		private ViewEngineResult CreateNestedView(
			ViewEngineResult result, 
			ControllerContext controllerContext)
		{
			if (result.View == null)
				return result;

			var parentContainer = controllerContext.HttpContext.GetContainer();

			var nestedContainer = parentContainer.GetNestedContainer();

			var webFormView = (WebFormView)result.View;

			var wrappedView = new WrappedView(webFormView, nestedContainer);

			var newResult = new ViewEngineResult(wrappedView, this);

			return newResult;
		}
	}
}