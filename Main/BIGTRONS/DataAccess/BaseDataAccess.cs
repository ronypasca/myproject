using com.SML.Lib.Common;
using com.SML.Lib.DBController;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using SW = System.Web;

namespace com.SML.BIGTRONS.DataAccess
{
    public abstract class BaseDataAccess
    {
        #region Private Property

        private string CreatedBy { get; set; }
        private DateTime? CreatedDate { get; set; }
        private string CreatedHost { get; set; }
        private string ModifiedBy { get; set; }
        private DateTime? ModifiedDate { get; set; }
        private string ModifiedHost { get; set; }

        #endregion

        #region Protected Fields

        /// <summary>
        /// Protected field used for holding the SQL parameters
        /// </summary>
        protected List<SqlParameter> SqlParams { get; set; }

        #endregion

        #region Public Property

        public string ConnectionStringName { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public int AffectedRows { get; set; }

        public string ServerName
        {
            get
            {
                DBConnection m_objDBConnection = new DBConnection();
                m_objDBConnection.ConnectionStringName = this.ConnectionStringName;
                return m_objDBConnection.ServerName;
            }
        }

        public string DatabaseName
        {
            get
            {
                DBConnection m_objDBConnection = new DBConnection();
                m_objDBConnection.ConnectionStringName = this.ConnectionStringName;
                return m_objDBConnection.DatabaseName;
            }
        }

        public string UserID
        {
            get
            {
                DBConnection m_objDBConnection = new DBConnection();
                m_objDBConnection.ConnectionStringName = this.ConnectionStringName;
                return m_objDBConnection.UserID;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor for BaseModelBL class
        /// </summary>
        public BaseDataAccess()
        {
            SqlParams = new List<SqlParameter>();
            AffectedRows = 0;
            Success = false;
            Message = string.Empty;
        }

        #endregion

        #region Abstract Method

        public abstract bool IsKey(string fieldName);
        public abstract void Insert(bool withTrans, object dbConnection = null);
        public abstract void Select(object dbConnection = null);
        public abstract void Update(bool withTrans, object dbConnection = null);
        public abstract void Delete(bool withTrans, object dbConnection = null);
        public abstract Dictionary<int, DataSet> SelectBC(int skip, int? length, bool isCount, List<string> select, Dictionary<string, List<object>> filter,
            Dictionary<string, List<object>> having, List<string> group, Dictionary<string, OrderDirection> orderBy, object dbConnection = null);
        public abstract void UpdateBC(Dictionary<string, List<object>> set, Dictionary<string, List<object>> filter, bool withTrans, object dbConnection = null);
        public abstract void DeleteBC(Dictionary<string, List<object>> filter, bool withTrans, object dbConnection = null);
        public abstract class SelectBCData { };

        #endregion

        #region Public Method

        /// <summary>
        /// Write the setting file
        /// </summary>
        /// <param name="connectionString">The connection string to be written to setting file</param>
        public void WriteSettingFile(string connectionString, string password)
        {
            DBConnection m_objDBConnection = new DBConnection();
            m_objDBConnection.ConnectionStringName = ConnectionStringName;
            Message = "";
            m_objDBConnection.Password = password;
            m_objDBConnection.ConnectionString = connectionString;
            Message = m_objDBConnection.Message;
        }

        public object BeginConnection()
        {
            DBConnection db = new DBConnection();
            db.ConnectionStringName = ConnectionStringName;
            db.Open();
            return db;
        }

        public void EndConnection(ref object db)
        {
            ((DBConnection)db).Close();
        }

        public object BeginTrans(string TransName, IsolationLevel isoLevel = IsolationLevel.ReadCommitted)
        {
            DBConnection db = new DBConnection();
            db.ConnectionStringName = ConnectionStringName;
            db.Open();
            db.BeginTrans(TransName, isoLevel);
            return db;
        }

        public void EndTrans(ref object db, string TransName)
        {
            EndTrans(ref db, Success, TransName);
        }

        public void EndTrans(ref object db, bool success, string TransName)
        {
            try
            {
                if (success)
                    ((DBConnection)db).Commit();
                else
                    ((DBConnection)db).Rollback(TransName);
                ((DBConnection)db).Close();
            }
            catch (Exception)
            {
            }
        }

        public string ExtractSelectField(List<string> select)
        {
            string m_strReturn = string.Empty;
            if (select != null && select.Count > 0)
                m_strReturn = String.Join(", ", select);
            return m_strReturn;
        }

        public string ExtractFilterField(Dictionary<string, List<object>> filter)
        {
            bool m_bolNotIncParenthesis = false;

            List<string> m_lstFilter = new List<string>();
            StringBuilder m_stbFilter = new StringBuilder();
            if (filter != null && filter.Count > 0)
            {
                string m_strFieldName = string.Empty;
                List<object> m_lstObjectFilter = new List<object>();
                Operator m_optField;

                foreach (KeyValuePair<string, List<object>> m_kvpFilter in filter)
                {
                    m_stbFilter.Clear();
                    m_strFieldName = m_kvpFilter.Key;
                    m_lstObjectFilter = m_kvpFilter.Value;
                    m_optField = (Operator)m_lstObjectFilter[0];

                    switch (m_optField)
                    {
                        case Operator.NotEqual:
                        case Operator.NotContain:
                        case Operator.NotStartWith:
                        case Operator.NotEndWith:
                        case Operator.NotBetween:
                        case Operator.NotIn:
                            m_stbFilter.Append("NOT");
                            break;
                    }

                    m_stbFilter.Append("( " + m_strFieldName);

                    switch (m_optField)
                    {
                        case Operator.Equals:
                        case Operator.NotEqual:
                            m_stbFilter.Append(" = ");
                            break;
                        case Operator.Contains:
                        case Operator.NotContain:
                        case Operator.StartsWith:
                        case Operator.NotStartWith:
                        case Operator.EndsWith:
                        case Operator.NotEndWith:
                            m_stbFilter.Append(" LIKE ");
                            break;
                        case Operator.Between:
                        case Operator.NotBetween:
                            m_stbFilter.Append(" BETWEEN ");
                            break;
                        case Operator.LessThan:
                            m_stbFilter.Append(" < ");
                            break;
                        case Operator.LessThanEqual:
                            m_stbFilter.Append(" <= ");
                            break;
                        case Operator.GreaterThan:
                            m_stbFilter.Append(" > ");
                            break;
                        case Operator.GreaterThanEqual:
                            m_stbFilter.Append(" >= ");
                            break;
                        case Operator.In:
                        case Operator.NotIn:
                            m_stbFilter.Append(" IN ( ");
                            break;
                    }

                    switch (m_optField)
                    {
                        case Operator.None:
                            break;
                        default:
                            m_stbFilter.Append("'");
                            break;
                    }

                    switch (m_optField)
                    {
                        case Operator.Contains:
                        case Operator.NotContain:
                        case Operator.StartsWith:
                        case Operator.NotStartWith:
                            m_stbFilter.Append("%");
                            break;
                    }

                    switch (m_optField)
                    {
                        case Operator.LessThan:
                        case Operator.LessThanEqual:
                            if (m_lstObjectFilter.Count > 2)
                            {
                                m_stbFilter.Append(m_lstObjectFilter[2]);
                            }
                            else
                            {
                                m_stbFilter.Append(m_lstObjectFilter[1]);
                            }

                            break;
                        case Operator.None:
                            m_bolNotIncParenthesis = m_lstObjectFilter.Any(d => d.ToString() == "NOTPARENTHESIS");
                            break;
                        case Operator.In:
                        case Operator.NotIn:
                            string m_strFilterTemp = m_lstObjectFilter[1].ToString().Replace("'", @"''");
                            string[] m_arrStrFilterTemp = m_strFilterTemp.Split(',').Select(m_strFilter => m_strFilter.Trim()).ToArray();
                            m_strFilterTemp = String.Join("','", m_arrStrFilterTemp);
                            m_stbFilter.Append(m_strFilterTemp);
                            break;
                        default:
                            m_stbFilter.Append(m_lstObjectFilter[1].ToString().Replace("'", @"''"));
                            break;
                    }

                    switch (m_optField)
                    {
                        case Operator.Contains:
                        case Operator.NotContain:
                        case Operator.EndsWith:
                        case Operator.NotEndWith:
                            m_stbFilter.Append("%");
                            break;
                    }

                    switch (m_optField)
                    {
                        case Operator.None:
                            break;
                        default:
                            m_stbFilter.Append("'");
                            break;
                    }

                    switch (m_optField)
                    {
                        case Operator.In:
                        case Operator.NotIn:
                            m_stbFilter.Append(" )");
                            break;
                    }

                    switch (m_optField)
                    {
                        case Operator.Between:
                        case Operator.NotBetween:
                            m_stbFilter.Append(" AND '" + m_lstObjectFilter[2] + "'");
                            break;
                    }

                    m_stbFilter.Append(" )");

                    if (m_bolNotIncParenthesis)
                    {
                        m_stbFilter.Replace("(", "", 0, 1);
                        m_stbFilter.Replace(")", "", m_stbFilter.Length - 1, 1);
                    }

                    m_lstFilter.Add(m_stbFilter.ToString());
                }
            }
            return String.Join(" AND ", m_lstFilter);
        }

        public string ExtractGroupField(List<string> group)
        {
            string m_strReturn = string.Empty;
            if (group != null && group.Count > 0)
                m_strReturn = String.Join(", ", group);
            return m_strReturn;
        }

        public string ExtractOrderField(Dictionary<string, OrderDirection> order)
        {
            List<string> m_lstOrder = new List<string>();
            if (order != null && order.Count > 0)
            {
                foreach (KeyValuePair<string, OrderDirection> m_kvpOrder in order)
                    m_lstOrder.Add(m_kvpOrder.Key + " " + General.EnumDesc(m_kvpOrder.Value));
            }
            return String.Join(", ", m_lstOrder);
        }

        public string ExtractSetField(Dictionary<string, List<object>> set)
        {
            StringBuilder m_stbSet = new StringBuilder();
            if (set != null && set.Count > 0)
            {
                if (!set.ContainsKey(General.GetVariableName(() => ModifiedBy)))
                    set.Add(General.GetVariableName(() => ModifiedBy), new List<object>() { typeof(string), SW.HttpContext.Current.User.Identity.Name });
                if (!set.ContainsKey(General.GetVariableName(() => ModifiedDate)))
                    set.Add(General.GetVariableName(() => ModifiedDate), new List<object>() { typeof(DateTime), DateTime.Now });
                if (!set.ContainsKey(General.GetVariableName(() => ModifiedHost)))
                    set.Add(General.GetVariableName(() => ModifiedHost), new List<object>() { typeof(string), Global.LocalHostName });

                string m_strFieldName = string.Empty;
                List<object> m_lstSet = new List<object>();
                string m_strValue = string.Empty;

                foreach (KeyValuePair<string, List<object>> m_kvpFilter in set)
                {
                    m_strFieldName = m_kvpFilter.Key;
                    m_lstSet = m_kvpFilter.Value;
                    m_strValue = m_lstSet[1].ToString();

                    if (((Type)m_lstSet[0]) == typeof(string) || ((Type)m_lstSet[0]) == typeof(DateTime))
                        m_stbSet.Append(m_strFieldName + " = '" + m_strValue + "'");
                    else
                        m_stbSet.Append(m_strFieldName + " = " + m_strValue);
                    m_stbSet.Append(", ");
                }

                if (m_stbSet.Length > 0)
                    m_stbSet = m_stbSet.Remove(m_stbSet.Length - 2, 2);
            }
            return m_stbSet.ToString();
        }

        /// <summary>
        /// Function for getting Table's schema
        /// </summary>
        public DataTable Schema(PropertyInfo[] properties)
        {
            DataTable m_dtSchema = new DataTable();

            foreach (PropertyInfo m_pifTMeterReading in properties)
                m_dtSchema.Columns.Add(new DataColumn(m_pifTMeterReading.Name, m_pifTMeterReading.PropertyType.GenericTypeArguments.Length > 0
                    ? m_pifTMeterReading.PropertyType.GenericTypeArguments[0] : m_pifTMeterReading.PropertyType));

            return m_dtSchema;
        }

        /// <summary>
        /// Function for checking if Field is Key
        /// </summary>
        /// <param name="FieldName">Name of Field to check</param>
        public bool IsPrimaryKey(PropertyInfo propertyInfo)
        {
            return propertyInfo != null && Attribute.GetCustomAttribute(propertyInfo, typeof(KeyAttribute)) != null;
        }

        /// <summary>
        /// Function for executing query string
        /// </summary>
        /// <param name="queryString">Query to execute</param>
        /// <param name="parameters">Pairs of Parameter and its Value</param>
        /// <param name="dbConnection">Connection to use. If omit then create new connection, else use existing connection</param>
        /// <returns>List of selected record(s) or number of record(s)</returns>
        public DataSet ExecuteQueryString(string select, string tableName, string filter, Dictionary<string, object> parameters, object dbConnection = null)
        {
            string m_strSelect = "SELECT " + select.Replace("'", @"''");
            string m_strFrom = "FROM " + tableName;
            //string m_strWhere = "WHERE " + filter.Replace("'", @"''");
            string m_strWhere = "WHERE " + filter;
            string m_strQueryString = m_strSelect + " " + m_strFrom + " " + m_strWhere;

            SqlParams.Clear();
            foreach (KeyValuePair<string, object> m_kvpParameter in parameters)
            {
                SqlParams.Add(new SqlParameter("@in_" + m_kvpParameter.Key, m_kvpParameter.Value));
            }

            DBConnection m_objDBConnection;
            if (dbConnection == null)
            {
                m_objDBConnection = new DBConnection();
                m_objDBConnection.ConnectionStringName = ConnectionStringName;
            }
            else
                m_objDBConnection = (DBConnection)dbConnection;

            DataSet ds = m_objDBConnection.ExecuteQuery(CommandType.Text,
                m_strQueryString,
                dbConnection != null,
                SqlParams.ToArray());
            AffectedRows = (m_objDBConnection.Message == string.Empty ? ds.Tables[0].Rows.Count : 0);
            Success = (m_objDBConnection.Message == string.Empty);
            Message = (Success ? (AffectedRows > 0 ? m_objDBConnection.Message : General.EnumDesc(MessageLib.NotExist)) : m_objDBConnection.Message);

            return ds;
        }

        #endregion
    }
}