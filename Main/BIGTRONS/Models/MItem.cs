using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MItem : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemID { get; set; }
        public string ItemDesc { get; set; }
        public string ItemGroupID { get; set; }
        public string UoMID { get; set; }
        public bool IsActive { get; set; }
        
        #endregion
    }
}