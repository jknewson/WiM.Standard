﻿//------------------------------------------------------------------------------
//----- dbOps -------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2015 WiM - USGS

//    authors:  Jeremy K. Newson USGS Wisconsin Internet Mapping
//             
// 
//   purpose: Manage databases, provides retrieval/creation/update/deletion
//          
//discussion:
//

#region "Comments"
//02.09.2015 jkn - Created
#endregion

#region "Imports"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using WIM.Utilities.Extensions;
using System.Diagnostics;


#endregion

namespace WIM.Utilities
{
    public abstract class dbOps : IDisposable
    {
        #region "Fields"
        private string connectionString = string.Empty;
        private DbConnection connection;
        public ConnectionType connectionType { get; private set; }
        #endregion
        #region Properties
        private List<string> _message = new List<string>();
        public List<string> Messages
        {
            get { return _message; }
        }
        #endregion
        #region "Constructor and IDisposable Support"
        #region Constructors
        public dbOps(string pSQLconnstring, ConnectionType pConnectionType, bool doResetTables = false)
        {
            this.connection = null;
            this.connectionString = pSQLconnstring;
            this.connectionType = pConnectionType;
            init();
            if (doResetTables) this.ResetTables();
        }
        #endregion
        #region IDisposable Support
        // Track whether Dispose has been called.
        private bool disposed = false;

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        } //End Dispose

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                if (disposing)
                {
                    // TODO:Dispose managed resources here.
                    if (this.connection.State != ConnectionState.Closed) this.connection.Close();
                    this.connection.Dispose();

                    //ie component.Dispose();

                }//EndIF

                // TODO:Call the appropriate methods to clean up
                // unmanaged resources here.
                //ComRelease(Extent);

                // Note disposing has been done.
                disposed = true;


            }//EndIf
        }//End Dispose
        #endregion
        #endregion
        #region "Methods"
        public virtual List<T> GetDBItems<T>(Int32 SqlTypeIdentifyer, params object[] args)
        {
            List<T> dbList = null;
            string sql = string.Empty;
            try
            {
                sql = string.Format(getSQL(SqlTypeIdentifyer), args);

                this.OpenConnection();
                DbCommand command = getCommand(sql);
                Func<IDataReader, T> fromdr = (Func<IDataReader, T>)Delegate.CreateDelegate(typeof(Func<IDataReader, T>), null, typeof(T).GetMethod("FromDataReader"));

                using (DbDataReader reader = command.ExecuteReader())
                {
                    dbList = reader.Select<T>(fromdr).ToList();
                    sm("DB return count: " + dbList.Count);
                }//end using

                return dbList;
            }
            catch (Exception ex)
            {
                this.sm(ex.Message);
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }
        public virtual bool Update(Int32 SqlTypeIdentifyer, Int32 pkID, Object[] args)
        {
            string sql = string.Empty;
            try
            {
                this.OpenConnection();
                DbCommand command = getCommand(String.Format(getSQL(SqlTypeIdentifyer), pkID, args));
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                this.sm(ex.Message);
                return false;
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }
        public virtual void ExecuteSql(string sql)
        {
            try
            {
                this.OpenConnection();
                DbCommand command = getCommand(sql);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                this.sm(ex.Message);
                throw ex;
            }
            finally
            {
                this.CloseConnection();
            }
        }
        public virtual void ExecuteSql(FileInfo file)
        {
            try
            {
                List<string> sqlList = null;

                using (StreamReader reader = new StreamReader(file.FullName))
                {
                    //sqlList = Regex.Split(reader.ReadToEnd(), @"(?<=[;])").ToList();   
                    ExecuteSql(reader.ReadToEnd());

                }//end using   
                //sm($"Count: {sqlList.Count}");
                //this.OpenConnection();
                //for (int i = 0; i < sqlList.Count; i++)
                //{
                //    var sql = sqlList[i];
                //    using (DbCommand command = getCommand(sql))
                //    {
                //        command.CommandTimeout = 2*60;// timeout
                //        var updatedRows = command.ExecuteNonQuery();
                //        sm($"Updated {i}, out of {sqlList.Count}");
                //    }//end using                        
                //}//next item                
            }
            catch (Exception ex)
            {
                this.sm(ex.Message);
                throw ex;
            }
            finally
            {
                //this.CloseConnection();
            }
        }
        public virtual Int32 AddItem(Int32 SqlTypeIdentifyer, Object[] args)
        {
            string sql = string.Empty;
            try
            {
                args = args.Select(a => a == null ? "null" : a).ToArray();
                sql = String.Format(getSQL(SqlTypeIdentifyer), args);

                this.OpenConnection();
                DbCommand command = getCommand(sql);
                command.ExecuteNonQuery();
                command.CommandText = "SELECT lastval();";
                var id = command.ExecuteScalar();
                return Convert.ToInt32(id);
            }
            catch (Exception ex)
            {
                this.sm(ex.Message);
                return -1;
            }
            finally
            {
                this.CloseConnection();
            }
        }
        public abstract bool ResetTables();
        #endregion
        #region "Helper Methods"
        protected virtual T FromDataReader<T>(IDataReader r)
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { });
        }
        protected abstract DbCommand getCommand(string sql);
        protected abstract string getSQL(Int32 SqlTypeIdentifyer);

        protected virtual void OpenConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Open) this.connection.Close();
                this.connection.Open();
            }
            catch (Exception ex)
            {
                this.CloseConnection();
                throw ex;
            }
        }
        protected virtual void CloseConnection()
        {
            try
            {
                if (this.connection.State == ConnectionState.Open) this.connection.Close();
            }
            catch (Exception ex)
            {
                if (this.connection.State == ConnectionState.Open) connection.Close();
                throw ex;
            }
        }
        protected abstract void init();
        protected virtual void sm(string msg)
        {
            Console.WriteLine(msg);
            Debug.Print(msg);
            this._message.Add(msg);
        }
        #endregion
        #region "Enumerated Constants"

        public enum ConnectionType
        {
            e_access,
            e_postgresql,
            e_mysql
        }
        #endregion

    }
}
