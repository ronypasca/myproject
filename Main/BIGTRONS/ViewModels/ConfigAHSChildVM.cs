using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace com.SML.BIGTRONS.ViewModels
{
    public class ConfigAHSChildVM
    {
        public string ItemTypeID { get; set; }
        public bool HasCoefficient { get; set; }
        public bool HasFormula { get; set; }
        public bool HasAlternativeItem { get; set; }


        #region Public Field Property

        public static class Prop
        {
            public static class HasCoefficient
            {
                public static string Desc { get { return "HasCoefficient"; } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class HasFormula
            {
                public static string Desc { get { return "HasFormula"; } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class HasAlternativeItem
            {
                public static string Desc { get { return "HasAlternative"; } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            
        }

        #endregion
    }
}