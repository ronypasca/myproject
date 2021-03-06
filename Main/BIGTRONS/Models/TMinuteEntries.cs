using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class TMinuteEntries : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string MinuteEntryID { get; set; }
		public string Subject { get; set; }
		public string FPTID { get; set; }
		public string MinuteTemplateID { get; set; }
		public int StatusID { get; set; }
		public string TaskID { get; set; }
		public string MailNotificationID { get; set; }
		public string ScheduleID { get; set; }
		#endregion
	}
}