using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DApprovalDelegationUser : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string ApprovalDelegationUserID { get; set; }
        public string ApprovalDelegateID { get; set; }
        public string DelegateUserID { get; set; }

        #endregion
    }
}