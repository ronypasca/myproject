using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MSchedules : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

		[Key, Column(Order = 1)]
		public string ScheduleID { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Subject { get; set; }
        public string ProjectID { get; set; }
        public string ClusterID { get; set; }
        public string Notes { get; set; }
		public string Weblink { get; set; }
		public string Location { get; set; }
		public int Priority { get; set; }
		public bool IsAllDay { get; set; }
		public string TaskID { get; set; }
		public string StatusID { get; set; }
		public string FPTID { get; set; }
        public string MailNotificationID { get; set; }
        public bool IsBatchMail { get; set; }

        #endregion
    }
}