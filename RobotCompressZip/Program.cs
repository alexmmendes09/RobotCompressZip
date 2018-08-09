using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Configuration;
using System.Diagnostics;

namespace RobotCompressZip
{
    public class Program
    {

        public static string zFilesType = "*.7z";
        public static string pdfFilesType = "*.pdf";

        public static void Main()
        {
            DirectoryInfo directorySelected = new DirectoryInfo(ConfigurationManager.AppSettings["PastaTemp"]);
            string[] dirs = Directory.GetDirectories(directorySelected.ToString(), "*", SearchOption.AllDirectories);
            dirs.ToList().ForEach(dir =>
            {
                DirectoryInfo dirRaiz = new DirectoryInfo(dir);
                dirRaiz.GetFiles(zFilesType).ToList().ForEach(zFile =>
                {
                    FileInfo fileDecompressCompress = zFile;
                    Decompress7zToCompressZip(fileDecompressCompress);
                    dirRaiz.GetFiles(pdfFilesType).ToList().ForEach(pdfFile =>
                    {
                        FileInfo fileDecompressPdf = pdfFile;
                        fileDecompressPdf.Delete();
                    });
                });
            });
        }

        private static void Decompress7zToCompressZip(FileInfo fileDecompressCompress)
        {
            var pastaSevenZip = ConfigurationManager.AppSettings["ComandoPastaSevenZip"];
            var DEFAUL_PASSKEY = ConfigurationManager.AppSettings["DEFAUL_PASSKEY"];
            var NEW_DEFAUL_PASSKEY = ConfigurationManager.AppSettings["NEW_DEFAUL_PASSKEY"];
            var CommandDecompress7z = ConfigurationManager.AppSettings["CommandDecompress7z"];
            var CommandCompressZip = ConfigurationManager.AppSettings["CommandCompressZip"];
            var ArquivoLog7z = ConfigurationManager.AppSettings["ArquivoLog7z"];
            string CMD = "cmd.exe";
            string adminRun = "runas";
            string replacePDfs = "PDfs";
            string replaceFolder = "ZIPADOS\\PDfs";
            string seteZip = "7z";
            string zip = "zip";

            string comandoExtracao7z = string.Format(
                CommandDecompress7z,
                fileDecompressCompress.Directory,
                fileDecompressCompress.Name,
                fileDecompressCompress.DirectoryName,
                DEFAUL_PASSKEY);

            string comandoCompressZip = string.Format(
                CommandCompressZip,
                fileDecompressCompress.Directory.ToString().Replace(replacePDfs, replaceFolder),
                fileDecompressCompress.Name.Replace(seteZip, zip),
                fileDecompressCompress.DirectoryName,
                RobotCompressZip.Common.FileType.PDF,
                NEW_DEFAUL_PASSKEY.ToString());

            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = CMD,
                WorkingDirectory = pastaSevenZip,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                Verb = adminRun,
            };
            Process process = Process.Start(info);
            try
            {
                process.StandardInput.WriteLine(comandoExtracao7z);
                process.StandardInput.WriteLine(comandoCompressZip);
                process.StandardInput.Close();
                string output = process.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
                File.WriteAllText(ArquivoLog7z, output);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}