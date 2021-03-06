using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class SMenu : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string MenuID { get; set; }
        public string MenuHierarchy { get; set; }
        public string MenuDesc { get; set; }
        public string MenuIcon { get; set; }
        public string MenuUrl { get; set; }
        public bool MenuVisible { get; set; }

        #endregion
    }
}