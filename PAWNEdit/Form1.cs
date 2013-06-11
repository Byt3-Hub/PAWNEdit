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

        // Functions
        public void UnIndentScintillaText(ScintillaNET.Scintilla scintilla)
        {
            try
            {
                string text = null;
                for (int lineind = 0; lineind < scintilla.Lines.Count; lineind++)
                {
                    text = scintilla.Lines[lineind].Text;
                    if (text.Length > 0)
                    {
                        // Remove new lines for the local variable.
                        if (text.EndsWith("\r\n")) text = text.Substring(0, text.Length - 2);
                        if (text.EndsWith("\n")) text = text.Substring(0, text.Length - 1);

                        foreach (char ch in text)
                        {
                            if (text.Length > 0)
                            {
                                if (ch != '\t' && ch != ' ') break;
                                text = text.Remove(0, 1);
                            }
                        }
                        scintilla.Lines[lineind].Text = text;
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
                for (int lineind = 0; lineind < scintilla.Lines.Count; lineind++)
                {
                    text = scintilla.Lines[lineind].Text;
                    if (text.Length > 0)
                    {
                        // Remove excess spaces and new lines for the local variable.
                        if (text.EndsWith("\r\n")) text = text.Substring(0, text.Length - 2);
                        if (text.EndsWith("\n")) text = text.Substring(0, text.Length - 1);
                        while (text.EndsWith(" ") || text.EndsWith("\t"))
                        {
                            if (text.Length < 1) break;
                            text = text.Substring(0, text.Length - 1);
                        }

                        for (int i = 0; i < tabind; i++)
                        {
                            text = text.Insert(0, "\t");
                        }

                        tabincdec = 0;
                        foreach (char ch in text)
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
                            }
                            if (removetab)
                            {
                                text = text.Remove(text.IndexOf("\t"), 1);
                            }
                        }
                        scintilla.Lines[lineind].Text = text;
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
