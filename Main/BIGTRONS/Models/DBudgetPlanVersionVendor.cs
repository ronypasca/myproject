using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DBudgetPlanVersionVendor : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanVersionVendorID { get; set; }
        public string BudgetPlanVersionPeriodID { get; set; }
        public string VendorID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal FeePercentage { get; set; }
        public int StatusID { get; set; }
        public string ScheduleID { get; set; }
        

        #endregion
    }
}