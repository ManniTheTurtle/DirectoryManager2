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
        // leider in diesem UserControl stringlisten verwendet statt DirectoryInfo und FileInfo Klassen
        List<string> maindirectoryfolders_List = new List<string>();
        List<string> secondarydirectoryfolders_List = new List<string>();
        List<string> DeletedFoldersCheckList = new List<string>();
        public bool maindirectoryexists;
        public bool secondarydirectoryexists;
        IEnumerable<string> maindirectoryfolders_IEnum;
        IEnumerable<string> secondarydirectoryfolders_IEnum;
        public int FoldersInMainCollection;
        public int FoldersToDelete;
        public int EquivaltentFoldersFound;

        public DeleteFolders()
        {
            InitializeComponent();

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
            listBoxControl1.Items.Clear();
            EquivaltentFoldersFound = 0;

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
            FoldersInMainCollection = maindirectoryfolders_IEnum.Count();
            FoldersToDelete = secondarydirectoryfolders_IEnum.Count();

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
                    EquivaltentFoldersFound++;
                }
                else
                {

                }
            }

            listBoxControl1.Items.Add("Alle Einträge in Sammlung: " + FoldersInMainCollection);
            listBoxControl1.Items.Add("Einträge zu löschen: " + FoldersToDelete);
            listBoxControl1.Items.Add("Löschbare Einträge in Sammlung gefunden: " + EquivaltentFoldersFound);

            // Schalte "Bereinigen" Button frei
            if (EquivaltentFoldersFound > 0)
            {
                simpleButtonBereinigen.Enabled = true;
            }
        }

        private void simpleButtonBereinigen_Click(object sender, EventArgs e)   // lösche übereinstimmende Ordner in Sammlung
        {
            listBoxControl1.Items.Clear();

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
                        Directory.Delete(match, true);
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
                }
            }

            listBoxControl1.Items.Add(counter.ToString() + " Verzeichnisse wurden gelöscht");
            simpleButtonBereinigen.Enabled = false;
        }
    }
}
