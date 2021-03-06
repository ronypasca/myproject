using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DFPTVendorRecommendations : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string VendorRecommendationID { get; set; }
        public string FPTID { get; set; }
        public string TaskID { get; set; }
        public string TCMemberID { get; set; }
        public string FPTVendorParticipantID { get; set; }
        public bool IsProposed { get; set; }
        public bool IsWinner { get; set; }
        public string Remarks { get; set; }
        public string LetterNumber { get; set; }

        #endregion
    }
}