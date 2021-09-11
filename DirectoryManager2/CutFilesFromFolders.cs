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
    public partial class CutFilesFromFolders : DevExpress.XtraEditors.XtraUserControl
    {

        public DirectoryInfo main_directoryinfo;
        public List<DirectoryInfo> folders_directoryinfo;


        public CutFilesFromFolders()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Verzeichnis Wählen
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
        /// Check!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textEdit1.Text))
            {
                main_directoryinfo = new DirectoryInfo(textEdit1.Text);
            }

            folders_directoryinfo = main_directoryinfo.GetDirectories("*", SearchOption.TopDirectoryOnly).ToList();

            int number = folders_directoryinfo.Where(x => x.EnumerateFiles().Count() > 0).Count();
            listBoxControl1.Items.Add("Verzeichnisse mit Inhalt: " + number);
        }


        /// <summary>
        /// Dateien aus Ordnern holen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            int successcounter = 0;
            foreach (var item in folders_directoryinfo)
            {
                foreach (var file in item.GetFiles().Where(x => x.Name.EndsWith(textEdit2.Text)))
                {
                    file.MoveTo(main_directoryinfo.FullName + "\\" + file.Name);
                    successcounter++;
                }
            }

            listBoxControl1.Items.Add("Dateien extrahiert: " + successcounter);
        }
    }
}
