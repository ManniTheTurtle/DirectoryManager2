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
    public partial class CreateFolders : DevExpress.XtraEditors.XtraUserControl
    {
        DirectoryInfo maindirectoryinfo = null;
        List<FileInfo> fileinfoList = null;
        List<DirectoryInfo> directoryinfoList = null;

        public CreateFolders()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)    // Verzeichnis wählen
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

        private void simpleButton4_Click(object sender, EventArgs e)    // Check directory for valid files
        {
            if (Directory.Exists(textEdit1.Text))
            {
                maindirectoryinfo = new DirectoryInfo(textEdit1.Text);
            }
            else
            {
                listBoxControl2.Items.Add("ungültiger Verzeichnispfad");
                return;
            }

            if (maindirectoryinfo == null || !maindirectoryinfo.Exists)
            {
                listBoxControl2.Items.Add("ungültiger Verzeichnispfad");
                return;
            }

            // Dateien nach diesen Endungen filtern
            string[] extensions = new[] { "*.mp4", "*.mp3", "*.m4a", "*.ogg", "*.flac" };
            fileinfoList = extensions.SelectMany(x => maindirectoryinfo.GetFiles(x, SearchOption.TopDirectoryOnly)).ToList();

            listBoxControl2.Items.Add("AudioFiles: " + fileinfoList.Count());

            if (fileinfoList.Count() > 0)
            {
                simpleButton2.Enabled = true;
            }

            if (maindirectoryinfo.GetDirectories().Length > 0)
            {
                simpleButton3.Enabled = true;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)    // Create Folders from Filenames
        {
            if (fileinfoList != null && fileinfoList.Count > 0)
            {
                foreach (var fi in fileinfoList)
                {
                    maindirectoryinfo.CreateSubdirectory(Path.GetFileNameWithoutExtension(fi.FullName) + AddAudioBookType());

                    fi.MoveTo(maindirectoryinfo.FullName + "\\" + Path.GetFileNameWithoutExtension(fi.FullName) + AddAudioBookType() + "\\" + fi.Name);
                }

                // directoryinfoList = maindirectoryinfo.GetDirectories().Where(x => x.Name.Contains("[Hörspiel]") || x.Name.Contains("[Lesung]")).ToList();
                directoryinfoList = maindirectoryinfo.GetDirectories().ToList();

                listBoxControl2.Items.Clear();

                listBoxControl2.Items.Add("Ordner erzeugt: " + directoryinfoList.Count());

                foreach (var item in directoryinfoList)
                {
                    listBoxControl2.Items.Add(item);
                }
            }
        }

        public string AddAudioBookType()    // füge Hörbuchart zu Ordnernamen hinzu
        {
            string hoerbuchTyp;

            if (checkEdit1.Checked)
            {
                hoerbuchTyp = " [Hörspiel]";
            }
            else if (checkEdit2.Checked)
            {
                hoerbuchTyp = " [Lesung]";
            }
            else
            {
                hoerbuchTyp = "";
            }
            
            return hoerbuchTyp;
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)  // lesung oder hörspiel auswählen checkboxes
        {
            if (checkEdit1.Checked)
            {
                checkEdit2.Checked = false;
            }
        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit2.Checked)
            {
                checkEdit1.Checked = false;
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)    // Ordnernamen bereinigen
        {
            if (maindirectoryinfo == null || !maindirectoryinfo.Exists)
            {
                if (Directory.Exists(textEdit1.Text))
                {
                    maindirectoryinfo = new DirectoryInfo(textEdit1.Text);
                }
                else
                {
                    listBoxControl1.Items.Add("ungültiger Verzeichnispfad");
                    return;
                }
            }

            directoryinfoList = maindirectoryinfo.GetDirectories().Where(x => x.Name.Contains("[Hörspiel]") || x.Name.Contains("[Lesung]")).ToList();

            if (directoryinfoList == null || directoryinfoList.Count == 0)
            {
                return;
            }

            foreach (var item in directoryinfoList)
            {
                string newname = TrimDirectoryName(item);
                if (!newname.Equals(item.Name))
                {
                    item.MoveTo(maindirectoryinfo.FullName + "\\" + newname);   // MoveTo = umbenennen
                }
            }

            listBoxControl1.Items.Add("Ordnernamen bereinigt: " + directorynameschangedcounter);
        }

        int directorynameschangedcounter = 0;
        private string TrimDirectoryName(DirectoryInfo item)
        {
            string newname = item.Name;
            string newname2;

            if (item.Name.Contains("("))
            {
                newname = item.Name.Split('(')[0];
                newname += item.Name.Split(')').Last();
            }

            if (newname.Contains("[Science-Fiction]"))
            {
                newname = newname.Replace("[Science-Fiction]", "");
            }

            if (newname.Contains("[Sci-Fi]"))
            {
                newname = newname.Replace("[Sci-Fi]", "");
            }

            if (newname.Contains("[19") || newname.Contains("[20"))
            {
                newname2 = newname.Split('[')[0];
                newname2 += newname.Split(']')[1];
                newname2 += "]"; // weil der dritte Teil fehlt, an der letzten ] wird nochmal gesplittet und dabei die Klammer gelöscht
                newname = newname2;
            }

            if (newname.Contains("  "))
            {
                newname = newname.Replace("  ", " ");
            }

            if (item.Name != newname)
            {
                listBoxControl1.Items.Add("Alt: " + item.Name);
                listBoxControl1.Items.Add("Neu: " + newname);
                directorynameschangedcounter++;
            }

            return newname;
        }
    }
}
