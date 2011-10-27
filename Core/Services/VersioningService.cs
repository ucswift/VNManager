using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WaveTech.VNManager.Common;
using WaveTech.VNManager.Model;
using WaveTech.VNManager.Model.Interfaces.Services;

namespace WaveTech.VNManager.Services
{
	public class VersioningService : IVersioningService
	{
		private string _assemblyFileVersionRule;
		private string _assemblyVersionRule;
		private string _tokenData;

		#region Public Methods
		public void ProcessAssemblyInfoDirectories(List<string> directories, string tokenData, string assemblyFileVersionRule, string assemblyVersionRule)
		{
			_tokenData = tokenData;
			_assemblyFileVersionRule = assemblyFileVersionRule;
			_assemblyVersionRule = assemblyVersionRule;

			var assemblyInfos = new List<string>();

			foreach (string s in directories)
			{
				assemblyInfos.AddRange(Directory.GetFiles(s, "*AssemblyInfo.cs"));
			}

			ProcessAssemblyInfos(assemblyInfos, tokenData, assemblyFileVersionRule, assemblyVersionRule);
		}

		public void ProcessAssemblyInfoDirectory(string directory, string tokenData, string assemblyFileVersionRule, string assemblyVersionRule)
		{
			_tokenData = tokenData;
			_assemblyFileVersionRule = assemblyFileVersionRule;
			_assemblyVersionRule = assemblyVersionRule;

			var assemblyInfos = new List<string>();
			assemblyInfos.AddRange(Directory.GetFiles(directory, "*AssemblyInfo.cs"));

			ProcessAssemblyInfos(assemblyInfos, tokenData, assemblyFileVersionRule, assemblyVersionRule);
		}

		public void ProcessAssemblyInfos(List<string> assemblyInfos, string tokenData, string assemblyFileVersionRule, string assemblyVersionRule)
		{
			_tokenData = tokenData;
			_assemblyFileVersionRule = assemblyFileVersionRule;
			_assemblyVersionRule = assemblyVersionRule;

			foreach (string s in assemblyInfos)
			{
				ProcessAssemblyInfo(s, tokenData, assemblyFileVersionRule, assemblyVersionRule);
			}
		}

		public void ProcessAssemblyInfo(string assemblyInfo, string tokenData, string assemblyFileVersionRule, string assemblyVersionRule)
		{
			_tokenData = tokenData;
			_assemblyFileVersionRule = assemblyFileVersionRule;
			_assemblyVersionRule = assemblyVersionRule;

			if (File.Exists(assemblyInfo))
			{
				string assemblyInfoText = UpdateVersionNumbers(File.ReadAllText(assemblyInfo));

				StreamWriter writer = File.CreateText(assemblyInfo);
				writer.Write(assemblyInfoText);
				writer.Close();
			}
		}
		#endregion Public Methods

		#region Internal Methods
		internal string UpdateVersionNumbers(string assemblyInfoTextOriginal)
		{
			string assemblyInfoText = null;
			string[] assemblyInfoLines = null;

			assemblyInfoText = assemblyInfoTextOriginal;
			assemblyInfoLines = assemblyInfoText.Replace("\r\n", "\n").Split(char.Parse("\n"));

			// update the AssemblyVersion attribute
			assemblyInfoText = SetNewAssemblyVersionNumber(assemblyInfoText, assemblyInfoLines.ToList());

			// update the AssemblyFileVersion attribute
			assemblyInfoText = SetNewAssemblyFileVersionNumber(assemblyInfoText, assemblyInfoLines.ToList());

			return assemblyInfoText;
		}

