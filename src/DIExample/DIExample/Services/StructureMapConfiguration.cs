using StructureMap;

namespace DIExample.Services
{
	public static class StructureMapConfiguration
	{
		public static void Initialize()
		{
			ObjectFactory.Initialize(cfg =>
			{
				cfg.Scan(scanner =>
				{
					scanner.TheCallingAssembly();
					scanner.LookForRegistries();
				});
			});
		}

	}
}