using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectoryManager2
{
    public partial class DuplicateFolders : DevExpress.XtraEditors.XtraUserControl
    {
        public DuplicateFolders()
        {
            InitializeComponent();
        }


        private void simpleButton1_Click(object sender, EventArgs e)    // Quellpfad wählen Button
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

        private void simpleButton2_Click(object sender, EventArgs e)    // Zielpfad wählen Button
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

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            DirectoryInfo quelle = new DirectoryInfo(textEdit1.Text);

            foreach (var item in quelle.GetDirectories())
            {
                Directory.CreateDirectory(textEdit2.Text + "\\" + item.Name);
            }

            DirectoryInfo ziel = new DirectoryInfo(textEdit2.Text);

            listBoxControl1.Items.Add("Ordner kopiert:");
            listBoxControl1.Items.AddRange(ziel.GetDirectories());
        }
    }
}