		internal string SetNewAssemblyFileVersionNumber(string aiText, List<string> aiLines)
		{
			string oldAFVL = FindOldFileVersionString(aiLines.ToList());

			if (oldAFVL.Length == 0)
			{
				Console.WriteLine("AssemblyFileVersion not found. The AssemblyFileVersion number has not been changed!");
				return aiText;
			}

			string oldV = GetOldVersionNumber(oldAFVL);
			string rule = GetRule(oldAFVL);

			// if no rule specified, use global rule from project
			if (string.IsNullOrEmpty(rule))
			{
				rule = _assemblyFileVersionRule;
			}

			// if no global rule specified in project, return
			if (string.IsNullOrEmpty(rule))
			{
				return aiText;
			}

			DateTime startDate = GetStartDate(oldAFVL);
      string newV = GetNewVersionNumber(oldV, rule, startDate);
      string newAFVL = oldAFVL.Replace(oldV, newV);

			// write the new version number to the output window
			Console.WriteLine("New AssemblyFileVersion: " + newV);

			// Thanks to Pavel Stoev! 
			// if version is the same, return original value
			if (oldAFVL == newAFVL)
			{
				return aiText;
			}

			return aiText.Replace(oldAFVL, newAFVL);
		}

		internal string SetNewAssemblyVersionNumber(string aiText, List<string> aiLines)
		{
			string oldAVL = FindOldVersionString(aiLines.ToList());

			if (oldAVL.Length == 0)
			{
				Console.WriteLine("AssemblyVersion not found. The AssemblyVersion number has not been changed!");
				return aiText;
			}

			string oldV = GetOldVersionNumber(oldAVL);
			string rule = GetRule(oldAVL);

			// if no rule specified, use global rule from project
			if (string.IsNullOrEmpty(rule))
			{
				rule = _assemblyVersionRule;
			}

			// if no global rule specified in project, return
			if (string.IsNullOrEmpty(rule))
			{
				return aiText;
			}

			DateTime startDate = GetStartDate(oldAVL);
      string newV = GetNewVersionNumber(oldV, rule, startDate);
      string newAVL = oldAVL.Replace(oldV, newV);

			// write the new version number to the output window
			Console.WriteLine("New AssemblyVersion: " + newV);

			// Thanks to Pavel Stoev! 
			// if version is the same, return original value
			if (oldAVL == newAVL)
			{
				return aiText;
			}

			return aiText.Replace(oldAVL, newAVL);
		}

    internal string GetRule(string versionLine)
		{
			string rule = string.Empty;
			string comment = string.Empty;

			// check for c# comment
			int index = versionLine.IndexOf("//");
			if (index > 0)
			{
				// got c# comment
				comment = versionLine.Substring(index).Replace("/", string.Empty);
			}
			else
			{
				index = versionLine.IndexOf("'");
				if (index > 0)
				{
					// got VB comment
					comment = versionLine.Substring(index).Replace("'", string.Empty);
				}
			}

			if (comment.Length > 0)
			{
				// got a comment line, get the rule (if any)
				index = comment.ToLower().IndexOf("rule:");
				if (index >= 0)
				{
					rule = comment.Substring(index + 5).Trim();

					index = rule.IndexOf(" ");
					if (index > 0)
					{
						rule = rule.Substring(0, index).Trim();
					}
				}
			}

			return rule;
		}

		internal DateTime GetStartDate(string versionLine)
		{
			string dateString = string.Empty;
			string comment = string.Empty;

			// check for c# comment
			int index = versionLine.IndexOf("//");
			if (index > 0)
			{
				// got c# comment
				comment = versionLine.Substring(index).Trim().Trim(char.Parse("/")).Trim();
			}
			else
			{
				index = versionLine.IndexOf("'");
				if (index > 0)
				{
					// got VB comment
					comment = versionLine.Substring(index).Replace("'", string.Empty);
				}
			}

			if (comment.Length > 0)
			{
				// got a comment line, get the date (if any)
				index = comment.ToLower().IndexOf("date:");
				if (index >= 0)
				{
					dateString = comment.Substring(index + 5).Trim();

					index = dateString.IndexOf(" ");
					if (index > 0)
					{
						dateString = dateString.Substring(0, index).Trim();
					}
				}
			}

			if (dateString.Length == 0)
			{
				return DateTime.Parse("01/01/2000 00:00:00");
			}

			return DateTime.Parse(dateString);
		}

