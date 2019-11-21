using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MAdditionalInfoItems : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTAdditionalInfoItemID { get; set; }
        public string Descriptions { get; set; }
        
        #endregion
    }
}