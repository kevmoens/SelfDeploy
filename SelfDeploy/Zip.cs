
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;

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
            string tempBuildDirectory = GetTempPath();
            ExtractFiles(OpenArchive(), tempBuildDirectory);
            ZipFiles(directory, tempBuildDirectory);
            CompileProject(tempBuildDirectory);
            string compiledFilePath = Path.Combine(tempBuildDirectory, "obj", "Release", "net48-windows", "SelfDeployUI.exe");
            File.Copy(compiledFilePath, outputFilePath);
            Directory.Delete(tempBuildDirectory, true);
        }

        private static string GetTempPath()
        {
            return Path.Combine(Path.GetTempPath(), $"LCBuild_{DateTime.Now.ToString("yyMMddHHmmssfff")}");
        }

        private static ZipArchive OpenArchive()
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "SelfDeploy.UI.zip";
            var resourceStream = assembly.GetManifestResourceStream(resourceName);
           
            return new ZipArchive(resourceStream, ZipArchiveMode.Read, false);
        }
        private static void ExtractFiles(ZipArchive archive, string tempDirectory)
        {
            foreach (var entry in archive.Entries)
            {
                if (entry.Length == 0)
                {
                    continue;
                }
                string toFilePath = Path.Combine(tempDirectory, entry.FullName);
                DirectoryInfo toFile = new DirectoryInfo(toFilePath);
                if (toFile.Parent.Exists == false)
                {
                    toFile.Parent.Create();
                }
                entry.ExtractToFile(toFilePath);
            }
        }

        private static void CompileProject(string tempDirectory)
        {
            string uiSolution = Path.Combine(tempDirectory, "SelfDeployUI.csproj");

            // Run dotnet publish against SelfDeployUI
            Process process = new Process();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"clean {uiSolution}";
            process.Start();
            process.WaitForExit();

            process = new Process();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = $"build {uiSolution} -c Release";
            process.StartInfo.UseShellExecute = false;

            process.Start();

            process.WaitForExit();
        }

        private static void ZipFiles(string directory, string tempBuildDirectory)
        {
            string outFileName = Path.Combine(tempBuildDirectory, "Archive.zip");
            if (File.Exists(outFileName))
            {
                File.Delete(outFileName);
            }
            ZipFile.CreateFromDirectory(directory, outFileName, CompressionLevel.Optimal, false);
        }
      
    }
}
