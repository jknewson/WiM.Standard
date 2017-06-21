using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using WiM.Resources;

namespace WiM.Utilities
{
    public abstract class DBAgentBase : IDisposable, IMessage
    {

        #region "Properties"
        protected DbContext context { get; private set; }
        #endregion
        #region "Collections & Dictionaries"
        private List<Message> _message = new List<Message>();
        public List<Message> Messages
        {
            get { return _message.Distinct().ToList(); }
        }
        #endregion
        #region "Constructor and IDisposable Support"
        #region Constructors
        public DBAgentBase(DbContext context)
        {
            this.context = context;
        }
        #endregion
        #region IDisposable Support
        // Track whether Dispose has been called.
        protected bool disposed = false;

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
                    if (this.context != null) this.context.Dispose();
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
        protected IQueryable<T> Select<T>() where T : class, new()
        {
            DbSet<T> set = GetDBSet<T>();
            return set;
        }
        protected IQueryable<T> FromSQL<T>(string sql, object[] parameters) where T : class, new()
        {
            DbSet<T> set = GetDBSet<T>();
            return set.FromSql(sql,parameters);
        }
        protected T Find<T>(Int32 pk) where T : class, new()
        {
            DbSet<T> set = GetDBSet<T>();
            return set.Find(pk);
        }
        protected T Add<T>(T item) where T : class, new()
        {
            DbSet<T> set = GetDBSet<T>();
            if (set.AsEnumerable().Contains(item))
            {
                sm(MessageType.warning, "Item already exists");
                return set.AsEnumerable<T>().FirstOrDefault(i => i.Equals(item));
            }
            set.Add(item);
            context.SaveChanges();
            return item;
        }
        protected T Update<T>(Int32 pkId, T item) where T : class, new()
        {
            DbSet<T> set = GetDBSet<T>();
            if (!setPKfield<T>(item, pkId)) return null;

            set.Attach(item);
            //set state to modified to force the update.
            context.Entry(item).State = EntityState.Modified;

            context.SaveChanges();
            sm(MessageType.info, "Item found and updated.");
            return this.Find<T>(pkId);
        }
        protected void Delete<T>(T item) where T : class, new()
        {
            //DbSet<T> set = GetDBSet(typeof(T)).GetValue(context, null) as DbSet<T>;
            //set.Remove(item);
            var entry = context.Entry(item);
            if (entry != null)
            {
                //set state to modified to force the update.
                context.Entry(item).State = EntityState.Deleted;
                context.SaveChanges();
            }//end if

        }
        #endregion
        #region "Helper Methods"        
        private DbSet<T> GetDBSet<T>() where T : class, new()
        {
            var properties = this.context.GetType().GetTypeInfo().DeclaredProperties.Where(item => item.PropertyType.Equals(typeof(DbSet<>).MakeGenericType(typeof(T))));
            return properties.First().GetValue(context, null) as DbSet<T>; ;
        }
        private bool setPKfield<T>(T obj, object value) where T : class, new()
        {
            try
            {
                
                var keyname = context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.Select(x => x.Name).Single();
                var prop = obj.GetType().GetTypeInfo().DeclaredProperties.Where(k => k.Name == keyname).Single();
                if (prop != null && prop.CanWrite && prop.GetValue(obj) != value)
                    prop.SetValue(obj, value, null);

                return true;
            }
            catch (Exception)
            {
                sm(MessageType.error, "Failed to set pk.");
                return false;
            }
        }
        protected void sm(MessageType t, string msg)
        {
            this._message.Add(new Message() { type = t, msg = msg });
        }
        protected void sm(List<Message> msg)
        {
            this._message.AddRange(msg);
        }
        #endregion
    }//end class
}//end namespace
