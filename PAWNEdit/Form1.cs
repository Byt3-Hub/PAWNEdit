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
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace PAWNEdit
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern bool GetCaretPos(out Point lpPoint);

        private Tabs tabs;
        private Build build;

        // Constructor
        public Form1()        
        {
            try
            {
                InitializeComponent();
                LoadIncludes();
                this.build = new Build(this);
                this.tabs = new Tabs(this);
            }
            catch (Exception ex) { CaughtException(ex); }
        }

        // Events
        private void newTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                tabs.NewTab();
            }
            catch (Exception ex) { CaughtException(ex); }
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedIndex > -1)
                {
                    tabs.CloseTab(tabControl1.SelectedTab);
                }
            }
            catch (Exception ex) { CaughtException(ex); }
        }

        private void closeEveryTabButCurrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex > -1)
            {
                try
                {
                    TabPage cur = tabControl1.SelectedTab;
                    reiterate:
                    foreach (Tabs.Tab_t tab in tabs.tabs)
                    {
                        if (tab.page != cur)
                        {
                            tabs.CloseTab(tab.page);
                            goto reiterate;
                        }
                    }
                    tabControl1.SelectedTab = cur;
                }
                catch (Exception ex) { CaughtException(ex); }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                TabPage cur = tabControl1.SelectedTab;
                foreach (Tabs.Tab_t tab in tabs.tabs)
                {
                    if (tab.page == cur)
                    {
                        Thread proc = new Thread(() => build.BuildFile(tab.build, tab.Filename, tab.Filedir));
                        proc.Start();
                        break;
                    }
                }
            }
            catch (Exception ex) { CaughtException(ex); }
        }


        private void removeExcessSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TabPage cur = tabControl1.SelectedTab;
                foreach (Tabs.Tab_t tab in tabs.tabs)
                {
                    if (tab.page == cur)
                    {
                        string text = null;
                        for (int lineind = 0; lineind < tab.scintilla.Lines.Count; lineind++)
                        {
                            text = tab.scintilla.Lines[lineind].Text;
                            if (text.Length > 0)
                            {
                                if (text.EndsWith("\r\n")) text = text.Substring(0, text.Length - 2);
                                if (text.EndsWith("\n")) text = text.Substring(0, text.Length - 1);
                                while (text.EndsWith(" ") || text.EndsWith("\t"))
                                {
                                    if (text.Length < 1) break;
                                    text = text.Substring(0, text.Length - 1);
                                }
                                tab.scintilla.Lines[lineind].Text = text;
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex) { CaughtException(ex); }
        }

        private void indentCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TabPage cur = tabControl1.SelectedTab;
                foreach (Tabs.Tab_t tab in tabs.tabs)
                {
                    if (tab.page == cur)
                    {
                        IndentScintillaText(tab.scintilla);
                    }
                }
            }
            catch (Exception ex) { CaughtException(ex); }
        }


        private void unIndentCoolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TabPage cur = tabControl1.SelectedTab;
                foreach (Tabs.Tab_t tab in tabs.tabs)
                {
                    if (tab.page == cur)
                    {
                        UnIndentScintillaText(tab.scintilla);
                    }
                }
            }
            catch (Exception ex) { CaughtException(ex); }
        }

        private void replace4SpacesWithTabsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TabPage cur = tabControl1.SelectedTab;
                foreach (Tabs.Tab_t tab in tabs.tabs)
                {
                    if (tab.page == cur)
                    {
                        string text = null;
                        bool streamcomment = false;
                        bool instring = false;
                        int ind = 0;
                        for (int lineind = 0; lineind < tab.scintilla.Lines.Count; lineind++)
                        {
                            text = tab.scintilla.Lines[lineind].Text;
                            if (text.Length > 0)
                            {
                                // Fixes for comments.
                                if (text.StartsWith("//"))
                                {
                                    continue;
                                }
                                if (text.IndexOf("/*") != -1 && text.IndexOf("*/") == -1)
                                {
                                    streamcomment = true;
                                }
                                else if (text.IndexOf("*/") != -1 && text.IndexOf("/*") == -1)
                                {
                                    streamcomment = false;
                                }
                                if (!streamcomment)
                                {
                                    // Remove new lines for the local variable.
                                    if (text.EndsWith("\r\n")) text = text.Substring(0, text.Length - 2);
                                    if (text.EndsWith("\n")) text = text.Substring(0, text.Length - 1);

                                reset:
                                    ind = 0;
                                    foreach (char ch in text)
                                    {
                                        if (text.Length > 0)
                                        {
                                            if (ch == '\"')
                                            {
                                                if (instring)
                                                {
                                                    instring = false;
                                                }
                                                else
                                                {
                                                    instring = true;
                                                }
                                            }
                                        }
                                        if (text.Length > ind + 4 && !instring)
                                        {
                                            if (text.Substring(ind, 4).CompareTo("    ") == 0)
                                            {
                                                text = text.Remove(ind, 4);
                                                text = text.Insert(ind, "\t");
                                                instring = false;
                                                goto reset;
                                            }
                                        }
                                        ind++;
                                    }
                                    tab.scintilla.Lines[lineind].Text = text;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { CaughtException(ex); }
        }

        // Functions
        public void UnIndentScintillaText(ScintillaNET.Scintilla scintilla)
        {
            try
            {
                string text = null;
                bool streamcomment = false;
                int ind = 0;
                for (int lineind = 0; lineind < scintilla.Lines.Count; lineind++)
                {
                    text = scintilla.Lines[lineind].Text;
                    if (text.Length > 0)
                    {
                        // Fixes for comments.
                        if (text.StartsWith("//"))
                        {
                            continue;
                        }
                        if (text.IndexOf("/*") != -1 && text.IndexOf("*/") == -1)
                        {
                            streamcomment = true;
                        }
                        else if (text.IndexOf("*/") != -1 && text.IndexOf("/*") == -1)
                        {
                            streamcomment = false;
                        }

                        // The unindenting.
                        if (!streamcomment)
                        {
                            // Remove new lines for the local variable.
                            if (text.EndsWith("\r\n")) text = text.Substring(0, text.Length - 2);
                            if (text.EndsWith("\n")) text = text.Substring(0, text.Length - 1);

                            // Unindent.
                            ind = 0;
                            foreach (char ch in text)
                            {
                                if (text.Length > ind + 1)
                                {
                                    if (text.Substring(ind, 2).CompareTo("//") == 0)
                                    {
                                        break;
                                    }
                                }
                                if (text.Length > 0)
                                {
                                    if (ch != '\t' && ch != ' ') break;
                                    text = text.Remove(0, 1);
                                }
                                ind++;
                            }

                            // Set the line.
                            scintilla.Lines[lineind].Text = text;
                        }
                    }
                }
            }
            catch (Exception ex) { CaughtException(ex); }
        }

        public void IndentScintillaText(ScintillaNET.Scintilla scintilla)
        {
            try
            {
                // First let's unindent.
                UnIndentScintillaText(scintilla);

                // Then indent.
                string text = null;
                int tabind = 0;
                int tabincdec = 0;
                bool streamcomment = false;
                bool instring = false;
                int ind = 0;
                for (int lineind = 0; lineind < scintilla.Lines.Count; lineind++)
                {
                    text = scintilla.Lines[lineind].Text;
                    if (text.Length > 0)
                    {
                        // Fixes for comments.
                        if (text.StartsWith("//"))
                        {
                            continue;
                        }
                        if (text.IndexOf("/*") != -1 && text.IndexOf("*/") == -1)
                        {
                            streamcomment = true;
                        }
                        else if (text.IndexOf("*/") != -1 && text.IndexOf("/*") == -1)
                        {
                            streamcomment = false;
                        }

                        // The indenting.
                        if (!streamcomment)
                        {
                            // Remove excess spaces and new lines for the local variable.
                            if (text.EndsWith("\r\n")) text = text.Substring(0, text.Length - 2);
                            if (text.EndsWith("\n")) text = text.Substring(0, text.Length - 1);
                            while (text.EndsWith(" ") || text.EndsWith("\t"))
                            {
                                if (text.Length < 1) break;
                                text = text.Substring(0, text.Length - 1);
                            }
                            // If the tab index is below 0, set it to 0.
                            if (tabind < 0) tabind = 0;

                            // Add the current tab index.
                            for (int i = 0; i < tabind; i++)
                            {
                                text = text.Insert(0, "\t");
                            }

                            // Indent.
                            tabincdec = 0;
                            ind = 0;
                            foreach (char ch in text)
                            {
                                if (ch == '\"')
                                {
                                    if (instring)
                                    {
                                        instring = false;
                                    }
                                    else
                                    {
                                        instring = true;
                                    }
                                }
                                if (text.Length > ind + 1)
                                {
                                    if (text.Substring(ind, 2).CompareTo("//") == 0)
                                    {
                                        break;
                                    }
                                }
                                if (!instring)
                                {
                                    if (ch == '{')
                                    {
                                        tabincdec++;
                                    }
                                    else if (ch == '}')
                                    {
                                        tabincdec--;
                                    }
                                }
                                ind++;
                            }

                            // A (failed) attempt to fix indentation for code like this
                            // if(1 == 1)
                            // int i = 0;
                            // else
                            // int i = 1;

                            /*if (lineind < scintilla.Lines.Count)
                            {
                                if (text.IndexOf("if") != -1 || text.IndexOf("else") != -1)
                                {
                                    text2 = scintilla.Lines[lineind + 1].Text;
                                    if (text2.EndsWith("\r\n")) text2 = text2.Substring(0, text2.Length - 2);
                                    if (text2.EndsWith("\n")) text2 = text2.Substring(0, text2.Length - 1);

                                    bool inc = true; // increment or no
                                    foreach (char ch in text2)
                                    {
                                        if (ch == '{')
                                        {
                                            inc = false;
                                        }
                                        if (ch == '}')
                                        {
                                            inc = true;
                                        }
                                    }
                                    foreach (char ch in text)
                                    {
                                        if (ch == '{')
                                        {
                                            inc = false;
                                        }
                                        if (ch == '}')
                                        {
                                            inc = true;
                                        }
                                    }
                                    if (inc)
                                    {
                                        MessageBox.Show("line: " + lineind + " " + text2);
                                        text2 = text2.Insert(0, "\t");
                                        scintilla.Lines[lineind + 1].Text = text2;
                                        MessageBox.Show("line2: " + lineind + " " + text2);
                                    }
            
                                }
                            }*/

                            // Set the new tab index.
                            tabind += tabincdec;

                            // Remove wrong tabs on closing braces (assuming there was one on this line)
                            if (tabincdec < 0)
                            {
                                bool removetab = true;
                                foreach (char ch in text)
                                {
                                    if (ch != '\t' && ch != '}')
                                    {
                                        removetab = false;
                                        break;
                                    }
                                    else if (ch == '}')
                                    {
                                        removetab = true;
                                        break;
                                    }
                                }
                                if (removetab && text.IndexOf("\t") >= 0)
                                {
                                    text = text.Remove(text.IndexOf("\t"), 1);
                                }
                            }

                            // Set the line.
                            scintilla.Lines[lineind].Text = text;
                        }
                    }
                }
            }
            catch (Exception ex) { CaughtException(ex); }
        }

        public void LoadIncludes()
        {
            try
            {
                if (!Directory.Exists("includes"))
                {
                    MessageBox.Show("It appears you do not have an includes folder, download the SA-MP includes at www.sa-mp.com/download by getting the Windows server package.", "Includes Don't Exist.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    string[] files = Directory.GetFiles("includes", "*.inc");
                    string nameonly = null;
                    TreeNode include = null;
                    string newfile = null;
                    string[] contents = null;
                    string funcname = null;
                    TreeNode func = null;

                    foreach (string file in files)
                    {
                        nameonly = file.Substring(9);
                        include = new TreeNode();
                        include.Text = nameonly;
                        treeView1.Nodes.Add(include);
                        newfile = Directory.GetCurrentDirectory() + "\\" + file;
                        contents = File.ReadAllLines(newfile);
                        foreach (string line in contents)
                        {
                            if (line.Contains("native "))
                            {
                                funcname = line.Substring(line.IndexOf("native ") + 7);
                                int len = funcname.IndexOf("(");
                                if (len < 1) continue;
                                funcname = funcname.Substring(0, len);
                                if (funcname.Contains(":"))
                                {
                                    funcname = funcname.Substring(funcname.IndexOf(":") + 1);
                                }
                                func = new TreeNode();
                                func.Text = "native " + funcname;
                                include.Nodes.Add(func);
                            }
                            else if (line.Contains("stock "))
                            {
                                funcname = line.Substring(line.IndexOf("stock ") + 6);
                                int len = funcname.IndexOf("(");
                                if (len < 1) continue;
                                funcname = funcname.Substring(0, len);
                                if (funcname.Contains(":"))
                                {
                                    funcname = funcname.Substring(funcname.IndexOf(":") + 1);
                                }
                                func = new TreeNode();
                                func.Text = "stock " + funcname;
                                include.Nodes.Add(func);
                            }
                            // TO-DO: Plain functions.
                        }
                    }
                }
            }
            catch (Exception ex) { CaughtException(ex); }
        }

        public void FreeControl(Control control)
        {
            try
            {
                control.Dispose();
                while (control.Controls.Count > 0) control.Controls[0].Dispose();
                control = null;
            }
            catch (Exception ex) { CaughtException(ex); }
        }

        public void CaughtException(Exception ex)
        {
            MessageBox.Show("Exception:\n" + ex.Message + "\n______________\n" + ex.InnerException + "\n______________\n" + ex.Source + "\n______________\n" + ex.StackTrace + "\n______________\n" + ex.TargetSite + "\n______________\n" + ex.Data);
        }
    }
}
