using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace CServiceClientLib.Service
{
    [GeneratedCode("System.Web.Services", "4.8.4084.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    public class GetDateTimeCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        public string Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return (string)results[0];
            }
        }

        internal GetDateTimeCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
            : base(exception, cancelled, userState)
        {
            this.results = results;
        }
    }
}