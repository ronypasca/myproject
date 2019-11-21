using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DFPTVendorWinner : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string VendorWinnerID { get; set; }
        public string FPTID { get; set; }
        public string TaskID { get; set; }
        public string FPTVendorParticipantID { get; set; }
        public string NegotiationEntryID { get; set; }
        public decimal BidFee { get; set; }
        public bool IsWinner { get; set; }
        public string LetterNumber { get; set; }

        #endregion
    }
}