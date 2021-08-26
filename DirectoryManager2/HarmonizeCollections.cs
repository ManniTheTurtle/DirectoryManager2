using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
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
    public partial class HarmonizeCollections : DevExpress.XtraEditors.XtraUserControl
    {
        List<DirectoryInfo> currentCollection;
        List<DirectoryInfo> backupCollection;
        List<DirectoryInfo> backupfolderstodelete;
        List<DirectoryInfo> missingfolders;

        public HarmonizeCollections()
        {
            InitializeComponent();

            textEdit1.Text = @"C:\Users\Hackm\Desktop\DirectoryManager Testing\MyCollection";
            textEdit2.Text = @"C:\Users\Hackm\Desktop\DirectoryManager Testing\StuffToDelete";
        }

        private void simpleButton1_Click(object sender, EventArgs e)    // aktuelle Sammlung wählen
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

        private void simpleButton2_Click(object sender, EventArgs e)    // Sicherung wählen
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

        private void simpleButton3_Click(object sender, EventArgs e)    // Check Button
        {
            listBoxControl1.Items.Clear();
            listBoxControl1.DataSource = null;
            listBoxControl2.Items.Clear();
            listBoxControl2.DataSource = null;

            if (!Directory.Exists(textEdit1.Text) || !Directory.Exists(textEdit2.Text) || textEdit1.Text == textEdit2.Text)
            {
                return;
            }

            currentCollection = new List<DirectoryInfo>();
            backupCollection = new List<DirectoryInfo>();

            DirectoryInfo main = new DirectoryInfo(textEdit1.Text);
            DirectoryInfo backup = new DirectoryInfo(textEdit2.Text);

            currentCollection = main.GetDirectories().ToList();
            backupCollection = backup.GetDirectories().ToList();

            listBoxControl1.Items.Add("Einträge Gesamt: " + currentCollection.Count);
            listBoxControl2.Items.Add("Einträge Gesamt: " + backupCollection.Count);

            foreach (var item in backupCollection)
            {
                var strg = item.Name;
            }

            // Überflüssige Verzeichnisse in Sicherung finden:
            backupfolderstodelete = new List<DirectoryInfo>();
            backupfolderstodelete = backupCollection.Where(x => !currentCollection.Select(y => y.Name).Contains(x.Name)).ToList();

            listBoxControl2.Items.Add("Löschbare Einträge: " + backupfolderstodelete.Count());

            // Fehlende Verzeichnisse in Sicherung finden:
            missingfolders = new List<DirectoryInfo>();
            missingfolders = currentCollection.Where(x => !backupCollection.Select(y => y.Name).Contains(x.Name)).ToList();

            listBoxControl2.Items.Add("Fehlende Einträge: " + missingfolders.Count());

            if (missingfolders.Count() > 0 || backupfolderstodelete.Count() > 0)
            {
                simpleButton5.Enabled = true;
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)    // Unterschiede anzeigen Button
        {
            listBoxControl1.Items.Clear();
            listBoxControl1.DataSource = null;
            listBoxControl2.Items.Clear();
            listBoxControl2.DataSource = null;

            listBoxControl2.Items.Add("Verzeichnisse zu löschen: " + backupfolderstodelete.Count());
            listBoxControl2.Items.AddRange(backupfolderstodelete.ToArray());

            listBoxControl1.Items.Add("Verzeichnisse zum Hinzufügen: " + missingfolders.Count());
            listBoxControl1.Items.AddRange(missingfolders.ToArray());

            simpleButton4.Enabled = true;
        }

        private void simpleButton4_Click(object sender, EventArgs e)    // Harmonize Button
        {
            if (backupfolderstodelete.Count() > 0)
            {
                foreach (var item in backupfolderstodelete)
                {
                    item.Delete(true);
                }
            }

            if (missingfolders.Count() > 0)
            {
                foreach (var item in missingfolders)
                {
                    item.Delete(true);
                }
            }

            simpleButton5.Enabled = false;
            simpleButton4.Enabled = false;
        }
    }
}
