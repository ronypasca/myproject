using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DItemVersionChildAlt : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemVersionChildID { get; set; }
        [Key, Column(Order = 2)]
        public string ItemID { get; set; }
        [Key, Column(Order = 3)]
        public int Version { get; set; }

        #endregion
    }
}