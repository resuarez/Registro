using RegistroPerforacion.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace RegistroPerforacion
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //DefaultConfig
            PathDefaultConfig = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "config.xml");
            OpenConfiguration();
            //initialisation Registro
            CurrentRegistro = new Registro()
            {
                Logo = DefaultConfig.LogoPorDefecto,
                Sondeos = new List<Sondeo> { new Sondeo() }
            };
            InitializeRegistro();
        }

        public Registro CurrentRegistro { get; set; }
        public Configuracion DefaultConfig { get; set; }
        public string PathDefaultConfig { get; set; }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void AddGridView1_Click(object sender, EventArgs e)
        {
            DefaultValueEnsayos(dataGridView1.Rows.Count);
        }

        private void AddGridView2_Click(object sender, EventArgs e)
        {
            DefaultValueMuestras();
        }

        private void AddGridView3_Click(object sender, EventArgs e)
        {
            DefaultValueFreaticos(dataGridView3.Rows.Count);
        }

        private void borrarFilaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dataGridView = (DataGridView)contextMenuStrip1.SourceControl;
            if (MessageBox.Show("¿Está seguro de borrar?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (DataGridViewRow item in dataGridView.SelectedRows)
                {
                    dataGridView.Rows.RemoveAt(item.Index);
                }
            }
        }

        private void coloresDeAchuradosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var coloresAchuradoForm1 = new ColoresAchuradoForm() { ColoresdeAchurados = DefaultConfig.ColoresdeAchurados };
            if (coloresAchuradoForm1.ShowDialog() == DialogResult.OK)
            {
                DefaultConfig.ColoresdeAchurados = coloresAchuradoForm1.ColoresdeAchurados;
                SaveConfiguration();
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                var prof1 = dataGridView1.Rows[e.RowIndex].Cells["ProfundidadArribaColumn"];
                var prof2 = dataGridView1.Rows[e.RowIndex].Cells["ProfundidadAbajoColumn"];
                if (prof1.Value == null)
                {
                    double prof0 = 0;
                    if (e.RowIndex > 0)
                    {
                        prof0 = (double)(dataGridView1.Rows[e.RowIndex - 1].Cells["ProfundidadAbajoColumn"].Value ?? 0D) + 1D;
                    }
                    prof1.Value = prof0;
                }
                var tipo = dataGridView1.Rows[e.RowIndex].Cells["TipoColumn"].Value.ToString();
                prof2.Value = (double)prof1.Value + DefaultConfig.TiposdeEnsayos.First(x => x.ShortName.Equals(tipo)).Longitud;
            }
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                {
                    cell.Value = cell.DefaultNewRowValue;
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex].Name.Equals("AchuradoColumn") && e.RowIndex >= 0)
            {
                var achuradoForm1 = new AchuradoForm();
                achuradoForm1.AchuradoInitial = (Bitmap)dataGridView2.Rows[e.RowIndex].Cells["AchuradoColumn"].Value;
                achuradoForm1.Colores = DefaultConfig.ColoresdeAchurados.Select(x => new { x.Name, x.HexCode }).ToDictionary(x => x.Name, x => x.HexCode);
                if (achuradoForm1.ShowDialog() == DialogResult.OK)
                {
                    dataGridView2.Rows[e.RowIndex].Cells["AchuradoColumn"].Value = achuradoForm1.AchuradoSelected;
                }
            }
        }

        private void DefaultValueEnsayos(int rowIndex)
        {
            var index = (int)indexSondeo.Value - 1;
            ensayoBindingSource.Add(new Ensayo());
            dataGridView1.Refresh();
        }

        private void DefaultValueFreaticos(int rowIndex)
        {
            var index = (int)indexSondeo.Value - 1;
            freaticoBindingSource.Add(new Freatico());
            dataGridView3.Refresh();
        }

        private void DefaultValueMuestras()
        {
            var rowIndex = dataGridView2.Rows.Count;
            var index = (int)indexSondeo.Value - 1;
            var bmp = new Bitmap(38, 38);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            muestraBindingSource.Add(new Muestra()
            {
                Achurado = bmp
            });
            dataGridView2.Refresh();
        }

        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SaveFile();
            }
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(saveFileDialog1.FileName) && saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            SaveFile();
        }

        private void indexSondeo_ValueChanged(object sender, EventArgs e)
        {
            InitializeSondeo((int)indexSondeo.Value);
        }

        private void InitializeRegistro()
        {
            //initialisation components
            logo.Image = CurrentRegistro.Logo;
            proyecto.DataBindings.Clear();
            proyecto.DataBindings.Add("Text", CurrentRegistro, "Proyecto");
            InitializeSondeo(1);
            indexSondeo.Value = 1;
            totalSondeos.Value = CurrentRegistro.Sondeos.Count;
        }

        private void InitializeSondeo(int i)
        {
            var index = i - 1;
            //nombre
            nombre.DataBindings.Clear();
            nombre.DataBindings.Add("Text", CurrentRegistro.Sondeos[index], "Nombre", true);
            //fechaInicio
            fechaInicio.DataBindings.Clear();
            fechaInicio.DataBindings.Add("Value", CurrentRegistro.Sondeos[index], "FechaInicio", true);
            //fechaFinal
            fechaFinal.DataBindings.Clear();
            fechaFinal.DataBindings.Add("Value", CurrentRegistro.Sondeos[index], "FechaFinal", true);
            //profundidad
            profundidad.DataBindings.Clear();
            profundidad.DataBindings.Add("Text", CurrentRegistro.Sondeos[index], "Profundidad", true);
            //Ensayos
            ensayoBindingSource.DataSource = CurrentRegistro.Sondeos[index].Ensayos;
            //Muestras
            muestraBindingSource.DataSource = CurrentRegistro.Sondeos[index].Muestras;
            //Freaticos
            freaticoBindingSource.DataSource = CurrentRegistro.Sondeos[index].Freaticos;
        }

        private void logoPorDefectoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenConfiguration();
            var logoDialog = new OpenFileDialog();
            logoDialog.FileName = "";
            logoDialog.Filter = "Archivo de Imagenes (*.bmp;*.jpg;*.png; *.wmf)|*.bmp;*.jpg;*.png;*.wmf|Todos los archivos (*.*)|*.*";
            if (logoDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    logo.Image = DefaultConfig.LogoPorDefecto = CurrentRegistro.Logo = new Bitmap(logoDialog.FileName);
                }
                catch
                {
                    // ignored
                }
                SaveConfiguration();
            }
        }

        private void OpenConfiguration()
        {
            // valores por defecto
            DefaultConfig = new Configuracion
            {
                TiposdeEnsayos = new List<TipoEnsayo> {
                        new TipoEnsayo { ShortName = "SS", Longitud = 0.45, LongName = "Penetración Estándar" },
                        new TipoEnsayo { ShortName = "SH", Longitud = 0.5, LongName = "Shelby" }
                    },
                ColoresdeAchurados = new List<ColoresAchurado> {
                        new ColoresAchurado{ Name="Negro", HexCode="#000000"},
                        new ColoresAchurado{ Name="Ocre Oscuro", HexCode="#8a5117"},
                        new ColoresAchurado{ Name="Ocre", HexCode="#cc7722"},
                        new ColoresAchurado{ Name="Ocre Claro", HexCode="#e39d57"},
                        new ColoresAchurado{ Name="Habano", HexCode="#f5f5dc"},
                        new ColoresAchurado{ Name="Gris Habano", HexCode="#d3d3d3"},
                        new ColoresAchurado{ Name="Blanco", HexCode="#ffffff"}
                    }
            };

            // Read configuration
            if (File.Exists(PathDefaultConfig))
            {
                var reader = new XmlSerializer(typeof(Configuracion));
                using (var file = new StreamReader(PathDefaultConfig))
                {
                    try
                    {
                        DefaultConfig = (Configuracion)reader.Deserialize(file);
                    }
                    catch
                    {
                        MessageBox.Show("Error leyendo el config.xml\nSe va utilizar valores por defecto");
                    }
                }
            }

            // initialize Objects
            if (CurrentRegistro != null)
            {
                CurrentRegistro.Logo = DefaultConfig.LogoPorDefecto;
            }
            TipoColumn.Items.AddRange(DefaultConfig.TiposdeEnsayos.Select(x => x.ShortName).ToArray());
        }

        private void OpenFile()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var reader = new XmlSerializer(typeof(Registro));
                using (var file = new StreamReader(openFileDialog1.FileName))
                    CurrentRegistro = (Registro)reader.Deserialize(file);
                InitializeRegistro();
                saveFileDialog1.FileName = openFileDialog1.FileName;
            }
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SaveConfiguration()
        {
            var writer = new XmlSerializer(typeof(Configuracion));
            using (var file = File.Create(PathDefaultConfig))
                writer.Serialize(file, DefaultConfig);
        }

        private void SaveFile()
        {
            var writer = new XmlSerializer(typeof(Registro));
            using (var file = File.Create(saveFileDialog1.FileName))
                writer.Serialize(file, CurrentRegistro);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(saveFileDialog1.FileName) && saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            SaveFile();
        }

        private void tipoDeEnsayosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tipoEnsayoForm1 = new TipoEnsayoForm() { TiposdeEnsayos = DefaultConfig.TiposdeEnsayos };
            if (tipoEnsayoForm1.ShowDialog() == DialogResult.OK)
            {
                TipoColumn.Items.Clear();
                TipoColumn.Items.AddRange(tipoEnsayoForm1.TiposdeEnsayos.Select(x => x.ShortName).ToArray());
                DefaultConfig.TiposdeEnsayos = tipoEnsayoForm1.TiposdeEnsayos;
                SaveConfiguration();
            }
        }

        private void totalSondeos_ValueChanged(object sender, EventArgs e)
        {
            indexSondeo.Maximum = totalSondeos.Value;
            while (CurrentRegistro.Sondeos.Count < totalSondeos.Value)
            {
                CurrentRegistro.Sondeos.Add(new Sondeo());
            }
        }
    }
}