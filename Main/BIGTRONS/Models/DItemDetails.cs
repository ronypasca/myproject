using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DItemDetails : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string ItemDetailID { get; set; }
		public string ItemDetailDesc { get; set; }
		public string ItemDetailTypeID { get; set; }
		public string ItemID { get; set; }
		public string VendorID { get; set; }

		#endregion
	}
}