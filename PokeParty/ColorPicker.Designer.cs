namespace PokeParty
{
    partial class ColorPicker
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PickerTitle = new System.Windows.Forms.GroupBox();
            this.ColorPreview = new System.Windows.Forms.PictureBox();
            this.ChooseColorButton = new System.Windows.Forms.Button();
            this.PickerTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ColorPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // PickerTitle
            // 
            this.PickerTitle.Controls.Add(this.ColorPreview);
            this.PickerTitle.Controls.Add(this.ChooseColorButton);
            this.PickerTitle.Location = new System.Drawing.Point(0, 0);
            this.PickerTitle.Name = "PickerTitle";
            this.PickerTitle.Size = new System.Drawing.Size(122, 50);
            this.PickerTitle.TabIndex = 0;
            this.PickerTitle.TabStop = false;
            this.PickerTitle.Text = "Name";
            // 
            // ColorPreview
            // 
            this.ColorPreview.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ColorPreview.Location = new System.Drawing.Point(6, 21);
            this.ColorPreview.Name = "ColorPreview";
            this.ColorPreview.Size = new System.Drawing.Size(24, 19);
            this.ColorPreview.TabIndex = 3;
            this.ColorPreview.TabStop = false;
            this.ColorPreview.BackColorChanged += new System.EventHandler(this.ColorPreview_BackColorChanged);
            // 
            // ChooseColorButton
            // 
            this.ChooseColorButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ChooseColorButton.Location = new System.Drawing.Point(36, 19);
            this.ChooseColorButton.Name = "ChooseColorButton";
            this.ChooseColorButton.Size = new System.Drawing.Size(80, 23);
            this.ChooseColorButton.TabIndex = 0;
            this.ChooseColorButton.Text = "Choose Color";
            this.ChooseColorButton.UseVisualStyleBackColor = true;
            this.ChooseColorButton.Click += new System.EventHandler(this.ChooseColorButton_Click);
            // 
            // ColorPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PickerTitle);
            this.Name = "ColorPicker";
            this.Size = new System.Drawing.Size(122, 50);
            this.PickerTitle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ColorPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox PickerTitle;
        private System.Windows.Forms.PictureBox ColorPreview;
        private System.Windows.Forms.Button ChooseColorButton;
    }
}
