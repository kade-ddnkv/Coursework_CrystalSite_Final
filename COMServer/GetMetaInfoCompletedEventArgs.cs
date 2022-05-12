using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace CServiceClientLib.Service
{
    [GeneratedCode("System.Web.Services", "4.8.4084.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    public class GetMetaInfoCompletedEventArgs : AsyncCompletedEventArgs
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

        public string Version
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[1];
            }
        }

        public string Comments
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[2];
            }
        }

        internal GetMetaInfoCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}