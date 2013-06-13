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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ScintillaNET;

namespace PAWNEdit
{
    public class Tabs
    {
        public class Tab_t
        {
            // Controls
            public TabPage page = new TabPage();
            public Scintilla scintilla = new Scintilla();
            public GroupBox groupbox = new GroupBox();
            public RichTextBox build = new RichTextBox();
            public Splitter split = new Splitter();

            // File info
            public string Filename;
            public string Filedir;
        };

        public List<Tab_t> tabs = new List<Tab_t>();

        private Form1 mainform;
        private Build build;
        private Settings settings;

        // Constructor
        public Tabs(Form1 form)
        {
            try
            {
                this.mainform = form;
            }
            catch(Exception ex) { form.CaughtException(ex); }
        }

        public void Update()
        {
            this.build = mainform.build;
            this.settings = mainform.settings;
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
                newtab.scintilla.Indentation.TabWidth = 4;

                // Style list - http://scintillanet.codeplex.com/SourceControl/changeset/view/99922#1941361
                newtab.scintilla.ConfigurationManager.Language = "cpp"; // Make this Pawn later.

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

                UpdateTabs();
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

        public void UpdateTabs()
        {
            try
            {
                foreach (Tab_t tab in tabs)
                {
                    if (tab.scintilla != null)
                    {
                        // Default
                        tab.scintilla.Styles[0].Font = settings.settings.defaulttext.font;
                        tab.scintilla.Styles[0].ForeColor = settings.settings.defaulttext.forecolor;
                        tab.scintilla.Styles[0].BackColor = settings.settings.defaulttext.backcolor;

                        // Line Comment
                        tab.scintilla.Styles[1].Font = settings.settings.linecomment.font;
                        tab.scintilla.Styles[1].ForeColor = settings.settings.linecomment.forecolor;
                        tab.scintilla.Styles[1].BackColor = settings.settings.linecomment.backcolor;

                        // Stream Comment
                        tab.scintilla.Styles[2].Font = settings.settings.streamcomment.font;
                        tab.scintilla.Styles[2].ForeColor = settings.settings.streamcomment.forecolor;
                        tab.scintilla.Styles[2].BackColor = settings.settings.streamcomment.backcolor;

                        // Document Comment
                        tab.scintilla.Styles[3].Font = settings.settings.doccomment.font;
                        tab.scintilla.Styles[3].ForeColor = settings.settings.doccomment.forecolor;
                        tab.scintilla.Styles[3].BackColor = settings.settings.doccomment.backcolor;

                        // Number
                        tab.scintilla.Styles[4].Font = settings.settings.number.font;
                        tab.scintilla.Styles[4].ForeColor = settings.settings.number.forecolor;
                        tab.scintilla.Styles[4].BackColor = settings.settings.number.backcolor;

                        // Keywords
                        tab.scintilla.Styles[5].Font = settings.settings.keyword.font;
                        tab.scintilla.Styles[5].ForeColor = settings.settings.keyword.forecolor;
                        tab.scintilla.Styles[5].BackColor = settings.settings.keyword.backcolor;

                        // String
                        tab.scintilla.Styles[6].Font = settings.settings.str.font;
                        tab.scintilla.Styles[6].ForeColor = settings.settings.str.forecolor;
                        tab.scintilla.Styles[6].BackColor = settings.settings.str.backcolor;

                        // Character
                        tab.scintilla.Styles[7].Font = settings.settings.character.font;
                        tab.scintilla.Styles[7].ForeColor = settings.settings.character.forecolor;
                        tab.scintilla.Styles[7].BackColor = settings.settings.character.backcolor;

                        // Preprocessor
                        tab.scintilla.Styles[9].Font = settings.settings.preprocessor.font;
                        tab.scintilla.Styles[9].ForeColor = settings.settings.preprocessor.forecolor;
                        tab.scintilla.Styles[9].BackColor = settings.settings.preprocessor.backcolor;

                        // Operator
                        tab.scintilla.Styles[10].Font = settings.settings.operat.font;
                        tab.scintilla.Styles[10].ForeColor = settings.settings.operat.forecolor;
                        tab.scintilla.Styles[10].BackColor = settings.settings.operat.backcolor;

                        // Identifier
                        tab.scintilla.Styles[11].Font = settings.settings.identifier.font;
                        tab.scintilla.Styles[11].ForeColor = settings.settings.identifier.forecolor;
                        tab.scintilla.Styles[11].BackColor = settings.settings.identifier.backcolor;

                        // String EOL
                        tab.scintilla.Styles[12].Font = settings.settings.stringeol.font;
                        tab.scintilla.Styles[12].ForeColor = settings.settings.stringeol.forecolor;
                        tab.scintilla.Styles[12].BackColor = settings.settings.stringeol.backcolor;

                        // Verbatim
                        tab.scintilla.Styles[13].Font = settings.settings.verbatim.font;
                        tab.scintilla.Styles[13].ForeColor = settings.settings.verbatim.forecolor;
                        tab.scintilla.Styles[13].BackColor = settings.settings.verbatim.backcolor;

                        // Regular Expression
                        tab.scintilla.Styles[14].Font = settings.settings.regex.font;
                        tab.scintilla.Styles[14].ForeColor = settings.settings.regex.forecolor;
                        tab.scintilla.Styles[14].BackColor = settings.settings.regex.backcolor;

                        // Comment line doc? or stream doc?
                        tab.scintilla.Styles[15].Font = settings.settings.doclinecomment.font;
                        tab.scintilla.Styles[15].ForeColor = settings.settings.doclinecomment.forecolor;
                        tab.scintilla.Styles[15].BackColor = settings.settings.doclinecomment.backcolor;

                        // Keywords 2
                        tab.scintilla.Styles[16].Font = settings.settings.keyword2.font;
                        tab.scintilla.Styles[16].ForeColor = settings.settings.keyword2.forecolor;
                        tab.scintilla.Styles[16].BackColor = settings.settings.keyword2.backcolor;
                    }
                }
            }
            catch (Exception ex) { mainform.CaughtException(ex); }
        }
    }
}
