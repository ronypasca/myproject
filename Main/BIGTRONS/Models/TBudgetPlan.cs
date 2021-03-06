using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class TBudgetPlan : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string BudgetPlanID { get; set; }
		public string BudgetPlanTemplateID { get; set; }
		public string ProjectID { get; set; }
		public string ClusterID { get; set; }
		public string UnitTypeID { get; set; }

		#endregion
	}
}