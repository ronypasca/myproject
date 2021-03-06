using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class MMinuteTemplates : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string MinuteTemplateID { get; set; }
		public string MinuteTemplateDescriptions { get; set; }
		public string Contents { get; set; }
		public string FunctionID { get; set; }

		#endregion
	}
}