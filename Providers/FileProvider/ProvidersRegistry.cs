using StructureMap.Configuration.DSL;
using WaveTech.VNManager.Model.Interfaces.Providers;

namespace WaveTech.VNManager.Providers.FileProviders
{
	public class ProvidersRegistry : Registry
	{
		protected override void configure()
		{
			ForRequestedType<IFileProcessorProvider>().TheDefault.Is.OfConcreteType<FileProcessorProvider>();
		}
	}
}