using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DBudgetPlanVersionAdditional : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanVersionAdditionalID { get; set; }
        public string BudgetPlanVersionVendorID { get; set; }
        public string ItemID { get; set; }
        public int Version { get; set; }
        public int Sequence { get; set; }
        public string ParentItemID { get; set; }
        public int ParentVersion { get; set; }
        public int ParentSequence { get; set; }
        public string Info { get; set; }
        public decimal Volume { get; set; }

        #endregion
    }
}