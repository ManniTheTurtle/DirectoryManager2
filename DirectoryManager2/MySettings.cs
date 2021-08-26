using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;

namespace DirectoryManager2
{
    public class MySettings
    {
        public static MySettings _instance;
        public static MySettings Instance()
        {
            if (_instance == null)
            {
                _instance = new MySettings();
            }
            return _instance;
        }

        private int _minimumfilesize;
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("in Byte")]
        [Category("Sammlung prüfen Settings:")]
        [DisplayName("Minimale Dateigröße:")]
        public int minimumfilesize
        {
            get 
            {
                return _minimumfilesize; 
            }
            set 
            { 
                _minimumfilesize = value; 
            }
        }

        private int _minimumfoldersize;
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("in Byte")]
        [Category("Sammlung prüfen Settings:")]
        [DisplayName("Minimale Dateigröße:")]
        public int minimumfoldersize
        {
            get
            {
                return _minimumfoldersize;
            }
            set
            {
                _minimumfoldersize = value;
            }
        }

        [Browsable(false)]
        public string path = "MySettings.json";
    }
}
