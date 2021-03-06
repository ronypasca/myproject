using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class MUoM : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string UoMID { get; set; }
        public string UoMDesc { get; set; }
        public string DimensionID { get; set; }

        #endregion
    }
}