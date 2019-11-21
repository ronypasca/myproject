using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace com.SML.BIGTRONS.Models
{
    public class DTemplateTags : BaseModel
    {
        public readonly string Name = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #region Public Property

        [Key, Column(Order = 1)]
        public string TemplateTagID { get; set; }
        public string TemplateID { get; set; }
        public string FieldTagID { get; set; }
        public string TemplateType { get; set; }

        #endregion
    }
}