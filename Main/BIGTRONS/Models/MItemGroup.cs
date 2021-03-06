using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MItemGroup : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemGroupID { get; set; }
        public string ItemGroupDesc { get; set; }
        public string ItemTypeID { get; set; }
        public bool HasParameter { get; set; }
        public bool HasPrice { get; set; }

        #endregion
    }
}