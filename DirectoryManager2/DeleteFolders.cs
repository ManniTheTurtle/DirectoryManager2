﻿using DevExpress.XtraEditors;
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
            var maindirectoryfolders_IEnum = Directory.EnumerateDirectories(textEdit_maindirectorypath.Text);
            var secondarydirectoryfolders_IEnum = Directory.EnumerateDirectories(textEdit_secondarydirectorypath.Text);

            // Zähle Ordner
            deletefolderstats.FoldersInMainCollection = maindirectoryfolders_IEnum.Count();
            deletefolderstats.FoldersToDelete = secondarydirectoryfolders_IEnum.Count();

            // Zähle übereinstimmende Verzeichnisse
            List<string> maindirectoryfolders_List = new List<string>();
            List<string> secondarydirectoryfolders_List = new List<string>();

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


            

        }
    }
}
