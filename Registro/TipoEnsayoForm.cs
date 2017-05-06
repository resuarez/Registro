using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RegistroPerforacion
{
    public partial class TipoEnsayoForm : Form
    {
        public TipoEnsayoForm()
        {
            InitializeComponent();
        }

        private Entities.Configuracion Config { get; set; }
        public List<Entities.TipoEnsayo> TiposdeEnsayos { get; set; }

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

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            TiposdeEnsayos = Config.TiposdeEnsayos;
        }

        private void TipoEnsayoForm_Load(object sender, EventArgs e)
        {
            Config = new Entities.Configuracion() { TiposdeEnsayos = new List<Entities.TipoEnsayo>(TiposdeEnsayos) ?? new List<Entities.TipoEnsayo>() };
            bindingSourceConfig.DataSource = Config;
        }
    }
}