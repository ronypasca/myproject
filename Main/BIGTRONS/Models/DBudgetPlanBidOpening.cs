using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DBudgetPlanBidOpening : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
        public string BPBidOpeningID { get; set; }
        public string BudgetPlanID { get; set; }
		public int BudgetPlanVersion { get; set; }
		public int StatusID { get; set; }
		public DateTime PeriodStart { get; set; }
		public DateTime PeriodEnd { get; set; }

        #endregion
    }
}