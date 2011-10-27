using System;
using System.IO;
using System.Reflection;
using StructureMap;

namespace WaveTech.VNManager.Common
{
	public static class Bootstrapper
	{
		public static void Configure()
		{
			ObjectFactory.Configure(x => x.Scan(
				scan =>
				{
					string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
					path = path.Replace("file:\\", "");

					scan.AssembliesFromPath(path, assembly => assembly.GetName().Name.Contains("VNManager"));
					scan.WithDefaultConventions();
					scan.LookForRegistries();
				}
			));
		}
	}
}