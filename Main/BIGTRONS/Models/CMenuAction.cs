using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class CMenuAction : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string MenuID { get; set; }
        [Key, Column(Order = 2)]
        public string ActionID { get; set; }

        #endregion
    }
}