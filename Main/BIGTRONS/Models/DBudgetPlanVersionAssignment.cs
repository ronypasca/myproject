using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DBudgetPlanVersionAssignment : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string BudgetPlanVersionStructureID { get; set; }
		[Key, Column(Order = 2)]
		public string VendorID { get; set; }

		#endregion
	}
}