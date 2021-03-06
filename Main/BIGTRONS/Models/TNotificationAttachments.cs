using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class TNotificationAttachments : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string AttachmentValueID { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public string RawData { get; set; }
        public string MailNotificationID { get; set; }

        #endregion
    }
}