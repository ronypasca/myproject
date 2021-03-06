using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class TNotificationValues : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string NotificationValueID { get; set; }
		public string Value { get; set; }
		public string MailNotificationID { get; set; }
        public string FieldTagID { get; set; }
       
        #endregion
    }
}