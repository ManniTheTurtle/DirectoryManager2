using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectoryManager2
{
    public partial class SettingsGrid : DevExpress.XtraEditors.XtraUserControl
    {
        public SettingsGrid()
        {
            InitializeComponent();

            propertyGridControl1.SelectedObject = MySettings.Instance();
        }
    }
}
