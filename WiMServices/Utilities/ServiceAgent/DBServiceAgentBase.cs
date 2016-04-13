using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using WiM.Security;
using WiM.Resources;

using System.Collections.Generic;

namespace WiM.Utilities.ServiceAgent
{
    public abstract class DBAgentBase:IDisposable
    {
        #region "Events"
        #endregion
        #region Fields
        protected string connectionString;
        #endregion
        #region "Properties"
        public DbContext context { get; protected set; }
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
        public DBAgentBase(string connectionstring)
        {
            this.connectionString = connectionstring;
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
                    this.connectionString = string.Empty;
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
        public IQueryable<T> Select<T>() where T :class,new()
        {
            DbSet<T> set = GetDBSet(typeof(T)).GetValue(this.context, null) as DbSet<T>;            
            return set;

        }
        public T Add<T>(T item) where T : class,new()
        {
            DbSet<T> set = GetDBSet(typeof(T)).GetValue(context, null) as DbSet<T>;
            if (set.AsEnumerable().Contains(item)) {                
                sm(MessageType.warning, "Item already exists");
                return set.AsEnumerable<T>().FirstOrDefault(i => item.Equals(i));
            }
            set.Add(item);
            context.SaveChanges();            
            return item;
        }
        public T Update<T>(T item) where T : class,new()
        {
            DbSet<T> set = GetDBSet(typeof(T)).GetValue(context, null) as DbSet<T>;
            
            set.Attach(item);            
            //set state to modified to force the update.
            context.Entry(item).State = EntityState.Modified;

            context.SaveChanges();
            sm(MessageType.info, "Item found and updated."); 
            return item;
        }
        public void Delete<T>(T item) where T : class,new()
        {
            DbSet<T> set = GetDBSet(typeof(T)).GetValue(context, null) as DbSet<T>;

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
        private PropertyInfo GetDBSet(Type itemType) 
        {
            var properties = this.context.GetType().GetProperties().Where(item => item.PropertyType.Equals(typeof(DbSet<>).MakeGenericType(itemType)));
            return properties.First();
        }
    
        protected void sm(MessageType t,string msg)
        {
            this._message.Add(new Message(){ type = t, msg=msg });
        }
        protected void sm(List<Message> msg)
        {
            this._message.AddRange(msg);
        }
        #endregion
        #region "Structures"
        //A structure is a value type. When a structure is created, the variable to which the struct is assigned holds
        //the struct's actual data. When the struct is assigned to a new variable, it is copied. The new variable and
        //the original variable therefore contain two separate copies of the same data. Changes made to one copy do not
        //affect the other copy.

        //In general, classes are used to model more complex behavior, or data that is intended to be modified after a
        //class object is created. Structs are best suited for small data structures that contain primarily data that is
        //not intended to be modified after the struct is created.
        #endregion
        #region "Asynchronous Methods"

        #endregion
        #region "Enumerated Constants"
        #endregion
    }//end class
}//end namespace
