using com.SML.Lib.Common;
using com.SML.BIGTRONS.App_Start;
using com.SML.BIGTRONS.Enum;
using com.SML.BIGTRONS.ViewModels;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Text.RegularExpressions;
using RazorEngine;
using RazorEngine.Templating;
using RazorEngine.Configuration;

namespace com.SML.BIGTRONS
{
    public class Global : System.Web.HttpApplication
    {
        #region Private Field

        private static DateTime _dateExpired;
        private static Dictionary<string, object> _dicTaskTimer;
        // Add new field beneath

        #endregion

        private static readonly string __strReportFolder = Properties.Settings.Default.ReportFolder;
        private static readonly string __strReportViewer = Properties.Settings.Default.ReportViewer;
        private static readonly string __strPinEvent = "mouseover";
        private static readonly string __strQueueName = "IPMessage";
        private static readonly string __strTemplateFolder = Properties.Settings.Default.TemplateFolder;
        private static readonly string __strExcelTemplateLocation = "~/" + __strTemplateFolder + "/Excel.xsl";
        private static readonly string __strCsvTemplateLocation = "~/" + __strTemplateFolder + "/Csv.xsl";
        private static readonly int __intNotificationHideDelay = 5000;
        private static readonly List<string> __lstAggregateFunction = new List<string>()
        {
            "AVG", "CHECKSUM_AGG","COUNT","COUNT_BIG","GROUPING","GROUPING_ID", "MAX","MIN","STDEV","STDEVP","SUM","VAR","VARP"
        };

        #region Public Constant Field

        public const string OpComparation = "compare";
        public const string OpEquals = "=";
        public const string OpStartsWith = "+";
        public const string OpEndsWith = "-";
        public const string OpNotEqual = "!";
        public const string OpContains = "*";
        public const string OpLessThan = "lt";
        public const string OpLessThanEqual = "lte";
        public const string OpGreaterThan = "gt";
        public const string OpGreaterThanEqual = "gte";

        public const string MaxDate = "9999/12/31";
        public const string MinDate = "1753/1/1";

        #endregion

        #region Public Static Readonly Field

        public static readonly int ExpiredTime = 20;
        public static readonly int ExpiredYear = 1000;
        public static readonly string ConnStrConfigName = Properties.Settings.Default.ConnStrConfigName;
        public static readonly string DefaultDateFormat = "dd-MM-yyyy";
        public static readonly string DateFormatLongMonth = "dd MMM yyyy";
        public static readonly string DateWithDay = "dddd, dd MMMM yyyy";
        public static readonly string DefaultDateTimeFormat = "dd-MM-yyyy HH:mm:ss";
        public static readonly string SqlDateFormat = "yyyy/MM/dd HH:mm:ss";
        public static readonly string ThreeWordMonthDateFormat = "MMM dd, yyyy";
        public static readonly string InternationalDateFormat = "MM/dd/yyyy";
        public static readonly string DefaultPeriodFormat = "yyyyMM";
        public static readonly string LongPeriodFormat = "MMM yyyy";
        public static readonly string DefaultTimeFormat = "HH:mm:ss";
        public static readonly string ShortTimeFormat = "HH:mm";
        public static readonly string DefaultNumberFormat = "#,##0.00";
        public static readonly string IntegerNumberFormat = "#,##0";
        public static readonly string OneLineSeparated = ", ";
        public static readonly string NewLineSeparated = "<br />";
        public static readonly List<string> AggregateFunctionList = new List<string>()
        {
            "AVG", "CHECKSUM_AGG","COUNT","COUNT_BIG","GROUPING","GROUPING_ID", "MAX","MIN","STDEV","STDEVP","SUM","VAR","VARP"
        };
        public static readonly string Config_MailTemplate_Key1 = "MailTemplate";
        public static readonly string Config_MailTemplate_Key2 = "Scheduller";
        public static readonly string Config_TemplateTag_Key1 = "UTemplateTag";
        public static readonly string Config_TemplateTag_Key2 = "Reminder";
        public static readonly string Config_TemplateTag_RecipientList = "RecipientList";
        public static readonly string Config_TemplateTag_DateDay = "DateDay";
        public static readonly string Config_TemplateTag_ListSchedule= "ListSchedule";
        public static readonly string Config_Reminder_Key1 = "Reminder";
        public static readonly string Config_Reminder_Subject = "Subject";

        #endregion

        #region Public Static Property

        public static HasAccessVM HasAccess { get; set; }

