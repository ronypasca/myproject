using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DItemComparisonDetails : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string ItemComparisonDetailID { get; set; }
		public string ItemComparisonID { get; set; }
		public string ItemPriceID { get; set; }
		public string VendorID { get; set; }
		public DateTime ValidFrom { get; set; }

		#endregion
	}
}