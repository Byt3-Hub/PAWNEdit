/*
    Copyright (C) 2013 Cody Cunningham

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace PAWNEdit
{
    public class Build
    {
        private Form1 mainform;
        private Tabs tabs;
        private Settings settings;

        // Constructor
        public Build(Form1 form)
        {
            try
            {
                this.mainform = form;
            }
            catch(Exception ex) { form.CaughtException(ex); }
        }

        public void Update()
        {
            this.tabs = mainform.tabs;
            this.settings = mainform.settings;
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
