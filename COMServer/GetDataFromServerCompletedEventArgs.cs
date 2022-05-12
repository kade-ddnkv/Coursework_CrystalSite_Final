using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace CServiceClientLib.Service
{
    [GeneratedCode("System.Web.Services", "4.8.4084.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    public class GetDataFromServerCompletedEventArgs : AsyncCompletedEventArgs
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

        public string Reserved
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[1];
            }
        }

        public string CurDateTime
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[2];
            }
        }

        public string OutData
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[3];
            }
        }

        public string Response
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[4];
            }
        }

        internal GetDataFromServerCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}