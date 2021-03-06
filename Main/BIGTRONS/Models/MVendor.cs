using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MVendor : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string VendorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string VendorSubcategoryID { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Postal { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IDNo { get; set; }
        public string NPWP { get; set; }

        #endregion
    }
}