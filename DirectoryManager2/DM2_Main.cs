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
        public DM2_Main()
        {
            InitializeComponent();

            this.Size = new Size(800, 600);
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (layoutControlItem1.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Never)
            {
                layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            else
            {
                layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            
        }
    }
}
