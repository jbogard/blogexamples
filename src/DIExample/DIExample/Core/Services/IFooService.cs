using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DIExample.Core.Services
{
    public interface IFooService
    {
        void DoSomething();
    }

    public class FooService : IFooService
    {
        private readonly RequestContext _requestContext;
        private readonly HttpContextBase _httpContext;
        private readonly UrlHelper _helper;
        private readonly Func<ControllerContext> _context;

        public FooService(
            RequestContext requestContext,
            HttpContextBase httpContext,
            UrlHelper helper,
            Func<ControllerContext> context
            )
        {
            _requestContext = requestContext;
            _httpContext = httpContext;
            _helper = helper;
            _context = context;
        }

        public void DoSomething()
        {
            _helper.Action("Foo");

            var context = _context();
        }
    }
}