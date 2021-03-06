using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MCompany : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string CompanyID { get; set; }
        public string CompanyDesc { get; set; }
        public string CountryID { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Postal { get; set; }

        #endregion
    }
}