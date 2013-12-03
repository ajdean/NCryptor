using Dokan;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ncryptor
{
    class Program
    {
        static void Main(string[] args)
        {
            //Cryptography.RSA(@"D:\Work\test\test");

            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.ImportCspBlob(Convert.FromBase64String(System.IO.File.ReadAllText(@"D:\Work\test\test.key")));

                    System.IO.File.Delete(@"D:\Work\test\test.txt.e");

                    System.IO.File.Delete(@"D:\Work\test\test.out.txt");

                    Cryptography.EncryptFile(@"D:\Work\test\test.txt", @"D:\Work\test\test.txt.e", "test", rsa);

                    Cryptography.DecryptFile(@"D:\Work\test\test.txt.e", @"D:\Work\test\test.out.txt", "test", rsa);


                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }


            //DokanOptions opt = new DokanOptions();
            //opt.MountPoint = ConfigurationManager.AppSettings["DriveLetter"];
            //opt.DebugMode = true;
            ////opt.UseStdErr = true;
            //opt.VolumeLabel = "ncryptor";
            ////opt.NetworkDrive = false;
            ////opt.NetworkDrive = false;
            ////opt.UseKeepAlive = true;
            //opt.ThreadCount = 5;
            //var operations = (IDokanOperations)new Operations(ConfigurationManager.AppSettings["Directory"]);
            //int status = Dokan.DokanMain(opt, operations);
            //switch (status)
            //{
            //    case Dokan.DOKAN_DRIVE_LETTER_ERROR:
            //        Console.WriteLine("Drive letter error");
            //        break;
            //    case Dokan.DOKAN_DRIVER_INSTALL_ERROR:
            //        Console.WriteLine("Driver install error");
            //        break;
            //    case Dokan.DOKAN_MOUNT_ERROR:
            //        Console.WriteLine("Mount error");
            //        break;
            //    case Dokan.DOKAN_START_ERROR:
            //        Console.WriteLine("Start error");
            //        break;
            //    case Dokan.DOKAN_ERROR:
            //        Console.WriteLine("Unknown error");
            //        break;
            //    case Dokan.DOKAN_SUCCESS:
            //        Console.WriteLine("Success");
            //        break;
            //    default:
            //        Console.WriteLine(string.Format("Unknown status: %d", status));
            //        break;

            //}

            DokanOptions opt = new DokanOptions();
            opt.DebugMode = true;
            opt.MountPoint = ConfigurationManager.AppSettings["DriveLetter"];
            opt.VolumeLabel = "NCryptor";
            opt.ThreadCount = 5;
            opt.UseKeepAlive = true;
            int status = DokanNet.DokanMain(opt, new Operations(ConfigurationManager.AppSettings["Directory"]));
            switch (status)
            {
                case DokanNet.DOKAN_DRIVE_LETTER_ERROR:
                    Console.WriteLine("Drvie letter error");
                    break;
                case DokanNet.DOKAN_DRIVER_INSTALL_ERROR:
                    Console.WriteLine("Driver install error");
                    break;
                case DokanNet.DOKAN_MOUNT_ERROR:
                    Console.WriteLine("Mount error");
                    break;
                case DokanNet.DOKAN_START_ERROR:
                    Console.WriteLine("Start error");
                    break;
                case DokanNet.DOKAN_ERROR:
                    Console.WriteLine("Unknown error");
                    break;
                case DokanNet.DOKAN_SUCCESS:
                    Console.WriteLine("Success");
                    break;
                default:
                    Console.WriteLine("Unknown status: %d", status);
                    break;

            }

        }
    }
}
