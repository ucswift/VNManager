using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace WaveTech.VNManager.Model
{
	public class Options
	{
		private HeadingInfo _headingInfo;

		public Options(HeadingInfo headingInfo)
		{
			_headingInfo = headingInfo;
		}

		[Option("a", "all",
						Required = false,
						HelpText = "Process all AssemblyInfo.cs files within or under the supplied folder")]
		public string DirectoryPath = String.Empty;

		[Option("t", "token",
						Required = false,
						HelpText = "Token data to be used in the version number, for example a build server build number")]
		public string TokenData = String.Empty;

		[Option("v", "versionRule",
						Required = false,
						HelpText = "Default rule to be used when no specific rule is present for AssemblyVersion")]
		public string VersionRule = String.Empty;

		[Option("f", "fileRule",
						Required = false,
						HelpText = "Default rule to be used when no specific rule is present for AssemblyFileVersion")]
		public string FileRule = String.Empty;

		[OptionList("s", "specific", Separator = ',',
				Required = false,
				HelpText = "AssemblyInfo.cs file paths to be processed." +
				" Include the entire paramater in double quotes \" if the paths contain spaces." +
				" Separate each AssemblyInfo.cs path with a comma." +
				" Do not include spaces between paths and separators.")]
		public IList<string> SpecificAssemblyInfos = null;

		[HelpOption(
						HelpText = "Dispaly this help screen.")]
		public string GetUsage()
		{
			var help = new HelpText(_headingInfo);
			help.AdditionalNewLineAfterOption = true;
			help.Copyright = new CopyrightInfo("WaveTech Digital Technologies, Inc.", 2009, 2010);
			help.AddPreOptionsLine(" ");
			help.AddPreOptionsLine("This software will modified the AssemblyInfo.cs and update");
			help.AddPreOptionsLine("the version numbers in accordance with the version number rules.");
			help.AddPreOptionsLine(" ");
			help.AddPreOptionsLine(" Homepage: http://www.wtdt.com/Products/OSSUtilities/VNManager.aspx");
			help.AddPreOptionsLine(" ");
			help.AddPreOptionsLine("This software is based on the original works of Gyrum Technologies");
			help.AddPreOptionsLine("and their Visual Studio Version Number Manager.");
			help.AddPreOptionsLine("(http://www.gyrum.com/Projects/VSVersion.aspx)");
			help.AddPreOptionsLine(" ");
			help.AddPreOptionsLine("Usage: VNManager -s\"C:\\My Solution\\Proj1\\AssemblyInfo.cs,C:\\My Solution\\Proj2\\AssemblyInfo.cs\"");
			help.AddPreOptionsLine("       VNManager -aC:\\MySolution");
			help.AddPreOptionsLine("       VNManager -aC:\\MySolution -v\"*.*.YYMM.DDHH\" -f\"*.*.YYMM.DDHH\"");
			help.AddPreOptionsLine("       VNManager -aC:\\MySolution -t\"1001\"");
			help.AddPreOptionsLine(" ");
			help.AddOptions(this);

			return help;
		}
	}
}
