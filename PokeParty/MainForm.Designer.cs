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

using System.Windows.Forms;

namespace PokeParty
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.grpFile = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnWatch = new System.Windows.Forms.Button();
            this.btnCheckSave = new System.Windows.Forms.Button();
            this.LowercaseOrLabel = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.grpTeamAppearance = new System.Windows.Forms.GroupBox();
            this.cbTeamOrientation = new System.Windows.Forms.ComboBox();
            this.lblTeamOrientation = new System.Windows.Forms.Label();
            this.btnRestoreDefaults = new System.Windows.Forms.Button();
            this.grpBadgeAppearence = new System.Windows.Forms.GroupBox();
            this.lblBadgeOrientation = new System.Windows.Forms.Label();
            this.cbBadgesOrientation = new System.Windows.Forms.ComboBox();
            this.cpTeamTextForeColor = new PokeParty.ColorPicker();
            this.cpTeamTextBackColor = new PokeParty.ColorPicker();
            this.cpTeamImageForeColor = new PokeParty.ColorPicker();
            this.cpTeamImageBackColor = new PokeParty.ColorPicker();
            this.cpBadgesForeColor = new PokeParty.ColorPicker();
            this.cpBadgesBackColor = new PokeParty.ColorPicker();
            this.grpFile.SuspendLayout();
            this.grpTeamAppearance.SuspendLayout();
            this.grpBadgeAppearence.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpFile
            // 
            this.grpFile.BackColor = System.Drawing.Color.Transparent;
            this.grpFile.Controls.Add(this.btnBrowse);
            this.grpFile.Controls.Add(this.txtFilePath);
            resources.ApplyResources(this.grpFile, "grpFile");
            this.grpFile.Name = "grpFile";
            this.grpFile.TabStop = false;
            // 
            // btnBrowse
            // 
            resources.ApplyResources(this.btnBrowse, "btnBrowse");
            this.btnBrowse.ForeColor = System.Drawing.Color.Black;
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowseDirectory_Click);
            // 
            // txtFilePath
            // 
            resources.ApplyResources(this.txtFilePath, "txtFilePath");
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.TabStop = false;
            this.txtFilePath.TextChanged += new System.EventHandler(this.txtFilePath_TextChanged);
            // 
            // btnWatch
            // 
            resources.ApplyResources(this.btnWatch, "btnWatch");
            this.btnWatch.Name = "btnWatch";
            this.btnWatch.UseVisualStyleBackColor = true;
            this.btnWatch.Click += new System.EventHandler(this.btnWatch_Click);
            // 
            // btnCheckSave
            // 
            resources.ApplyResources(this.btnCheckSave, "btnCheckSave");
            this.btnCheckSave.Name = "btnCheckSave";
            this.btnCheckSave.UseVisualStyleBackColor = true;
            this.btnCheckSave.Click += new System.EventHandler(this.btnCheckSave_Click);
            // 
            // LowercaseOrLabel
            // 
            resources.ApplyResources(this.LowercaseOrLabel, "LowercaseOrLabel");
            this.LowercaseOrLabel.Name = "LowercaseOrLabel";
            // 
            // lblAuthor
            // 
            resources.ApplyResources(this.lblAuthor, "lblAuthor");
            this.lblAuthor.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.lblAuthor.Name = "lblAuthor";
            // 
            // grpTeamAppearance
            // 
            this.grpTeamAppearance.Controls.Add(this.lblTeamOrientation);
            this.grpTeamAppearance.Controls.Add(this.cbTeamOrientation);
            this.grpTeamAppearance.Controls.Add(this.cpTeamTextForeColor);
            this.grpTeamAppearance.Controls.Add(this.cpTeamTextBackColor);
            this.grpTeamAppearance.Controls.Add(this.cpTeamImageForeColor);
            this.grpTeamAppearance.Controls.Add(this.cpTeamImageBackColor);
            resources.ApplyResources(this.grpTeamAppearance, "grpTeamAppearance");
            this.grpTeamAppearance.Name = "grpTeamAppearance";
            this.grpTeamAppearance.TabStop = false;
            // 
            // cbTeamOrientation
            // 
            this.cbTeamOrientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTeamOrientation.FormattingEnabled = true;
            this.cbTeamOrientation.Items.AddRange(new object[] {
            resources.GetString("cbTeamOrientation.Items"),
            resources.GetString("cbTeamOrientation.Items1")});
            resources.ApplyResources(this.cbTeamOrientation, "cbTeamOrientation");
            this.cbTeamOrientation.Name = "cbTeamOrientation";
            this.cbTeamOrientation.SelectedIndexChanged += new System.EventHandler(this.cbTeamOrientation_SelectedIndexChanged);
            // 
            // lblTeamOrientation
            // 
            resources.ApplyResources(this.lblTeamOrientation, "lblTeamOrientation");
            this.lblTeamOrientation.Name = "lblTeamOrientation";
            // 
            // btnRestoreDefaults
            // 
            resources.ApplyResources(this.btnRestoreDefaults, "btnRestoreDefaults");
            this.btnRestoreDefaults.Name = "btnRestoreDefaults";
            this.btnRestoreDefaults.UseVisualStyleBackColor = true;
            this.btnRestoreDefaults.Click += new System.EventHandler(this.btnRestoreDefaults_Click);
            // 
            // grpBadgeAppearence
            // 
            this.grpBadgeAppearence.Controls.Add(this.lblBadgeOrientation);
            this.grpBadgeAppearence.Controls.Add(this.cbBadgesOrientation);
            this.grpBadgeAppearence.Controls.Add(this.cpBadgesForeColor);
            this.grpBadgeAppearence.Controls.Add(this.cpBadgesBackColor);
            resources.ApplyResources(this.grpBadgeAppearence, "grpBadgeAppearence");
            this.grpBadgeAppearence.Name = "grpBadgeAppearence";
            this.grpBadgeAppearence.TabStop = false;
            // 
            // lblBadgeOrientation
            // 
            resources.ApplyResources(this.lblBadgeOrientation, "lblBadgeOrientation");
            this.lblBadgeOrientation.Name = "lblBadgeOrientation";
            // 
            // cbBadgesOrientation
            // 
            this.cbBadgesOrientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBadgesOrientation.FormattingEnabled = true;
            this.cbBadgesOrientation.Items.AddRange(new object[] {
            resources.GetString("cbBadgesOrientation.Items"),
            resources.GetString("cbBadgesOrientation.Items1")});
            resources.ApplyResources(this.cbBadgesOrientation, "cbBadgesOrientation");
            this.cbBadgesOrientation.Name = "cbBadgesOrientation";
            this.cbBadgesOrientation.SelectedIndexChanged += new System.EventHandler(this.cbBadgesOrientation_SelectedIndexChanged);
            // 
            // cpTeamTextForeColor
            // 
            this.cpTeamTextForeColor.Color = System.Drawing.Color.Black;
            resources.ApplyResources(this.cpTeamTextForeColor, "cpTeamTextForeColor");
            this.cpTeamTextForeColor.Name = "cpTeamTextForeColor";
            this.cpTeamTextForeColor.Title = "Text Foreground";
            this.cpTeamTextForeColor.ColorChanged += new PokeParty.ColorPicker.ColorChangedEventHandler(this.TeamAppearance_ColorChanged);
            // 
            // cpTeamTextBackColor
            // 
            this.cpTeamTextBackColor.Color = System.Drawing.Color.Lime;
            resources.ApplyResources(this.cpTeamTextBackColor, "cpTeamTextBackColor");
            this.cpTeamTextBackColor.Name = "cpTeamTextBackColor";
            this.cpTeamTextBackColor.Title = "Text Background";
            this.cpTeamTextBackColor.ColorChanged += new PokeParty.ColorPicker.ColorChangedEventHandler(this.TeamAppearance_ColorChanged);
            // 
            // cpTeamImageForeColor
            // 
            this.cpTeamImageForeColor.Color = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.cpTeamImageForeColor, "cpTeamImageForeColor");
            this.cpTeamImageForeColor.Name = "cpTeamImageForeColor";
            this.cpTeamImageForeColor.Title = "Image Foreground";
            this.cpTeamImageForeColor.ColorChanged += new PokeParty.ColorPicker.ColorChangedEventHandler(this.TeamAppearance_ColorChanged);
            // 
            // cpTeamImageBackColor
            // 
            this.cpTeamImageBackColor.Color = System.Drawing.Color.Black;
            resources.ApplyResources(this.cpTeamImageBackColor, "cpTeamImageBackColor");
            this.cpTeamImageBackColor.Name = "cpTeamImageBackColor";
            this.cpTeamImageBackColor.Title = "Image Background";
            this.cpTeamImageBackColor.ColorChanged += new PokeParty.ColorPicker.ColorChangedEventHandler(this.TeamAppearance_ColorChanged);
            // 
            // cpBadgesForeColor
            // 
            this.cpBadgesForeColor.Color = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.cpBadgesForeColor, "cpBadgesForeColor");
            this.cpBadgesForeColor.Name = "cpBadgesForeColor";
            this.cpBadgesForeColor.Title = "Foreground";
            // 
            // cpBadgesBackColor
            // 
            this.cpBadgesBackColor.Color = System.Drawing.Color.Black;
            resources.ApplyResources(this.cpBadgesBackColor, "cpBadgesBackColor");
            this.cpBadgesBackColor.Name = "cpBadgesBackColor";
            this.cpBadgesBackColor.Title = "Background";
            this.cpBadgesBackColor.ColorChanged += new PokeParty.ColorPicker.ColorChangedEventHandler(this.BadgesAppearance_ColorChanged);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpBadgeAppearence);
            this.Controls.Add(this.btnRestoreDefaults);
            this.Controls.Add(this.grpTeamAppearance);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.LowercaseOrLabel);
            this.Controls.Add(this.grpFile);
            this.Controls.Add(this.btnWatch);
            this.Controls.Add(this.btnCheckSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.grpFile.ResumeLayout(false);
            this.grpFile.PerformLayout();
            this.grpTeamAppearance.ResumeLayout(false);
            this.grpTeamAppearance.PerformLayout();
            this.grpBadgeAppearence.ResumeLayout(false);
            this.grpBadgeAppearence.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnWatch;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnCheckSave;
        private System.Windows.Forms.Label LowercaseOrLabel;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.GroupBox grpTeamAppearance;
        private ColorPicker cpTeamImageBackColor;
        private ColorPicker cpTeamTextForeColor;
        private ColorPicker cpTeamTextBackColor;
        private ColorPicker cpTeamImageForeColor;
        private Label lblTeamOrientation;
        private ComboBox cbTeamOrientation;
        private Button btnRestoreDefaults;
        private GroupBox grpBadgeAppearence;
        private Label lblBadgeOrientation;
        private ComboBox cbBadgesOrientation;
        private ColorPicker cpBadgesForeColor;
        private ColorPicker cpBadgesBackColor;
    }
}

