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
    public class FPTNegotiationRoundVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string RoundID { get; set; }
        [Key, Column(Order = 1)]
        public string FPTID { get; set; }
        public string FPTDesc { get; set; }
        public DateTime? StartDateTimeStamp { get; set; }
        public DateTime? EndDateTimeStamp { get; set; }

        public int? Duration { get; set; }
        public int? RoundRemaining { get { return TotalRound - RoundNo; } }
        public int? TotalRound { get; set; }
        public int? NextRound { get { return (RoundNo + 1) > TotalRound ? null : RoundNo + 1; } }
        public int? RoundNo { get; set; }
        public int? TotalVendors { get; set; }
        public DateTime? Schedule { get; set; }
        public string NextRoundID { get; set; }
        public string PreviousRoundID { get; set; }

        public string Status { get; set; }

        public string AdditionalInfo1Desc { get; set; }

        public string LiveStatus
        {
            get
            {
                string retVal = string.Empty;

                if (StartDateTimeStamp == new DateTime(9999, 12, 31, 0, 0, 0) || EndDateTimeStamp == new DateTime(9999, 12, 31, 0, 0, 0) || StartDateTimeStamp == null || EndDateTimeStamp == null)
                {
                    retVal = "Not Active";
                }
                else
                {
                    if (DateTime.Now >= StartDateTimeStamp && DateTime.Now <= EndDateTimeStamp)
                    {
                        retVal = "Running";
                    }
                    else
                    {
                        retVal = "End";
                    }
                }
                return retVal;
            }
        }
        public string FPTWinnerStatus { get; set; }
        public string TRMLead { get; set; }
        public List<FPTNegotiationRoundVM> ListNegotiationRoundsVM { get; set; }

        public string Remarks { get; set; }
        public string TimeRemaining { get; set; }

        public DateTime FPTScheduleStart { get; set; }
        public DateTime FPTScheduleEnd { get; set; }
        public string FPTDuration { get; set; }
        public string MaintenancePeriod { get; set; }
        public string Guarantee { get; set; }
        public string ContractType { get; set; }
        public string PaymentMethod { get; set; }

        public string ProjectUpload { get; set; }
        public string BudgetPlanUpload { get; set; }

        public decimal TopValue { get; set; }
        public decimal BottomValue { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class AdditionalInfo1Desc
            {
                public static string Desc { get { return "AdditionalInfo1Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RoundID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTID
            {
                public static string Desc { get { return "FPT ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTDesc
            {
                public static string Desc { get { return "Descriptions"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Descriptions
            {
                public static string Desc { get { return "Descriptions"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class StartDateTimeStamp
            {
                public static string Desc { get { return "Last Round Start"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class EndDateTimeStamp
            {
                public static string Desc { get { return "Last Round End"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TotalVendors
            {
                public static string Desc { get { return "Total Vendors"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Duration
            {
                public static string Desc { get { return "Duration"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class RoundRemaining
            {
                public static string Desc { get { return "Round(s) Remaining"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Status
            {
                public static string Desc { get { return "Status"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class LiveStatus
            {
                public static string Desc { get { return "Status"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Schedule
            {
                public static string Desc { get { return "Schedule"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NextRoundID
            {
                public static string Desc { get { return "NextRound ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class PreviousRoundID
            {
                public static string Desc { get { return "Previous Round ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RoundTime
            {
                public static string Desc { get { return "RoundTime"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Round
            {
                public static string Desc { get { return "Total Round(s)"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class RoundNo
            {
                public static string Desc { get { return "Round No"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class NextRound
            {
                public static string Desc { get { return "Next Round"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ListVendorParticipant
            {
                public static string Desc { get { return "Vendor Participant"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListTCParticipant
            {
                public static string Desc { get { return "TC Participant"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TaskStatusID
            {
                public static string Desc { get { return "Task Status ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TaskStatus
            {
                public static string Desc { get { return "Task Status"; } }
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

            public static class TotalRound
            {
                public static string Desc { get { return "Total Round"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TimeRemaining
            {
                public static string Desc { get { return "Time Remaining"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TopValue
            {
                public static string Desc { get { return "Upper Limit (%)"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class BottomValue
            {
                public static string Desc { get { return "Lower Limit (%)"; } }
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
                DFPTNegotiationRounds m_objDFPTNegotiationRounds = new DFPTNegotiationRounds();
                DFPTVendorParticipants m_objDFPTVendorParticipants = new DFPTVendorParticipants();
                CNegotiationConfigurations m_objCNegotiationConfigurations = new CNegotiationConfigurations();
                MFPT m_objMFPT = new MFPT();

                if (fieldName == RoundID.Name)
                    m_strReturn = m_objDFPTNegotiationRounds.Name + "." + fieldName;
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objDFPTNegotiationRounds.Name + "." + fieldName;
                else if (fieldName == FPTDesc.Name)
                    m_strReturn = m_objMFPT.Name + "." + FPTVM.Prop.Descriptions.Name;
                else if (fieldName == StartDateTimeStamp.Name)
                    m_strReturn = m_objDFPTNegotiationRounds.Name + "." + fieldName;
                else if (fieldName == EndDateTimeStamp.Name)
                    m_strReturn = m_objDFPTNegotiationRounds.Name + "." + fieldName;
                else if (fieldName == TotalVendors.Name)
                    m_strReturn = m_objDFPTVendorParticipants.Name + "." + fieldName;
                else if (fieldName == Duration.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + RoundTime.Name;
                else if (fieldName == Round.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == RoundNo.Name)
                    m_strReturn = m_objDFPTNegotiationRounds.Name + "." + fieldName;
                else if (fieldName == Schedule.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == NextRoundID.Name)
                    m_strReturn = m_objDFPTNegotiationRounds.Name + "." + fieldName;
                else if (fieldName == PreviousRoundID.Name)
                    m_strReturn = m_objDFPTNegotiationRounds.Name + "." + fieldName;
                else if (fieldName == TaskStatus.Name)
                    m_strReturn = m_objDFPTNegotiationRounds.Name + "." + fieldName;
                else if (fieldName == TaskStatusID.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == Remarks.Name)
                    m_strReturn = m_objDFPTNegotiationRounds.Name + "." + fieldName;
                else if (fieldName == TopValue.Name)
                    m_strReturn = m_objDFPTNegotiationRounds.Name + "." + fieldName;
                else if (fieldName == BottomValue.Name)
                    m_strReturn = m_objDFPTNegotiationRounds.Name + "." + fieldName;

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