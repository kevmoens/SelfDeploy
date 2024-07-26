using System;
using System.IO;
class Program
{
    static void Main(string[] args)
    {

        string startupPath = AppDomain.CurrentDomain.BaseDirectory;
        string filesPath = $"{startupPath}\\Files";
        string resultFile = $"{startupPath}\\SelfDeploy.exe";
        if (File.Exists(resultFile))
        {
            File.Delete(resultFile);
        }
        if (Directory.Exists(filesPath))
        {
            Directory.Delete(filesPath, true);
        }
        LoadFilesDirectory(filesPath);
        SelfDeploy.Zip.CreateSelfDeploy(filesPath, resultFile);
        Console.ReadLine();
    }

    static void LoadFilesDirectory(string filesPath)
    {
        Directory.CreateDirectory(filesPath);
        // ./Install.txt
        // ./DATABASE/DBInstall.txt
        // ./DATABASE/Install.txt
        string installTxt = $"{filesPath}\\Install.txt";
        File.WriteAllText(installTxt, "Application=Doc-Trak\r\nPackage=\r\nKeyTable=\r\nShowVersion=2025");
        string dbFolder = $"{filesPath}\\DATABASE";
        Directory.CreateDirectory(dbFolder);
        string dbInstallTxt = $"{filesPath}\\DATABASE\\DBInstall.txt";
        File.WriteAllText(dbInstallTxt, "DATABASE\\DBInstall.txt");
        string dbInstallTxt2 = $"{filesPath}\\DATABASE\\Install.txt";
        File.WriteAllText(dbInstallTxt2, "DATABASE\\Install.txt");
    }
}