//------------------------------------------------------------------------------
//----- ########## -------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2015 WiM - USGS

//    authors:  Jeremy K. Newson USGS Wisconsin Internet Mapping
//             
// 
//   purpose:  
//          
//discussion:
//

#region "Comments"
//04.23.2015 jkn - Created
#endregion

#region "Imports"
using System;
using System.Data;
using System.Xml;
using System.Xml.XPath;
using System.IO;



#endregion
namespace WiM.Utilities.ServiceAgent
{
public abstract class XMLAgentBase
{

#region " asynchronous delegates "
// these asynchronous delegates are pointers to procedures. we use the delegate's BeginInvoke
// method to call the procedure asynchronously or we use the delegate's Invoke method to call
// the routine synchronously. the delegate's BeginInvoke method takes the same arguments as
// the procedure that the delegate points to, plus two additional arguments. Unlike the Invoke method,
// BeginInvoke returns an IAsyncResult. The returned IAsyncResult's IsCompleted property is then
// queried and if this property returns true then the delegate's EndInvoke method is called. this
// method is used to retrieve both the return value and the value of any argument passed byRef.

// there is one delegate declared for each procedure.
#endregion
#region "Events"
#endregion
#region "Fields"
    private XPathNavigator XNav;
#endregion
#region "Properties"   
    public string XMLFile { get; private set; }
#endregion
#region "Collections & Dictionaries"
#endregion
#region "Constructor and IDisposable Support"
#region Constructors   
    public XMLAgentBase(string file)
    {
        if (!File.Exists(file)) throw new Exception("file doesn't exist. " + file);
        this.XMLFile = file;
        loadXMLNavigator();
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
#endregion
#region "Helper Methods"
    private void loadXMLNavigator() {
        XPathDocument xdoc = new XPathDocument(this.XMLFile);
        this.XNav = xdoc.CreateNavigator();
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

}//end class XMLAgentBase
}//end namespace
