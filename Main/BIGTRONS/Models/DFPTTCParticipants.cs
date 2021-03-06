using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DFPTTCParticipants : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTTCParticipantID { get; set; }
        public string FPTID { get; set; }
        public string TCMemberID { get; set; }
        public bool StatusID { get; set; }
        public bool IsDelegation { get; set; }

        #endregion
    }
}