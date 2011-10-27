using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using CommandLine.Text;
using WaveTech.VNManager.Common;
using WaveTech.VNManager.Model;
using WaveTech.VNManager.Model.Interfaces.Services;
using WaveTech.VNManager.Services;

namespace WaveTech.VNManager.Manager
{
	class Program
	{
		private static readonly HeadingInfo _headingInfo = new HeadingInfo("VNManager", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

		static void Main(string[] args)
		{
			_headingInfo.WriteMessage(" ");
			ServicesStub.Associate();

			var options = new Options(_headingInfo);
			ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));

			if (!parser.ParseArguments(args, options))
				Environment.Exit(1);

			PerformTask(options);

			Environment.Exit(0);
		}

		private static void PerformTask(Options options)
		{
			try
			{
				if (String.IsNullOrEmpty(options.DirectoryPath) == false)
				{
					IVersioningService versioning = ObjectLocator.GetInstance<IVersioningService>();

					string path;

					Console.WriteLine("Version all AssemblyInfo.cs's in path");
					Console.WriteLine(string.Format("Path Parameter: {0}", options.DirectoryPath));

					if (options.DirectoryPath.Trim().StartsWith("."))
						path = Directory.GetParent(options.DirectoryPath).FullName;
					else
						path = options.DirectoryPath;


					Console.WriteLine(string.Format("Path to Version: {0}", path));

					versioning.ProcessAssemblyInfoDirectory(path, options.TokenData, options.FileRule, options.VersionRule);
				}
				else if (options.SpecificAssemblyInfos != null && options.SpecificAssemblyInfos.Count > 0)
				{
					IVersioningService versioning = ObjectLocator.GetInstance<IVersioningService>();

					List<string> paths = new List<string>();

					foreach (string s in options.SpecificAssemblyInfos)
					{
						if (s.Trim().StartsWith("."))
						{
							string fileName = Path.GetFileName(s);
							string directory = Directory.GetParent(s).FullName;

							paths.Add(Path.Combine(directory, fileName));
						}
						else
							paths.Add(s);
					}

					versioning.ProcessAssemblyInfos(paths, options.TokenData, options.FileRule, options.VersionRule);
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("An error occurred while trying to version the solution. {0}", ex.Message));
			}
		}
	}
}