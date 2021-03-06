using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MProject : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string ProjectID { get; set; }
        public string ProjectDesc { get; set; }
        public string CompanyID { get; set; }
        public string DivisionID { get; set; }
        public string LocationID { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Postal { get; set; }

        #endregion
    }
}