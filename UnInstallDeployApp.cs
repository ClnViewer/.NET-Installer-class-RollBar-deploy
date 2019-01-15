using System;
using System.Diagnostics;
using System.IO;

namespace UnInstallDeploy
{
    class AppUninstall
    {
        static void Main(string[] args)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            foreach (string argument in arguments)
            {
                string[] pargs = argument.Split('=');
                if (pargs[0].ToLower() == "/u")
                {
                    string productCode = pargs[1];
                    string evp = Environment.GetFolderPath(Environment.SpecialFolder.System);
                    Process proc = new Process();
                    proc.StartInfo.FileName = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.System),
                        "msiexec.exe"
                        );
                    proc.StartInfo.Arguments = String.Format(" /x {0} /qr", pargs[1]);
                    proc.Start();
                    break;
                }
            }
        }
    }
}
