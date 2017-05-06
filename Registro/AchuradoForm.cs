using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RegistroPerforacion
{
    public partial class AchuradoForm : Form
    {
        public Bitmap AchuradoSelected { get; set; }
        public Bitmap AchuradoInitial { get; set; }

        public Dictionary<string, string> Colores;

        public AchuradoForm()
        {
            InitializeComponent();
            openAchurados.InitialDirectory = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Achurados");
        }

        private void AchuradoForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = AchuradoInitial ?? new Bitmap(38, 38);
            pictureBox1.Height = pictureBox1.Image.Height;
            string[] nombres = Colores.Keys.ToArray();
            comboBox1.Items.AddRange(nombres);
            comboBox1.Text = nombres[0];
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            AchuradoSelected = (Bitmap)pictureBox1.Image;
            this.DialogResult = DialogResult.OK;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void BtnColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                comboBox1.Text = "Personalizado";
                PaintAchurado();
            }
        }

        private void PaintAchurado()
        {
            var orig = new Bitmap(pictureBox1.Image);
            for (var x = 0; x < orig.Width; x++)
                for (var y = 0; y < orig.Height; y++)
                {
                    var pixel = orig.GetPixel(x, y);
                    if (pixel.R != 255 || pixel.G != 255 && pixel.B != 255)
                        orig.SetPixel(x, y, colorDialog1.Color);
                }
            pictureBox1.Image = orig;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (openAchurados.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openAchurados.FileName);
                pictureBox1.Height = pictureBox1.Image.Height;
                PaintAchurado();
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (!comboBox1.Text.Equals("Personalizado"))
            {
                colorDialog1.Color = ColorTranslator.FromHtml(Colores[comboBox1.Text]);
                PaintAchurado();
            }
        }
    }
}