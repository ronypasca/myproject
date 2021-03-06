using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DFPTDeviations : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTDeviationID { get; set; }
        public string FPTID { get; set; }
        public string DeviationTypeID { get; set; }
        public string RefNumber { get; set; }
        public string RefTitle { get; set; }
        public DateTime RefDate { get; set; }

        #endregion
    }
}