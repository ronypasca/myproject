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
    public class DBConfigDA : BaseDataAccess
    {
        #region Public Property

        public DBConfig Data { get; set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor for DBConfigDA class
        /// </summary>
        public DBConfigDA()
        {
            Data = new DBConfig();
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
            PropertyInfo m_pifDBConfig = Data.GetType().GetProperty(fieldName);
            return IsPrimaryKey(m_pifDBConfig);
        }

        /// <summary>
        /// Function for inserting data
        /// </summary>
        /// <param name="withTrans">Using transaction or not. If using transaction, then dbConnection cannot be null</param>
        /// <param name="dbConnection">Connection to use. If omit then create new connection, else use existing connection</param>
        public override void Insert(bool withTrans, object dbConnection = null)
        {
           
        }

        /// <summary>
        /// Function for reading data
        /// </summary>
        /// <param name="dbConnection">Connection to use. If omit then create new connection, else use existing connection</param>
        public override void Select(object dbConnection = null)
        {
            
        }

        /// <summary>
        /// Function for updating data
        /// </summary>
        /// <param name="withTrans">Using transaction or not. If using transaction, then dbConnection cannot be null</param>
        /// <param name="dbConnection">Connection to use. If omit then create new connection, else use existing connection</param>
        public override void Update(bool withTrans, object dbConnection = null)
        {
            
        }

        /// <summary>
        /// Function for deleting data
        /// </summary>
        /// <param name="withTrans">Using transaction or not. If using transaction, then dbConnection cannot be null</param>
        /// <param name="dbConnection">Connection to use. If omit then create new connection, else use existing connection</param>
        public override void Delete(bool withTrans, object dbConnection = null)
        {
            
        }

        public override Dictionary<int, DataSet> SelectBC(int skip, int? length, bool isCount, List<string> select, Dictionary<string, List<object>> filter, Dictionary<string, List<object>> having, List<string> group, Dictionary<string, OrderDirection> orderBy, object dbConnection = null)
        {
            throw new NotImplementedException();
        }

        public override void UpdateBC(Dictionary<string, List<object>> set, Dictionary<string, List<object>> filter, bool withTrans, object dbConnection = null)
        {
            throw new NotImplementedException();
        }

        public override void DeleteBC(Dictionary<string, List<object>> filter, bool withTrans, object dbConnection = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}