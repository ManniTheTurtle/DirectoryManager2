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
    public partial class CheckCollection : DevExpress.XtraEditors.XtraUserControl
    {
        DirectoryInfo main_directoryinfo;
        List<DirectoryInfo> hoerbuecher_List = new List<DirectoryInfo>();

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

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            // Verzeichnispfad prüfen
            if (Directory.Exists(textEdit1.Text))
            {
                main_directoryinfo = new DirectoryInfo(textEdit1.Text);
            }
            else
            {
                listBoxControl1.Items.Add("ungültiger Verzeichnispfad");
                return;
            }

            // Hörbücher auflisten:
            hoerbuecher_List = main_directoryinfo.GetDirectories().ToList();
            listBoxControl1.Items.Add("Anzahl Einträge: " + hoerbuecher_List.Count());

            LeereOrdnerfinden();
        }

        int leereOrdner;
        public void LeereOrdnerfinden()
        {
            leereOrdner = 0;

            foreach (var item in hoerbuecher_List)
            {
                if (item.GetFiles("*", SearchOption.AllDirectories).Length == 0)    // sucht auch in Unterverzeichnissen
                {
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

            foreach (var item in hoerbuecher_List)
            {
                if (item.GetFiles("*", SearchOption.AllDirectories).Length == 0)
                {
                    listBoxControl1.Items.Add(item.FullName);
                }
            }

            simpleButton5.Enabled = true;
        }

        private void simpleButton5_Click(object sender, EventArgs e)    // leere Ordner anzeigen Button
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
    }
}