		internal string GetNewVersionNumber(string oldVersion, string rule, DateTime startDate)
		{
			// split the rule and old version numbers int arrays
			string[] oldVersionArray = oldVersion.Split(char.Parse("."));
			string[] ruleArray = rule.Split(char.Parse("."));

			string newMajor;
			string newMinor;
			string newBuild;
			string newRevision;

			DateTime buildDate = DateTime.Now;

			if ((oldVersionArray.Length > 0) & (ruleArray.Length > 0))
			{
				newMajor = GetNewVersionPart(oldVersionArray[0], ruleArray[0], buildDate, startDate);
			}
			else if (oldVersionArray.Length > 0)
			{
				newMajor = oldVersionArray[0];
			}
			else if (ruleArray.Length > 0)
			{
				newMajor = GetNewVersionPart("1", ruleArray[0], buildDate, startDate);
			}
			else
			{
				newMajor = "1";
			}

			if ((oldVersionArray.Length > 1) & (ruleArray.Length > 1))
			{
				newMinor = GetNewVersionPart(oldVersionArray[1], ruleArray[1], buildDate, startDate);
			}
			else if (oldVersionArray.Length > 1)
			{
				newMinor = oldVersionArray[1];
			}
			else if (ruleArray.Length > 1)
			{
				newMinor = GetNewVersionPart("01", ruleArray[1], buildDate, startDate);
			}
			else
			{
				newMinor = "01";
			}

			if ((oldVersionArray.Length > 2) & (ruleArray.Length > 2))
			{
				newBuild = GetNewVersionPart(oldVersionArray[2], ruleArray[2], buildDate, startDate);
			}
			else if (oldVersionArray.Length > 2)
			{
				newBuild = oldVersionArray[2];
			}
			else if (ruleArray.Length > 2)
			{
				newBuild = GetNewVersionPart("0001", ruleArray[2], buildDate, startDate);
			}
			else
			{
				newBuild = "0001";
			}

			if ((oldVersionArray.Length > 3) & (ruleArray.Length > 3))
			{
				newRevision = GetNewVersionPart(oldVersionArray[3], ruleArray[3], buildDate, startDate);
			}
			else if (oldVersionArray.Length > 3)
			{
				newRevision = oldVersionArray[3];
			}
			else if (ruleArray.Length > 3)
			{
				newRevision = GetNewVersionPart("0001", ruleArray[3], buildDate, startDate);
			}
			else
			{
				newRevision = "0001";
			}


			return newMajor + "." + newMinor + "." + newBuild + "." + newRevision;
		}

