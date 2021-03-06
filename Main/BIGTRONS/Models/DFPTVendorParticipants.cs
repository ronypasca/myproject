using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DFPTVendorParticipants : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTVendorParticipantID { get; set; }
        public string NegotiationConfigID { get; set; }
        public string VendorID { get; set; }
        public string StatusID { get; set; }

        #endregion
    }
}