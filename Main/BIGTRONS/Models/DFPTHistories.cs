using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DFPTHistories : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTHistoryID { get; set; }
        public string FPTID { get; set; }
        public string Descriptions { get; set; }
        public string Remarks { get; set; }

        #endregion
    }
}