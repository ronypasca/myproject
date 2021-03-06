using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class MItemShowCase : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string ShowCaseID { get; set; }
		public string Filename { get; set; }
		public string ContentType { get; set; }
		public string RawData { get; set; }
		public string ItemID { get; set; }
		public string VendorID { get; set; }
        public bool IsDefault { get; set; }

		#endregion
	}
}