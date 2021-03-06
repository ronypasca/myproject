using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DFPTStatus : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTStatusID { get; set; }
        public string FPTID { get; set; }
        public DateTime StatusDateTimeStamp { get; set; }
        public int StatusID { get; set; }
        public string Remarks { get; set; }
        #endregion
    }
}