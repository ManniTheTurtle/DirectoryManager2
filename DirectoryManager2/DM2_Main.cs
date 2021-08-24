using DevExpress.XtraLayout;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DirectoryManager2
{
    public partial class DM2_Main : DevExpress.XtraBars.Ribbon.RibbonForm
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (var item in usercontrols_List)
            {
                item.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }

            layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (var item in usercontrols_List)
            {
                item.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }

            layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
        }
    }
}
