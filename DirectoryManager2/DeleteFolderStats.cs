using System;
using System.ComponentModel;

namespace DirectoryManager2
{
    public class DeleteFolderStats : IDisposable
    {
        private int _FoldersInMainCollection;
        [ReadOnly(true)]
        [Description("")]
        [Category("Vergleichswerte")]
        [DisplayName("Einträge in Sammlung:")]
        public int FoldersInMainCollection
        {
            get { return _FoldersInMainCollection; }
            set { _FoldersInMainCollection = value; }
        }

        private int _FoldersToDelete;
        [ReadOnly(true)]
        [Description("")]
        [Category("Vergleichswerte")]
        [DisplayName("Einträge zu löschen:")]
        public int FoldersToDelete
        {
            get { return _FoldersToDelete; }
            set { _FoldersToDelete = value; }
        }

        private int _EquivalentFoldersFound;
        [ReadOnly(true)]
        [Description("")]
        [Category("Vergleichswerte")]
        [DisplayName("Entsprechende Einträge in Sammlung gefunden:")]
        public int EquivaltentFoldersFound
        {
            get { return _EquivalentFoldersFound; }
            set { _EquivalentFoldersFound = value; }
        }

        public void Dispose()
        {
        }
    }
}
