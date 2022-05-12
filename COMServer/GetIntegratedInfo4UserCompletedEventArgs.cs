using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace CServiceClientLib.Service
{
    [GeneratedCode("System.Web.Services", "4.8.4084.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    public class GetIntegratedInfo4UserCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public int Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (int)results[0];
            }
        }

        public string CurDateTime
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[1];
            }
        }

        public string Response
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[2];
            }
        }

        public int NumDB
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (int)results[3];
            }
        }

        public int NumSystems
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (int)results[4];
            }
        }

        public int NumRecords
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (int)results[5];
            }
        }

        internal GetIntegratedInfo4UserCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}