using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;

namespace DirectoryManager2
{
    [Serializable]
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
        [DisplayName("Minimale Ordnergröße:")]
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

        [Browsable(true)]
        [ReadOnly(true)]
        public string path = "MySettings.json";

        private int _subfolderLimit;
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Meldeschwelle für die Tiefe der UnterordnerStruktur (Ebenen) (um tief verschachtelte Ordner zu finden)")]
        [Category("Sammlung prüfen Settings:")]
        [DisplayName("Meldung bei Ordnertiefe:")]
        public int subfolderLimit
        {
            get
            {
                return _subfolderLimit;
            }
            set
            {
                _subfolderLimit = value;
            }
        }

        private int _subfoldernamelength;
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Meldeschwelle für zu lange Ordnernamen (Anzahl Zeichen)")]
        [Category("Sammlung prüfen Settings:")]
        [DisplayName("Meldung bei Ordnernamenlänge:")]
        public int subfoldernamelength
        {
            get
            {
                return _subfoldernamelength;
            }
            set
            {
                _subfoldernamelength = value;
            }
        }
    }
}
