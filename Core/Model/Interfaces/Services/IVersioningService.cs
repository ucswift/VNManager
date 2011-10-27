using System.Collections.Generic;

namespace WaveTech.VNManager.Model.Interfaces.Services
{
	public interface IVersioningService
	{
		void ProcessAssemblyInfoDirectories(List<string> directories, string tokenData, string assemblyFileVersionRule, string assemblyVersionRule);
		void ProcessAssemblyInfoDirectory(string directory, string tokenData, string assemblyFileVersionRule, string assemblyVersionRule);
		void ProcessAssemblyInfos(List<string> assemblyInfos, string tokenData, string assemblyFileVersionRule, string assemblyVersionRule);
		void ProcessAssemblyInfo(string assemblyInfo, string tokenData, string assemblyFileVersionRule, string assemblyVersionRule);
	}
}