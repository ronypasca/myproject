using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MParameter : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string ParameterID { get; set; }
        public string ParameterDesc { get; set; }
        public string DataType { get; set; }
        public int Length { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public string RefTable { get; set; }
        public string RefIDColumn { get; set; }
        public string RefDescColumn { get; set; }

        #endregion
    }
}