        public static string LocalHostIP
        {
            get { return HttpContext.Current.Session["HostIP"].ToString(); }
        }

        public static string LocalHostName
        {
            get
            {
                return (HttpContext.Current.Session == null || HttpContext.Current.Session["HostName"] == null ? string.Empty
                  : HttpContext.Current.Session["HostName"].ToString());
            }
        }

        public static string LoggedInUser
        {
            get
            {
                return HttpContext.Current.User.Identity.Name;
            }
        }

        public static string LoggedInRoleID
        {
            get
            {
                HttpCookie m_cookRoleID = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName + ".RoleID"];
                return m_cookRoleID == null ? string.Empty : m_cookRoleID.Value;
            }
        }

        public static string LoggedInFullName
        {
            get
            {
                HttpCookie m_cookRoleID = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName + ".FullName"];
                return m_cookRoleID == null ? string.Empty : m_cookRoleID.Value;
            }
        }

        public static string AbsoluteServerName
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["HTTP_ORIGIN"]
                    + HttpContext.Current.Request.ApplicationPath
                    + (HttpContext.Current.Request.ApplicationPath.EndsWith("/") ? "" : "/");
            }
        }

        public static string MenuUrl
        {
            get
            {
                HttpRequest m_objRequest = HttpContext.Current.Request;
                string m_strMenuUrl = (m_objRequest.UrlReferrer == null || (m_objRequest.UrlReferrer.AbsoluteUri
                    != (m_objRequest.Url.Scheme + "://" + m_objRequest.Url.Authority + (m_objRequest.Url.Authority.EndsWith("/") ? "" : "/")) ?
                    !m_objRequest.Url.AbsoluteUri.Contains(m_objRequest.UrlReferrer.AbsoluteUri) : true) ?
                    m_objRequest.FilePath : m_objRequest.UrlReferrer.AbsolutePath);
                return m_strMenuUrl;
            }
        }

        public static string ReportFolder
        {
            get
            {
                return __strReportFolder;
            }
        }

        public static string ReportViewer
        {
            get
            {
                return __strReportViewer;
            }
        }

        public static string ExcelTemplateLocation
        {
            get { return __strExcelTemplateLocation; }
        }

        public static string CsvTemplateLocation
        {
            get { return __strCsvTemplateLocation; }
        }

        public static Dictionary<string, object> TaskTimer { get { return _dicTaskTimer; } }

        #endregion

        #region Public Static Method

        public static void HoldLoginData(string roleID, string userID, string fullName, string password, bool remember)
        {
            //roleGroupID = roleGroupID.Trim();
            roleID = roleID.Trim();
            userID = userID.Trim();
            fullName = fullName.Trim();
            password = password.Trim();

            // get the role now
            // Create forms authentication ticket
            FormsAuthenticationTicket m_fatTicket = new FormsAuthenticationTicket(
            1, // Ticket version
            userID, // Username to be associated with this ticket
            DateTimeOffset.Now.DateTime, // Date/time ticket was issued
            _dateExpired, // Date and time the cookie will expire
            remember, // if user has chcked rememebr me then create persistent cookie
            roleID, // store the user data, in this case roles of the user
            FormsAuthentication.FormsCookiePath); // Cookie path specified in the web.config file in <Forms> tag if any.
            HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(userID), new string[] { roleID });

            // To give more security it is suggested to hash it
            string m_strHashCookies = FormsAuthentication.Encrypt(m_fatTicket);
            HttpCookie m_cookHttpCookie = new HttpCookie(FormsAuthentication.FormsCookieName, m_strHashCookies); // Hashed ticket
            //m_cookHttpCookie.Expires = _dateExpired;
            // Add the cookie to the response, user browser
            m_cookHttpCookie.Path = FormsAuthentication.FormsCookiePath;
            HttpContext.Current.Response.Cookies.Add(m_cookHttpCookie);

            //m_cookHttpCookie = new HttpCookie(FormsAuthentication.FormsCookieName + ".RoleGroupID", roleGroupID); // Hashed ticket
            //m_cookHttpCookie.Expires = _dateExpired;
            //m_cookHttpCookie.Path = FormsAuthentication.FormsCookiePath;
            //HttpContext.Current.Response.Cookies.Add(m_cookHttpCookie);

            m_cookHttpCookie = new HttpCookie(FormsAuthentication.FormsCookieName + ".RoleID", roleID); // Hashed ticket
            m_cookHttpCookie.Expires = _dateExpired;
            m_cookHttpCookie.Path = FormsAuthentication.FormsCookiePath;
            HttpContext.Current.Response.Cookies.Add(m_cookHttpCookie);

            m_cookHttpCookie = new HttpCookie(FormsAuthentication.FormsCookieName + ".FullName", fullName); // Hashed ticket
            m_cookHttpCookie.Expires = _dateExpired;
            m_cookHttpCookie.Path = FormsAuthentication.FormsCookiePath;
            HttpContext.Current.Response.Cookies.Add(m_cookHttpCookie);

            if (remember)
            {
                m_cookHttpCookie = new HttpCookie(FormsAuthentication.FormsCookieName + "-userID-" + userID, password); // Hashed ticket
                m_cookHttpCookie.Expires = _dateExpired.AddYears(ExpiredYear);
                m_cookHttpCookie.Path = FormsAuthentication.FormsCookiePath;
                HttpContext.Current.Response.Cookies.Add(m_cookHttpCookie);
            }
            else
            {
                m_cookHttpCookie = new HttpCookie(FormsAuthentication.FormsCookieName + "-userID-" + userID, password); // Hashed ticket
                m_cookHttpCookie.Expires = DateTime.Now.AddDays(-1);
                m_cookHttpCookie.Path = FormsAuthentication.FormsCookiePath;
                HttpContext.Current.Response.Cookies.Add(m_cookHttpCookie);
            }

            //if (!remember)
            //{
            //    userID = password = "";
            //}
            //m_cookHttpCookie = new HttpCookie(FormsAuthentication.FormsCookieName + ".RememberUserID", userID);
            //m_cookHttpCookie.Expires = _dateExpired.AddYears(1);
            //m_cookHttpCookie.Path = FormsAuthentication.FormsCookiePath;
            //HttpContext.Current.Response.Cookies.Add(m_cookHttpCookie);

            //m_cookHttpCookie = new HttpCookie(FormsAuthentication.FormsCookieName + ".RememberPassword", password);
            //m_cookHttpCookie.Expires = _dateExpired.AddYears(1);
            //m_cookHttpCookie.Path = FormsAuthentication.FormsCookiePath;
            //HttpContext.Current.Response.Cookies.Add(m_cookHttpCookie);
        }

        //public static void RewriteLoginCache(DateTime expiredDate, bool isSignOut)
        public static void RewriteLoginCache(bool isSignOut)
        {
            try
            {
                string m_strCookieName;
                int m_intLimit = HttpContext.Current.Request.Cookies.Count;
                for (int m_intCount = 0; m_intCount < m_intLimit; m_intCount++)
                {
                    m_strCookieName = HttpContext.Current.Request.Cookies[m_intCount].Name;
                    if (m_strCookieName.StartsWith(FormsAuthentication.FormsCookieName + "-userID-") ||
                        (m_strCookieName == FormsAuthentication.FormsCookieName && !isSignOut))
                        continue;
                    _dateExpired = isSignOut ? DateTime.Now.AddDays(-1) : DateTime.Now.AddMinutes(ExpiredTime);
                    HttpCookie m_cookData = new HttpCookie(m_strCookieName, HttpContext.Current.Request.Cookies[m_intCount].Value);
                    m_cookData.Path = HttpContext.Current.Request.Cookies[m_intCount].Path;
                    m_cookData.Expires = _dateExpired;
                    HttpContext.Current.Response.Cookies.Add(m_cookData);
                }
            }
            catch
            {
            }
        }

        public static string GetTerbilang(long number)
        {
            string m_strTerbilang = string.Empty;
            string[] m_arrAngka = { "", "satu", "dua", "tiga", "empat", "lima", "enam", "tujuh", "delapan", "sembilan", "sepuluh", "sebelas" };
            if (number < 12)
            {
                m_strTerbilang = " " + m_arrAngka[number];
            }
            else if (number < 20)
            {
                m_strTerbilang = GetTerbilang(number - 10).ToString() + " belas";
            }
            else if (number < 100)
            {
                m_strTerbilang = GetTerbilang(number / 10) + " puluh" + GetTerbilang(number % 10);
            }
            else if (number < 200)
            {
                m_strTerbilang = " seratus" + GetTerbilang(number - 100);
            }
            else if (number < 1000)
            {
                m_strTerbilang = GetTerbilang(number / 100) + " ratus" + GetTerbilang(number % 100);
            }
            else if (number < 2000)
            {
                m_strTerbilang = " seribu" + GetTerbilang(number - 1000);
            }
            else if (number < 1000000)
            {
                m_strTerbilang = GetTerbilang(number / 1000) + " ribu" + GetTerbilang(number % 1000);
            }
            else if (number < 1000000000)
            {
                m_strTerbilang = GetTerbilang(number / 1000000) + " juta" + GetTerbilang(number % 1000000);
            }
            else if (number < 1000000000000)
            {
                m_strTerbilang = GetTerbilang(number / 1000000000) + " milyar" + GetTerbilang(number % 1000000000);
            }
            else if (number < 1000000000000000)
            {
                m_strTerbilang = GetTerbilang(number / 1000000000000) + " triliyun" + GetTerbilang(number % 1000000000000);
            }

            m_strTerbilang = Regex.Replace(m_strTerbilang, @"\s+", " ");

            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(m_strTerbilang.ToLower());
        }

        public static string GetTerbilangEN(decimal number)
        {
            if (number == 0)
                return "ZERO";

            if (number < 0)
                return "MINUS " + GetTerbilangEN(Math.Abs(number));

            string words = String.Empty;

            long intPortion = (long)number;
            decimal fraction = (number - intPortion);
            int decimalPrecision = GetDecimalPrecision(number);

            fraction = CalculateFraction(decimalPrecision, fraction);

            long decPortion = (long)fraction;

            words = IntToWords(intPortion);
            if (decPortion > 0)
            {
                words += " POINT ";
                words += IntToWords(decPortion);
            }
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(words.Trim().ToLower());
        }
        private static int GetDecimalPrecision(decimal number)
        {
            return (Decimal.GetBits(number)[3] >> 16) & 0x000000FF;
        }
        private static decimal CalculateFraction(int decimalPrecision, decimal fraction)
        {
            switch (decimalPrecision)
            {
                case 1:
                    return fraction * 10;
                case 2:
                    return fraction * 100;
                case 3:
                    return fraction * 1000;
                case 4:
                    return fraction * 10000;
                case 5:
                    return fraction * 100000;
                case 6:
                    return fraction * 1000000;
                case 7:
                    return fraction * 10000000;
                case 8:
                    return fraction * 100000000;
                case 9:
                    return fraction * 1000000000;
                case 10:
                    return fraction * 10000000000;
                case 11:
                    return fraction * 100000000000;
                case 12:
                    return fraction * 1000000000000;
                case 13:
                    return fraction * 10000000000000;
                default:
                    return fraction * 10000000000000;
            }
        }
        public static string IntToWords(long number)
        {
            if (number == 0)
                return "ZERO";

            if (number < 0)
                return "MINUS " + IntToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000000000000) > 0)
            {
                words += IntToWords(number / 1000000000000000) + " QUADRILLION ";
                number %= 1000000000000000;
            }

            if ((number / 1000000000000) > 0)
            {
                words += IntToWords(number / 1000000000000) + " TRILLION ";
                number %= 1000000000000;
            }

            if ((number / 1000000000) > 0)
            {
                words += IntToWords(number / 1000000000) + " BILLION ";
                number %= 1000000000;
            }

            if ((number / 1000000) > 0)
            {
                words += IntToWords(number / 1000000) + " MILLION ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += IntToWords(number / 1000) + " THOUSAND ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += IntToWords(number / 100) + " HUNDRED ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != String.Empty)
                    words += "AND ";

                var unitsMap = new[] { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
                var tensMap = new[] { "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(words.Trim().ToLower());
        }

        public static string GenerateUID(int length, string headCode = "", string tailCode = "")
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            System.Text.StringBuilder id = new System.Text.StringBuilder();
            using (System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    id.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return headCode + id.ToString() + tailCode;
        }

        /// <summary>
        /// Create object of button
        /// </summary>
        /// <param name="button">Type of button</param>
        /// <param name="paramList">Parameters of DirectEvents</param>
        /// <returns></returns>
        public static Button.Builder Button(Buttons button, List<Parameter> paramList)
        {
            return Button(button, null, null, null, false, false, false, null, paramList);
        }

        /// <summary>
        /// Create object of button
        /// </summary>
        /// <param name="button">Type of button</param>
        /// <param name="id">ID of button. If null or empty string then use default ID (Name of button)</param>
        /// <param name="text">Text of button. If null then use default Text (Desc of button). If empty string then button has no text</param>
        /// <param name="action">Action of DirectEvents. If null then use default Action Name (Desc of button). If empty string then no DirectEvents</param>
        /// <param name="startWithDefaultId">Wheter to start ID of button with default ID or not. Omitted if 'id' is null or empty string</param>
        /// <param name="startWithDefaultText">Wheter to start Text of button with default Text or not. Omitted if 'text' is null or empty string</param>
        /// <param name="startWithDefaultAction">Wheter to start Action Name with default Action Name (Desc of button) or not. Omitted if 'action' is null or empty string</param>
        /// <param name="paramList">Parameters of DirectEvents. Omitted if 'action' is empty string</param>
        /// <param name="before">Function to call before call Action. If null or empty string then do nothing</param>
        /// <param name="success">Function to call if success. If null or empty string then do nothing</param>
        /// <param name="failure">Function to call if failure. If null or empty string then do nothing</param>
        /// <returns>Object of button</returns>
        public static Button.Builder Button(Buttons button, string id = null, string text = null, string action = null, bool startWithDefaultId = false,
            bool startWithDefaultText = false, bool startWithDefaultAction = false, System.Enum caller = null, List<Parameter> paramList = null, string before = null,
            string success = null, string failure = null)
        {
            var X = Html.X();

            Button.Builder m_btnReturn = X.Button();
            m_btnReturn.ID(id == null || id == string.Empty ? General.EnumName(button) : (startWithDefaultId ? General.EnumName(button) : string.Empty) + id);
            m_btnReturn.Text(text == null ? General.EnumDesc(button) : (text == string.Empty ? string.Empty :
                ((startWithDefaultText ? General.EnumDesc(button) + " " : string.Empty) + text).Trim()));
            if (action == null || action.Trim() != string.Empty)
                m_btnReturn.DirectEvents(events =>
                {
                    events.Click.Action = (action == null ? General.EnumDesc(button) : (startWithDefaultAction ? General.EnumDesc(button) : string.Empty) + action);
                    if (!String.IsNullOrEmpty(before))
                        events.Click.Before = before;
                    events.Click.EventMask.ShowMask = true;
                    if (caller != null)
                    {
                        Parameter m_objCallerParam = new Parameter(General.EnumDesc(Params.Caller), General.EnumDesc(caller), ParameterMode.Value, false);
                        events.Click.ExtraParams.Add(m_objCallerParam);
                    }
                    switch (button)
                    {
                        case Buttons.ButtonDelete:
                            events.Click.Confirmation.ConfirmRequest = true;
                            events.Click.Confirmation.Title = "Confirmation";
                            events.Click.Confirmation.Message = "Are you sure want to delete selected record(s)?";
                            break;
                    }
                    if (paramList != null && paramList.Count > 0)
                        events.Click.ExtraParams.AddRange(paramList);
                    if (success != null && success.Trim() != string.Empty)
                        events.Click.Success = success;
                    if (failure != null && failure.Trim() != string.Empty)
                        events.Click.Failure = failure;
                });

            switch (button)
            {
                case Buttons.ButtonAdd: m_btnReturn.Icon(Icon.Add); break;
                case Buttons.ButtonDetail: m_btnReturn.Icon(Icon.ApplicationForm); break;
                case Buttons.ButtonUpdate: m_btnReturn.Icon(Icon.Pencil); break;
                case Buttons.ButtonDelete: m_btnReturn.Icon(Icon.Delete); break;
                case Buttons.ButtonList: m_btnReturn.Icon(Icon.Table); break;
                case Buttons.ButtonBrowse: m_btnReturn.Icon(Icon.Find); break;
                case Buttons.ButtonErase: m_btnReturn.Icon(Icon.Erase); break;
                case Buttons.ButtonSelect: m_btnReturn.Icon(Icon.Tick); break;
                case Buttons.ButtonSave: m_btnReturn.Icon(Icon.Disk); break;
                case Buttons.ButtonCancel: m_btnReturn.Icon(Icon.Cancel); break;
                case Buttons.ButtonVerify: m_btnReturn.Icon(Icon.ThumbUp); break;
                case Buttons.ButtonUnverify: m_btnReturn.Icon(Icon.ThumbDown); break;
                case Buttons.ButtonGenerate: m_btnReturn.Icon(Icon.Cog); break;
                case Buttons.ButtonPrint: m_btnReturn.Icon(Icon.Printer); break;
                case Buttons.ButtonExport: m_btnReturn.Icon(Icon.PageGo); break;
                case Buttons.ButtonSearch: m_btnReturn.Icon(Icon.Find); break;
                case Buttons.ButtonPreview: m_btnReturn.Icon(Icon.Zoom); break;
            }

            return m_btnReturn;
        }

        public static object GetFilterConditionValue(FilterHeaderCondition condition)
        {
            object m_objReturn = null;
            switch (condition.Type)
            {
                case FilterType.Boolean:
                    m_objReturn = condition.Value<bool>();
                    break;

                case FilterType.Date:
                    switch (condition.Operator)
                    {
                        case OpEquals:
                            m_objReturn = condition.Value<DateTime>();
                            break;

                        case OpComparation:
                            m_objReturn = FilterHeaderComparator<object>.Parse(condition.JsonValue).ToList();
                            break;
                    }
                    break;

                case FilterType.Number:
                    switch (condition.Operator)
                    {
                        case OpEquals:
                            //if (isInt)
                            //m_objReturn = condition.Value<int>();
                            //else
                            m_objReturn = condition.Value<double>();
                            break;

                        case OpComparation:
                            m_objReturn = FilterHeaderComparator<object>.Parse(condition.JsonValue).ToList();
                            //if (isInt)
                            //    m_objReturn = FilterHeaderComparator<int>.Parse(condition.JsonValue).ToList();
                            //else
                            //    m_objReturn = FilterHeaderComparator<double>.Parse(condition.JsonValue).ToList();
                            break;
                    }
                    break;
                case FilterType.String:
                    m_objReturn = condition.Value<string>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return m_objReturn;
        }

        public static Operator GetOperator(string filterOperator)
        {
            Operator m_optMappedOperator = Operator.None;

            switch (filterOperator)
            {
                case OpEquals:
                    m_optMappedOperator = Operator.Equals;
                    break;
                case OpStartsWith:
                    m_optMappedOperator = Operator.StartsWith;
                    break;
                case OpEndsWith:
                    m_optMappedOperator = Operator.EndsWith;
                    break;
                case OpNotEqual:
                    m_optMappedOperator = Operator.NotEqual;
                    break;
                case OpContains:
                    m_optMappedOperator = Operator.Contains;
                    break;
                case OpLessThan:
                    m_optMappedOperator = Operator.LessThan;
                    break;
                case OpLessThanEqual:
                    m_optMappedOperator = Operator.LessThanEqual;
                    break;
                case OpGreaterThan:
                    m_optMappedOperator = Operator.GreaterThan;
                    break;
                case OpGreaterThanEqual:
                    m_optMappedOperator = Operator.GreaterThanEqual;
                    break;
            }
            return m_optMappedOperator;
        }

        public static void ShowInfo(string title, string message)
        {
            X.Msg.Info(new InfoPanel
            {
                Closable = true,
                Html = message,
                PinEvent = __strPinEvent,
                HideDelay = __intNotificationHideDelay,
                UI = UI.Info,
                Title = title,
                Icon = Icon.Information,
                QueueName = __strQueueName,
                WidthSpec = "50%",
                Margin = 10,
                StyleSpec = "float:right;"
            }).Show();
        }

        public static void ShowInfoAlert(string title, string message)
        {
            X.MessageBox.Show(new MessageBoxConfig { Closable = true, Buttons = MessageBox.Button.OK, Icon = MessageBox.Icon.INFO, Title = title, Message = message });
        }

        public static void ShowError(string title, string message)
        {
            X.MessageBox.Info(new InfoPanel
            {
                Closable = true,
                Html = message,
                PinEvent = __strPinEvent,
                HideDelay = __intNotificationHideDelay,
                UI = UI.Danger,
                Title = title,
                Icon = Icon.Exclamation,
                QueueName = __strQueueName,
                WidthSpec = "50%",
                Margin = 10,
                StyleSpec = "float:right;"
            }).Show();
        }

        public static void ShowErrorAlert(string title, string message)
        {
            X.MessageBox.Show(new MessageBoxConfig { Closable = true, Buttons = MessageBox.Button.OK, Icon = MessageBox.Icon.ERROR, Title = title, Message = message });
        }

        public static void ShowWarning(string title, string message)
        {
            X.Msg.Info(new InfoPanel
            {
                Closable = true,
                Html = message,
                PinEvent = __strPinEvent,
                HideDelay = __intNotificationHideDelay,
                UI = UI.Warning,
                Title = title,
                Icon = Icon.Error,
                QueueName = __strQueueName,
                WidthSpec = "50%",
                Margin = 10,
                StyleSpec = "float:right;"
            }).Show();
        }

        public static void ShowWarningAlert(string title, string message)
        {
            X.MessageBox.Show(new MessageBoxConfig { Closable = true, Buttons = MessageBox.Button.OK, Icon = MessageBox.Icon.WARNING, Title = title, Message = message });
        }
        public static void ShowMessage(string title, string message, MessageBox.Button button, MessageBox.Icon icon, string fn)
        {
            X.Msg.Show(new MessageBoxConfig
            {
                Title = title,
                Message = message,
                Buttons = button,
                Icon = icon,
                Fn = new JFunction { Fn = fn }
            });
        }
        public static void ShowPrompt(string title, string message, string handler)
        {
            X.Msg.Prompt(title, message, handler).Show();
        }
        public static List<string> AggregateFunction
        {
            get
            {
                return __lstAggregateFunction;
            }
        }
        public static string GetTitle(Type typeClass)
        {
            Regex m_rgx = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z]) | (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
            return m_rgx.Replace(typeClass.Name.Replace("Controller", ""), " ");
        }
        public static bool ViewExists(Controller controller, string viewName)
        {
            ViewEngineResult result = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, null);
            return (result.View != null);
        }

        public static string ParseParameter(string templateContent, Dictionary<string,string> parameters){
            string m_strResult = string.Empty;
            try
            {
                m_strResult = Regex.Replace(templateContent, @"\[\[(.+?)\]\]", m => { return parameters.ContainsKey(m.Groups[1].Value) ? parameters[m.Groups[1].Value] : m.Groups[1].Value; });

            }
            catch (Exception keyEx)
            {

                throw keyEx;
            }
            return m_strResult;
        }
        public static string ParseParameter(string templateContent, Dictionary<string, string> parameters, string MailNotificationID, ref string messageError, object m_objConnectionIFTrans = null)
        {
            List<RecipientsVM> lst_ScheduleRecipient = new List<RecipientsVM>();
            if (!string.IsNullOrEmpty(MailNotificationID))
            {
                DataAccess.DRecipientsDA m_objDRecipientsDA = new DataAccess.DRecipientsDA();
                m_objDRecipientsDA.ConnectionStringName = Global.ConnStrConfigName;

                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(RecipientsVM.Prop.RecipientID.MapAlias);
                m_lstSelect.Add(RecipientsVM.Prop.RecipientDesc.MapAlias);

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(MailNotificationID);
                m_objFilter.Add(RecipientsVM.Prop.MailNotificationID.Map, m_lstFilter);

                Dictionary<int, System.Data.DataSet> RecipientDataset = m_objDRecipientsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, m_objConnectionIFTrans);
                if (m_objDRecipientsDA.Message == string.Empty)
                {
                    foreach (System.Data.DataRow dr in RecipientDataset[0].Tables[0].Rows)
                    {
                        lst_ScheduleRecipient.Add(new RecipientsVM()
                        {
                            RecipientID = dr[RecipientsVM.Prop.RecipientID.Name].ToString(),
                            MailNotificationID = dr[RecipientsVM.Prop.MailNotificationID.Name].ToString(),
                            OwnerID = dr[RecipientsVM.Prop.OwnerID.Name].ToString(),
                            RecipientDesc = dr[RecipientsVM.Prop.RecipientDesc.Name].ToString(),
                            RecipientTypeID = dr[RecipientsVM.Prop.RecipientTypeID.Name].ToString(),
                            MailAddress = dr[RecipientsVM.Prop.MailAddress.Name].ToString()
                        });
                    }
                }
                else
                    messageError = m_objDRecipientsDA.Message;
            }
            return ParseParameter(templateContent, parameters, lst_ScheduleRecipient, ref messageError);
        }
        public static string ParseParameter(string templateContent, Dictionary<string, string> parameters, List<RecipientsVM> lstRecipientDesc, ref string messageError, object objectConnectionIFTrans = null)
        {
            string ParseResult = "";
            if (lstRecipientDesc != null)
            {
                DataAccess.UConfigDA m_objUconfig = new DataAccess.UConfigDA();
                m_objUconfig.ConnectionStringName = Global.ConnStrConfigName;

                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ConfigVM.Prop.Key1.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Key2.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Key4.MapAlias);

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("InvitationTemplateTag");
                m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);                
                                
                Dictionary<int, System.Data.DataSet> ConfigTagTemplateTO = m_objUconfig.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, objectConnectionIFTrans);
                if (string.IsNullOrEmpty(m_objUconfig.Message))
                {
                    List<string> lstRecpTO = new List<string>();
                    List<string> lstRecpCC = new List<string>();

                    foreach (RecipientsVM obj in lstRecipientDesc.Where(x=>x.RecipientTypeID== ((int)RecipientTypes.TO).ToString()))
                        if(!lstRecpTO.Any(p=>p.Equals(obj.RecipientDesc)))
                        lstRecpTO.Add(obj.RecipientDesc);

                    foreach (RecipientsVM obj in lstRecipientDesc.Where(x => x.RecipientTypeID == ((int)RecipientTypes.CC).ToString()))
                        if (!lstRecpCC.Any(p => p.Equals(obj.RecipientDesc)))
                            lstRecpCC.Add(obj.RecipientDesc);

                    foreach (System.Data.DataRow dr in ConfigTagTemplateTO[0].Tables[0].Rows)
                    {
                        if (dr[ConfigVM.Prop.Key2.Name].ToString() == General.EnumDesc(RecipientTypes.TO))
                        {
                            if (parameters.ContainsKey(dr[ConfigVM.Prop.Key3.Name].ToString()))
                                parameters[dr[ConfigVM.Prop.Key3.Name].ToString()] = string.Join(dr[ConfigVM.Prop.Key4.Name].ToString(), lstRecpTO);
                            else
                                parameters.Add(dr[ConfigVM.Prop.Key3.Name].ToString(), string.Join(dr[ConfigVM.Prop.Key4.Name].ToString(), lstRecpTO));
                        }
                        else if (dr[ConfigVM.Prop.Key2.Name].ToString() == General.EnumDesc(RecipientTypes.CC))
                        {
                            if (parameters.ContainsKey(dr[ConfigVM.Prop.Key3.Name].ToString()))
                                parameters[dr[ConfigVM.Prop.Key3.Name].ToString()] = string.Join(dr[ConfigVM.Prop.Key4.Name].ToString(), lstRecpCC);
                            else
                                parameters.Add(dr[ConfigVM.Prop.Key3.Name].ToString(), string.Join(dr[ConfigVM.Prop.Key4.Name].ToString(), lstRecpCC));
                        }
                    }
                    try
                    {
                        ParseResult = ParseParameter(templateContent, parameters);
                    }
                    catch (Exception e)
                    {
                        messageError = e.Message;
                    }
                }
                else {
                    messageError = m_objUconfig.Message;
                }
            }
            return ParseResult;
        }


        #endregion

        protected void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            _dicTaskTimer = new Dictionary<string, object>();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

            // look if any security information exists for this request
            if (HttpContext.Current.User != null)
            {
                // see if this user is authenticated, any authenticated cookie (ticket) exists for this user
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    // see if the authentication is done using FormsAuthentication
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        // Get the roles stored for this request from the ticket
                        // get the identity of the user
                        FormsIdentity m_fid_Identity = (FormsIdentity)HttpContext.Current.User.Identity;
                        // get the forms authetication ticket of the user
                        FormsAuthenticationTicket m_fatTicket = m_fid_Identity.Ticket;
                        // get the roles stored as UserData into the ticket
                        string[] m_arrStrRoles = m_fatTicket.UserData.Split(',');
                        // create generic principal and assign it to the current request
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(m_fid_Identity, m_arrStrRoles);
                        RewriteLoginCache(false);
                    }
                }
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (Session["HostName"] == null)
            {
                string m_strLocalHostIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                try
                {
                    string m_strLocalHostName = Dns.GetHostEntry(m_strLocalHostIP).HostName;
                    if (m_strLocalHostName == "")
                        Session["HostName"] = m_strLocalHostIP;
                    else
                        Session["HostName"] = m_strLocalHostName.Split('.')[0];
                }
                catch (Exception)
                {
                    Session["HostName"] = m_strLocalHostIP;
                }
            }

            if (Session["HostIP"] == null)
            {
                Session["HostIP"] = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Cache.SetExpires(DateTimeOffset.Now.AddDays(-1).DateTime);
            HttpContext.Current.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Current.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            //_dateExpired = DateTimeOffset.Now.AddMinutes(ExpiredTime).DateTime;
            RewriteLoginCache(false);
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{

        //}

        //protected void Session_End(object sender, EventArgs e)
        //{

        //}

        protected void Application_End(object sender, EventArgs e)
        {
            Global.RewriteLoginCache(true);
        }

    }
}
