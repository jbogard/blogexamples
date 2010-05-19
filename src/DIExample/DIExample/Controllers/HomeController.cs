using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DIExample.Core.Services;
using DIExample.Services;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace DIExample.Controllers
{
    public interface ILogger
    {
        void LogError(Exception ex, string message);
    }

    public class Logger : ILogger
    {
        public void LogError(Exception ex, string message)
        {
            
        }
    }

    public class LogErrorAttribute : FilterAttribute, IExceptionFilter
    {
        public ILogger Logger { get; set; }
    
        public void OnException(ExceptionContext filterContext)
        {
            var controllerName = filterContext.Controller.GetType().Name;
            var message = string.Format("Controller {0} generated an error.", controllerName);

            Logger.LogError(filterContext.Exception, message);
        }
    }

    public interface ICommandMessageHandler<T>
    {
        void Handle(T message);
    }

    public class CommandMethodResult<TModel> : IActionMethodResult
    {
        public CommandMethodResult(TModel model,
            Func<ActionResult> successContinuation,
            Func<ActionResult> failureContinuation)
        {
            Model = model;
            SuccessContinuation = successContinuation;
            FailureContinuation = failureContinuation;
        }

        public TModel Model { get; private set; }
        public Func<ActionResult> SuccessContinuation { get; private set; }
        public Func<ActionResult> FailureContinuation { get; private set; }
    }

    public class CommandMethodResultInvoker<TModel> 
        : IActionMethodResultInvoker<CommandMethodResult<TModel>> 
    {
        private readonly ICommandMessageHandler<TModel> _handler;

        public CommandMethodResultInvoker(ICommandMessageHandler<TModel> handler)
        {
            _handler = handler;
        }

        public ActionResult Invoke(
            CommandMethodResult<TModel> actionMethodResult, 
            ControllerContext context)
        {
            if (!context.Controller.ViewData.ModelState.IsValid)
            {
                return actionMethodResult.FailureContinuation();
            }

            _handler.Handle(actionMethodResult.Model);

            return actionMethodResult.SuccessContinuation();
        }
    }

    public class FooEditModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Nickname { get; set; }
    }

    public class FooEditModelHandler : ICommandMessageHandler<FooEditModel>
    {
        private readonly IFooService _service;

        public FooEditModelHandler(IFooService service)
        {
            _service = service;
        }

        public void Handle(FooEditModel message)
        {
            // handle this edit model somehow
        }
    }

    public class CommandMessageConvention : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            if (type.ImplementsInterfaceTemplate(typeof(ICommandMessageHandler<>)))
            {
                var interfaceType = type.FindFirstInterfaceThatCloses(typeof (ICommandMessageHandler<>));
                var commandMessageType = interfaceType.GetGenericArguments()[0];
                
                var openCommandMethodResultType = typeof (CommandMethodResult<>);
                var closedCommandMethodResultType = openCommandMethodResultType.MakeGenericType(commandMessageType);

                var openActionMethodInvokerType = typeof (IActionMethodResultInvoker<>);
                var closedActionMethodInvokerType =
                    openActionMethodInvokerType.MakeGenericType(closedCommandMethodResultType);

                var openCommandMethodResultInvokerType = typeof (CommandMethodResultInvoker<>);
                var closedCommandMethodResultInvokerType =
                    openCommandMethodResultInvokerType.MakeGenericType(commandMessageType);

                registry.For(closedActionMethodInvokerType).Use(closedCommandMethodResultInvokerType);
            }
        }
    }

    public class DefaultController : Controller
    {
        protected CommandMethodResult<T> Command<T>(
            T model, 
            Func<ActionResult> successContinuation)
        {
            return new CommandMethodResult<T>(
                model, 
                successContinuation, 
                () => View(model));
        }
    }

    [LogError]
    public class HomeController : DefaultController
    {
        private readonly IFooService _fooService;

        public HomeController(IFooService fooService)
        {
            _fooService = fooService;
        }

        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            _fooService.DoSomething();

            return View();
        }

        public void Error()
        {
            throw new ApplicationException("Asplode");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View(new FooEditModel());
        }

        [HttpPost]
        public CommandMethodResult<FooEditModel> Edit(FooEditModel form)
        {
            return Command(form, () => RedirectToAction("Index"));
        }
    }
}
