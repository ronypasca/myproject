using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class MPreBuildRecipientTemplates : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string PreBuildRecTemplateID { get; set; }
		public string PreBuildDesc { get; set; }
		public bool IsPIC { get; set; }
		public string FunctionID { get; set; }

		#endregion
	}
}