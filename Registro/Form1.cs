﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using RegistroPerforacion.Entities;

namespace RegistroPerforacion
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            // Default Paths
            AppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            PathDefaultConfig = Path.Combine(AppPath, "config.xml");

            // Initialize
            Modified = false;
            InitializeComponent();
            //DefaultConfig
            OpenConfiguration();
            NewRegistro();
        }

        public static string AppPath { get; set; }
        public static bool Modified { get; set; }

        public static Registro CurrentRegistro { get; set; }
        public static Configuracion DefaultConfig { get; set; }
        public static string PathDefaultConfig { get; set; }

        private void AbrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void AcercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new AboutBox1();
            about.ShowDialog();
        }

        private void AddGridView1_Click(object sender, EventArgs e)
        {
            Modified = true;
            DefaultValueEnsayos();
        }

        private void AddGridView2_Click(object sender, EventArgs e)
        {
            Modified = true;
            DefaultValueMuestras();
        }

        private void AddGridView3_Click(object sender, EventArgs e)
        {
            Modified = true;
            DefaultValueFreaticos();
        }

        private void BorrarFilaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dataGridView = (DataGridView)contextMenuStrip1.SourceControl;
            if (MessageBox.Show(@"¿Está seguro de borrar?", "", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            Modified = true;
            foreach (DataGridViewRow item in dataGridView.SelectedRows)
                dataGridView.Rows.RemoveAt(item.Index);
        }

        private void ColoresDeAchuradosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var coloresAchuradoForm1 = new ColoresAchuradoForm { ColoresdeAchurados = DefaultConfig.ColoresdeAchurados };
            if (coloresAchuradoForm1.ShowDialog() != DialogResult.OK) return;
            DefaultConfig.ColoresdeAchurados = coloresAchuradoForm1.ColoresdeAchurados;
            SaveConfiguration();
        }

        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                SelectNextControl((Control)sender, true, true, true, true);
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            Modified = true;
            var column = dataGridView1.Columns[e.ColumnIndex];
            switch (column.Name)
            {
                case "TipoColumn":
                    {
                        var prof1 = dataGridView1.Rows[e.RowIndex].Cells["ProfundidadArribaColumn"];
                        if (prof1.Value == null)
                        {
                            double prof0 = 0;
                            if (e.RowIndex > 0)
                                prof0 = (double)(dataGridView1.Rows[e.RowIndex - 1].Cells["ProfundidadAbajoColumn"]
                                                      .Value ??
                                                  0D) + 1D;
                            prof1.Value = prof0;
                        }
                    }
                    break;
                case "ProfundidadArribaColumn":
                    {
                        var prof1 = dataGridView1.Rows[e.RowIndex].Cells["ProfundidadArribaColumn"];
                        var prof2 = dataGridView1.Rows[e.RowIndex].Cells["ProfundidadAbajoColumn"];
                        var tipo = dataGridView1.Rows[e.RowIndex].Cells["TipoColumn"].Value.ToString();
                        prof2.Value = (double)prof1.Value + DefaultConfig.TiposdeEnsayos
                                          .First(x => x.ShortName.Equals(tipo)).Longitud;
                    }
                    break;
                case "ProfundidadAbajoColumn":
                    {
                        var prof1 = dataGridView1.Rows[e.RowIndex].Cells["ProfundidadArribaColumn"];
                        var prof2 = dataGridView1.Rows[e.RowIndex].Cells["ProfundidadAbajoColumn"];
                        var longitud = dataGridView1.Rows[e.RowIndex].Cells["LongitudPerforadaColumn"];
                        if (prof1.Value == null || prof2.Value == null) return;
                        longitud.Value = (double)prof2.Value - (double)prof1.Value;
                    }
                    break;
                case "LongitudRecobradaColumn":
                    {
                        var longitudPerforada = (double)dataGridView1.Rows[e.RowIndex].Cells["LongitudPerforadaColumn"]
                            .Value;
                        var longitudRecobrada = (double)dataGridView1.Rows[e.RowIndex].Cells["LongitudRecobradaColumn"]
                            .Value;
                        var recuperacion = dataGridView1.Rows[e.RowIndex].Cells["RecuperacionColumn"];
                        recuperacion.Value = longitudRecobrada / longitudPerforada;
                        break;
                    }

                case "N1Column":
                    {
                        var n1 = dataGridView1.Rows[e.RowIndex].Cells["N1Column"];
                        var p1 = dataGridView1.Rows[e.RowIndex].Cells["P1Column"];
                        var r1 = dataGridView1.Rows[e.RowIndex].Cells["R1Column"];
                        if (n1.Value == null || p1.Value != null) return;
                        p1.Value = 6;
                        r1.Value = false;
                    }
                    break;
                case "R1Column":
                    {
                        var n1 = dataGridView1.Rows[e.RowIndex].Cells["N1Column"];
                        var p1 = dataGridView1.Rows[e.RowIndex].Cells["P1Column"];
                        var r1 = dataGridView1.Rows[e.RowIndex].Cells["R1Column"];
                        EnableCell(n1, !(bool)r1.Value);
                        EnableCell(p1, !(bool)r1.Value);
                    }
                    break;
                case "N2Column":
                    {
                        var n2 = dataGridView1.Rows[e.RowIndex].Cells["N2Column"];
                        var p2 = dataGridView1.Rows[e.RowIndex].Cells["P2Column"];
                        var r2 = dataGridView1.Rows[e.RowIndex].Cells["R2Column"];
                        if (n2.Value == null || p2.Value != null) return;
                        p2.Value = 6;
                        r2.Value = false;
                    }
                    break;
                case "R2Column":
                    {
                        var n2 = dataGridView1.Rows[e.RowIndex].Cells["N2Column"];
                        var p2 = dataGridView1.Rows[e.RowIndex].Cells["P2Column"];
                        var r2 = dataGridView1.Rows[e.RowIndex].Cells["R2Column"];
                        EnableCell(n2, !(bool)r2.Value);
                        EnableCell(p2, !(bool)r2.Value);
                    }
                    break;
                case "N3Column":
                    {
                        var n3 = dataGridView1.Rows[e.RowIndex].Cells["N3Column"];
                        var p3 = dataGridView1.Rows[e.RowIndex].Cells["P3Column"];
                        var r3 = dataGridView1.Rows[e.RowIndex].Cells["R3Column"];
                        if (n3.Value == null || p3.Value != null) return;
                        p3.Value = 6;
                        r3.Value = false;
                    }
                    break;
                case "R3Column":
                    {
                        var n3 = dataGridView1.Rows[e.RowIndex].Cells["N3Column"];
                        var p3 = dataGridView1.Rows[e.RowIndex].Cells["P3Column"];
                        var r3 = dataGridView1.Rows[e.RowIndex].Cells["R3Column"];
                        EnableCell(n3, !(bool)r3.Value);
                        EnableCell(p3, !(bool)r3.Value);
                    }
                    break;
            }
        }

        private void DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (!senderGrid.Columns[e.ColumnIndex].Name.Equals("AchuradoColumn") || e.RowIndex < 0) return;
            var achuradoForm1 = new AchuradoForm
            {
                AchuradoInitial = (Bitmap)dataGridView2.Rows[e.RowIndex].Cells["AchuradoColumn"].Value,
                Colores = DefaultConfig.ColoresdeAchurados.Select(x => new { x.Name, x.HexCode })
                    .ToDictionary(x => x.Name, x => x.HexCode)
            };
            if (achuradoForm1.ShowDialog() == DialogResult.OK)
                dataGridView2.Rows[e.RowIndex].Cells["AchuradoColumn"].Value = achuradoForm1.AchuradoSelected;
        }

        private void DataGridView2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.F2 && e.KeyCode != Keys.Space) return;
            var cell = dataGridView2.CurrentCell;
            if (dataGridView2.Columns[cell.ColumnIndex].Name.Equals("AchuradoColumn"))
                DataGridView2_CellContentClick(dataGridView2,
                    new DataGridViewCellContextMenuStripNeededEventArgs(cell.ColumnIndex, cell.RowIndex));
        }

        private void DefaultValueEnsayos()
        {
            ensayoBindingSource.Add(new Ensayo());
            dataGridView1.Refresh();
        }

        private void DefaultValueFreaticos()
        {
            freaticoBindingSource.Add(new Freatico());
            dataGridView3.Refresh();
        }

        private void DefaultValueMuestras()
        {
            var bmp = new Bitmap(38, 38);
            var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            muestraBindingSource.Add(new Muestra
            {
                Achurado = bmp
            });
            dataGridView2.Refresh();
        }

        /// <summary>
        ///     Toggles the "enabled" status of a cell in a DataGridView. There is no native
        ///     support for disabling a cell, hence the need for this method. The disabled state
        ///     means that the cell is read-only and grayed out.
        /// </summary>
        /// <param name="dc">Cell to enable/disable</param>
        /// <param name="enabled">Whether the cell is enabled or disabled</param>
        private static void EnableCell(DataGridViewCell dc, bool enabled)
        {
            //toggle read-only state
            dc.ReadOnly = !enabled;
            if (enabled)
            {
                //restore cell style to the default value
                dc.Style.BackColor = dc.OwningColumn.DefaultCellStyle.BackColor;
                dc.Style.ForeColor = dc.OwningColumn.DefaultCellStyle.ForeColor;
            }
            else
            {
                //gray out the cell
                dc.Style.BackColor = Color.LightGray;
                dc.Style.ForeColor = Color.DarkGray;
            }
        }

        private void GuardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                SaveFile();
        }

        private void GuardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(saveFileDialog1.FileName) && saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            SaveFile();
        }

        private void IndexSondeo_ValueChanged(object sender, EventArgs e)
        {
            InitializeSondeo((int)IndexSondeo.Value);
        }

        private void InformationChanged(object sender, EventArgs e)
        {
            Modified = true;
        }

        private void InitializeRegistro()
        {
            //initialisation components
            logo.Image = CurrentRegistro.Logo;
            Proyecto.DataBindings.Clear();
            Proyecto.DataBindings.Add("Text", CurrentRegistro, "Proyecto");
            InitializeSondeo(1);
            IndexSondeo.Value = 1;
            TotalSondeos.Value = CurrentRegistro.Sondeos.Count;
        }

        private void InitializeSondeo(int i)
        {
            var index = i - 1;
            //nombre
            Nombre.DataBindings.Clear();
            Nombre.DataBindings.Add("Text", CurrentRegistro.Sondeos[index], "Nombre", true);
            //fechaInicio
            FechaInicio.DataBindings.Clear();
            FechaInicio.DataBindings.Add("Value", CurrentRegistro.Sondeos[index], "FechaInicio", true);
            //fechaFinal
            FechaFinal.DataBindings.Clear();
            FechaFinal.DataBindings.Add("Value", CurrentRegistro.Sondeos[index], "FechaFinal", true);
            //Escala
            Escala.DataBindings.Clear();
            Escala.DataBindings.Add("Text", CurrentRegistro.Sondeos[index], "Escala", true);
            //Operador
            Operador.DataBindings.Clear();
            Operador.DataBindings.Add("Text", CurrentRegistro.Sondeos[index], "Operador", true);
            //Equipo
            Equipo.DataBindings.Clear();
            Equipo.DataBindings.Add("Text", CurrentRegistro.Sondeos[index], "Equipo", true);
            //Localizacion
            Localizacion.DataBindings.Clear();
            Localizacion.DataBindings.Add("Text", CurrentRegistro.Sondeos[index], "Localizacion", true);
            //InclinacionVertical
            InclinacionVertical.DataBindings.Clear();
            InclinacionVertical.DataBindings.Add("Text", CurrentRegistro.Sondeos[index], "InclinacionVertical", true);
            //profundidadFinal
            ProfundidadFinal.DataBindings.Clear();
            ProfundidadFinal.DataBindings.Add("Text", CurrentRegistro.Sondeos[index], "ProfundidadFinal", true);
            //CoordenadaNorte
            CoordenadaNorte.DataBindings.Clear();
            CoordenadaNorte.DataBindings.Add("Text", CurrentRegistro.Sondeos[index], "CoordenadaNorte", true);
            //CoordenadaEste
            CoordenadaEste.DataBindings.Clear();
            CoordenadaEste.DataBindings.Add("Text", CurrentRegistro.Sondeos[index], "CoordenadaEste", true);
            //Observaciones
            Observaciones.DataBindings.Clear();
            Observaciones.DataBindings.Add("Text", CurrentRegistro.Sondeos[index], "Observaciones", true);
            //Ensayos
            ensayoBindingSource.DataSource = CurrentRegistro.Sondeos[index].Ensayos;
            //Muestras
            muestraBindingSource.DataSource = CurrentRegistro.Sondeos[index].Muestras;
            //Freaticos
            freaticoBindingSource.DataSource = CurrentRegistro.Sondeos[index].Freaticos;
        }

        private void Logo_Click(object sender, EventArgs e)
        {
            var logoDialog = new OpenFileDialog
            {
                InitialDirectory = Path.Combine(AppPath, "Logos"),
                FileName = "",
                Filter =
                    @"Archivo de Imagenes (*.bmp;*.jpg;*.png; *.wmf)|*.bmp;*.jpg;*.png;*.wmf|Todos los archivos (*.*)|*.*"
            };
            if (logoDialog.ShowDialog() != DialogResult.OK) return;
            Modified = true;
            try
            {
                logo.Image = CurrentRegistro.Logo = new Bitmap(logoDialog.FileName);
            }
            catch
            {
                // ignored
            }
        }

        private void LogoPorDefectoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenConfiguration();
            var logoDialog = new OpenFileDialog
            {
                InitialDirectory = Path.Combine(AppPath, "Logos"),
                FileName = "",
                Filter =
                    @"Archivo de Imagenes (*.bmp;*.jpg;*.png; *.wmf)|*.bmp;*.jpg;*.png;*.wmf|Todos los archivos (*.*)|*.*"
            };
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

        private void NewRegistro()
        {
            //initialisation Registro
            Sondeo.Cont = 0;
            CurrentRegistro = new Registro
            {
                Logo = DefaultConfig.LogoPorDefecto,
                Sondeos = new List<Sondeo> { new Sondeo() }
            };
            InitializeRegistro();
        }

        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            if (Modified)
                SaveToolStripButton_Click(null, null);
            NewRegistro();
        }

        private void OpenConfiguration()
        {
            // valores por defecto
            DefaultConfig = new Configuracion
            {
                TiposdeEnsayos = new List<TipoEnsayo>
                {
                    new TipoEnsayo {ShortName = "SS", Longitud = 0.45, LongName = "Penetración Estándar"},
                    new TipoEnsayo {ShortName = "SH", Longitud = 0.5, LongName = "Shelby"}
                },
                ColoresdeAchurados = new List<ColoresAchurado>
                {
                    new ColoresAchurado {Name = "Negro", HexCode = "#000000"},
                    new ColoresAchurado {Name = "Ocre Oscuro", HexCode = "#8a5117"},
                    new ColoresAchurado {Name = "Ocre", HexCode = "#cc7722"},
                    new ColoresAchurado {Name = "Ocre Claro", HexCode = "#e39d57"},
                    new ColoresAchurado {Name = "Habano", HexCode = "#f5f5dc"},
                    new ColoresAchurado {Name = "Gris Habano", HexCode = "#d3d3d3"},
                    new ColoresAchurado {Name = "Blanco", HexCode = "#ffffff"}
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
                        MessageBox.Show(@"Error leyendo el config.xml\nSe va utilizar valores por defecto");
                    }
                }
            }

            // initialize Objects
            if (CurrentRegistro != null)
                CurrentRegistro.Logo = DefaultConfig.LogoPorDefecto;
            TipoColumn.Items.Clear();
            // ReSharper disable once CoVariantArrayConversion
            TipoColumn.Items.AddRange(DefaultConfig.TiposdeEnsayos.Select(x => x.ShortName).ToArray());
        }

        private void OpenFile()
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            var reader = new XmlSerializer(typeof(Registro));
            using (var file = new StreamReader(openFileDialog1.FileName))
            {
                CurrentRegistro = (Registro)reader.Deserialize(file);
            }
            InitializeRegistro();
            saveFileDialog1.FileName = openFileDialog1.FileName;
            Text = $@"Registro de Perforación - [{Path.GetFileName(openFileDialog1.FileName)}]";
        }

        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void PreviewToolStripButton_Click(object sender, EventArgs e)
        {
            var preview = new PrintPreviewDialog();
            var pd = new PrintDocument();
            pd.PrintPage += PrintPage;
            preview.Document = pd;
            preview.WindowState = FormWindowState.Maximized;
            preview.ShowDialog();
            pd.Dispose();
        }

        private static void PrintPage(object sender, PrintPageEventArgs ev)
        {
            // 215.9 by 279.4 mm
            ev.Graphics.PageUnit = GraphicsUnit.Millimeter;
            var pen1 = new Pen(Color.Black, 1);
            var pen2 = new Pen(Color.Black, 2);
            var regular = new Font("Times New Roman", 11, FontStyle.Regular);
            var bold = new Font("Times New Roman", 11, FontStyle.Bold);
            var brushBlack = new SolidBrush(Color.Black);
            var center = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            //
            ev.Graphics.DrawString("Hello World! 1234567", regular, brushBlack, 11, 50);
            // achurado
            FillPattern(ev.Graphics, CurrentRegistro.Sondeos[0].Muestras[0].Achurado, 50, 50, 200);
            // Observacion
            ev.Graphics.DrawString("OBSERVACIONES:", bold, brushBlack, 11, 251);
            ev.Graphics.DrawString(CurrentRegistro.Sondeos[0].Observaciones, regular, brushBlack, new Rectangle(11, 255, 193, 15), center);


            // borders
            ev.Graphics.DrawRectangle(pen2, new Rectangle(10, 10, 195, 259));
            ev.Graphics.DrawLine(pen2, 10, 50, 205, 50);
            ev.Graphics.DrawLine(pen2, 10, 250, 205, 250);

            // Dispose objects
            pen1.Dispose();
            pen2.Dispose();
            regular.Dispose();
            bold.Dispose();
            brushBlack.Dispose();
            ev.HasMorePages = false;
        }

        public static void FillPattern(Graphics ev, Bitmap image, int x0, int y0, int height)
        {
            const double scale = 3.8D;
            var heightInPixels = height * scale;
            var times = Math.Ceiling(heightInPixels / image.Height * scale);
            var newImage = new Bitmap(38, (int)heightInPixels);
            using (var g = Graphics.FromImage(newImage))
                for (var x = 0; x < times; x++)
                {
                    g.DrawImage(image, 0, image.Height * x);
                }
            ev.DrawImage(newImage, x0, y0);
        }

        private void PrintToolStripButton_Click(object sender, EventArgs e)
        {
            var pd = new PrintDocument();
            pd.PrintPage += PrintPage;
            pd.Print();
            pd.Dispose();
        }

        private void SalirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private static void SaveConfiguration()
        {
            var writer = new XmlSerializer(typeof(Configuracion));
            using (var file = File.Create(PathDefaultConfig))
            {
                writer.Serialize(file, DefaultConfig);
            }
        }

        private void SaveFile()
        {
            var writer = new XmlSerializer(typeof(Registro));
            using (var file = File.Create(saveFileDialog1.FileName))
            {
                writer.Serialize(file, CurrentRegistro);
            }
            Text = $@"Registro de Perforación - [{Path.GetFileName(saveFileDialog1.FileName)}]";
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(saveFileDialog1.FileName) && saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            SaveFile();
        }

        private void TipoDeEnsayosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tipoEnsayoForm1 = new TipoEnsayoForm { TiposdeEnsayos = DefaultConfig.TiposdeEnsayos };
            if (tipoEnsayoForm1.ShowDialog() != DialogResult.OK) return;
            TipoColumn.Items.Clear();
            // ReSharper disable once CoVariantArrayConversion
            TipoColumn.Items.AddRange(tipoEnsayoForm1.TiposdeEnsayos.Select(x => x.ShortName).ToArray());
            DefaultConfig.TiposdeEnsayos = tipoEnsayoForm1.TiposdeEnsayos;
            SaveConfiguration();
        }

        private void TotalSondeos_ValueChanged(object sender, EventArgs e)
        {
            Modified = true;
            IndexSondeo.Maximum = TotalSondeos.Value;
            while (CurrentRegistro.Sondeos.Count < TotalSondeos.Value)
                CurrentRegistro.Sondeos.Add(new Sondeo());
        }
    }
}