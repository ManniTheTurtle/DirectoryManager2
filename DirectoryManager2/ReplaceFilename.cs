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
    public partial class ReplaceFilename : DevExpress.XtraEditors.XtraUserControl
    {
        DirectoryInfo main_folder;
        List<FileInfo> allFiles = new List<FileInfo>();
        List<FileInfo> filesThatContainString = new List<FileInfo>();
        string wantedPart;
        string replacingPart;

        public ReplaceFilename()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Verzeichnis wählen Button
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
        /// Dateien mit Zeichenfolge finden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            wantedPart = textEdit2.Text;
            if (!Directory.Exists(textEdit1.Text))
            {
                return;
            }
            DirectoryInfo di = new DirectoryInfo(textEdit1.Text);
            allFiles = di.GetFiles("*.*", SearchOption.AllDirectories).ToList();
            if (allFiles == null || allFiles.Count == 0)
            {
                return;
            }
            filesThatContainString = allFiles.Where(x => x.Name.Contains(wantedPart)).ToList();
            listBoxControl1.DataSource = filesThatContainString;

            if (filesThatContainString.Count > 0)
            {
                simpleButton3.Enabled = true;
            }
        }

        /// <summary>
        /// Dateinamen verändern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            replacingPart = textEdit3.Text;
            if (string.IsNullOrWhiteSpace(replacingPart))
            {
                return;
            }

            int itemcounter = 0;
            try
            {
                // einen Teil des Dateinamens ersetzen durch etwas anderes mit .Replace()
                foreach (var file in filesThatContainString)
                {
                    var newfilename = file.Name;
                    newfilename = newfilename.Replace(wantedPart, replacingPart);
                    file.MoveTo(file.DirectoryName + "\\" + newfilename);
                    itemcounter++;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                listBoxControl1.DataSource = null;
                listBoxControl1.Items.Add($"{itemcounter} Dateien umbennant");
            }
        }

        string newname;
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            main_folder = new DirectoryInfo(textEdit1.Text);

            List<FileInfo> alldocumentaries = new List<FileInfo>();

            alldocumentaries = main_folder.GetFiles("*", SearchOption.AllDirectories).ToList();

            foreach (var item in alldocumentaries)
            {
                if (!item.Name.Contains("."))
                {
                    if (item.Name.Contains(" avi"))
                    {
                        item.MoveTo(item.FullName + ".avi");
                    }
                    else if (item.Name.Contains(" mpg"))
                    {
                        item.MoveTo(item.FullName + ".mpg");
                    }
                    else if (item.Name.Contains(" mpeg"))
                    {
                        item.MoveTo(item.FullName + ".mpeg");
                    }
                    else if (item.Name.Contains(" mp4"))
                    {
                        item.MoveTo(item.FullName + ".mp4");
                    }
                    else if (item.Name.Contains(" divx"))
                    {
                        item.MoveTo(item.FullName + ".avi");
                    }
                }
            }

            foreach (var item in alldocumentaries)
            {
                if (item.Name.Contains(" avi"))
                {
                    newname = item.Name.Replace(" avi", "");

                    item.MoveTo(item.FullName.Replace(item.Name, newname));
                }
                else if (item.Name.Contains(" mpg"))
                {
                    newname = item.Name.Replace(" mpg", "");

                    item.MoveTo(item.FullName.Replace(item.Name, newname));
                }
                else if (item.Name.Contains(" mpeg"))
                {
                    newname = item.Name.Replace(" mpeg", "");

                    item.MoveTo(item.FullName.Replace(item.Name, newname));
                }
                else if (item.Name.Contains(" mp4"))
                {
                    newname = item.Name.Replace(" mp4", "");

                    item.MoveTo(item.FullName.Replace(item.Name, newname));
                }
                else if (item.Name.Contains(" divx"))
                {
                    newname = item.Name.Replace(" divx", "");

                    item.MoveTo(item.FullName.Replace(item.Name, newname));
                }
            }
        }
    }
}
