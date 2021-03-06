using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DFPTNegotiationRounds : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string RoundID { get; set; }
        public string FPTID { get; set; }
        public DateTime StartDateTimeStamp { get; set; }
        public DateTime EndDateTimeStamp { get; set; }
        public string Remarks { get; set; }
        public decimal TopValue { get; set; }
        public decimal BottomValue { get; set; }
        #endregion
    }
}