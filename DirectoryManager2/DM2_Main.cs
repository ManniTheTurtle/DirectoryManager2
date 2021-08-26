﻿using DevExpress.XtraLayout;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DirectoryManager2
{
    public partial class DM2_Main : DevExpress.XtraBars.Ribbon.RibbonForm, IObserver
    {

        List<LayoutControlItem> usercontrols_List = new List<LayoutControlItem>();


        public DM2_Main()
        {
            InitializeComponent();

            this.Size = new Size(800, 600);

            foreach (var item in this.layoutControl1.Items)
            {
                if (item is LayoutControlItem)
                {
                    usercontrols_List.Add(item as LayoutControlItem);
                }
            }
        }

        private void DM2_Main_Load(object sender, System.EventArgs e)
        {
            if (File.Exists(MySettings.Instance().path))
            {
                string jsonstring = File.ReadAllText(MySettings.Instance().path);
                MySettings._instance = JsonConvert.DeserializeObject<MySettings>(jsonstring);
            }
        }

        // Ordner löschen Button
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (var item in usercontrols_List)
            {
                item.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }

            layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        }

        // Ordnernamen aus Dateinamen erstellen Button
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (var item in usercontrols_List)
            {
                item.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }

            layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        }

        // Sammlung auf Fehler prüfen
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)  
        {
            foreach (var item in usercontrols_List)
            {
                item.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }

            layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        }

        public void UseWaitForm(bool waitformstatus)
        {
            if (waitformstatus)
            {
                this.splashScreenManager2.CloseWaitForm();
            }
            else
            {
                this.splashScreenManager2.ShowWaitForm();
            }
        }

        // Sicherungen angleichen
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (var item in usercontrols_List)
            {
                item.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }

            layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        }

        // Settings
        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (var item in usercontrols_List)
            {
                item.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }

            layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        }

        // Beim beenden
        private void DM2_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            string jsonstring = JsonConvert.SerializeObject(MySettings.Instance(), Formatting.Indented);
            File.WriteAllText(MySettings.Instance().path, jsonstring);
        }
    }

    public interface IObserver
    {
        void UseWaitForm(bool waitformstatus);
    }
}
