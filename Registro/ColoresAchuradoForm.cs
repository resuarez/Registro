using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RegistroPerforacion.Entities;

namespace RegistroPerforacion
{
    public partial class ColoresAchuradoForm : Form
    {
        public ColoresAchuradoForm()
        {
            InitializeComponent();
        }

        public List<ColoresAchurado> ColoresdeAchurados { get; set; }
        private Configuracion Config { get; set; }

        private void borrarFilaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dataGridView = (DataGridView) contextMenuStrip1.SourceControl;
            if (MessageBox.Show(@"¿Está seguro de borrar?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                foreach (DataGridViewRow item in dataGridView.SelectedRows)
                    dataGridView.Rows.RemoveAt(item.Index);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            ColoresdeAchurados = Config.ColoresdeAchurados;
        }

        private void ColoresAchuradoForm_Load(object sender, EventArgs e)
        {
            if (ColoresdeAchurados != null)
                Config = new Configuracion
                {
                    ColoresdeAchurados = new List<ColoresAchurado>(ColoresdeAchurados)
                };
            bindingSourceConfig.DataSource = Config;
        }
    }
}