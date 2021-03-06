using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DItemVersionChild : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemVersionChildID { get; set; }
        public string ItemID { get; set; }
        public int Version { get; set; }
        public string ChildItemID { get; set; }
        public int ChildVersion { get; set; }
        public int Sequence { get; set; }

        #endregion
    }
}