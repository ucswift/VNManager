using StructureMap.Configuration.DSL;
using WaveTech.VNManager.Model.Interfaces.Services;

namespace WaveTech.VNManager.Services
{
	public class ServicesRegistry : Registry
	{
		protected override void configure()
		{
			ForRequestedType<IVersioningService>().TheDefault.Is.OfConcreteType<VersioningService>();
		}
	}
}