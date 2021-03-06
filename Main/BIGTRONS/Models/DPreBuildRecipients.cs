using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DPreBuildRecipients : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string PreBuildRecID { get; set; }
		public string PreBuildRecTemplateID { get; set; }
		public string EmployeeID { get; set; }
		public string RecipientTypeID { get; set; }

		#endregion
	}
}