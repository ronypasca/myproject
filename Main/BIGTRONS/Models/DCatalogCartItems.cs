using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DCatalogCartItems : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string CatalogCartItemID { get; set; }
		public string CatalogCartID { get; set; }
        public string ItemID { get; set; }
        public decimal Qty { get; set; }
        public decimal Amount { get; set; }
        public string ItemPriceID { get; set; }
		public string VendorID { get; set; }
        public DateTime ValidFrom { get; set; }

		#endregion
	}
}