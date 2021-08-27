using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
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
    public partial class CheckCollection : DevExpress.XtraEditors.XtraUserControl
    {
        Dictionary<FileInfo, FileInfo> equalFiles_Dict;
        List<FileInfo> allFiles_List;
        List<DirectoryInfo> tinyfolders_List;
        DirectoryInfo main_directoryinfo;
        List<DirectoryInfo> hoerbuecher_List;
        List<FileInfo> wrongFileTypes_List;
        List<DirectoryInfo> leereOrdner_List;
        int leereOrdner;
        List<FileInfo> smallFiles_List;
        IObserver dm2_main;

        public CheckCollection()
        {
            InitializeComponent();

        }

        private void simpleButton1_Click(object sender, EventArgs e)    // Verzeichnis wählen Button
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

        private void simpleButton6_Click(object sender, EventArgs e) // Check Button
        {
            dm2_main = this.Parent.Parent as IObserver;

            dm2_main.UseWaitForm(false);

            equalFiles_Dict = new Dictionary<FileInfo, FileInfo>();
            allFiles_List = new List<FileInfo>();
            tinyfolders_List = new List<DirectoryInfo>();
            smallFiles_List = new List<FileInfo>();
            hoerbuecher_List = new List<DirectoryInfo>();
            wrongFileTypes_List = new List<FileInfo>();
            leereOrdner_List = new List<DirectoryInfo>();
            leereOrdner = 0;
            listBoxControl1.DataSource = null;
            listBoxControl1.Items.Clear();

            // Verzeichnispfad prüfen
            if (Directory.Exists(textEdit1.Text))
            {
                main_directoryinfo = new DirectoryInfo(textEdit1.Text);
            }
            else
            {
                listBoxControl1.Items.Add("ungültiger Verzeichnispfad");
                dm2_main.UseWaitForm(true);
                return;
            }

            // Hörbücher Verzeichnisse auflisten:
            hoerbuecher_List = main_directoryinfo.GetDirectories().ToList();
            listBoxControl1.Items.Add("Anzahl Einträge: " + hoerbuecher_List.Count());

            // Alle Dateien auflisten:
            allFiles_List.AddRange(hoerbuecher_List.SelectMany(x => x.GetFiles("*", SearchOption.AllDirectories)).ToList());
            listBoxControl1.Items.Add("Anzahl Dateien: " + allFiles_List.Count());

            FalscheDateitypenFinden();

            LeereOrdnerfinden();

            SehrKleineDateienFinden();

            SehrkleineOrdnerfinden();

            //DoppelteDateienFinden();

            dm2_main.UseWaitForm(true);
        }

        public void verschachtelteOrdnerfinden()
        {
            foreach (var hoerbuch in hoerbuecher_List)
            {
                if (hoerbuch.EnumerateDirectories().Count() > 0)
                {
                    counter = 0;
                    verzeichnisbaumRekursiv(hoerbuch);
                }
            }
        }

        public int counter;
        public void verzeichnisbaumRekursiv(DirectoryInfo hoerbuch)
        {
            if (hoerbuch.EnumerateDirectories().Count() > 0)
            {
                counter++;
                foreach (var item in hoerbuch.EnumerateDirectories())
                {
                    verzeichnisbaumRekursiv(item);
                }
            }
        }
        
        public void DoppelteDateienFinden() // die byte arrays sind leider nie identisch, auch nicht bei direkten Kopien. --> vorerst deaktiviert.
        { 
            while (allFiles_List.Count > 1)
            {
                NextFile:
                FileInfo file = allFiles_List[0];
                allFiles_List.Remove(file);

                foreach (var item in allFiles_List)
                {
                    if (item.Length == file.Length)
                    {
                        byte[] file1 = File.ReadAllBytes(item.FullName);
                        byte[] file2 = File.ReadAllBytes(file.FullName);

                        for (int i = 0; i < file1.Length; i++)
                        {
                            if (file1[i] != file2[i])
                            {
                                goto NextFile;
                            }
                        }

                        if (!equalFiles_Dict.ContainsKey(item) && !equalFiles_Dict.ContainsKey(file))
                        {
                            equalFiles_Dict.Add(item, file);
                        }
                    }
                }
            }

            listBoxControl1.Items.Add("Doppelte Dateien: " + equalFiles_Dict.Count());

            if (equalFiles_Dict.Count > 0)
            {
                simpleButton12.Enabled = true;
            }
        }


        public void SehrKleineDateienFinden()
        {
            foreach (var item in hoerbuecher_List)
            {
                smallFiles_List.AddRange(item.GetFiles("*", SearchOption.AllDirectories)
                    .Where(x => x.Length < MySettings.Instance().minimumfilesize));
            }
            var minfilesize = MySettings.Instance().minimumfilesize / 1000;
            listBoxControl1.Items.Add($"Dateien unter {minfilesize} KB: " + smallFiles_List.Count());

            if (smallFiles_List.Count > 0)
            {
                simpleButton8.Enabled = true;
            }
        }

        
        public void SehrkleineOrdnerfinden()
        {
            foreach (var item in hoerbuecher_List)
            {
                long dirSize = item.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
                if (dirSize < MySettings.Instance().minimumfoldersize)
                {
                    tinyfolders_List.Add(item);
                }
            }
            var minfoldersize = MySettings.Instance().minimumfoldersize / 1000000;
            listBoxControl1.Items.Add($"Ordner unter {minfoldersize} MB: " + tinyfolders_List.Count());

            if (tinyfolders_List.Count > 0)
            {
                simpleButton10.Enabled = true;
            }
        }

        
        public void FalscheDateitypenFinden()
        {
            foreach (var item in hoerbuecher_List)
            {
                wrongFileTypes_List.AddRange(item.GetFiles("*", SearchOption.AllDirectories)
                    .Where(x => !x.FullName.Contains(".mp3") && !x.FullName.Contains(".mp4") && !x.FullName.Contains(".m4a")));
            }

            listBoxControl1.Items.Add("Falsche Dateitypen: " + wrongFileTypes_List.Count());

            if (wrongFileTypes_List.Count() > 0)
            {
                simpleButton2.Enabled = true;
            }
        }


        public void LeereOrdnerfinden()
        {
            leereOrdner = 0;

            foreach (var item in hoerbuecher_List)
            {
                if (item.GetFiles("*", SearchOption.AllDirectories).Length == 0)    // sucht auch in Unterverzeichnissen
                {
                    leereOrdner_List.Add(item);
                    leereOrdner++;
                }
            }
            listBoxControl1.Items.Add("Leere Ordner: " + leereOrdner);

            if (leereOrdner > 0)
            {
                simpleButton3.Enabled = true;
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)    // leere Ordner anzeigen Button
        {
            listBoxControl1.Items.Clear();
            listBoxControl1.DataSource = null;

            listBoxControl1.DataSource = leereOrdner_List;

            simpleButton5.Enabled = true;
        }

        private void simpleButton5_Click(object sender, EventArgs e)    // leere Ordner löschen Button
        {
            foreach (var item in hoerbuecher_List)
            {
                if (item.GetFiles("*", SearchOption.AllDirectories).Length == 0)
                {
                    item.Delete();
                }
            }

            simpleButton5.Enabled = false;
            simpleButton3.Enabled = false;
        }

        private void simpleButton2_Click(object sender, EventArgs e)    // Falsche Dateitypen anzeigen Button
        {
            listBoxControl1.Items.Clear();
            listBoxControl1.DataSource = null;

            listBoxControl1.DataSource = wrongFileTypes_List.Select(x => x.FullName);
            simpleButton4.Enabled = true;
        }

        private void simpleButton4_Click(object sender, EventArgs e)    // Falsche Dateitypen löschen Button
        {
            foreach (var item in wrongFileTypes_List)
            {
                item.Delete();
            }

            simpleButton2.Enabled = false;
            simpleButton4.Enabled = false;
        }

        private void simpleButton8_Click(object sender, EventArgs e)    // Winzige Dateien anzeigen Button
        {
            listBoxControl1.DataSource = smallFiles_List.Select(x => x.FullName);

            simpleButton7.Enabled = true;
        }

        private void simpleButton7_Click(object sender, EventArgs e)    // Winzige Dateien löschen Button
        {
            foreach (var item in smallFiles_List)
            {
                item.Delete();
            }

            simpleButton7.Enabled = false;
            simpleButton8.Enabled = false;
        }

        private void simpleButton10_Click(object sender, EventArgs e)   // Winzige Ordner anzeigen Button
        {
            listBoxControl1.DataSource = tinyfolders_List;

            simpleButton9.Enabled = true;
        }

        private void simpleButton9_Click(object sender, EventArgs e)    // Winzige Ordner löschen Button
        {
            foreach (var item in tinyfolders_List)
            {
                item.Delete(true);
            }

            simpleButton9.Enabled = false;
            simpleButton10.Enabled = false;
        }

        private void simpleButton12_Click(object sender, EventArgs e)   // Doppelte Dateien anzeigen Button
        {
            listBoxControl1.DataSource = equalFiles_Dict;

            simpleButton11.Enabled = true;
        }

        private void simpleButton11_Click(object sender, EventArgs e)   // Doppelte Dateien löschen Button
        {
            simpleButton11.Enabled = false;
            simpleButton12.Enabled = false;
        }
    }
}
