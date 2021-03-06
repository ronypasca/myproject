using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class TNumbering : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string Header { get; set; }
        [Key, Column(Order = 2)]
        public string Year { get; set; }
        [Key, Column(Order = 3)]
        public string Month { get; set; }
        [Key, Column(Order = 4)]
        public string CompanyID { get; set; }
        [Key, Column(Order = 5)]
        public string ProjectID { get; set; }
        public int LastNo { get; set; }

        #endregion
    }
}