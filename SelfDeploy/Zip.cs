
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using System;

namespace SelfDeploy
{
    public static class Zip
    {
        //Can't publish single file .NET Core with Trimmed... so 100MB 
        //Roslyn compiling WinForms is a struggle
        //Thus it is WinForms .NET Framework project

        private static DirectoryInfo _baseDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        public static void CreateSelfDeploy(string directory, string outputFilePath)
        {
            ZipFiles(directory);
            CompileProject();
            string compiledFilePath = Path.Combine(GetParentDir(), "obj", "Release", "net48-windows", "SelfDeployUI.exe");
            File.Copy(compiledFilePath, outputFilePath);
        }

        private static void CompileProject()
        {
            string uiSolution = Path.Combine(GetParentDir(), "SelfDeployUI.csproj");

            // Run dotnet publish against SelfDeployUI
            Process process = new Process();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"clean {uiSolution}";
            process.Start();
            process.WaitForExit();

            process = new Process();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"build {uiSolution} -c Release";
            process.StartInfo.UseShellExecute = false;

            process.Start();

            process.WaitForExit();
        }

        private static void ZipFiles(string directory)
        {
            string outFileName = Path.Combine(GetParentDir() ,"Archive.zip");
            if (File.Exists(outFileName))
            {
                File.Delete(outFileName);
            }
            ZipFile.CreateFromDirectory(directory, outFileName, CompressionLevel.Optimal, false);
        }
        private static string GetParentDir()
        {
            string outParentDir = Path.Combine(_baseDir.Parent.Parent.Parent.Parent.FullName, "SelfDeployUI");
            if (Debugger.IsAttached == false)
            {
                outParentDir = Path.Combine(_baseDir.FullName, "SelfDeployUI");
            }
            return outParentDir;
        }
    }
}
