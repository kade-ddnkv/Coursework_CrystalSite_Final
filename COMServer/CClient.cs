#region сборка CServiceClientLib, Version=1.0.7959.21528, Culture=neutral, PublicKeyToken=null
// C:\Users\cyril\source\repos\Coursework_CrystalSite_Final\Coursework_CrystalSite_Final\libs\CServiceClientLib.dll
// Decompiled with ICSharpCode.Decompiler 6.1.0.5902
#endregion

using CServiceClientLib.Service;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;

namespace CServiceClientLib
{
    public class CClient
    {
        protected string DebugFileNameValue = "";

        protected string WebServiceURLValue = "https://meta.imet-db.ru/service/service.asmx";

        protected string RetValue = "";

        protected string RetValue2 = "";

        protected string OutData_ = "";

        protected string LoginValue = "";

        protected string _userName = "";

        protected string _userPass = "";

        protected string _userDomain = "";

        protected bool _AllowAutoRedirect;

        protected bool _PreAuthenticate;

        protected string _WebProxyString = "";

        protected int NumDBValue;

        protected int NumSystemsValue;

        protected int NumRecordsValue;

        protected MD5CryptoServiceProvider md5obj;

        protected MetaService cService;

        protected WebProxy myWebProxy;

        protected NetworkCredential crd;

        public string DebugFileName
        {
            get
            {
                return DebugFileNameValue;
            }
            set
            {
                DebugFileNameValue = value;
            }
        }

        public string WebServiceURL
        {
            get
            {
                return WebServiceURLValue;
            }
            set
            {
                WebServiceURLValue = value;
            }
        }

        public string ReturnValue
        {
            get
            {
                return RetValue;
            }
            set
            {
                RetValue = value;
            }
        }

        public string ReturnValue2
        {
            get
            {
                return RetValue2;
            }
            set
            {
                RetValue2 = value;
            }
        }

        public string OutData
        {
            get
            {
                return OutData_;
            }
            set
            {
                OutData_ = value;
            }
        }

        public string Login
        {
            get
            {
                return LoginValue;
            }
            set
            {
                LoginValue = value;
            }
        }

