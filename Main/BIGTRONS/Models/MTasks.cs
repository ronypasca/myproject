using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MTasks : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string TaskID { get; set; }
        public string TaskDesc { get; set; }
        public string TaskTypeID { get; set; }
        public DateTime TaskDateTimeStamp { get; set; }
        public string TaskOwnerID { get; set; }
        public int StatusID { get; set; }
        public int CurrentApprovalLvl { get; set; }
        public string Remarks { get; set; }
        public string Summary { get; set; }

        #endregion
    }
}