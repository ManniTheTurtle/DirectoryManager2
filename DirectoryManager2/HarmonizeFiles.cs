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


        DirectoryInfo persistentfolder;
        DirectoryInfo modifiablefolder;
        /// <summary>
        /// Check Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            persistentfolder = new DirectoryInfo(textEdit1.Text);
            modifiablefolder = new DirectoryInfo(textEdit2.Text);

            listBoxControl1.Items.Clear();

            if (!Directory.Exists(textEdit1.Text) || !Directory.Exists(textEdit2.Text))
            {
                listBoxControl1.Items.Add("VerzeichnisPfad existiert nicht");
                return;
            }

            int counterFilesToDelete = 0;
            int counterFilesToAdd = 0;
            foreach (var item in modifiablefolder.GetFiles())   // prüfen welche Dateien überflüssig sind
            {
                if (!persistentfolder.GetFiles().Where(x => x.Name.Equals(item.Name)).Any())
                {
                    counterFilesToDelete++;
                }
            }

            foreach (var item in persistentfolder.GetFiles())  // prüfen welche Dateien noch fehlen
            {
                if (!modifiablefolder.GetFiles().Where(x => x.Name.Equals(item.Name)).Any())
                {
                    counterFilesToAdd++;
                }
            }

            listBoxControl1.Items.Add("Überflüssige Dateien: " + counterFilesToDelete);
            listBoxControl1.Items.Add("Fehlende Dateien: " + counterFilesToAdd);

            if (counterFilesToDelete > 0)
            {
                simpleButton3.Enabled = true;
            }

            if (counterFilesToAdd > 0)
            {
                simpleButton4.Enabled = true;
            }
            
        }

        /// <summary>
        /// überflüssige Dateien löschen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            foreach (var item in modifiablefolder.GetFiles())
            {
                if (!persistentfolder.GetFiles().Where(x => x.Name.Equals(item.Name)).Any())
                {
                    listBoxControl1.Items.Add(item.Name + " wird gelöscht");
                    item.Delete();
                }
            }
        }

        /// <summary>
        /// fehlende Dateien ergänzen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            foreach (var item in persistentfolder.GetFiles())
            {
                if (!modifiablefolder.GetFiles().Where(x => x.Name.Equals(item.Name)).Any())
                {
                    item.CopyTo(Path.Combine(textEdit2.Text, item.Name), true);
                }
            }
        }
    }
}
