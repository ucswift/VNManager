using System.IO;
using WaveTech.VNManager.Model.Interfaces.Providers;

namespace WaveTech.VNManager.Providers.FileProviders
{
	public class FileProcessorProvider : IFileProcessorProvider
	{
		public string ReadAllText(string path)
		{
			return File.ReadAllText(path);
		}

		public StreamWriter CreateText(string path)
		{
			return File.CreateText(path);
		}
	}
}