		internal string GetNewVersionPart(string oldVersionPart, string rule, DateTime buildDate, DateTime startDate)
		{
			string newVersionPart;
			string length;

			if (rule.IndexOf(";") >= 0)
			{
				newVersionPart = GetNewMultiPartVersionNumber(rule, buildDate, startDate);
				if (newVersionPart.Length == 0)
				{
					return oldVersionPart;
				}
				return ValidateVersionPart(rule, newVersionPart);
			}

			int partLength = 0;
			int index = rule.IndexOf(":");
			if (index > 0)
			{
				length = rule.Substring(index + 1);
				rule = rule.Substring(0, index);
				if (length.Length > 0)
				{
					partLength = int.Parse(length);
				}
			}

			rule = rule.Trim();
			if (rule == "*")
			{
				// leave it alone
				// and bypass length processing
				// NewVersionPart = oldVersionPart
				return ValidateVersionPart(rule, oldVersionPart);
			}

			if (rule == "+")
			{
				if ((oldVersionPart.Length == 0) | (oldVersionPart == "*"))
				{
					newVersionPart = "1";
				}
				else
				{
					newVersionPart = (Int32.Parse(oldVersionPart) + 1).ToString();
				}
				return ValidateVersionPart(rule, AddLeadingZeros(newVersionPart, oldVersionPart.Length));
			}

			if (String.Compare(rule, "Y", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) == 0)
			{
				int Year = buildDate.Year;
				if (Year > 2000)
				{
					Year = Year - 2000;
				}
				return ValidateVersionPart(rule, AddLeadingZeros(Year.ToString(), partLength));
			}

			if (String.Compare(rule, "YY", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Year = buildDate.Year;
				if (Year > 2000)
				{
					Year = Year - 2000;
				}
				return ValidateVersionPart(rule, AddLeadingZeros(Year.ToString(), 2));
			}

			if (String.Compare(rule, "YYYY", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Year = buildDate.Year;
				return ValidateVersionPart(rule, AddLeadingZeros(Year.ToString(), partLength));
			}

			if (String.Compare(rule, "M", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Month = buildDate.Month;
				return ValidateVersionPart(rule, AddLeadingZeros(Month.ToString(), partLength));
			}

			if (String.Compare(rule, "MM", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Month = buildDate.Month;
				return ValidateVersionPart(rule, AddLeadingZeros(Month.ToString(), 2));
			}

			if (String.Compare(rule, "D", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Day = buildDate.Day;
				return ValidateVersionPart(rule, AddLeadingZeros(Day.ToString(), partLength));
			}

			if (String.Compare(rule, "DD", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				return ValidateVersionPart(rule, buildDate.Day.ToString().PadLeft(2, char.Parse("0")));
			}

			if (String.Compare(rule, "h", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Hour = buildDate.Hour;
				return ValidateVersionPart(rule, AddLeadingZeros(Hour.ToString(), partLength));
			}

			if (String.Compare(rule, "hh", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Hour = buildDate.Hour;
				return ValidateVersionPart(rule, AddLeadingZeros(Hour.ToString(), 2));
			}

			if (String.Compare(rule, "m", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Minute = buildDate.Minute;
				return ValidateVersionPart(rule, AddLeadingZeros(Minute.ToString(), partLength));
			}

			if (String.Compare(rule, "mm", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Minute = buildDate.Minute;
				return ValidateVersionPart(rule, AddLeadingZeros(Minute.ToString(), 2));
			}

			if (String.Compare(rule, "s", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Second = buildDate.Second;
				return ValidateVersionPart(rule, AddLeadingZeros(Second.ToString(), partLength));
			}

			if (String.Compare(rule, "ss", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Second = buildDate.Second;
				return ValidateVersionPart(rule, AddLeadingZeros(Second.ToString(), 2));
			}

			if (String.Compare(rule, "YYMM", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Year = buildDate.Year;
				if (Year > 2000)
				{
					Year = Year - 2000;
				}
				int Month = buildDate.Month;
				return ValidateVersionPart(rule, AddLeadingZeros(Year.ToString(), 2) + AddLeadingZeros(Month.ToString(), 2));
			}

			if (String.Compare(rule, "MMDD", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Month = buildDate.Month;
				int Day = buildDate.Day;
				return ValidateVersionPart(rule, AddLeadingZeros(Month.ToString(), 2) + AddLeadingZeros(Day.ToString(), 2));
			}

			if (String.Compare(rule, "DDhh", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Day = buildDate.Day;
				int Hour = buildDate.Hour;
				return ValidateVersionPart(rule, AddLeadingZeros(Day.ToString(), 2) + AddLeadingZeros(Hour.ToString(), 2));
			}

			if (String.Compare(rule, "hhmm", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Hour = buildDate.Hour;
				int Minute = buildDate.Minute;
				return ValidateVersionPart(rule, AddLeadingZeros(Hour.ToString(), 2) + AddLeadingZeros(Minute.ToString(), 2));
			}

			if (String.Compare(rule, "hhmmss", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Hour = buildDate.Hour;
				int Minute = buildDate.Minute;
				int Second = buildDate.Second;
				return ValidateVersionPart(rule,
				                           AddLeadingZeros(Hour.ToString(), 2) + AddLeadingZeros(Minute.ToString(), 2) +
				                           AddLeadingZeros(Second.ToString(), 2));
			}

			if (String.Compare(rule, "mmss", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				int Minute = buildDate.Minute;
				int Second = buildDate.Second;
				return ValidateVersionPart(rule, AddLeadingZeros(Minute.ToString(), 2) + AddLeadingZeros(Second.ToString(), 2));
			}

			if (String.Compare(rule, "nY", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				long Years = DateAndTime.DateDiff(DateInterval.Year, startDate, buildDate);
				return ValidateVersionPart(rule, AddLeadingZeros(Years.ToString(), partLength));
			}

			if (String.Compare(rule, "nM", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				long Months = DateAndTime.DateDiff(DateInterval.Month, startDate, buildDate);
				return ValidateVersionPart(rule, AddLeadingZeros(Months.ToString(), partLength));
			}

			if (String.Compare(rule, "nW", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				long Weeks = DateAndTime.DateDiff(DateInterval.Day, startDate, buildDate)/7;
				return ValidateVersionPart(rule, AddLeadingZeros(Weeks.ToString(), partLength));
			}

			if (String.Compare(rule, "nD", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				long Days = DateAndTime.DateDiff(DateInterval.Day, startDate, buildDate);
				return ValidateVersionPart(rule, AddLeadingZeros(Days.ToString(), partLength));
			}

			if (String.Compare(rule, "TT", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				if (String.IsNullOrEmpty(_tokenData))
					return ValidateVersionPart(rule, AddLeadingZeros("", 2));
				else
					return ValidateVersionPart(rule, AddLeadingZeros(_tokenData, 2));
			}

			if (String.Compare(rule, "TTTT", CultureInfo.CurrentCulture, CompareOptions.None) == 0)
			{
				if (String.IsNullOrEmpty(_tokenData))
					return ValidateVersionPart(rule, AddLeadingZeros("", 4));
				else
					return ValidateVersionPart(rule, AddLeadingZeros(_tokenData, 4));
			}

			// if we get here, just leave it alone
			Console.WriteLine("rule part: " + rule + " is invalid. The build number has not been changed!");

			return ValidateVersionPart(rule, oldVersionPart);
		}

		internal string ValidateVersionPart(string rule, string versionPart)
		{
			// check if the version part is out of bounds (i.e. > 65535
			int number = 0;
			bool valid = int.TryParse(versionPart, out number);

			if (valid)
			{
				if (number > 65535)
				{
					valid = false;
				}
			}

			if (!valid)
			{
				return "65534";
			}

			return versionPart;
		}

		internal string AddLeadingZeros(string number, int length)
		{
			if (number.Trim().Length >= length)
			{
				return number.Trim();
			}

			string retVal = new string(char.Parse("0"), length) + number.Trim();


			return retVal.Substring(retVal.Length - length);
		}

		internal string FindOldVersionString(List<string> lines)
		{
			string assemblyVersionLine = String.Empty;

			foreach (string line in lines)
			{
				if (line.Count() > 0 && line.ToLower().Contains("assemblyversion"))
				{
					if (!line.Trim().StartsWith("/") && !line.Trim().StartsWith("'"))
					{
						Console.WriteLine(string.Format("Found AsseinblyVersion Line: {0}", line));
						assemblyVersionLine = line;
					}
				}
			}

			return assemblyVersionLine;
		}

		internal string FindOldFileVersionString(List<string> lines)
		{
			string assemblyVersionLine = String.Empty;

			foreach (string line in lines)
			{
				if (line.Count() > 0 && line.ToLower().Contains("assemblyfileversion"))
				{
					if (!line.Trim().StartsWith("/") && !line.Trim().StartsWith("'"))
					{
						Console.WriteLine(string.Format("Found AsseinblyFileVersion Line: {0}", line));
						assemblyVersionLine = line;
					}
				}
			}

			return assemblyVersionLine;
		}

		internal string GetNewMultiPartVersionNumber(string rule, DateTime buildDate, DateTime startDate)
		{
			string newPart;
			string newVersion = string.Empty;
			string[] parts = rule.Split(char.Parse(";"));

			foreach (string part in parts)
			{
				if (part.Length > 0)
				{
					newPart = GetNewVersionPart(string.Empty, part, buildDate, startDate);
					if (newPart.Length == 0)
					{
						return string.Empty;
					}
					newVersion = newVersion + newPart;
				}
			}
			return newVersion;
		}

		internal string GetOldVersionNumber(string oldVersionLine)
		{
			int index = oldVersionLine.ToLower().IndexOf("version");

			if (index < 0)
			{
				return null;
			}

			string version = oldVersionLine.Substring(index + 7);
			// get everything after "...Version"

			index = version.IndexOf("\"");
			version = version.Substring(index + 1);

			index = version.IndexOf("\"");
			version = version.Substring(0, index);

			return version;
		}
		#endregion Internal Methods
	}
}