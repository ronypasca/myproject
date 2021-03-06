using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class CMenuObject : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string MenuID { get; set; }
        [Key, Column(Order = 2)]
        public string ObjectID { get; set; }
        public string ObjectDesc { get; set; }
        public string ObjectLongDesc { get; set; }

        #endregion
    }
}