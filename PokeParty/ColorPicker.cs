using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokeParty
{
    public partial class ColorPicker : UserControl
    {
        public delegate void ColorChangedEventHandler(ColorPicker sender, EventArgs e);

        [Browsable(true)]
        [Category("Appearance")]
        public event ColorChangedEventHandler ColorChanged = delegate { };

        public ColorPicker()
        {
            InitializeComponent();
        }

        [Category("Appearance")]
        [Description("Gets or sets the name of the picker")]
        public string Title
        {
            get { return this.PickerTitle.Text; }
            set { this.PickerTitle.Text = value; }
        }

        [Category("Appearance")]
        [Description("Gets or sets the default color")]
        public Color Color
        {
            get { return this.ColorPreview.BackColor; }
            set { this.ColorPreview.BackColor = value; }
        }

        private void ChooseColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog colordiag = new ColorDialog();
            colordiag.Color = this.Color;

            if (colordiag.ShowDialog() == DialogResult.OK)
            {
                this.Color = colordiag.Color;
            }
        }

        private void ColorPreview_BackColorChanged(object sender, EventArgs e)
        {
            ColorChanged(this, e);
        }
    }
}
