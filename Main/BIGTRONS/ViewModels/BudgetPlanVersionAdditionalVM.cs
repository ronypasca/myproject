using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class BudgetPlanVersionAdditionalVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanID { get; set; }
        [Key, Column(Order = 2)]
        public int BudgetPlanVersion { get; set; }
        [Key, Column(Order = 3)]
        public string VendorID { get; set; }
        [Key, Column(Order = 4)]
        public string ItemID { get; set; }
        [Key, Column(Order = 5)]
        public int Version { get; set; }
        [Key, Column(Order = 6)]
        public int Sequence { get; set; }
        public string ParentItemID { get; set; }
        public int ParentVersion { get; set; }
        public int ParentSequence { get; set; }
        public string Info { get; set; }
        public decimal? Volume { get; set; }
        public string ItemDesc { get; set; }
        public string UoMDesc { get; set; }

        public string BudgetPlanVersionVendorID { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class BudgetPlanID
            {
                public static string Desc { get { return "BudgetPlanID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class BudgetPlanVersion
            {
                public static string Desc { get { return "BudgetPlanVersion"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanVersionVendorID
            {
                public static string Desc { get { return "BudgetPlanVersionVendorID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VendorID
            {
                public static string Desc { get { return "VendorID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ItemID
            {
                public static string Desc { get { return "ItemID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Version
            {
                public static string Desc { get { return "Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Sequence
            {
                public static string Desc { get { return "Sequence"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ParentItemID
            {
                public static string Desc { get { return "ParentItemID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ParentVersion
            {
                public static string Desc { get { return "ParentVersion"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ParentSequence
            {
                public static string Desc { get { return "ParentSequence"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Info
            {
                public static string Desc { get { return "Info"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Volume
            {
                public static string Desc { get { return "ParentItemID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class UoMDesc
            {
                public static string Desc { get { return "UoMDesc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ItemDesc
            {
                public static string Desc { get { return "ItemDesc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            /// <summary>
            /// Function for mapping ViewModel Field with its Model Field
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="variable">ViewModel Field to map</param>
            /// <param name="withAlias">Whether to set its alias, usually for select list</param>
            /// <returns>Mapped Field Name</returns>
            private static string Mapping<T>(Expression<Func<T>> variable, bool withAlias = false)
            {
                string m_strFieldName = General.GetVariableName(variable);
                return Map(m_strFieldName, withAlias);
            }

            /// <summary>
            /// Function for mapping ViewModel Field with its Model Field (by ViewModel Field Name)
            /// </summary>
            /// <param name="fieldName">ViewModel Field Name to map</param>
            /// <param name="withAlias">Whether to set its alias, usually for select list</param>
            /// <returns>Mapped Field Name or empty string if not found</returns>
            public static string Map(string fieldName, bool withAlias = false)
            {
                string m_strReturn = string.Empty;

                DBudgetPlanVersionAdditional m_objDBudgetPlanVersionAdditional = new DBudgetPlanVersionAdditional();
                DBudgetPlanVersionVendor m_objDBudgetPlanVersionVendor = new DBudgetPlanVersionVendor();
                DBudgetPlanVersionPeriod m_objDBudgetPlanVersionPeriod = new DBudgetPlanVersionPeriod();
                MItem m_objMItem = new MItem();
                MUoM m_objMUoM = new MUoM();
                //BudgetPlanVersionVendorID
                if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersion.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objDBudgetPlanVersionVendor.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersionVendorID.Name)
                    m_strReturn = m_objDBudgetPlanVersionAdditional.Name + "." + fieldName;
                else if (fieldName == ItemID.Name)
                    m_strReturn = m_objDBudgetPlanVersionAdditional.Name + "." + fieldName;
                else if (fieldName == Version.Name)
                    m_strReturn = m_objDBudgetPlanVersionAdditional.Name + "." + fieldName;
                else if (fieldName == Sequence.Name)
                    m_strReturn = m_objDBudgetPlanVersionAdditional.Name + "." + fieldName;
                else if (fieldName == ParentItemID.Name)
                    m_strReturn = m_objDBudgetPlanVersionAdditional.Name + "." + fieldName;
                else if (fieldName == ParentSequence.Name)
                    m_strReturn = m_objDBudgetPlanVersionAdditional.Name + "." + fieldName;
                else if (fieldName == ParentVersion.Name)
                    m_strReturn = m_objDBudgetPlanVersionAdditional.Name + "." + fieldName;
                else if (fieldName == Info.Name)
                    m_strReturn = m_objDBudgetPlanVersionAdditional.Name + "." + fieldName;
                else if (fieldName == Volume.Name)
                    m_strReturn = m_objDBudgetPlanVersionAdditional.Name + "." + fieldName;
                else if (fieldName == ItemDesc.Name)
                    m_strReturn = m_objMItem.Name + "." + fieldName;
                else if (fieldName == UoMDesc.Name)
                    m_strReturn = m_objMUoM.Name + "." + fieldName;


                m_strReturn += (withAlias ? " AS [" + fieldName + "]" : "");
                return m_strReturn;
            }
        }

        #endregion

        /// <summary>
        /// Function for checking if Field is Key
        /// </summary>
        /// <param name="FieldName">Name of Field to check</param>
        public bool IsKey(string fieldName)
        {
            PropertyInfo m_pifBudgetPlanVersionAdditionalVM = this.GetType().GetProperty(fieldName);
            return m_pifBudgetPlanVersionAdditionalVM != null && Attribute.GetCustomAttribute(m_pifBudgetPlanVersionAdditionalVM, typeof(KeyAttribute)) != null;
        }
    }
}