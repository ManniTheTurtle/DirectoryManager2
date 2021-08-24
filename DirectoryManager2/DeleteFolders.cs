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
    public partial class DeleteFolders : DevExpress.XtraEditors.XtraUserControl
    {
        public bool maindirectoryexists;
        public bool secondarydirectoryexists;
        public DeleteFolderStats deletefolderstats;
        IEnumerable<string> maindirectoryfolders_IEnum;
        IEnumerable<string> secondarydirectoryfolders_IEnum;
        List<string> maindirectoryfolders_List = new List<string>();
        List<string> secondarydirectoryfolders_List = new List<string>();
        List<string> DeletedFoldersCheckList = new List<string>();

        public DeleteFolders()
        {
            InitializeComponent();

            // Testing Presets:
            textEdit_maindirectorypath.Text = @"C:\Users\Hackm\Desktop\DirectoryManager Testing\MyCollection";
            textEdit_secondarydirectorypath.Text = @"C:\Users\Hackm\Desktop\DirectoryManager Testing\StuffToDelete";
        }

        private void simpleButton1_Click(object sender, EventArgs e)    // Hauptverzeichnis wählen
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textEdit_maindirectorypath.Text = fbd.SelectedPath;
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)    // Sekundäres Verzeichnis wählen
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textEdit_secondarydirectorypath.Text = fbd.SelectedPath;
                }
            }
        }


        private void simpleButton4_Click(object sender, EventArgs e)    // Check Chosen Directories
        {
            deletefolderstats = new DeleteFolderStats();

            if (Directory.Exists(textEdit_maindirectorypath.Text))
            {
                maindirectoryexists = true;
            }
            if (Directory.Exists(textEdit_secondarydirectorypath.Text))
            {
                secondarydirectoryexists = true;
            }

            if (maindirectoryexists != true || secondarydirectoryexists != true || textEdit_maindirectorypath.Text == textEdit_secondarydirectorypath.Text)
            {
                return;
            }

            // Liste alle Ordner
            maindirectoryfolders_IEnum = Directory.EnumerateDirectories(textEdit_maindirectorypath.Text);
            secondarydirectoryfolders_IEnum = Directory.EnumerateDirectories(textEdit_secondarydirectorypath.Text);

            // Zähle Ordner
            deletefolderstats.FoldersInMainCollection = maindirectoryfolders_IEnum.Count();
            deletefolderstats.FoldersToDelete = secondarydirectoryfolders_IEnum.Count();

            // Zähle übereinstimmende Verzeichnisse
            maindirectoryfolders_List = new List<string>();
            secondarydirectoryfolders_List = new List<string>();

            foreach (var item in maindirectoryfolders_IEnum)
            {
                string s = item.Split('\\').Last();
                maindirectoryfolders_List.Add(s);
            }
            foreach (var item in secondarydirectoryfolders_IEnum)
            {
                string s = item.Split('\\').Last();
                secondarydirectoryfolders_List.Add(s);
            }

            foreach (var item in secondarydirectoryfolders_List)
            {
                if (maindirectoryfolders_List.Contains(item))
                {
                    deletefolderstats.EquivaltentFoldersFound++;
                }
                else // Nicht gefundene Dateien anzeigen:
                {
                    if (labelControl2.Visible == false || labelControl2.Enabled == false)
                    {
                        labelControl2.Visible = true;
                        labelControl2.Enabled = true;
                        labelControl2.Text = "In Sammlung nicht vorhandene Ordner:" + Environment.NewLine;
                    }
                    labelControl2.Text += item + Environment.NewLine;
                }
            }

            // Zeige Werte im PropertyGrid an
            propertyGridControl1.SelectedObject = deletefolderstats;
            propertyGridControl1.Visible = true;
            propertyGridControl1.Enabled = true;


            // Schalte "Bereinigen" Button frei
            if (deletefolderstats.EquivaltentFoldersFound > 0)
            {
                simpleButtonBereinigen.Enabled = true;
            }
        }

        private void simpleButtonBereinigen_Click(object sender, EventArgs e)   // lösche übereinstimmende Ordner in Sammlung
        {
            if (maindirectoryfolders_IEnum == null || maindirectoryfolders_IEnum.Count() == 0 || secondarydirectoryfolders_List == null || secondarydirectoryfolders_List.Count == 0)
            {
                return;
            }

            foreach (var item in secondarydirectoryfolders_List)
            {
                if (maindirectoryfolders_IEnum.Any(x => x.Contains(item)))
                {
                    var match = maindirectoryfolders_IEnum.Where(x => x.Contains(item)).FirstOrDefault();

                    if (match != null)
                    {
                        Directory.Delete(match);
                        DeletedFoldersCheckList.Add(match);
                    }
                }
            }

            //Teste ob Ordner gelöscht wurden
            int counter = 0;
            foreach (var item in DeletedFoldersCheckList)
            {
                if (!Directory.Exists(item))
                {
                    counter++;
                    labelControl3.Text += item + " wurde gelöscht" + Environment.NewLine;
                }

                labelControl2.Text = counter.ToString() + " Verzeichnisse wurden gelöscht";

                if (deletefolderstats.EquivaltentFoldersFound == counter)
                {
                    labelControl2.ForeColor = Color.DarkGreen;
                    labelControl2.Text += " (Alle)";
                }
            }
        }
    }
}
