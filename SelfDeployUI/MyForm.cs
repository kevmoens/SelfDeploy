using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SelfDeployUI
{
    public partial class MyForm : Form
    {
        public MyForm()
        {
            InitializeComponent();
            Load += MyForm_Load;
            Shown += MyForm_Shown;
        }
        string _tempDirectory = GetTempPath();
        ZipArchive _archive;
        Stream _resourceStream;

        private void MyForm_Load(object sender, EventArgs e)
        {
            //Read in readme.txt to get application installer name
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "SelfDeployUI.Archive.zip";
            _resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (_resourceStream == null)
            {
                MessageBox.Show("Resource not found: " + resourceName);
                this.Close(); //Should never happen
                return;
            }
            _archive = new ZipArchive(_resourceStream, ZipArchiveMode.Read, false);
            UpdateFormCaption();

        }

        private void UpdateFormCaption()
        {
            var entry = _archive.GetEntry("Install.txt");
            if (entry == null)
            {
                return; //Can't update Caption
            }
            using (var installStream = entry.Open())
            {
                using (var reader = new StreamReader(installStream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("Application="))
                        {
                            this.Text = line.Substring("Application=".Length) + " Install";
                            return;
                        }
                    }
                }
            }
        }

        private async void MyForm_Shown(object sender, EventArgs e)
        {

            Directory.CreateDirectory(_tempDirectory);

            await ExtractFiles();

            RunLCInstallEXE();
            this.Close();
        }

        private async Task ExtractFiles()
        {
            progressBar1.Maximum = _archive.Entries.Count;
            foreach (var entry in _archive.Entries)
            {
                string toFilePath = Path.Combine(_tempDirectory, entry.FullName);
                DirectoryInfo toFile = new DirectoryInfo(toFilePath);
                if (toFile.Parent.Exists == false)
                {
                    toFile.Parent.Create();
                }
                await Task.Run(() => entry.ExtractToFile(toFilePath));
                progressBar1.Value++;
                Application.DoEvents();
            }
        }


        private void RunLCInstallEXE()
        {
            string installExe = Path.Combine(_tempDirectory, "LCInstall.exe");
            if (File.Exists(installExe) == false)
            {
                return;
            }
            Process.Start(installExe);
        }

        private static string GetTempPath()
        {
            return Path.Combine(Path.GetTempPath(), $"LCInstall_{DateTime.Now.ToString("yyMMddHHmmssfff")}");
        }
    }
}
