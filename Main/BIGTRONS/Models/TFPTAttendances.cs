using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class TFPTAttendances : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTAttendanceID { get; set; }
        public string FPTID { get; set; }
        public string AttendeeType { get; set; }
        public string IDAttendee { get; set; }
        public bool IsAttend { get; set; }
        public string AttendanceDesc { get; set; }
        
        #endregion
    }
}