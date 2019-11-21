using com.SML.Lib.Common;
using com.SML.Lib.DBController;
using com.SML.BIGTRONS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace com.SML.BIGTRONS.DataAccess
{
    public class MAdditionalInfoItemsDA : BaseDataAccess
    {
        #region Public Property

        public MAdditionalInfoItems Data { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor for MAdditionalInfoItemsDA class
        /// </summary>
        public MAdditionalInfoItemsDA()
        {
            Data = new MAdditionalInfoItems();
        }
        #endregion

        #region Private Method

        #endregion

        #region Public Method

        /// <summary>
        /// Function for checking if Field is Key
        /// </summary>
        /// <param name="fieldName">Name of Field to check</param>
        public override bool IsKey(string fieldName)
        {
            PropertyInfo m_pifMAdditionalInfoItems = Data.GetType().GetProperty(fieldName);
            return IsPrimaryKey(m_pifMAdditionalInfoItems);
        }

        /// <summary>
        /// Function for inserting data
        /// </summary>
        /// <param name="withTrans">Using transaction or not. If using transaction, then dbConnection cannot be null</param>
        /// <param name="dbConnection">Connection to use. If omit then create new connection, else use existing connection</param>
        public override void Insert(bool withTrans, object dbConnection = null)
        {
            if (withTrans && dbConnection == null)
            {
                base.AffectedRows = 0;
                base.Success = false;
                base.Message = General.EnumDesc(MessageLib.TransNoConnection);
                return;
            }

            base.SqlParams.Clear();
            PropertyInfo[] m_arrPifMAdditionalInfoItems = Data.GetType().GetProperties();

            foreach (PropertyInfo m_pifMAdditionalInfoItems in m_arrPifMAdditionalInfoItems)
            {
                object m_objValue = m_pifMAdditionalInfoItems.GetValue(Data);
                base.SqlParams.Add(new SqlParameter("@in_" + m_pifMAdditionalInfoItems.Name, m_objValue));
            }
            base.SqlParams.Add(new SqlParameter("@out_RowCount", SqlDbType.Int));
            base.SqlParams[base.SqlParams.Count - 1].Direction = ParameterDirection.Output;

            DBConnection m_objDBConnection;
            if (dbConnection == null)
            {
                m_objDBConnection = new DBConnection();
                m_objDBConnection.ConnectionStringName = base.ConnectionStringName;
            }
            else
                m_objDBConnection = (DBConnection)dbConnection;

            if (withTrans)
            {
                m_objDBConnection.ExecuteUpdateTrans(CommandType.StoredProcedure,
                "ins_MAdditionalInfoItems",
                base.SqlParams.ToArray());
            }
            else
            {
                m_objDBConnection.ExecuteUpdate(CommandType.StoredProcedure,
                "ins_MAdditionalInfoItems",
                dbConnection != null,
                base.SqlParams.ToArray());
            }

            object m_intResult = base.SqlParams[base.SqlParams.Count - 1].Value;
            base.AffectedRows = (m_intResult == null ? 0 : (int)m_intResult);
            base.Success = (m_objDBConnection.Message == string.Empty);
            base.Message = (base.Success ? (base.AffectedRows > 0 ? m_objDBConnection.Message : General.EnumDesc(MessageLib.Unknown)) : m_objDBConnection.Message);
        }

        /// <summary>
        /// Function for reading data
        /// </summary>
        /// <param name="dbConnection">Connection to use. If omit then create new connection, else use existing connection</param>
        public override void Select(object dbConnection = null)
        {
            base.SqlParams.Clear();
            PropertyInfo[] m_arrPifMAdditionalInfoItems = Data.GetType().GetProperties();

            foreach (PropertyInfo m_pifMAdditionalInfoItems in m_arrPifMAdditionalInfoItems)
            {
                if (IsPrimaryKey(m_pifMAdditionalInfoItems))
                {
                    // Do something, to read from the property:
                    object m_objValue = m_pifMAdditionalInfoItems.GetValue(Data);
                    base.SqlParams.Add(new SqlParameter("@in_" + m_pifMAdditionalInfoItems.Name, m_objValue));
                }
            }

            DBConnection m_objDBConnection;
            if (dbConnection == null)
            {
                m_objDBConnection = new DBConnection();
                m_objDBConnection.ConnectionStringName = base.ConnectionStringName;
            }
            else
                m_objDBConnection = (DBConnection)dbConnection;

            SqlDataReader m_sdrData = m_objDBConnection.ExecuteReader(CommandType.StoredProcedure,
            "sel_MAdditionalInfoItems",
            dbConnection != null,
            base.SqlParams.ToArray());

            if (m_sdrData != null)
            {
                if (m_sdrData.HasRows)
                {
                    m_sdrData.Read();
                    for (int m_intFieldCount = 0; m_intFieldCount < m_sdrData.FieldCount; m_intFieldCount++)
                    {
                        Data.GetType().GetProperty(m_sdrData.GetName(m_intFieldCount)).SetValue(Data,
                            (m_sdrData.GetValue(m_intFieldCount) == DBNull.Value ? null : m_sdrData.GetValue(m_intFieldCount)));
                    }
                    base.Success = true;
                }
                else
                    base.Success = false;

                m_sdrData.Close();
            }
            else
                base.Success = false;
            base.Message = m_objDBConnection.Message;

            base.AffectedRows = (base.Success ? 1 : 0);
            base.Message = (m_objDBConnection.Message == string.Empty ?
                (base.Success ? m_objDBConnection.Message : General.EnumDesc(MessageLib.Unknown)) : m_objDBConnection.Message);

            if (dbConnection == null)
                m_objDBConnection.Close();
        }

        /// <summary>
        /// Function for updating data
        /// </summary>
        /// <param name="withTrans">Using transaction or not. If using transaction, then dbConnection cannot be null</param>
        /// <param name="dbConnection">Connection to use. If omit then create new connection, else use existing connection</param>
        public override void Update(bool withTrans, object dbConnection = null)
        {
            if (withTrans && dbConnection == null)
            {
                base.AffectedRows = 0;
                base.Success = false;
                base.Message = General.EnumDesc(MessageLib.TransNoConnection);
                return;
            }

            base.SqlParams.Clear();
            Data.PrepareModifiedInfo();
            PropertyInfo[] m_arrPifMAdditionalInfoItems = Data.GetType().GetProperties();

            foreach (PropertyInfo m_pifMAdditionalInfoItems in m_arrPifMAdditionalInfoItems)
            {
                if (IsPrimaryKey(m_pifMAdditionalInfoItems))
                {
                    // Do something, to read from the property:
                    object m_objValue = m_pifMAdditionalInfoItems.GetValue(Data);
                    base.SqlParams.Add(new SqlParameter("@in_Key" + m_pifMAdditionalInfoItems.Name, m_objValue));
                }
            }
            foreach (PropertyInfo m_pifMAdditionalInfoItems in m_arrPifMAdditionalInfoItems)
            {
                object m_objValue = m_pifMAdditionalInfoItems.GetValue(Data);
                base.SqlParams.Add(new SqlParameter("@in_" + m_pifMAdditionalInfoItems.Name, m_objValue));
            }
            base.SqlParams.Add(new SqlParameter("@out_RowCount", SqlDbType.Int));
            base.SqlParams[base.SqlParams.Count - 1].Direction = ParameterDirection.Output;

            DBConnection m_objDBConnection;
            if (dbConnection == null)
            {
                m_objDBConnection = new DBConnection();
                m_objDBConnection.ConnectionStringName = base.ConnectionStringName;
            }
            else
                m_objDBConnection = (DBConnection)dbConnection;

            if (withTrans)
            {
                m_objDBConnection.ExecuteUpdateTrans(CommandType.StoredProcedure,
                "upd_MAdditionalInfoItems",
                base.SqlParams.ToArray());
            }
            else
            {
                m_objDBConnection.ExecuteUpdate(CommandType.StoredProcedure,
                "upd_MAdditionalInfoItems",
                dbConnection != null,
                base.SqlParams.ToArray());
            }

            object m_intResult = base.SqlParams[base.SqlParams.Count - 1].Value;
            base.AffectedRows = (m_intResult == null ? 0 : (int)m_intResult);
            base.Success = (m_objDBConnection.Message == string.Empty);
            base.Message = (base.Success ? (base.AffectedRows > 0 ? m_objDBConnection.Message : General.EnumDesc(MessageLib.NotExist)) : m_objDBConnection.Message);
        }

        /// <summary>
        /// Function for deleting data
        /// </summary>
        /// <param name="withTrans">Using transaction or not. If using transaction, then dbConnection cannot be null</param>
        /// <param name="dbConnection">Connection to use. If omit then create new connection, else use existing connection</param>
        public override void Delete(bool withTrans, object dbConnection = null)
        {
            if (withTrans && dbConnection == null)
            {
                base.AffectedRows = 0;
                base.Success = false;
                base.Message = General.EnumDesc(MessageLib.TransNoConnection);
                return;
            }

            base.SqlParams.Clear();
            PropertyInfo[] m_arrPifMAdditionalInfoItems = Data.GetType().GetProperties();

            foreach (PropertyInfo m_pifMAdditionalInfoItems in m_arrPifMAdditionalInfoItems)
            {
                if (IsPrimaryKey(m_pifMAdditionalInfoItems))
                {
                    // Do something, to read from the property:
                    object m_objValue = m_pifMAdditionalInfoItems.GetValue(Data);
                    base.SqlParams.Add(new SqlParameter("@in_" + m_pifMAdditionalInfoItems.Name, m_objValue));
                }
            }
            base.SqlParams.Add(new SqlParameter("@out_RowCount", SqlDbType.Int));
            base.SqlParams[base.SqlParams.Count - 1].Direction = ParameterDirection.Output;

            DBConnection m_objDBConnection;
            if (dbConnection == null)
            {
                m_objDBConnection = new DBConnection();
                m_objDBConnection.ConnectionStringName = base.ConnectionStringName;
            }
            else
                m_objDBConnection = (DBConnection)dbConnection;

            if (withTrans)
            {
                m_objDBConnection.ExecuteUpdateTrans(CommandType.StoredProcedure,
                "del_MAdditionalInfoItems",
                base.SqlParams.ToArray());
            }
            else
            {
                m_objDBConnection.ExecuteUpdate(CommandType.StoredProcedure,
                "del_MAdditionalInfoItems",
                dbConnection != null,
                base.SqlParams.ToArray());
            }

            object m_intResult = base.SqlParams[base.SqlParams.Count - 1].Value;
            base.AffectedRows = (m_intResult == null ? 0 : (int)m_intResult);
            base.Success = (m_objDBConnection.Message == string.Empty);
            base.Message = (base.Success ? (base.AffectedRows > 0 ? m_objDBConnection.Message : General.EnumDesc(MessageLib.NotExist)) : m_objDBConnection.Message);
        }

        /// <summary>
        /// Function for reading data by criteria
        /// </summary>
        /// <param name="skip">Skip the first x records(s)</param>
        /// <param name="length">How many record(s) to select. Set null to select all</param>
        /// <param name="isCount">Whether to just get the count of the record(s) or the record(s)</param>
        /// <param name="select">List of field(s) to select</param>
        /// <param name="filter">Criteria of the record(s) to select</param>
        /// <param name="having">Agregate criteria of the record(s) to select</param>
        /// <param name="group">Grouping of data</param>
        /// <param name="orderBy">Order of the record(s)</param>
        /// <param name="dbConnection">Connection to use. If omit then create new connection, else use existing connection</param>
        /// <returns>List of selected record(s) or number of record(s)</returns>
        public override Dictionary<int, DataSet> SelectBC(int skip, int? length, bool isCount, List<string> select, Dictionary<string, List<object>> filter,
            Dictionary<string, List<object>> having, List<string> group, Dictionary<string, OrderDirection> orderBy, object dbConnection = null)
        {
            base.SqlParams.Clear();
            base.SqlParams.Add(new SqlParameter("@in_Skip", isCount ? 0 : skip));
            if (length != null)
                base.SqlParams.Add(new SqlParameter("@in_Length", isCount ? 0 : length));
            base.SqlParams.Add(new SqlParameter("@in_IsCount", isCount));

            string m_strSelect = string.Empty;
            string m_strFilter = string.Empty;
            string m_strHaving = string.Empty;
            string m_strGroup = string.Empty;
            string m_strOrderBy = string.Empty;

            m_strSelect = ExtractSelectField(select);
            if (m_strSelect.Length > 0)
                base.SqlParams.Add(new SqlParameter("@in_Select", m_strSelect));

            m_strFilter = ExtractFilterField(filter);
            if (m_strFilter.Length > 0)
                base.SqlParams.Add(new SqlParameter("@in_Filter", m_strFilter));

            m_strHaving = ExtractFilterField(having);
            if (m_strHaving.Length > 0)
                base.SqlParams.Add(new SqlParameter("@in_Having", m_strHaving));

            m_strGroup = ExtractGroupField(group);
            if (m_strGroup.Length > 0)
                base.SqlParams.Add(new SqlParameter("@in_GroupBy", m_strGroup));

            m_strOrderBy = ExtractOrderField(orderBy);
            if (m_strOrderBy.Length > 0)
                base.SqlParams.Add(new SqlParameter("@in_OrderBy", m_strOrderBy));

            Dictionary<int, DataSet> m_dicData = new Dictionary<int, DataSet>();
            DBConnection m_objDBConnection;
            if (dbConnection == null)
            {
                m_objDBConnection = new DBConnection();
                m_objDBConnection.ConnectionStringName = base.ConnectionStringName;
            }
            else
                m_objDBConnection = (DBConnection)dbConnection;

            if (isCount)
            {
                object n = m_objDBConnection.ExecuteScalar(CommandType.StoredProcedure,
                "sel_MAdditionalInfoItems_BC",
                dbConnection != null,
                base.SqlParams.ToArray());

                base.AffectedRows = (n == null ? 0 : Convert.ToInt32(n));
                m_dicData.Add(base.AffectedRows, null);
            }
            else
            {
                DataSet ds = m_objDBConnection.ExecuteQuery(CommandType.StoredProcedure,
                "sel_MAdditionalInfoItems_BC",
                dbConnection != null,
                base.SqlParams.ToArray());

                base.AffectedRows = (m_objDBConnection.Message == string.Empty ? ds.Tables[0].Rows.Count : 0);
                m_dicData.Add(0, ds);
            }
            base.Success = (m_objDBConnection.Message == string.Empty);
            base.Message = (base.Success ? (base.AffectedRows > 0 ? m_objDBConnection.Message : General.EnumDesc(MessageLib.NotExist)) : m_objDBConnection.Message);

            return m_dicData;
        }

        /// <summary>
        /// Function for updating data by criteria
        /// </summary>
        /// <param name="set">List of field(s) to set with its corresponding value</param>
        /// <param name="filter">Criteria of the field(s) to update</param>
        /// <param name="withTrans">Using transaction or not. If using transaction, then dbConnection cannot be null</param>
        /// <param name="dbConnection">Connection to use. If omit then create new connection, else use existing connection</param>
        public override void UpdateBC(Dictionary<string, List<object>> set, Dictionary<string, List<object>> filter, bool withTrans, object dbConnection = null)
        {
            if (withTrans && dbConnection == null)
            {
                base.AffectedRows = 0;
                base.Success = false;
                base.Message = General.EnumDesc(MessageLib.TransNoConnection);
                return;
            }

            base.SqlParams.Clear();
            string m_strSet = string.Empty;
            string m_strFilter = string.Empty;

            m_strSet = ExtractSetField(set);
            if (m_strSet.Length > 0)
                base.SqlParams.Add(new SqlParameter("@in_Set", m_strSet));

            m_strFilter = ExtractFilterField(filter);
            if (m_strFilter.Length > 0)
                base.SqlParams.Add(new SqlParameter("@in_Filter", m_strFilter));

            base.SqlParams.Add(new SqlParameter("@out_RowCount", SqlDbType.Int));
            base.SqlParams[base.SqlParams.Count - 1].Direction = ParameterDirection.Output;

            DBConnection m_objDBConnection;
            if (dbConnection == null)
            {
                m_objDBConnection = new DBConnection();
                m_objDBConnection.ConnectionStringName = base.ConnectionStringName;
            }
            else
                m_objDBConnection = (DBConnection)dbConnection;

            if (withTrans)
            {
                m_objDBConnection.ExecuteUpdateTrans(CommandType.StoredProcedure,
                "upd_MAdditionalInfoItems_BC",
                base.SqlParams.ToArray());
            }
            else
            {
                m_objDBConnection.ExecuteUpdate(CommandType.StoredProcedure,
                "upd_MAdditionalInfoItems_BC",
                dbConnection != null,
                base.SqlParams.ToArray());
            }

            object m_intResult = base.SqlParams[base.SqlParams.Count - 1].Value;
            base.AffectedRows = (m_intResult == null ? 0 : (int)m_intResult);
            base.Success = (m_objDBConnection.Message == string.Empty);
            base.Message = (base.Success ? (base.AffectedRows > 0 ? m_objDBConnection.Message : General.EnumDesc(MessageLib.NotExist)) : m_objDBConnection.Message);
        }

        /// <summary>
        /// Function for deleting data by criteria
        /// </summary>
        /// <param name="filter">Criteria of the field(s) to delete</param>
        /// <param name="withTrans">Using transaction or not. If using transaction, then dbConnection cannot be null</param>
        /// <param name="dbConnection">Connection to use. If omit then create new connection, else use existing connection</param>
        public override void DeleteBC(Dictionary<string, List<object>> filter, bool withTrans, object dbConnection = null)
        {
            if (withTrans && dbConnection == null)
            {
                base.AffectedRows = 0;
                base.Success = false;
                base.Message = General.EnumDesc(MessageLib.TransNoConnection);
                return;
            }

            base.SqlParams.Clear();
            PropertyInfo[] m_arrPifMAdditionalInfoItems = Data.GetType().GetProperties();

            string m_strFilter = ExtractFilterField(filter);
            if (m_strFilter.Length > 0)
                this.SqlParams.Add(new SqlParameter("@in_Filter", m_strFilter));
            base.SqlParams.Add(new SqlParameter("@out_RowCount", SqlDbType.Int));
            base.SqlParams[base.SqlParams.Count - 1].Direction = ParameterDirection.Output;

            DBConnection m_objDBConnection;
            if (dbConnection == null)
            {
                m_objDBConnection = new DBConnection();
                m_objDBConnection.ConnectionStringName = base.ConnectionStringName;
            }
            else
                m_objDBConnection = (DBConnection)dbConnection;

            if (withTrans)
            {
                m_objDBConnection.ExecuteUpdateTrans(CommandType.StoredProcedure,
                "del_MAdditionalInfoItems_BC",
                base.SqlParams.ToArray());
            }
            else
            {
                m_objDBConnection.ExecuteUpdate(CommandType.StoredProcedure,
                "del_MAdditionalInfoItems_BC",
                dbConnection != null,
                base.SqlParams.ToArray());
            }

            object m_intResult = base.SqlParams[base.SqlParams.Count - 1].Value;
            base.AffectedRows = (m_intResult == null ? 0 : (int)m_intResult);
            base.Success = (m_objDBConnection.Message == string.Empty);
            base.Message = (base.Success ? (base.AffectedRows > 0 ? m_objDBConnection.Message : General.EnumDesc(MessageLib.NotExist)) : m_objDBConnection.Message);
        }

        #endregion
    }
}