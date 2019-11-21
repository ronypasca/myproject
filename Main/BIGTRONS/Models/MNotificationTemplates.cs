using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class MNotificationTemplates : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string NotificationTemplateID { get; set; }
		public string NotificationTemplateDesc { get; set; }
		public string Contents { get; set; }

		#endregion
	}
}