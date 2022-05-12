#region сборка CServiceClientLib, Version=1.0.7959.21528, Culture=neutral, PublicKeyToken=null
// C:\Users\cyril\source\repos\Coursework_CrystalSite_Final\COMServer\libs\CServiceClientLib.dll
// Decompiled with ICSharpCode.Decompiler 6.1.0.5902
#endregion

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace CServiceClientLib.Service
{
    [GeneratedCode("System.Web.Services", "4.8.4084.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [WebServiceBinding(Name = "MetaServiceSoap", Namespace = "http://meta.imet-db.ru/")]
    public class MetaService : SoapHttpClientProtocol
    {
        private SendOrPostCallback AddOperationCompleted;

        private SendOrPostCallback GetDateTimeOperationCompleted;

        private SendOrPostCallback GetMetaInfoOperationCompleted;

        private SendOrPostCallback GetIntegratedInfo4UserOperationCompleted;

        private SendOrPostCallback GetIntegratedInfo4UserByCompatibilityClassOperationCompleted;

        private SendOrPostCallback GetDataFromServerOperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

        public new string Url
        {
            get
            {
                ConfigurationManager.AppSettings.Get("SomeValue");
                return base.Url;
            }
            set
            {
                if (IsLocalFileSystemWebService(base.Url) && !useDefaultCredentialsSetExplicitly && !IsLocalFileSystemWebService(value))
                {
                    base.UseDefaultCredentials = false;
                }

                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                useDefaultCredentialsSetExplicitly = true;
            }
        }

        public event AddCompletedEventHandler AddCompleted;

        public event GetDateTimeCompletedEventHandler GetDateTimeCompleted;

        public event GetMetaInfoCompletedEventHandler GetMetaInfoCompleted;

        public event GetIntegratedInfo4UserCompletedEventHandler GetIntegratedInfo4UserCompleted;

        public event GetIntegratedInfo4UserByCompatibilityClassCompletedEventHandler GetIntegratedInfo4UserByCompatibilityClassCompleted;

        public event GetDataFromServerCompletedEventHandler GetDataFromServerCompleted;

        public MetaService()
        {
            Url = "http://meta.imet-db.ru/Service/Service.asmx";
            if (IsLocalFileSystemWebService(Url))
            {
                UseDefaultCredentials = true;
                useDefaultCredentialsSetExplicitly = false;
            }
            else
            {
                useDefaultCredentialsSetExplicitly = true;
            }
        }

        [SoapDocumentMethod("http://meta.imet-db.ru/Add", RequestNamespace = "http://meta.imet-db.ru/", ResponseNamespace = "http://meta.imet-db.ru/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int Add(int a, int b)
        {
            return (int)Invoke("Add", new object[2]
            {
                a,
                b
            })[0];
        }

        public IAsyncResult BeginAdd(int a, int b, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("Add", new object[2]
            {
                a,
                b
            }, callback, asyncState);
        }

        public int EndAdd(IAsyncResult asyncResult)
        {
            return (int)EndInvoke(asyncResult)[0];
        }

        public void AddAsync(int a, int b)
        {
            AddAsync(a, b, null);
        }

        public void AddAsync(int a, int b, object userState)
        {
            if (AddOperationCompleted == null)
            {
                AddOperationCompleted = OnAddOperationCompleted;
            }

            InvokeAsync("Add", new object[2]
            {
                a,
                b
            }, AddOperationCompleted, userState);
        }

        private void OnAddOperationCompleted(object arg)
        {
            if (this.AddCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
                this.AddCompleted(this, new AddCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
            }
        }

        [SoapDocumentMethod("http://meta.imet-db.ru/GetDateTime", RequestNamespace = "http://meta.imet-db.ru/", ResponseNamespace = "http://meta.imet-db.ru/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public string GetDateTime()
        {
            return (string)Invoke("GetDateTime", new object[0])[0];
        }

        public IAsyncResult BeginGetDateTime(AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetDateTime", new object[0], callback, asyncState);
        }

        public string EndGetDateTime(IAsyncResult asyncResult)
        {
            return (string)EndInvoke(asyncResult)[0];
        }

        public void GetDateTimeAsync()
        {
            GetDateTimeAsync(null);
        }

        public void GetDateTimeAsync(object userState)
        {
            if (GetDateTimeOperationCompleted == null)
            {
                GetDateTimeOperationCompleted = OnGetDateTimeOperationCompleted;
            }

            InvokeAsync("GetDateTime", new object[0], GetDateTimeOperationCompleted, userState);
        }

        private void OnGetDateTimeOperationCompleted(object arg)
        {
            if (this.GetDateTimeCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
                this.GetDateTimeCompleted(this, new GetDateTimeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
            }
        }

        [SoapDocumentMethod("http://meta.imet-db.ru/GetMetaInfo", RequestNamespace = "http://meta.imet-db.ru/", ResponseNamespace = "http://meta.imet-db.ru/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int GetMetaInfo(int mode, ref string Version, ref string Comments)
        {
            object[] array = Invoke("GetMetaInfo", new object[3]
            {
                mode,
                Version,
                Comments
            });
            Version = (string)array[1];
            Comments = (string)array[2];
            return (int)array[0];
        }

        public IAsyncResult BeginGetMetaInfo(int mode, string Version, string Comments, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetMetaInfo", new object[3]
            {
                mode,
                Version,
                Comments
            }, callback, asyncState);
        }

        public int EndGetMetaInfo(IAsyncResult asyncResult, out string Version, out string Comments)
        {
            object[] array = EndInvoke(asyncResult);
            Version = (string)array[1];
            Comments = (string)array[2];
            return (int)array[0];
        }

        public void GetMetaInfoAsync(int mode, string Version, string Comments)
        {
            GetMetaInfoAsync(mode, Version, Comments, null);
        }

        public void GetMetaInfoAsync(int mode, string Version, string Comments, object userState)
        {
            if (GetMetaInfoOperationCompleted == null)
            {
                GetMetaInfoOperationCompleted = OnGetMetaInfoOperationCompleted;
            }

            InvokeAsync("GetMetaInfo", new object[3]
            {
                mode,
                Version,
                Comments
            }, GetMetaInfoOperationCompleted, userState);
        }

        private void OnGetMetaInfoOperationCompleted(object arg)
        {
            if (this.GetMetaInfoCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
                this.GetMetaInfoCompleted(this, new GetMetaInfoCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
            }
        }

        [SoapDocumentMethod("http://meta.imet-db.ru/GetIntegratedInfo4User", RequestNamespace = "http://meta.imet-db.ru/", ResponseNamespace = "http://meta.imet-db.ru/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int GetIntegratedInfo4User(string DBLogin, string SystemID, string UserID, ref string CurDateTime, ref string Response, ref int NumDB, ref int NumSystems, ref int NumRecords)
        {
            object[] array = Invoke("GetIntegratedInfo4User", new object[8]
            {
                DBLogin,
                SystemID,
                UserID,
                CurDateTime,
                Response,
                NumDB,
                NumSystems,
                NumRecords
            });
            CurDateTime = (string)array[1];
            Response = (string)array[2];
            NumDB = (int)array[3];
            NumSystems = (int)array[4];
            NumRecords = (int)array[5];
            return (int)array[0];
        }

        public IAsyncResult BeginGetIntegratedInfo4User(string DBLogin, string SystemID, string UserID, string CurDateTime, string Response, int NumDB, int NumSystems, int NumRecords, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetIntegratedInfo4User", new object[8]
            {
                DBLogin,
                SystemID,
                UserID,
                CurDateTime,
                Response,
                NumDB,
                NumSystems,
                NumRecords
            }, callback, asyncState);
        }

        public int EndGetIntegratedInfo4User(IAsyncResult asyncResult, out string CurDateTime, out string Response, out int NumDB, out int NumSystems, out int NumRecords)
        {
            object[] array = EndInvoke(asyncResult);
            CurDateTime = (string)array[1];
            Response = (string)array[2];
            NumDB = (int)array[3];
            NumSystems = (int)array[4];
            NumRecords = (int)array[5];
            return (int)array[0];
        }

        public void GetIntegratedInfo4UserAsync(string DBLogin, string SystemID, string UserID, string CurDateTime, string Response, int NumDB, int NumSystems, int NumRecords)
        {
            GetIntegratedInfo4UserAsync(DBLogin, SystemID, UserID, CurDateTime, Response, NumDB, NumSystems, NumRecords, null);
        }

        public void GetIntegratedInfo4UserAsync(string DBLogin, string SystemID, string UserID, string CurDateTime, string Response, int NumDB, int NumSystems, int NumRecords, object userState)
        {
            if (GetIntegratedInfo4UserOperationCompleted == null)
            {
                GetIntegratedInfo4UserOperationCompleted = OnGetIntegratedInfo4UserOperationCompleted;
            }

            InvokeAsync("GetIntegratedInfo4User", new object[8]
            {
                DBLogin,
                SystemID,
                UserID,
                CurDateTime,
                Response,
                NumDB,
                NumSystems,
                NumRecords
            }, GetIntegratedInfo4UserOperationCompleted, userState);
        }

        private void OnGetIntegratedInfo4UserOperationCompleted(object arg)
        {
            if (this.GetIntegratedInfo4UserCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
                this.GetIntegratedInfo4UserCompleted(this, new GetIntegratedInfo4UserCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
            }
        }

        [SoapDocumentMethod("http://meta.imet-db.ru/GetIntegratedInfo4UserByCompatibilityClass", RequestNamespace = "http://meta.imet-db.ru/", ResponseNamespace = "http://meta.imet-db.ru/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int GetIntegratedInfo4UserByCompatibilityClass(string DBLogin, string SystemID, string UserID, int CompatibilityClass, ref string CurDateTime, ref string Response, ref int NumDB, ref int NumSystems, ref int NumRecords)
        {
            object[] array = Invoke("GetIntegratedInfo4UserByCompatibilityClass", new object[9]
            {
                DBLogin,
                SystemID,
                UserID,
                CompatibilityClass,
                CurDateTime,
                Response,
                NumDB,
                NumSystems,
                NumRecords
            });
            CurDateTime = (string)array[1];
            Response = (string)array[2];
            NumDB = (int)array[3];
            NumSystems = (int)array[4];
            NumRecords = (int)array[5];
            return (int)array[0];
        }

        public IAsyncResult BeginGetIntegratedInfo4UserByCompatibilityClass(string DBLogin, string SystemID, string UserID, int CompatibilityClass, string CurDateTime, string Response, int NumDB, int NumSystems, int NumRecords, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetIntegratedInfo4UserByCompatibilityClass", new object[9]
            {
                DBLogin,
                SystemID,
                UserID,
                CompatibilityClass,
                CurDateTime,
                Response,
                NumDB,
                NumSystems,
                NumRecords
            }, callback, asyncState);
        }

        public int EndGetIntegratedInfo4UserByCompatibilityClass(IAsyncResult asyncResult, out string CurDateTime, out string Response, out int NumDB, out int NumSystems, out int NumRecords)
        {
            object[] array = EndInvoke(asyncResult);
            CurDateTime = (string)array[1];
            Response = (string)array[2];
            NumDB = (int)array[3];
            NumSystems = (int)array[4];
            NumRecords = (int)array[5];
            return (int)array[0];
        }

        public void GetIntegratedInfo4UserByCompatibilityClassAsync(string DBLogin, string SystemID, string UserID, int CompatibilityClass, string CurDateTime, string Response, int NumDB, int NumSystems, int NumRecords)
        {
            GetIntegratedInfo4UserByCompatibilityClassAsync(DBLogin, SystemID, UserID, CompatibilityClass, CurDateTime, Response, NumDB, NumSystems, NumRecords, null);
        }

        public void GetIntegratedInfo4UserByCompatibilityClassAsync(string DBLogin, string SystemID, string UserID, int CompatibilityClass, string CurDateTime, string Response, int NumDB, int NumSystems, int NumRecords, object userState)
        {
            if (GetIntegratedInfo4UserByCompatibilityClassOperationCompleted == null)
            {
                GetIntegratedInfo4UserByCompatibilityClassOperationCompleted = OnGetIntegratedInfo4UserByCompatibilityClassOperationCompleted;
            }

            InvokeAsync("GetIntegratedInfo4UserByCompatibilityClass", new object[9]
            {
                DBLogin,
                SystemID,
                UserID,
                CompatibilityClass,
                CurDateTime,
                Response,
                NumDB,
                NumSystems,
                NumRecords
            }, GetIntegratedInfo4UserByCompatibilityClassOperationCompleted, userState);
        }

        private void OnGetIntegratedInfo4UserByCompatibilityClassOperationCompleted(object arg)
        {
            if (this.GetIntegratedInfo4UserByCompatibilityClassCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
                this.GetIntegratedInfo4UserByCompatibilityClassCompleted(this, new GetIntegratedInfo4UserByCompatibilityClassCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
            }
        }

        [SoapDocumentMethod("http://meta.imet-db.ru/GetDataFromServer", RequestNamespace = "http://meta.imet-db.ru/", ResponseNamespace = "http://meta.imet-db.ru/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public int GetDataFromServer(string DBLogin, string Command, ref string Reserved, ref string CurDateTime, ref string OutData, ref string Response)
        {
            object[] array = Invoke("GetDataFromServer", new object[6]
            {
                DBLogin,
                Command,
                Reserved,
                CurDateTime,
                OutData,
                Response
            });
            Reserved = (string)array[1];
            CurDateTime = (string)array[2];
            OutData = (string)array[3];
            Response = (string)array[4];
            return (int)array[0];
        }

        public IAsyncResult BeginGetDataFromServer(string DBLogin, string Command, string Reserved, string CurDateTime, string OutData, string Response, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetDataFromServer", new object[6]
            {
                DBLogin,
                Command,
                Reserved,
                CurDateTime,
                OutData,
                Response
            }, callback, asyncState);
        }

        public int EndGetDataFromServer(IAsyncResult asyncResult, out string Reserved, out string CurDateTime, out string OutData, out string Response)
        {
            object[] array = EndInvoke(asyncResult);
            Reserved = (string)array[1];
            CurDateTime = (string)array[2];
            OutData = (string)array[3];
            Response = (string)array[4];
            return (int)array[0];
        }

        public void GetDataFromServerAsync(string DBLogin, string Command, string Reserved, string CurDateTime, string OutData, string Response)
        {
            GetDataFromServerAsync(DBLogin, Command, Reserved, CurDateTime, OutData, Response, null);
        }

        public void GetDataFromServerAsync(string DBLogin, string Command, string Reserved, string CurDateTime, string OutData, string Response, object userState)
        {
            if (GetDataFromServerOperationCompleted == null)
            {
                GetDataFromServerOperationCompleted = OnGetDataFromServerOperationCompleted;
            }

            InvokeAsync("GetDataFromServer", new object[6]
            {
                DBLogin,
                Command,
                Reserved,
                CurDateTime,
                OutData,
                Response
            }, GetDataFromServerOperationCompleted, userState);
        }

        private void OnGetDataFromServerOperationCompleted(object arg)
        {
            if (this.GetDataFromServerCompleted != null)
            {
                InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
                this.GetDataFromServerCompleted(this, new GetDataFromServerCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
            }
        }

        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (url == null || url == string.Empty)
            {
                return false;
            }

            Uri uri = new Uri(url);
            if (uri.Port >= 1024 && string.Compare(uri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }
    }
}