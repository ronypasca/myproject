using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
namespace com.SML.BIGTRONS.ViewModels
{
    public class FPTVendorRecommendationsVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string VendorRecommendationID { get; set; }
        public string FPTID { get; set; }
        public string TaskID { get; set; }
        public string TCMemberID { get; set; }
        public string TCMemberName { get; set; }
        public string FPTVendorParticipantID { get; set; }
        public bool IsProposed { get; set; }
        public bool IsWinner { get; set; }
        public string Remarks { get; set; }
        public string LetterNumber { get; set; }
        public DateTime CreatedDate { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class VendorRecommendationID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTID
            {
                public static string Desc { get { return "FPT Ref.ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskID
            {
                public static string Desc { get { return "Task ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TCMemberID
            {
                public static string Desc { get { return "TC Member ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TCMemberName
            {
                public static string Desc { get { return "TC Member Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTVendorParticipantID
            {
                public static string Desc { get { return "FPTVendorParticipantID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class IsProposed
            {
                public static string Desc { get { return "FPT VendorParticipant ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class IsWinner
            {
                public static string Desc { get { return "IsWinner"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Remarks
            {
                public static string Desc { get { return "Remarks"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class LetterNumber
            {
                public static string Desc { get { return "LetterNumber"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CreatedDate
            {
                public static string Desc { get { return "Created Date"; } }
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
                DFPTVendorRecommendations m_objDFPTVendorRecommendations = new DFPTVendorRecommendations();
                TTCMembers m_objTTCMembers = new TTCMembers();
                MEmployee m_objMEmployee = new MEmployee();

                if (fieldName == VendorRecommendationID.Name)
                    m_strReturn = m_objDFPTVendorRecommendations.Name + "." + fieldName;
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objDFPTVendorRecommendations.Name + "." + fieldName;
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objDFPTVendorRecommendations.Name + "." + fieldName;
                else if (fieldName == TCMemberID.Name)
                    m_strReturn = m_objDFPTVendorRecommendations.Name + "." + fieldName;
                else if (fieldName == TCMemberName.Name)
                    m_strReturn =  m_objMEmployee.Name + ".LastName ";
                else if (fieldName == FPTVendorParticipantID.Name)
                    m_strReturn = m_objDFPTVendorRecommendations.Name + "." + fieldName;
                else if (fieldName == IsProposed.Name)
                    m_strReturn = m_objDFPTVendorRecommendations.Name + "." + fieldName;
                else if (fieldName == IsWinner.Name)
                    m_strReturn = m_objDFPTVendorRecommendations.Name + "." + fieldName;
                else if (fieldName == Remarks.Name)
                    m_strReturn = m_objDFPTVendorRecommendations.Name + "." + fieldName;
                else if (fieldName == LetterNumber.Name)
                    m_strReturn = m_objDFPTVendorRecommendations.Name + "." + fieldName;
                else if (fieldName == CreatedDate.Name)
                    m_strReturn = m_objDFPTVendorRecommendations.Name + "." + fieldName;
                
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
            PropertyInfo m_pifTaskTypesVM = this.GetType().GetProperty(fieldName);
            return m_pifTaskTypesVM != null && Attribute.GetCustomAttribute(m_pifTaskTypesVM, typeof(KeyAttribute)) != null;
        }
    }
}