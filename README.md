# Irony
A modified version of the Irony project ([https://irony.codeplex.com](https://irony.codeplex.com)) with .NET Core support.

Irony is a .NET Language Implementation Kit written originally by Roman Ivantsov, you should be able to find his blog related to Irony via [http://irony-roman.blogspot.com/](http://irony-roman.blogspot.com/). He also developed an ORM framework, VITA, which can be found [here](http://vita.codeplex.com/ "here").

Based on the fact that the project on its official site hasn't been updated for a long time (last commit was on Dec 13th 2013) and cannot support .NET Core, I just made a copy of the project and made some modifications in order to support .NET Core. I still kept the MIT license and made the project to be licensed under Roman's name.  

## Major Changes
- Fixed the compile issues found during .NET Core migration
	- Changed `StringComparer.InvariantCulture(IgnoreCase)` to `StringComparer.CurrentCulture(IgnoreCase)`
	- Changed `char.GetUnicodeCategory()` to `CharUnicodeInfo.GetUnicodeCategory(current)`
	- Temporary removed `ParseTreeExtensions` implementation
	- Removed the original `Test`, `Sample`, `GrammarExplorer` projects from the Visual Studio solution. Unit tests will be migrated to xUnit in a later stage. And the GrammarExplorer is supposed to be provided in another repo

