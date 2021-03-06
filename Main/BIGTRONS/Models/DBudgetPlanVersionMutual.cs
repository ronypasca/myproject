using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DBudgetPlanVersionMutual : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanVersionStructureID { get; set; }
        public string Info { get; set; }
        public decimal Volume { get; set; }
        public decimal MaterialAmount { get; set; }
        public decimal WageAmount { get; set; }
        public decimal MiscAmount { get; set; }

        #endregion
    }
}