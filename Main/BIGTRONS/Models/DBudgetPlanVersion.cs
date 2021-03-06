using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DBudgetPlanVersion : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string BudgetPlanID { get; set; }
		[Key, Column(Order = 2)]
		public int BudgetPlanVersion { get; set; }
		public string Description { get; set; }
		public decimal Area { get; set; }
		public decimal Unit { get; set; }
        public decimal FeePercentage { get; set; }
        public int StatusID { get; set; }
        public bool IsBidOpen { get; set; }
        public string BlockNo { get; set; }
        #endregion
    }
}