using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DFPTAdditionalInfo : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTAdditionalInfoID { get; set; }
        public string FPTID { get; set; }
        public string FPTAdditionalInfoItemID { get; set; }
        public string Value { get; set; }
        
        #endregion
    }
}