using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectoryManager2
{
    public partial class HarmonizeFiles : DevExpress.XtraEditors.XtraUserControl
    {
        public HarmonizeFiles()
        {
            InitializeComponent();
        }

        /// <summary>
        /// nicht zu verändernden Ordner wählen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textEdit1.Text = fbd.SelectedPath;
                }
            }
        }

        /// <summary>
        /// zu verändernden Ordner wählen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textEdit2.Text = fbd.SelectedPath;
                }
            }
        }
    }
}
