using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace PAWNEdit
{
    class Build
    {
        private Form1 mainform;

        // Constructor
        public Build(Form1 form)
        {
            try
            {
                this.mainform = form;
            }
            catch(Exception ex) { form.CaughtException(ex); }
        }
        
        // Functions
        public void BuildFile(RichTextBox buildbox, string filename, string directory)
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.Arguments = filename + " -i=\"" + Directory.GetCurrentDirectory() + "\\includes\"";
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = "pawncc.exe";
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.WorkingDirectory = directory;
                proc.Start();

                string output = proc.StandardOutput.ReadToEnd();
                string error = proc.StandardError.ReadToEnd();
                proc.WaitForExit();

                buildbox.Invoke(new MethodInvoker(delegate { buildbox.Text = error + "\n" + output.Replace("\r\n\r\n", "\r\n"); }));
            }
            catch (Exception ex) { mainform.Invoke(new MethodInvoker(delegate { mainform.CaughtException(ex); })); }
        }
    }
}
