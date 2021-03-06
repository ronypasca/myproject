using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class TCatalogCart : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string CatalogCartID { get; set; }
		public string CatalogCartDesc { get; set; }
		public string UserID { get; set; }
		public int StatusID { get; set; }

		#endregion
	}
}