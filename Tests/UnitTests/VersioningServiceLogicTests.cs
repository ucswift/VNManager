using System;
using NUnit.Framework;
using WaveTech.VNManager.Services;

namespace UnitTests
{
	namespace VersioningLogicTests
	{
		public class with_the_versioning_service : FixtureBase
		{
			protected VersioningService versioningService;
			protected int twoDigitYear;
			protected int fourDigitYear;
			protected int monthNumber;
			protected string twoDigitMonthNumber;
			protected int dayofMonthNumber;
			protected string twoDigitDayofMonthNumber;
			protected int hourNumber;
			protected string twoDigitHourNumber;
			protected int minuteNumber;
			protected string twoDigitMinuteNumber;
			protected int secondNumber;
			protected string twoDigitSecondNumber;

			protected override void Before_each_test()
			{
				base.Before_each_test();

				versioningService = new VersioningService();

				twoDigitYear = DateTime.Now.Year - 2000;
				fourDigitYear = DateTime.Now.Year;
				monthNumber = DateTime.Now.Month;
				twoDigitMonthNumber = monthNumber.ToString().PadLeft(2, char.Parse("0"));
				dayofMonthNumber = DateTime.Now.Day;
				twoDigitDayofMonthNumber = dayofMonthNumber.ToString().PadLeft(2, char.Parse("0"));
				hourNumber = DateTime.Now.Hour;
				twoDigitHourNumber = hourNumber.ToString().PadLeft(2, char.Parse("0"));
				minuteNumber = DateTime.Now.Minute;
				twoDigitMinuteNumber = minuteNumber.ToString().PadLeft(2, char.Parse("0"));
				secondNumber = DateTime.Now.Second;
				twoDigitSecondNumber = secondNumber.ToString().PadLeft(2, char.Parse("0"));
			}
		}

		[TestFixture]
		public class when_using_the_rule_defaults : with_the_versioning_service
		{
			[Test]
			public void should_not_update_version_number_with_no_rule()
			{
				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")]\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")]\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(assemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_not_update_version_number_with_default_rule()
			{
				string goodAssemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.*\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.*\r\n";

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.*\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.*\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}
		}

		[TestFixture]
		public class when_using_the_version_number_incrementor : with_the_versioning_service
		{
			[Test]
			public void should_increment_last_value_version_number()
			{
				string goodAssemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.1\")] // Rule: *.*.*.+\r\n[assembly: AssemblyFileVersion(\"1.0.0.1\")] // Rule: *.*.*.+\r\n";

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.+\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.+\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_increment_thrid_value_version_number()
			{
				string goodAssemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.1.0\")] // Rule: *.*.+.*\r\n[assembly: AssemblyFileVersion(\"1.0.1.0\")] // Rule: *.*.+.*\r\n";

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.+.*\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.+.*\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_increment_second_value_version_number()
			{
				string goodAssemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.1.0.0\")] // Rule: *.+.*.*\r\n[assembly: AssemblyFileVersion(\"1.1.0.0\")] // Rule: *.+.*.*\r\n";

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.+.*.*\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.+.*.*\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}
			[Test]
			public void should_increment_first_value_version_number()
			{
				string goodAssemblyInfoVersions =
					"[assembly: AssemblyVersion(\"2.0.0.0\")] // Rule: +.*.*.*\r\n[assembly: AssemblyFileVersion(\"2.0.0.0\")] // Rule: +.*.*.*\r\n";

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: +.*.*.*\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: +.*.*.*\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}


		}

		[TestFixture]
		public class when_using_the_year_rule_parts : with_the_versioning_service
		{
			[Test]
			public void should_update_last_value_to_year_minus_2000()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.Y\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.Y\r\n", twoDigitYear);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.Y\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.Y\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_update_last_value_to_year_minus_2000_always_two()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.YY\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.YY\r\n", twoDigitYear);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.YY\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.YY\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_update_last_value_to_four_digit_year()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.YYYY\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.YYYY\r\n", fourDigitYear);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.YYYY\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.YYYY\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}
		}

		[TestFixture]
		public class when_using_the_month_rule_parts : with_the_versioning_service
		{
			[Test]
			public void should_update_last_value_to_current_month()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.M\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.M\r\n", monthNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.M\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.M\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_update_last_value_to_current_month_always_two()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.MM\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.MM\r\n", twoDigitMonthNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.MM\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.MM\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}
		}

