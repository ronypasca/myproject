using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class MFieldTagReferences : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string FieldTagID { get; set; }
		public string TagDesc { get; set; }
		public string RefTable { get; set; }
		public string RefIDColumn { get; set; }

		#endregion
	}
}