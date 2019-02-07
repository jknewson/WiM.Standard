//------------------------------------------------------------------------------
//----- ExternalProcessServiceAgentBase ----------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2012 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   The service agent is responsible for initiating to a stand alone 
//              process using a process shell request, capturing the data that's 
//              returned and forwarding the data back to the requestor.
//
//discussion:   delegated hunting and gathering responsibilities.   
//
//  
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using WiM.Utilities.Resources;
using System.Linq;

namespace WiM.Utilities.ServiceAgent
{
    public abstract class ExternalProcessServiceAgentBase
    {
        #region "Events"

        #endregion

        #region Properties & Fields
        public string BaseEXE { get; private set; }
        public string BasePath { get; private set; }
        public string ExecuteMessages { get; private set; }
        #endregion

        #region Constructors
        protected ExternalProcessServiceAgentBase(string baseExe = null, string basePath = null)
        {
            this.BaseEXE = baseExe;
            this.BasePath = basePath;
        }

        #endregion

        #region Methods
        protected void ExecuteAsync(ProcessStartInfo psi, Action<string> CallBackOnSuccess, Action<string> CallBackOnFail)
        {
            String response = null;
            Process task = null;
            try
            {

                if (psi == null) throw new ArgumentNullException("processInfo");
                task = new Process();
                task = Process.Start(psi);
                response = task.StandardOutput.ReadToEnd();
                var err = task.StandardError.ReadToEnd();

                if (!String.IsNullOrEmpty(response))
                {
                    CallBackOnSuccess(response);
                }//else
                else
                {
                    CallBackOnFail(psi.FileName + " response is null, Errors:" + err);
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //local clean up
                if (task != null) { task.Dispose(); task = null; }
            }


        }//end ExecuteAsync<T>

        protected ProcessResponse Execute<T>(ProcessStartInfo psi)
        {
            ProcessResponse response = null;
            Process task = null;
            try
            {
                if (psi == null) throw new ArgumentNullException("processInfo");
                task = new Process();
                task = Process.Start(psi);
                response = new ProcessResponse()
                {
                    Output = task.StandardOutput.ReadToEnd(),
                    Errors = task.StandardError.ReadToEnd()
                };                
                task.WaitForExit();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing " + psi.Arguments + " & " + psi.FileName + " dir " + psi.WorkingDirectory + " error1 msg: " + ex.Message);
            }
            finally
            {
                //local clean up
                if (task != null) { task.Close(); task.Dispose(); task = null; }
            }
        }//endExecute

        #endregion
        #region Helper Methods
        protected ProcessStartInfo getProcessRequest(string filename=null, string args=null)
        {

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = this.BaseEXE;
            psi.Arguments = string.Format("{0} {1}", filename, args);
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            psi.WorkingDirectory = this.BasePath;
            psi.WindowStyle = ProcessWindowStyle.Normal;

            return psi;
        }//end BuildRestRequest
        #endregion

    }
}