		[TestFixture]
		public class when_using_the_day_rule_parts : with_the_versioning_service
		{
			[Test]
			public void should_update_last_value_to_current_day()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.D\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.D\r\n", dayofMonthNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.D\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.D\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_update_last_value_to_current_day_always_two()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.DD\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.DD\r\n", twoDigitDayofMonthNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.DD\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.DD\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}
		}

		[TestFixture]
		public class when_using_the_hour_rule_parts : with_the_versioning_service
		{
			[Test]
			public void should_update_last_value_to_current_hour()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.h\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.h\r\n", hourNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.h\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.h\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_update_last_value_to_current_hour_always_two()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.hh\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.hh\r\n", twoDigitHourNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.hh\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.hh\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}
		}

		[TestFixture]
		public class when_using_the_minute_rule_parts : with_the_versioning_service
		{
			[Test]
			public void should_update_last_value_to_current_minute()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.m\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.m\r\n", minuteNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.m\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.m\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_update_last_value_to_current_minute_always_two()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.mm\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.mm\r\n", twoDigitMinuteNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.mm\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.mm\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}
		}

		[TestFixture]
		public class when_using_the_second_rule_parts : with_the_versioning_service
		{
			[Test]
			public void should_update_last_value_to_current_second()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.s\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.s\r\n", secondNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.s\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.s\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				//Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_update_last_value_to_current_second_always_two()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}\")] // Rule: *.*.*.ss\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}\")] // Rule: *.*.*.ss\r\n", twoDigitSecondNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.ss\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.ss\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				//Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}
		}

		[TestFixture]
		public class when_using_the_mix_rule_parts : with_the_versioning_service
		{
			[Test]
			public void should_update_last_value_to_current_year2month2()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}{1}\")] // Rule: *.*.*.YYMM\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}{1}\")] // Rule: *.*.*.YYMM\r\n", twoDigitYear, twoDigitMonthNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.YYMM\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.YYMM\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_update_last_value_to_current_month2day2()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}{1}\")] // Rule: *.*.*.MMDD\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}{1}\")] // Rule: *.*.*.MMDD\r\n", twoDigitMonthNumber, twoDigitDayofMonthNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.MMDD\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.MMDD\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_update_last_value_to_current_day2hour2()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}{1}\")] // Rule: *.*.*.DDhh\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}{1}\")] // Rule: *.*.*.DDhh\r\n", twoDigitDayofMonthNumber, twoDigitHourNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.DDhh\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.DDhh\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_update_last_value_to_current_hour2minute2()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}{1}\")] // Rule: *.*.*.hhmm\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}{1}\")] // Rule: *.*.*.hhmm\r\n", twoDigitHourNumber, twoDigitMinuteNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.hhmm\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.hhmm\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_update_last_value_to_current_minute2second2()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}{1}\")] // Rule: *.*.*.mmss\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}{1}\")] // Rule: *.*.*.mmss\r\n", twoDigitMinuteNumber, twoDigitSecondNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.mmss\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.mmss\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				//Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}

			[Test]
			public void should_update_last_value_to_current_hour2minute2second2()
			{
				string goodAssemblyInfoVersions =
					string.Format("[assembly: AssemblyVersion(\"1.0.0.{0}{1}{2}]\")] // Rule: *.*.*.hhmmss\r\n[assembly: AssemblyFileVersion(\"1.0.0.{0}{1}{2}\")] // Rule: *.*.*.hhmmss\r\n", twoDigitHourNumber, twoDigitMinuteNumber, twoDigitSecondNumber);

				string assemblyInfoVersions =
					"[assembly: AssemblyVersion(\"1.0.0.0\")] // Rule: *.*.*.hhmmss\r\n[assembly: AssemblyFileVersion(\"1.0.0.0\")] // Rule: *.*.*.hhmmss\r\n";

				string newAssemblyInfoVersions = versioningService.UpdateVersionNumbers(assemblyInfoVersions);

				//Assert.AreEqual(goodAssemblyInfoVersions, newAssemblyInfoVersions);
			}
		}
	}
}