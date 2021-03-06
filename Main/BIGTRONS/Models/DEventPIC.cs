using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DEventPIC : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string EventPICID { get; set; }
		public string PICID { get; set; }
		public bool IsAttend { get; set; }
		public string FPTID { get; set; }
		public string PICTypeID { get; set; }
		public string FunctionID { get; set; }

		#endregion
	}
}