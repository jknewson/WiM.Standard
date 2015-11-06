//------------------------------------------------------------------------------
//----- PipeLineContributors ---------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2016 WiM - USGS

//    authors:  Jeremy Newson          
//  
//   purpose:   Add error response if exception is thrown.
//
//discussion:   
//
//     
#region Comments
// 10.13.15 - JKN - Created
#endregion
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

using OpenRasta.Pipeline;
using OpenRasta.Web;

namespace WiM.PipeLineContributors
{
    public class ErrorCheckingContributor:IPipelineContributor
    {
        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner
                .Notify(CheckRequestDecoding)
                .After<KnownStages.IOperationResultInvocation>()
                .And.Before<KnownStages.ICodecResponseSelection>();
        }

        private static PipelineContinuation CheckRequestDecoding(ICommunicationContext context)
        {
            if (context.ServerErrors.Count == 0)
            {
                return PipelineContinuation.Continue;
            }

            var first = context.ServerErrors[0];
         
                context.Response.Entity.ContentType = MediaType.TextPlain;
                context.Response.Entity.ContentLength = first.Exception.Message.Length;
                using (var sw = new StreamWriter(context.Response.Entity.Stream))
                {
                    sw.Write(first.Exception.Message);
                }

            return PipelineContinuation.Continue;
        } 

    }
}