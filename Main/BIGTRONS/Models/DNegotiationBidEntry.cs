using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DNegotiationBidEntry : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string NegotiationEntryID { get; set; }
        public string BidTypeID { get; set; }
        public string NegotiationBidID { get; set; }
        public string RoundID { get; set; }
        public Decimal BidValue { get; set; }
        public string FPTVendorParticipantID { get; set; }

        #endregion
    }
}