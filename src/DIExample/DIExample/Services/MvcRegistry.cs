using System.Web.Mvc;
using System.Web.Routing;
using DIExample.Controllers;
using StructureMap.Configuration.DSL;

namespace DIExample.Services
{
	public class MvcRegistry : Registry
	{
		public MvcRegistry()
		{
			Scan(scanner =>
			{
				scanner.TheCallingAssembly();
				scanner.WithDefaultConventions();
				scanner.ConnectImplementationsToTypesClosing(typeof(IActionMethodResultInvoker<>));
				scanner.ConnectImplementationsToTypesClosing(typeof(ICommandMessageHandler<>));
				scanner.Convention<CommandMessageConvention>();
			});

			For<IActionInvoker>().Use<InjectingActionInvoker>();
			For<ITempDataProvider>().Use<SessionStateTempDataProvider>();
			For<RouteCollection>().Use(RouteTable.Routes);

			SetAllProperties(c =>
			{
				c.OfType<IActionInvoker>();
				c.OfType<ITempDataProvider>();
				c.WithAnyTypeFromNamespaceContainingType<ViewPageBase>();
				c.WithAnyTypeFromNamespaceContainingType<LogErrorAttribute>();
			});
		}
	}
}