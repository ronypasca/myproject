using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class TTCMemberDelegations : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string TCDelegationID { get; set; }
        public string TCMemberID { get; set; }
        public string DelegateTo { get; set; }
        public DateTime DelegateStartDate { get; set; }
        public DateTime DelegateEndDate { get; set; }

        #endregion
    }
}