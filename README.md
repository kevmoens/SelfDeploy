# SelfDeploy
SelfDeploy is a way to create self extracting zip files using only standard .NET.

This is a .NET Project runninng on .NET 4.8.
Why .NET Framework 4.8?
  - This only creates EXEs that run on Windows. 
  - .NET Framework is built into the Windows OS.
  - All versions of supported Windows OS has .NET Framework without installing it.
  - WinForms in .NET 8.0 doesn't support Trimmed EXE
  - Using .NET 8.0 and SelfContained adds a 100mb to EXE


