using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ScintillaNET;

namespace PAWNEdit
{
    public class Tabs
    {
        public struct Tab_t
        {
            // Controls
            public TabPage page;
            public Scintilla scintilla;
            public GroupBox groupbox;
            public RichTextBox build;
            public Splitter split;

            // File info
            public string Filename;
            public string Filedir;
        };

        public List<Tab_t> tabs = new List<Tab_t>();

        private Form1 mainform;
        private Build builder;

        // Constructor
        public Tabs(Form1 form)
        {
            try
            {
                this.mainform = form;
                this.builder = new Build(form);
            }
            catch(Exception ex) { form.CaughtException(ex); }
        }

        public void NewTab()
        {
            try
            {
                string title = (mainform.tabControl1.TabPages.Count + 1).ToString();
                Tab_t newtab = new Tab_t();
                newtab.build = new RichTextBox();
                newtab.groupbox = new GroupBox();
                newtab.page = new TabPage();
                newtab.scintilla = new Scintilla();
                newtab.split = new Splitter();

                Random randomizer = new Random();

                // build
                newtab.build.Dock = System.Windows.Forms.DockStyle.Fill;
                newtab.build.Location = new System.Drawing.Point(3, 16);
                newtab.build.Name = "buildRichTextBox1" + title;
                newtab.build.Size = new System.Drawing.Size(561, 81);
                newtab.build.TabIndex = 0;
                newtab.build.Text = "";

                // groupbox
                newtab.groupbox.Controls.Add(newtab.build);
                newtab.groupbox.Dock = System.Windows.Forms.DockStyle.Bottom;
                newtab.groupbox.Location = new System.Drawing.Point(3, 362);
                newtab.groupbox.Name = "buildGroupBox" + title;
                newtab.groupbox.Size = new System.Drawing.Size(567, 100);
                newtab.groupbox.TabIndex = 0;
                newtab.groupbox.TabStop = false;
                newtab.groupbox.Text = "Build Output";
                newtab.groupbox.ForeColor = Color.Black;

                // page 
                newtab.page.Controls.Add(newtab.scintilla);
                newtab.page.Controls.Add(newtab.split);
                newtab.page.Controls.Add(newtab.groupbox);
                newtab.page.Location = new System.Drawing.Point(4, 22);
                newtab.page.Name = "buildTabPage" + title;
                newtab.page.Padding = new System.Windows.Forms.Padding(3);
                newtab.page.Size = new System.Drawing.Size(573, 465);
                newtab.page.TabIndex = 0;
                newtab.page.Text = "buildTabPage" + title;

                // scintilla
                newtab.scintilla.Dock = System.Windows.Forms.DockStyle.Fill;
                newtab.scintilla.Location = new System.Drawing.Point(3, 3);
                newtab.scintilla.Name = "buildScintilla" + title;
                newtab.scintilla.Size = new System.Drawing.Size(567, 356);
                newtab.scintilla.TabIndex = 2;

                // split
                newtab.split.Dock = System.Windows.Forms.DockStyle.Bottom;
                newtab.split.Location = new System.Drawing.Point(3, 359);
                newtab.split.Name = "buildSplitter" + title;
                newtab.split.Size = new System.Drawing.Size(567, 6);
                newtab.split.TabIndex = 1;
                newtab.split.TabStop = false;

                mainform.tabControl1.TabPages.Add(newtab.page);

                newtab.Filename = "bare.pwn";
                newtab.Filedir = "C:\\Users\\Byt3\\Desktop\\SA-MP\\Test\\gamemodes";

                tabs.Add(newtab);

                mainform.tabControl1.SelectedIndex++;
            }
            catch (Exception ex) { mainform.CaughtException(ex); }
        }


        public void CloseTab(TabPage page)
        {
            try
            {
                foreach (Tab_t tab in tabs)
                {
                    if (tab.page == page)
                    {
                        Tab_t rtab = tab;
                        mainform.FreeControl(rtab.build);
                        mainform.FreeControl(rtab.groupbox);
                        mainform.FreeControl(rtab.scintilla);
                        mainform.FreeControl(rtab.split);
                        mainform.FreeControl(rtab.page);
                        tabs.Remove(rtab);
                        break;
                    }
                }
            }
            catch (Exception ex) { mainform.CaughtException(ex); }
        }
    }
}
