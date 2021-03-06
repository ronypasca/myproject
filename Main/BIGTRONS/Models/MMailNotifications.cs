using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class MMailNotifications : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string MailNotificationID { get; set; }
		public bool Importance { get; set; }
        public string FunctionID { get; set; }
        public string NotificationTemplateID { get; set; }
        public string Subject { get; set; }
		public string Contents { get; set; }
		public int StatusID { get; set; }
		public string TaskID { get; set; }
		public string FPTID { get; set; }

		#endregion
	}
}