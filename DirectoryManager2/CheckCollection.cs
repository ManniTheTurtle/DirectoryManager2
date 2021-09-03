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
        public List<string> SchreibfehlerListe;
        public List<DirectoryInfo> zuLangeOrdnerNamenListe;
        public Dictionary<DirectoryInfo, int> foldersWithDeepSubfolders;
        public Dictionary<FileInfo, FileInfo> equalFiles_Dict;
        public List<FileInfo> allFiles_List;
        public List<DirectoryInfo> tinyfolders_List;
        public DirectoryInfo main_directoryinfo;
        public List<DirectoryInfo> hoerbuecher_List;
        public List<FileInfo> wrongFileTypes_List;
        public List<DirectoryInfo> leereOrdner_List;
        public List<DirectoryInfo> leereUnterOrdner_List;
        public List<FileInfo> smallFiles_List;
        public IObserver dm2_main;

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

            SchreibfehlerListe = new List<string>();
            zuLangeOrdnerNamenListe = new List<DirectoryInfo>();
            foldersWithDeepSubfolders = new Dictionary<DirectoryInfo, int>();
            equalFiles_Dict = new Dictionary<FileInfo, FileInfo>();
            allFiles_List = new List<FileInfo>();
            tinyfolders_List = new List<DirectoryInfo>();
            smallFiles_List = new List<FileInfo>();
            hoerbuecher_List = new List<DirectoryInfo>();
            wrongFileTypes_List = new List<FileInfo>();
            leereOrdner_List = new List<DirectoryInfo>();
            leereUnterOrdner_List = new List<DirectoryInfo>();
            listBoxControl1.DataSource = null;
            listBoxControl1.Items.Clear();

            foreach (var item in layoutControl1.Controls)
            {
                if (item is SimpleButton)
                {
                    if ((item as SimpleButton).Text != "Check!" && (item as SimpleButton).Text != "Wählen..." && (item as SimpleButton).Text != "MP3 zu mp3")
                    {
                        (item as SimpleButton).Enabled = false;
                    }
                }
            }

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

            LeereUnterordnerfinden();

            SehrKleineDateienFinden();

            SehrkleineOrdnerfinden();

            //DoppelteDateienFinden();

            verschachtelteOrdnerfinden();

            OrdnernamenAufLaengePruefen();

            AufSchreibfehlerPruefen();

            dm2_main.UseWaitForm(true);
        }

        public void AufSchreibfehlerPruefen()
        {
            foreach (var item in hoerbuecher_List)
            {
                // prüfe nach korrekter Endung
                if (!item.Name.Contains("[Hörspiel]") && !item.Name.Contains("[Lesung]"))
                {
                    SchreibfehlerListe.Add(item.Name + " --> fehlt [Hörspiel]/[Lesung]");
                }

                // prüfe nach Anfangsgroßbuchstaben
                var uppertest = item.Name.Split('-')[0];
                var uppertestArray = uppertest.Split(' ');
                foreach (var part in uppertestArray)
                {
                    if (!string.IsNullOrEmpty(part) && !part.Equals("von") && !part.Equals("de") && !part.Equals("la") && !part.Equals("van")
                        && !part.Equals("der") && (!Char.IsLetter(part[0]) || !Char.IsUpper(part[0])))
                    {
                        SchreibfehlerListe.Add(item.Name + " --> Autorenname ist klein");
                    }
                }

                var nextuppertest = item.Name.Split('-').Last();
                if (!Char.IsWhiteSpace(nextuppertest[0]) && (!Char.IsLetter(nextuppertest[1]) && !Char.IsDigit(nextuppertest[1])) && !Char.IsUpper(nextuppertest[1]))
                {
                    SchreibfehlerListe.Add(item.Name + " --> Fehler nach Bindestrich");
                }

                // prüfe auf Doppelleerzeichen
                if (item.Name.Contains("  "))
                {
                    SchreibfehlerListe.Add(item.Name + " --> Doppeltes Leerzeichen");
                }

                // prüfe auf Bindestrich
                if (!item.Name.Contains(" - "))
                {
                    SchreibfehlerListe.Add(item.Name + " --> Falsche Trennung");
                }
            }

            listBoxControl1.Items.Add("Benennungsfehler: " + SchreibfehlerListe.Count());

            if (SchreibfehlerListe.Count() > 0)
            {
                simpleButton20.Enabled = true;
            }
        }

        public void OrdnernamenAufLaengePruefen()
        {
            foreach (var item in hoerbuecher_List)
            {
                var itemsToCheck = item.GetDirectories("*", SearchOption.AllDirectories);

                foreach (var subitem in itemsToCheck)
                {
                    if (subitem.Name.Length >= MySettings.Instance().subfoldernamelength)
                    {
                        zuLangeOrdnerNamenListe.Add(subitem);
                    }
                }
            }

            listBoxControl1.Items.Add("Ordner mit zu langen Namen: " + zuLangeOrdnerNamenListe.Count());

            if (zuLangeOrdnerNamenListe.Count() > 0)
            {
                simpleButton18.Enabled = true;
            }
        }

        public void verschachtelteOrdnerfinden()
        {
            foreach (var hoerbuch in hoerbuecher_List)
            {
                if (hoerbuch.EnumerateDirectories().Count() > 0)
                {
                    counter = 0;
                    currentEntry = hoerbuch;
                    verzeichnisbaumRekursiv(hoerbuch);

                    if (counter >= MySettings.Instance().subfolderLimit)
                    {
                        foldersWithDeepSubfolders.Add(currentEntry, counter);
                    }
                }
            }

            listBoxControl1.Items.Add("Einträge mit zu tiefer Ordnerstruktur: " + foldersWithDeepSubfolders.Count());

            if (foldersWithDeepSubfolders.Count > 0)
            {
                simpleButton14.Enabled = true;
            }
        }

        public DirectoryInfo currentEntry;
        public int counter;
        public void verzeichnisbaumRekursiv(DirectoryInfo hoerbuch)
        {
            foreach (var item in hoerbuch.GetDirectories())
            {
                if (item.EnumerateDirectories().Count() > 0)
                {
                    counter++;
                    verzeichnisbaumRekursiv(item);
                }
            }
        }
        
        public void DoppelteDateienFinden() 
        { 
            while (allFiles_List.Count > 1)
            {
                //NextFile:
                FileInfo file = allFiles_List[0];
                allFiles_List.Remove(file);

                foreach (var item in allFiles_List)
                {
                    if (item.Length == file.Length)
                    {
                        // Byte für Byte Vergleich (0-255): die byte arrays sind leider nie identisch, auch nicht bei direkten Kopien. --> vorerst deaktiviert.
                        //byte[] file1 = File.ReadAllBytes(item.FullName);
                        //byte[] file2 = File.ReadAllBytes(file.FullName);

                        //for (int i = 0; i < file1.Length; i++)
                        //{
                        //    if (file1[i] != file2[i])
                        //    {
                        //        goto NextFile;
                        //    }
                        //}

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

            int minfoldersize = MySettings.Instance().minimumfoldersize / 1000000;
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
                    .Where(x => !x.FullName.EndsWith(".mp3") && !x.FullName.EndsWith(".mp4") 
                    && !x.FullName.EndsWith(".m4a") && !x.FullName.EndsWith(".wma") && !x.FullName.EndsWith(".ogg") 
                    && !x.FullName.EndsWith(".mp2") && !x.FullName.EndsWith(".Mp3")));
            }

            listBoxControl1.Items.Add("Falsche Dateitypen: " + wrongFileTypes_List.Count());

            if (wrongFileTypes_List.Count() > 0)
            {
                simpleButton2.Enabled = true;
            }
        }


        public void LeereOrdnerfinden()
        {
            foreach (var item in hoerbuecher_List)
            {
                if (item.GetFiles("*", SearchOption.AllDirectories).Length == 0)    // sucht auch in Unterverzeichnissen
                {
                    leereOrdner_List.Add(item);
                }
            }

            listBoxControl1.Items.Add("Leere Ordner: " + leereOrdner_List.Count);

            if (leereOrdner_List.Count > 0)
            {
                simpleButton3.Enabled = true;
            }
        }

        public void LeereUnterordnerfinden()
        {
            foreach (var item in hoerbuecher_List)
            {
                UnterordnerSucheRekursiv(item);
            }

            listBoxControl1.Items.Add("Leere Unterordner: " + leereUnterOrdner_List.Count);

            if (leereUnterOrdner_List.Count > 0)
            {
                simpleButton16.Enabled = true;
            }
        }

        public void UnterordnerSucheRekursiv(DirectoryInfo item)
        {
            foreach (var subitem in item.GetDirectories())
            {
                if (subitem.GetDirectories().Length == 0 && subitem.GetFiles().Length == 0)
                {
                    leereUnterOrdner_List.Add(subitem);
                }
                else
                {
                    UnterordnerSucheRekursiv(subitem);
                }
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
            listBoxControl1.DataSource = null;
            foreach (var item in equalFiles_Dict)
            {
                listBoxControl1.Items.Add(item.Key.FullName + " = " + item.Value.FullName);
            }
            
            simpleButton11.Enabled = true;
        }

        private void simpleButton14_Click(object sender, EventArgs e)   // Tiefe Ordner anzeigen
        {
            listBoxControl1.DataSource = foldersWithDeepSubfolders;
            simpleButton14.Enabled = false;
        }

        private void simpleButton16_Click(object sender, EventArgs e)   // Leere Unterordner anzeigen Button
        {
            listBoxControl1.DataSource = leereUnterOrdner_List;
            simpleButton15.Enabled = true;
        }

        private void simpleButton15_Click(object sender, EventArgs e)   // Leere Unterordner löschen Button
        {
            foreach (var item in leereUnterOrdner_List)
            {
                item.Delete();
            }
            simpleButton15.Enabled = false;
            simpleButton16.Enabled = false;
        }

        private void simpleButton18_Click(object sender, EventArgs e)   // Zu lange Ordnernamen anzeigen Button
        {
            listBoxControl1.DataSource = zuLangeOrdnerNamenListe;
        }

        private void simpleButton20_Click(object sender, EventArgs e)   // Bennenungsfehler anzeigen Button
        {
            listBoxControl1.DataSource = SchreibfehlerListe;
        }
    }
}
