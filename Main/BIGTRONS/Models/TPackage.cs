using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class TPackage : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string PackageID { get; set; }
        public string PackageDesc { get; set; }
        public int StatusID { get; set; }
        public string ProjectID { get; set; }
        public string CompanyID { get; set; }

        #endregion
    }
}