using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
	public class DNotificationMap : BaseModel
	{
		public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

		#region Public Property

		[Key, Column(Order = 1)]
		public string NotifMapID { get; set; }
        public bool IsDefault { get; set; }
        public string FunctionID { get; set; }
		public string NotificationTemplateID { get; set; }

        #endregion
    }
}