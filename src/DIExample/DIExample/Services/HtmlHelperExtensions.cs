using System.Web.Mvc;
using System.Web.Mvc.Html;
using StructureMap;

namespace DIExample.Services
{
	public interface ILocalizationProvider
	{
		string GetValue(string key);
	}
	public static class HtmlHelperExtensions
	{
		public static MvcHtmlString Text<TModel>(this HtmlHelper<TModel> helper, string key)
		{
			var provider = ObjectFactory.GetInstance<ILocalizationProvider>();

			var text = provider.GetValue(key);

			return MvcHtmlString.Create(text);
		}
	}
}