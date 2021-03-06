using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MFPT : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTID { get; set; }
        public string ClusterID { get; set; }
        public string Descriptions { get; set; }
        public string ProjectID { get; set; }
        public string DivisionID { get; set; }
        public string BusinessUnitID { get; set; }
        public bool IsSync { get; set; }

        #endregion
    }
}