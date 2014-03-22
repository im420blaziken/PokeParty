/**
 * This file is part of PokeParty.
 * 
 * Copyright (C) 2014 Ashlee Katzenbaer
 * All Rights Reserved.
 * 
 * @github im420blaziken
 *  
 * PokeParty is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * PokeParty is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with PokeParty.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokeParty
{
    public partial class MainForm : Form
    {
        TeamForm teamForm = null;
        BadgesForm badgesForm = null;

        public MainForm(string defaultPath)
        {
            InitializeComponent();

            txtFilePath.Text = (defaultPath == null) ? "" : defaultPath;
            if (txtFilePath.Text.Length == 0) txtFilePath.Text = Properties.Settings.Default.FilePath;

            // Other Settings
            this.cbTeamOrientation.SelectedIndex = Properties.Settings.Default.TeamOrientation;
            this.cpTeamImageBackColor.Color = Properties.Settings.Default.TeamImageBackColor;
            this.cpTeamImageForeColor.Color = Properties.Settings.Default.TeamImageForeColor;
            this.cpTeamTextBackColor.Color = Properties.Settings.Default.TeamTextBackColor;
            this.cpTeamTextForeColor.Color = Properties.Settings.Default.TeamTextForeColor;
            this.cbBadgesOrientation.SelectedIndex = Properties.Settings.Default.BadgesOrientation;
            this.cpBadgesBackColor.Color = Properties.Settings.Default.BadgesBackColor;
            this.cpBadgesForeColor.Color = Properties.Settings.Default.BadgesForeColor;
        }

        private void btnBrowseDirectory_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdiag = new OpenFileDialog();
            //fdiag.Filter = "VisualBoyAdvance Save State (*.sgm)|*.sgm|All files (*.*)|*.*";
            fdiag.Filter = Properties.Resources.OpenDialogFilter + " (*.sgm)|*.sgm";
            fdiag.DefaultExt = "sgm";
            if (txtFilePath.Text.Length > 0) fdiag.FileName = txtFilePath.Text;

            if (fdiag.ShowDialog() == DialogResult.OK) txtFilePath.Text = fdiag.FileName;
        }

        private void btnCheckSave_Click(object sender, EventArgs e)
        {
            ParseFile();
        }

        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            Thread.Sleep(4000);
            ParseFile();
        }

        FileSystemWatcher fsw;
        bool isWatching = false;
        string[] watchText = new string[] { 
            Properties.Resources.ResourceManager.GetString("StartWatch"), 
            Properties.Resources.ResourceManager.GetString("StopWatch") 
        };
        private void btnWatch_Click(object sender, EventArgs e)
        {
            if (!isWatching && ParseFile())
            {
                this.isWatching = true;

                this.fsw = new FileSystemWatcher(Path.GetDirectoryName(txtFilePath.Text));
                this.fsw.NotifyFilter = NotifyFilters.LastWrite;
                this.fsw.Filter = Path.GetFileName(txtFilePath.Text);
                this.fsw.Changed += new FileSystemEventHandler(OnFileChanged);
            }
            else if (isWatching) this.isWatching = false;

            if (this.fsw == null) return;

            this.txtFilePath.Enabled = this.btnCheckSave.Enabled = this.btnBrowse.Enabled = !(this.fsw.EnableRaisingEvents = isWatching);
            this.btnWatch.Text = watchText[(isWatching) ? 1 : 0];
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (txtFilePath.Text.Length > 0) ParseFile();
        }

        private object _parselock = new object();
        int save_count = 0;
        private bool ParseFile()
        {
            if (txtFilePath.Text.Length == 0 || !File.Exists(txtFilePath.Text))
            {
                MessageBox.Show(Properties.Resources.ResourceManager.GetString("ValidSaveStateMsg"));
                return false;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        if (teamForm == null)
                        {
                            teamForm = new TeamForm((Orientation)cbTeamOrientation.SelectedIndex);
                            teamForm.Show();
                        }
                        teamForm.ImageBackColor = this.cpTeamImageBackColor.Color;
                        teamForm.ImageForeColor = this.cpTeamImageForeColor.Color;
                        teamForm.TextBackColor = this.cpTeamTextBackColor.Color;
                        teamForm.TextForeColor = this.cpTeamTextForeColor.Color;
                    }
                    catch
                    {
                        MessageBox.Show(Properties.Resources.InvalidOrientation);
                        return;
                    }

                    try
                    {
                        if (badgesForm == null)
                        {
                            badgesForm = new BadgesForm((Orientation)cbBadgesOrientation.SelectedIndex);
                            badgesForm.Show();
                        }
                        badgesForm.BackColor = this.cpBadgesBackColor.Color;
                        badgesForm.ForeColor = this.cpBadgesForeColor.Color;
                    }
                    catch
                    {
                        MessageBox.Show(Properties.Resources.InvalidOrientation);
                        return;
                    }
                }));
            }
            else if (Program.IsMainThread)
            {
                try
                {
                    if (teamForm == null)
                    {
                        teamForm = new TeamForm((Orientation)cbTeamOrientation.SelectedIndex);
                        teamForm.Show();
                    }
                    teamForm.ImageBackColor = this.cpTeamImageBackColor.Color;
                    teamForm.ImageForeColor = this.cpTeamImageForeColor.Color;
                    teamForm.TextBackColor = this.cpTeamTextBackColor.Color;
                    teamForm.TextForeColor = this.cpTeamTextForeColor.Color;
                }
                catch
                {
                    MessageBox.Show(Properties.Resources.InvalidOrientation);
                    return false;
                }

                try
                {
                    if (badgesForm == null)
                    {
                        badgesForm = new BadgesForm((Orientation)cbBadgesOrientation.SelectedIndex);
                        badgesForm.Show();
                    }
                    badgesForm.BackColor = this.cpBadgesBackColor.Color;
                    badgesForm.ForeColor = this.cpBadgesForeColor.Color;
                }
                catch
                {
                    MessageBox.Show(Properties.Resources.InvalidOrientation);
                    return false;
                }
            }

            Thread parseThread = new Thread(() =>
            {
                if (Monitor.TryEnter(_parselock))
                {
                    try // Periodically backup the file
                    {
                        if (save_count++ % 5 == 0)
                        {
                            if (Directory.Exists("save_backups") == false) Directory.CreateDirectory("save_backups");
                            File.Copy(txtFilePath.Text, @"save_backups\" + DateTime.Now.ToLocalTime().ToString("yyyy_MM_dd_hh_mm_") + Path.GetFileName(txtFilePath.Text));
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Unable to backup the save.");
                    }

                    try
                    {

                        PokeSave save = new PokeSave(txtFilePath.Text);
                        try
                        {
                            List<Pokemon> pokes = save.Team;
                            teamForm.ShowTeam(pokes);
                        }
                        catch
                        {
                            Console.WriteLine("Unable to retrieve Team from file.");
                        }
                        try
                        {
                            byte badges = save.Badges;
                            badgesForm.ShowBadges(badges, save.CurrentGame);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Unable to show badges: " + e.Message);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Something went wrong when reading the file.");
                    }
                    finally
                    {
                        Monitor.Exit(_parselock);
                    }
                }
                else Console.WriteLine("Couldn't get parselock.");
            });
            parseThread.IsBackground = true;
            parseThread.Start();

            return true;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Count() != 1)
                {
                    MessageBox.Show(Properties.Resources.ResourceManager.GetString("PleaseOneFile"));
                    return;
                } // This validation might be unnecessary, but just in case?
                
                this.txtFilePath.Text = files.First();
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Count() == 1)
                {
                    if (Path.GetExtension(files.First()).Equals(".sgm"))
                    {
                        e.Effect = DragDropEffects.Move;
                        return;
                    }
                    else Console.WriteLine("Bad FileDrop Extension: " + Path.GetExtension(files.First()));
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void txtFilePath_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.FilePath = txtFilePath.Text;
            Properties.Settings.Default.Save();
        }

        private void TeamAppearance_ColorChanged(ColorPicker sender, EventArgs e)
        {
            if (teamForm != null)
            {
                switch (sender.Name) {
                    case "cpImageBackColor":
                        Properties.Settings.Default.TeamImageBackColor = teamForm.ImageBackColor = sender.Color;
                        break;
                    case "cpImageForeColor":
                        Properties.Settings.Default.TeamImageForeColor = teamForm.ImageForeColor = sender.Color;
                        break;
                    case "cpTextBackColor":
                        Properties.Settings.Default.TeamTextBackColor = teamForm.TextBackColor = sender.Color;
                        break;
                    case "cpTextForeColor":
                        Properties.Settings.Default.TeamTextForeColor = teamForm.TextForeColor = sender.Color;
                        break;
                }
                Properties.Settings.Default.Save();
            }
        }

        private void cbTeamOrientation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (teamForm != null) teamForm.Orientation = (Orientation)cbTeamOrientation.SelectedIndex;
                Properties.Settings.Default.TeamOrientation = cbTeamOrientation.SelectedIndex;
                Properties.Settings.Default.Save();
            }
            catch
            {
                MessageBox.Show(Properties.Resources.InvalidOrientation);
            }
        }

        private void BadgesAppearance_ColorChanged(ColorPicker sender, EventArgs e)
        {
            if (badgesForm != null)
            {
                switch (sender.Name)
                {
                    case "cpBadgesBackColor":
                        Properties.Settings.Default.BadgesBackColor = badgesForm.BackColor = sender.Color;
                        break;
                    case "cpBadgesForeColor":
                        Properties.Settings.Default.BadgesForeColor = badgesForm.ForeColor = sender.Color;
                        break;
                }
                Properties.Settings.Default.Save();
            }
        }

        private void cbBadgesOrientation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (badgesForm != null) badgesForm.Orientation = (Orientation)cbBadgesOrientation.SelectedIndex;
                Properties.Settings.Default.BadgesOrientation = cbBadgesOrientation.SelectedIndex;
                Properties.Settings.Default.Save();
            }
            catch
            {
                MessageBox.Show(Properties.Resources.InvalidOrientation);
            }
        }

        private void btnRestoreDefaults_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.FilePath = txtFilePath.Text = "";
            Properties.Settings.Default.TeamOrientation = cbTeamOrientation.SelectedIndex = 0;
            Properties.Settings.Default.TeamImageBackColor = cpTeamImageBackColor.Color = Color.Black;
            Properties.Settings.Default.TeamImageForeColor = cpTeamImageForeColor.Color = Color.Transparent;
            Properties.Settings.Default.TeamTextBackColor = cpTeamTextBackColor.Color = Color.Lime;
            Properties.Settings.Default.TeamTextForeColor = cpTeamTextForeColor.Color = Color.Black;
            Properties.Settings.Default.BadgesOrientation = cbTeamOrientation.SelectedIndex = 0;
            Properties.Settings.Default.BadgesBackColor = cpBadgesBackColor.Color = Color.Lime;
            Properties.Settings.Default.BadgesForeColor = cpBadgesForeColor.Color = Color.Transparent;
            Properties.Settings.Default.Save();
        }

    }
}
