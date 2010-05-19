using System.Web.Mvc;

namespace DIExample.Services
{
	public interface IActionMethodResultInvoker
	{
		ActionResult Invoke(object actionMethodResult, ControllerContext context);
	}

	public interface IActionMethodResultInvoker<T>
		where T : IActionMethodResult
	{
		ActionResult Invoke(T actionMethodResult, ControllerContext context);
	}
}