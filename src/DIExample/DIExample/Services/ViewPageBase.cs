using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using System.Web.Mvc.Ajax;
using MvcContrib;

namespace DIExample.Services
{
	public interface IHtmlBuilder<TModel>
	{
		MvcHtmlString EditLink<TController>(
			Expression<Action<TController>> action) 
			where TController : Controller;
	}

	public class HtmlBuilder<TModel> : IHtmlBuilder<TModel>
	{
		private readonly HtmlHelper<TModel> _htmlHelper;
		private readonly AjaxHelper<TModel> _ajaxHelper;
		private readonly UrlHelper _urlHelper;

		public HtmlBuilder(
			HtmlHelper<TModel> htmlHelper, 
			AjaxHelper<TModel> ajaxHelper, 
			UrlHelper urlHelper)
		{
			_htmlHelper = htmlHelper;
			_ajaxHelper = ajaxHelper;
			_urlHelper = urlHelper;
		}

		public MvcHtmlString EditLink<TController>(
			Expression<Action<TController>> action) 
			where TController : Controller
		{
			var url = _urlHelper.Action(action);

			var foo = _ajaxHelper.GlobalizationScript();

			return _htmlHelper.ActionLink(action, "Edit");
		}
	}

	public abstract class ViewPageBase<TModel> : ViewPage<TModel>
	{
		public IHtmlBuilder<TModel> HtmlBuilder { get; set; }
	}

	public abstract class ViewPageBase : ViewPageBase<object>
	{
	}
}