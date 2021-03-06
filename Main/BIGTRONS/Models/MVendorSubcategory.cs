using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MVendorSubcategory : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string VendorSubcategoryID { get; set; }
        public string VendorSubcategoryDesc { get; set; }
        public string VendorCategoryID { get; set; }

        #endregion
    }
}