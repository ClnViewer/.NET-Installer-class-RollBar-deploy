/*
    MIT License

    Copyright (c) 2018 PS
    GitHub .NET Installer class Custom action + RollBar deploy:
    https://github.com/ClnViewer/.NET-Installer-class-RollBar-deploy/

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sub license, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
 */

///
/// Debug defines
/// 
// #define DEBUGVAR_Commit
// #define DEBUGVAR_Committed
// #define DEBUGVAR_Committing

/// RollBar deploy post
// #define DEBUGVAR_Deploy

// #define DEBUGVAR_Uninstall
// #define DEBUGVAR_Install
// #define DEBUGVAR_Rollback

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

#if DEBUGVAR_Install || DEBUGVAR_Uninstall || DEBUGVAR_Commit || DEBUGVAR_Committed || DEBUGVAR_Committing || DEBUGVAR_Rollback
using System.Reflection;
#endif 

namespace CoCAnnounce
{
    [RunInstaller(true)]
    public partial class InstallerCoCAnnouncer : System.Configuration.Install.Installer
    {
        private const string __log = "intaller_VariableDebug.log";
        private const string __exe = "YouExecutable.exe";
        private const string __tag = "xTargetDir";
        private const string __rltoken = "You-RollBar-Id";
        private const string __rlurl = "https://api.rollbar.com/api/1/deploy/";
        private const string __logKeyFmt = "\t  key [{0}] = {1}\n";

        private string _LogPath
        {
            get
            {
                return Path.Combine(
                    Path.GetDirectoryName(
                        Context.Parameters[__tag].ToString()
                    ),
                    __log
                );
            }
        }

        private string _IdName
        {
            get
            {
                string name = Environment.MachineName;
                if (!String.IsNullOrWhiteSpace(name)) return name;
                name = System.Net.Dns.GetHostName();
                if (!String.IsNullOrWhiteSpace(name)) return name;
                name = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
                if (!String.IsNullOrWhiteSpace(name)) return name;
                return AppVersionInfo.xAppVersion + " " + AppVersionInfo.xAppDateBuild;
            }
        }

        public InstallerCoCAnnouncer() : base()
        {
            InitializeComponent();

            this.Committed += new InstallEventHandler(_Committed);
            this.Committing += new InstallEventHandler(_Committing);
        }
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            stateSaver.Add(__tag, Context.Parameters[__tag].ToString());
#if DEBUGVAR_Install
            _VariableDebug(stateSaver, MethodBase.GetCurrentMethod().Name);
#endif
        }
        public override void Uninstall(IDictionary savedState)
        {
            Process app = null;
            foreach (var process in Process.GetProcesses())
            {
                if (!process.ProcessName.ToLower().Contains(Path.GetFileNameWithoutExtension(__exe)))
                    continue;

                app = process;
                break;
            }
            if (app != null && app.Responding)
            {
                app.Kill();
            }

            base.Uninstall(savedState);
#if DEBUGVAR_Uninstall
            _VariableDebug(savedState, MethodBase.GetCurrentMethod().Name);
#elif DEBUGVAR_Install || DEBUGVAR_Commit || DEBUGVAR_Committed || DEBUGVAR_Committing || DEBUGVAR_Rollback

            try
            {
                string log = Path.Combine(
                    Context.Parameters[__tag].ToString(),
                    __log
                );
                if (File.Exists(log))
                    File.Delete(log);
            }
            catch (Exception) { }
#endif
        }
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
#if DEBUGVAR_Commit
            _VariableDebug(savedState, MethodBase.GetCurrentMethod().Name);
#endif
        }
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
#if DEBUGVAR_Rollback
            _VariableDebug(savedState, MethodBase.GetCurrentMethod().Name);
#endif
        }

        private void _Committed(object sender, InstallEventArgs e)
        {
            string exec = Path.Combine(
                    Path.GetDirectoryName(
                        Context.Parameters[__tag].ToString()
                    ),
                    __exe
                );

#if DEBUGVAR_Committed
            _VariableDebug(null, MethodBase.GetCurrentMethod().Name);
#endif
            if (!File.Exists(exec))
                return;

            Process.Start(exec);
        }
        private void _Committing(object sender, InstallEventArgs e)
        {
#if DEBUGVAR_Committing
            _VariableDebug(null, MethodBase.GetCurrentMethod().Name);
#endif
            _Post();
        }

        private void _Post()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var args = new Dictionary<string, string>
                    {
                        {"access_token", __rltoken },
                        {"environment","production" },
                        {"revision", AppVersionInfo.xAppRevision },
                        {"local_username", _IdName }
                    };
                    Task<HttpResponseMessage> t = client.PostAsync(
                        __rlurl,
                        new FormUrlEncodedContent(args)
                        );
                    t.Wait(new TimeSpan(0, 1, 30));

#if DEBUGVAR_Deploy
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("\n{0} - RollBar deploy:\n", DateTime.Now.ToShortTimeString());

                    foreach (KeyValuePair<string, string> kvp in args)
                    {
                        sb.AppendFormat(__logKeyFmt, kvp.Key, kvp.Value);
                    }
                    File.AppendAllText(_LogPath, sb.ToString());
#endif
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(_LogPath, e.ToString());
            }
        }

        private void _VariableDebug(IDictionary savedState, string mname)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\n{0} - method: {1}\n", DateTime.Now.ToShortTimeString(), mname);

            if (savedState != null)
            {
                sb.Append("State:\n");
                sb.AppendFormat("\tCount  : {0}\n", savedState.Count.ToString());
                sb.AppendFormat("\tKeys   : {0}\n", savedState.Keys.Count.ToString());
                sb.AppendFormat("\tValues : {0}\n", savedState.Values.Count.ToString());

                if (savedState.Count > 0)
                {
                    foreach (string k in savedState.Keys)
                    {
                        sb.AppendFormat(__logKeyFmt, k, savedState[k].ToString());
                    }
                }
            }

            sb.Append("Context:\n");
            sb.AppendFormat("\tCount  : {0}\n", Context.Parameters.Count.ToString());
            sb.AppendFormat("\tKeys   : {0}\n", Context.Parameters.Keys.Count.ToString());
            sb.AppendFormat("\tValues : {0}\n", Context.Parameters.Values.Count.ToString());

            if (this.Context.Parameters.Count > 0)
            {
                foreach (string k in Context.Parameters.Keys)
                {
                    sb.AppendFormat(__logKeyFmt, k, Context.Parameters[k].ToString());
                }
            }
            sb.Append("End:\n\n");
            File.AppendAllText(_LogPath, sb.ToString());
        }
    }
}
