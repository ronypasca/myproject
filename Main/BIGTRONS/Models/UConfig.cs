using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class UConfig
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string Key1 { get; set; }
        [Key, Column(Order = 2)]
        public string Key2 { get; set; }
        [Key, Column(Order = 3)]
        public string Key3 { get; set; }
        [Key, Column(Order = 4)]
        public string Key4 { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Desc3 { get; set; }
        public string Desc4 { get; set; }

        #endregion
    }
}