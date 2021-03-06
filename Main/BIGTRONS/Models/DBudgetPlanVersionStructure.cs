using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DBudgetPlanVersionStructure : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string BudgetPlanVersionStructureID { get; set; }
		public string BudgetPlanID { get; set; }
		public int BudgetPlanVersion { get; set; }
		public string ItemID { get; set; }
		public int Version { get; set; }
		public int Sequence { get; set; }
		public string ParentItemID { get; set; }
		public int ParentVersion { get; set; }
		public int ParentSequence { get; set; }
		public string ItemVersionChildID { get; set; }
		public string Specification { get; set; }
		public decimal Volume { get; set; }
        public decimal MaterialAmount { get; set; }
		public decimal WageAmount { get; set; }
		public decimal MiscAmount { get; set; }

		#endregion
	}
}