        public string Version
        {
            get
            {
                string text = $"MetabaseService WebClient .Net 4.0 ({IntPtr.Size * 8} bit). Copyright Dudarev Victor (IMET RAS). 16 October 2021";
                if (string.IsNullOrEmpty(DebugFileName))
                {
                    return text;
                }

                return text + "\r\nLocation = " + Assembly.GetExecutingAssembly().Location + "\r\nImageRuntimeVersion = " + Assembly.GetExecutingAssembly().ImageRuntimeVersion + "\r\nEnvironment.Version = " + Environment.Version.ToString();
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        public string UserPass
        {
            get
            {
                return _userPass;
            }
            set
            {
                _userPass = value;
            }
        }

        public string UserDomain
        {
            get
            {
                return _userDomain;
            }
            set
            {
                _userDomain = value;
            }
        }

        public bool AllowAutoRedirect
        {
            get
            {
                return _AllowAutoRedirect;
            }
            set
            {
                _AllowAutoRedirect = value;
            }
        }

        public bool PreAuthenticate
        {
            get
            {
                return _PreAuthenticate;
            }
            set
            {
                _PreAuthenticate = value;
            }
        }

        public string WebProxyString
        {
            get
            {
                return _WebProxyString;
            }
            set
            {
                _WebProxyString = value;
            }
        }

        public int NumDB => NumDBValue;

        public int NumSystems => NumSystemsValue;

        public int NumRecords => NumRecordsValue;

        public int GetMetaInfo(int mode, ref string Version, ref string Comments)
        {
            int num = 0;
            Comments = "";
            try
            {
                myInit();
                num = cService.GetMetaInfo(mode, ref Version, ref Comments);
            }
            catch (Exception ex)
            {
                Comments = "GetMetaInfo: System.Exception(" + ex.GetType().ToString() + "): " + ex.Message;
                num = 1000;
            }

            RetValue = Comments;
            return num;
        }

        public int CallGetIntegratedInfo4User(int SystemID, int UserID, ref string CurDateTime, ref string Response)
        {
            return CallGetIntegratedInfo4UserString(SystemID.ToString(), UserID.ToString(), ref CurDateTime, ref Response);
        }

        public int CallGetIntegratedInfo4UserString(string SystemID, string UserID, ref string CurDateTime, ref string Response)
        {
            int num = 0;
            CurDateTime = "";
            Response = "";
            NumDBValue = 0;
            NumSystemsValue = 0;
            NumRecordsValue = 0;
            try
            {
                myInit();
                num = cService.GetIntegratedInfo4User(LoginValue, SystemID, UserID, ref CurDateTime, ref Response, ref NumDBValue, ref NumSystemsValue, ref NumRecordsValue);
                RetValue2 = CurDateTime;
            }
            catch (Exception ex)
            {
                Response = "CallGetIntegratedInfo4User: System.Exception(" + ex.GetType().ToString() + "): " + ex.Message;
                num = 1000;
            }

            RetValue = Response;
            return num;
        }

        public int CallGetIntegratedInfo4UserByCompatibilityClass(int SystemID, int UserID, int CompatibilityClass, ref string CurDateTime, ref string Response)
        {
            return CallGetIntegratedInfo4UserByCompatibilityClassString(SystemID.ToString(), UserID.ToString(), CompatibilityClass, ref CurDateTime, ref Response);
        }

        public int CallGetIntegratedInfo4UserByCompatibilityClassString(string SystemID, string UserID, int CompatibilityClass, ref string CurDateTime, ref string Response)
        {
            int num = 0;
            CurDateTime = "";
            Response = "";
            NumDBValue = 0;
            NumSystemsValue = 0;
            NumRecordsValue = 0;
            try
            {
                myInit();
                num = cService.GetIntegratedInfo4UserByCompatibilityClass(LoginValue, SystemID, UserID, CompatibilityClass, ref CurDateTime, ref Response, ref NumDBValue, ref NumSystemsValue, ref NumRecordsValue);
                RetValue2 = CurDateTime;
            }
            catch (Exception ex)
            {
                Response = "CallGetIntegratedInfo4UserByCompatibilityClass: System.Exception(" + ex.GetType().ToString() + "): " + ex.Message;
                num = 1000;
            }

            RetValue = Response;
            return num;
        }

        public int CallGetDataFromServer(string Command, ref string Reserved, ref string CurDateTime, ref string OutData, ref string Response)
        {
            int num = 0;
            CurDateTime = "";
            Response = "";
            NumDBValue = 0;
            NumSystemsValue = 0;
            NumRecordsValue = 0;
            try
            {
                myInit();
                num = cService.GetDataFromServer(LoginValue, Command, ref Reserved, ref CurDateTime, ref OutData, ref Response);
                RetValue2 = CurDateTime;
                OutData_ = OutData;
            }
            catch (Exception ex)
            {
                Response = "CallGetDataFromServer: System.Exception(" + ex.GetType().ToString() + "): " + ex.Message;
                num = 1000;
            }

            RetValue = Response;
            return num;
        }

        protected string Write2TextFile(string FileName, string str)
        {
            string result = "";
            try
            {
                int num = 0;
                bool flag = true;
                while (flag)
                {
                    try
                    {
                        flag = false;
                        StreamWriter streamWriter = new StreamWriter(FileName, append: true, Encoding.Default);
                        streamWriter.WriteLine(str.ToString());
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                    catch (IOException ex)
                    {
                        if (num < 30)
                        {
                            num++;
                            flag = true;
                            Thread.Sleep(100);
                        }
                        else
                        {
                            result = "Write2TextFile: System.Exception(" + ex.GetType().ToString() + "): " + ex.Message;
                        }
                    }
                    catch
                    {
                        result = "Write2TextFile: Непредвиденное исключение при работе с файлом";
                    }
                    finally
                    {
                    }
                }

                return result;
            }
            catch
            {
                return "Write2TextFile: Непредвиденное исключение";
            }
        }

        protected string debug(string str)
        {
            string result = "";
            if (DebugFileName.Length > 0)
            {
                result = Write2TextFile(DebugFileName, DateTime.Now.ToString() + " " + LoginValue + ": " + str);
            }

            return result;
        }

        public DateTime GetDate(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                return XmlConvert.ToDateTime(date, "yyyy-MM-ddTHH:mm:ss");
            }

            return DateTime.MinValue;
        }

        public int SafeGetInt32(object str, int defaultValue)
        {
            int result = defaultValue;
            if (str == null)
            {
                return defaultValue;
            }

            if (str.GetType() == typeof(int))
            {
                return (int)str;
            }

            if (str.GetType() == typeof(bool))
            {
                if (!(bool)str)
                {
                    return 0;
                }

                return 1;
            }

            int.TryParse(str.ToString(), out result);
            return result;
        }

        public int CheckDateTime(string XMLRequestDateTime, int ValidGateTimeLimitHours, int mode, ref string Response)
        {
            int num = 0;
            Response = "";
            DateTime minValue = DateTime.MinValue;
            DateTime d = DateTime.MinValue;
            TimeSpan maxValue = TimeSpan.MaxValue;
            try
            {
                minValue = GetDate(XMLRequestDateTime);
                if (minValue == DateTime.MinValue)
                {
                    Response = "CheckDateTime(1): Дата (" + XMLRequestDateTime + ") в неверном формате";
                    num = 1;
                }

                if (num == 0)
                {
                    if (mode == 1)
                    {
                        myInit();
                        string dateTime = cService.GetDateTime();
                        d = GetDate(dateTime);
                        if (minValue == DateTime.MinValue)
                        {
                            Response = "CheckDateTime(2): Дата (" + dateTime + ") в неверном формате";
                            num = 2;
                        }
                    }
                    else
                    {
                        d = DateTime.UtcNow;
                    }
                }

                if (num == 0 && (d - minValue).TotalHours > (double)ValidGateTimeLimitHours)
                {
                    Response = $"CheckDateTime(3): Превышен временной порог в {ValidGateTimeLimitHours} часов";
                    num = 3;
                }
            }
            catch (Exception ex)
            {
                Response = "CheckDateTime(100): System.Exception(" + ex.GetType().ToString() + "): " + ex.Message;
                num = 100;
            }

            RetValue = Response;
            return num;
        }

        public string GetSAPCheckSum(string dt, int ids, int idp)
        {
            string sourceStr = $"{dt.ToUpper()}#Ok!#{ids}#{idp}";
            return MD5(sourceStr).ToUpper();
        }

        public int GetSAPerror(string sap, string dt, string ids, string idp, string chk, int ValidGateTimeLimitHours)
        {
            if (string.CompareOrdinal(sap, "1") != 0)
            {
                return 1;
            }

            DateTime date = GetDate(dt);
            if (date == DateTime.MinValue)
            {
                return 2;
            }

            if ((DateTime.UtcNow - date).TotalHours > (double)ValidGateTimeLimitHours)
            {
                return 3;
            }

            if (string.IsNullOrEmpty(chk))
            {
                return 4;
            }

            chk = chk.ToUpper();
            int ids2 = SafeGetInt32(ids, 0);
            int idp2 = SafeGetInt32(idp, 0);
            string sAPCheckSum = GetSAPCheckSum(dt, ids2, idp2);
            if (string.CompareOrdinal(chk, sAPCheckSum) != 0)
            {
                return 5;
            }

            return 0;
        }

        protected string myInit()
        {
            string result = "";
            try
            {
                if (cService == null)
                {
                    cService = new MetaService();
                    debug("Constructor - START, URL = " + WebServiceURLValue);
                    cService.Url = WebServiceURLValue;
                    cService.Timeout = -1;
                    cService.Url = WebServiceURLValue;
                    cService.UseDefaultCredentials = true;
                    if (!string.IsNullOrEmpty(UserName))
                    {
                        debug("UserName = " + UserName);
                        if (!string.IsNullOrEmpty(UserDomain))
                        {
                            debug("UserDomain = " + UserDomain);
                            crd = new NetworkCredential(UserName, UserPass, UserDomain);
                        }
                        else
                        {
                            debug("UserDomain is not specified");
                            crd = new NetworkCredential(UserName, UserPass);
                        }
                    }
                    else
                    {
                        debug("UserName is not specified => CredentialCache.DefaultCredentials");
                        crd = (NetworkCredential)CredentialCache.DefaultCredentials;
                    }

                    if (crd != null)
                    {
                        debug("Me.Credentials = crd");
                        cService.Credentials = crd;
                    }

                    if (!string.IsNullOrEmpty(WebProxyString))
                    {
                        debug("WebProxyString = " + WebProxyString);
                        if (crd == null)
                        {
                            debug("New WebProxy(WebProxyString, True)");
                            myWebProxy = new WebProxy(WebProxyString, BypassOnLocal: true);
                        }
                        else
                        {
                            debug("New WebProxy(WebProxyString, True, BypassList, crd)");
                            string[] bypassList = new string[0];
                            myWebProxy = new WebProxy(WebProxyString, BypassOnLocal: true, bypassList, crd);
                        }
                    }
                    else
                    {
                        debug("WebProxyString is not specified => WebProxy.GetDefaultProxy");
                        myWebProxy = WebProxy.GetDefaultProxy();
                    }

                    if (myWebProxy != null)
                    {
                        if (crd != null)
                        {
                            debug("myWebProxy.Credentials = crd");
                            myWebProxy.Credentials = crd;
                        }

                        debug("Me.Proxy = myWebProxy");
                        cService.Proxy = myWebProxy;
                    }

                    debug("Me.AllowAutoRedirect = True");
                    cService.AllowAutoRedirect = true;
                    cService.PreAuthenticate = true;
                    debug("Constructor - END");
                    return result;
                }

                return result;
            }
            catch (Exception ex)
            {
                return "myInit: System.Exception(" + ex.GetType().ToString() + "): " + ex.Message;
            }
        }

        protected byte[] MD5hash(string SourceStr)
        {
            byte[] array = new byte[SourceStr.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (byte)SourceStr[i];
            }

            if (md5obj == null)
            {
                md5obj = new MD5CryptoServiceProvider();
            }

            byte[] result = md5obj.ComputeHash(array);
            array = null;
            return result;
        }

        public string FileMD5(string FileName)
        {
            string text = "";
            FileStream fileStream = null;
            try
            {
                int num = 0;
                bool flag = true;
                while (flag)
                {
                    try
                    {
                        flag = false;
                        if (File.Exists(FileName))
                        {
                            fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                        }
                    }
                    catch (IOException)
                    {
                        fileStream = null;
                        if (num < 30)
                        {
                            num++;
                            flag = true;
                            Thread.Sleep(100);
                        }
                        else
                        {
                            text = "";
                        }
                    }
                }

                if (fileStream != null)
                {
                    if (md5obj == null)
                    {
                        md5obj = new MD5CryptoServiceProvider();
                    }

                    byte[] array = md5obj.ComputeHash(fileStream);
                    for (int i = 0; i < array.Length; i++)
                    {
                        string text2 = array[i].ToString("X");
                        if (text2.Length < 2)
                        {
                            text2 = "0" + text2;
                        }

                        text += text2;
                    }

                    return text;
                }

                return text;
            }
            catch
            {
                return "";
            }
            finally
            {
                fileStream?.Close();
                fileStream = null;
            }
        }

        public string MD5(string SourceStr)
        {
            string text = "";
            try
            {
                byte[] array = MD5hash(SourceStr);
                for (int i = 0; i < array.Length; i++)
                {
                    string text2 = array[i].ToString("X");
                    if (text2.Length < 2)
                    {
                        text2 = "0" + text2;
                    }

                    text += text2;
                }

                return text;
            }
            catch (Exception ex)
            {
                return "MD5: System.Exception(" + ex.GetType().ToString() + "): " + ex.Message;
            }
        }

        ~CClient()
        {
            cService = null;
        }
    }
}