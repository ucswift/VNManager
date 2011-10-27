# VNManager

VNManager allows you to automate complex version number management for your projects and solutions. Using VNManager you can have date based, increment based or other static version number assignment in many different ways. VNManager can intergrate with your Visual Studio solutions, MSBuild, Team City or many other CI/Building solutions.

## License

Licensed under the Microsoft Public License (MS-PL)

## Resources

* **WaveTech's Home page:** <http://www.wtdt.com>
* **VNManager's Home Page:** <http://www.wtdt.com/Products/Tools/VNManager.aspx>

## Prerequisites

You will need the .Net Framework 4.

# Version Number Rules

A version number consists of four parts separated by the '.' character. The four parts are:

Major Version
Minor Version
Build Number
Revision
For example: 1.2.3.4 or 1.07.0345.6789

Each part of the version number has it's own rule and each rule part is also separated by the '.' character.

Rule Parts

Rule parts are made up of predefined tags that are used to update the related version number part.

The available rule parts are:

* - leave the version number part unchanged
+ - increment the version number by one
Y - the current year (minus 2000; i.e. 2008 returns 8)
YY - same as Y except returns two characters
YYYY - returns four digit year
M - current month
MM - current month (two characters)
D - current day of month
DD - two character day of month
h - current hour
hh - current hour (two characters)
m - current minute
mm - current minute (two characters)
s - current second
ss - current second (two characters)
YYMM - two digit year and two digit month
MMDD - two digit month and two digit day
DDhh - two digit day and two digit hour
hhmm - two digit hour and two digit minute
mmss - two digit minute and two digit second
hhmmss - two digit hour, two digit minute, and two digit second
nY - number of years from the given date (default of 1/1/2000)
nM - number of months from the given date (default of 1/1/2000)
nW - number of weeks from the given date (default of 1/1/2000)
nD - number of days from the given date (default of 1/1/2000)
TT - two token spaces to be passed in via the token command line paramater
TTTT - four token spaces to be passed in via the token command line paramater

Note: date parts are upper-case and time parts are lower-case.

Specifying rule part length

A rule part can have a length specifier appended to force the rule part to have a minimum number of characters. The lenght specifier takes the form ":n" where n is the desired minimum length. If the resulting number is longer than the length specified, the result is NOT truncated.

For example:

M:2 - returns a two digit month with a leading zero if required 02 for February, etc.
nW:4 - returns the number of weeks from the given date (default of 1/1/2000)

Multi-part rules

Rules can be combiled with the ";" character to create composite rules.

For example:

h:2;m:2 - returns four characters; two for the hour and two for the minute (same as hhmm)

Creating the rule

Now that you know what a rule is made of, it might be nice to actually create one.

On the same line as the version you want to apply the rule to, append a comment in the following format:

C#:

[assembly: AssemblyVersion("1.00.0806.0007")]        // Rule: *.*.YYMM.+
[assembly: AssemblyFileVersion("1.08.0627.1857")]    // Rule: *.YY.MMDD.hhmm
VB:

        ' Rule: *.*.YYMM.+    ' Rule: *.YY.MMDD.hhmm
Notice that the AssemblyVersion and the AssemblyFileVersion each have their own rule.

Specifying the base date

The default date (1/1/2000) used in calculations can be overridden with any date of your choosing. One option is to use the date that the project was started so that the rules such as "nD" will calculate from the project start date.

To specify a custom start date, add a StartDate: entry to the rule comment like this:

C#:

[assembly: AssemblyVersion("1.00.0806.0007")]        // Rule: *.*.YYMM.+        StartDate: 1/1/2008
[assembly: AssemblyFileVersion("1.08.0627.1857")]    // Rule: *.YY.MMDD.hhmm    StartDate: 1/1/2008
VB:

        ' Rule: *.*.YYMM.+        StartDate: 1/1/2008    ' Rule: *.YY.MMDD.hhmm    StartDate: 1/1/2008
AssemblyVersion and the AssemblyFileVersion can have different dates specified if desired.

Invalid Rules

Any rule part that generates a number greater than 65535 will cause the build to be cancelled.
An error message will be written to the output window and an entry added to the error list window.

If you want to know why I'm doing this, see here.

Default Rules

If you don't specify a default rule when you run V.N.Manager any version attirbutes that do not have a rule next to them will NOT be modified. If you want to modifiy every version attirbute you need to specific a default rule which will be applied to all AssemblyInfo.cs files that do not have their own rule.

       -v DefaultVersionRule
       -s DefaultFileVersionRule

       Example Usage: VNManager -a"C:\MyProject" -s"*.*.YYMM.DDhh" -v"*.*.YYMM.DDhh"
  
The default rules will apply to all AssemblyInfo.cs files that do not have their own rule specified. Rules defined at the version line level take precedence.

Citation

Most documentation above is from Gryum Technologies (http://www.gyrum.com/Projects/VSVersion.aspx).