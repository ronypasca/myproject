using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DRecipients : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string RecipientID { get; set; }
		public string RecipientDesc { get; set; }
		public string MailAddress { get; set; }
		public string OwnerID { get; set; }
		public string RecipientTypeID { get; set; }
		public string MailNotificationID { get; set; }

		#endregion
	}
}