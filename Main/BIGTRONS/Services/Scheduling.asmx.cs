using com.SML.BIGTRONS.Controllers;
using com.SML.BIGTRONS.DataAccess;
using com.SML.BIGTRONS.Enum;
using com.SML.BIGTRONS.Models;
using com.SML.BIGTRONS.ViewModels;
using com.SML.Lib.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace com.SML.BIGTRONS.Services
{
    /// <summary>
    /// Summary description for Scheduling
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ResultScheduler
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class Scheduling : System.Web.Services.WebService
    {

        [WebMethod]
        public ResultScheduler SendMailWithCheck()
        {
            ResultScheduler resultReturn = new ResultScheduler();
            bool successSent = false;
            //Dictionary<string, object> lstReturn = new Dictionary<string, object>();
            BaseController BaseController_ = new BaseController();
            List<string> m_lstmsg = new List<string>();
            List<RecipientsVM> lstRecipient = new List<RecipientsVM>();
            RecipientsVM recipientsVM = new RecipientsVM();
            string message_ = "";
            string paramRecipient = "";
            string paramDateDay = "";
            string paramListSchedule = "";
            string paramSubjectReminder = "";
            string ContentBody = "";
            string ConfigContentBody = "";
            Dictionary<string, string> paramToSend;

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<string> m_lstSelect = new List<string>();

            try
            {

                #region GET CONFIG
                try
                {
                    try
                    {
                        paramRecipient = GetConfig(Global.Config_TemplateTag_Key1, Global.Config_TemplateTag_Key2, Global.Config_TemplateTag_RecipientList)[0].Key4;
                        paramDateDay = GetConfig(Global.Config_TemplateTag_Key1, Global.Config_TemplateTag_Key2, Global.Config_TemplateTag_DateDay)[0].Key4;
                        paramListSchedule = GetConfig(Global.Config_TemplateTag_Key1, Global.Config_TemplateTag_Key2, Global.Config_TemplateTag_ListSchedule)[0].Key4;
                        paramSubjectReminder = GetConfig(Global.Config_Reminder_Key1, Global.Config_Reminder_Subject)[0].Key4;
                    }
                    catch (Exception e)
                    {
                        message_ += e.Message;
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Error Get Config : " + ex.Message);
                }

                #endregion

                #region Get Template Body
                MNotificationTemplatesDA m_objTemplateMailDA = new MNotificationTemplatesDA();
                m_objTemplateMailDA.ConnectionStringName = Global.ConnStrConfigName;
                if (ConfigContentBody == "")
                {
                    m_objFilter = new Dictionary<string, List<object>>();
                    m_lstFilter = new List<object>();
                    m_lstSelect = new List<string>();
                    m_lstSelect = new List<string>();
                    m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateID.MapAlias);
                    m_lstSelect.Add(NotificationTemplateVM.Prop.Contents.MapAlias);
                    m_objFilter = new Dictionary<string, List<object>>();
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);

                    List<ConfigVM> templateSchedule = GetConfig("MailTemplate", "Scheduller");
                    message_ += (!templateSchedule.Any() ? templateSchedule[0].Key3 : "");
                    m_lstFilter.Add(templateSchedule.Any() ? templateSchedule[0].Key3 : "");
                    m_objFilter.Add(NotificationTemplateVM.Prop.NotificationTemplateID.Map, m_lstFilter);
                    Dictionary<int, DataSet> m_dicDAResult = new Dictionary<int, DataSet>();
                    m_dicDAResult = string.IsNullOrEmpty(message_) ? m_objTemplateMailDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null) : null;
                    if (m_objTemplateMailDA.Message == string.Empty && m_objTemplateMailDA.AffectedRows > 0)
                    {
                        ConfigContentBody = m_dicDAResult[0].Tables[0].Rows[0][NotificationTemplateVM.Prop.Contents.Name].ToString();
                    }
                    else
                    {
                        throw new Exception("Error getting content from Template Mail " + m_objTemplateMailDA.Message);
                    }
                }
                #endregion

                DRecipientsDA objRecipient = new DRecipientsDA
                {
                    ConnectionStringName = Global.ConnStrConfigName
                };

                m_objFilter = new Dictionary<string, List<object>>();

                m_lstSelect = new List<string>();

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Between);
                DateTime DayRange1 = DateTime.Now.AddDays(1).Date + new TimeSpan(0, 0, 0);
                m_lstFilter.Add(DayRange1.ToString(Global.SqlDateFormat));
                DateTime DayRange2 = DateTime.Now.AddDays(1).Date + new TimeSpan(23, 59, 59);
                m_lstFilter.Add(DayRange2.ToString(Global.SqlDateFormat));
                m_objFilter.Add(MailNotificationsVM.Prop.ScheduleStartDate.Map, m_lstFilter);

                m_lstFilter = new List<object>
                {
                    Operator.Equals,
                    (int)TaskStatus.Approved
                };
                m_objFilter.Add(MailNotificationsVM.Prop.TaskStatusID.Map, m_lstFilter);

                m_lstFilter = new List<object>
                {
                    Operator.Equals,
                    (int)RecipientTypes.TO
                };
                m_objFilter.Add(RecipientsVM.Prop.RecipientTypeID.Map, m_lstFilter);

                //m_lstFilter = new List<object>
                //{
                //    Operator.None,
                //    String.Empty
                //};
                //m_objFilter.Add(String.Format("{0} LIKE '%sinarmasland.com'", RecipientsVM.Prop.MailAddress.Map), m_lstFilter);

                //m_lstFilter = new List<object>
                //{
                //    Operator.None,
                //    String.Empty
                //};
                //m_objFilter.Add(String.Format("{0} IN ('taufik.setyawan@sinarmasland.com', 'revo.christie@sinarmasland.com', 'Novia.Silalahi@sinarmasland.com', 'Dita.Djatmiko@sinarmasland.com')", RecipientsVM.Prop.MailAddress.Map), m_lstFilter);

                m_lstSelect.Add(RecipientsVM.Prop.RecipientID.MapAlias);
                m_lstSelect.Add(RecipientsVM.Prop.RecipientDesc.MapAlias);
                m_lstSelect.Add(RecipientsVM.Prop.MailAddress.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.StartDate.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.Subject.MapAlias);

                Dictionary<int, DataSet> dicMMailNotif = objRecipient.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

                int sentCounter = 0;

                List<RecipientsVM> ListSentItems = new List<RecipientsVM>();

                if (objRecipient.Success && objRecipient.AffectedRows > 0)
                {
                    MailNotificationsVM mailNotificationsVM;
                    foreach (DataRow dr in dicMMailNotif[0].Tables[0].DefaultView.ToTable(true, RecipientsVM.Prop.RecipientDesc.Name, RecipientsVM.Prop.MailAddress.Name).Rows)
                    {
                        sentCounter++;

                        recipientsVM = new RecipientsVM
                        {
                            MailAddress = dr[RecipientsVM.Prop.MailAddress.Name].ToString().Trim(),
                            RecipientDesc = dr[RecipientsVM.Prop.RecipientDesc.Name].ToString().Trim(),
                            ScheduleDate = DateTime.Now
                        };
                        recipientsVM.lstMailNotification = new List<MailNotificationsVM>();
                        mailNotificationsVM = new MailNotificationsVM();

                        IEnumerable<DataRow> schedules = from s in dicMMailNotif[0].Tables[0].AsEnumerable()
                                                         where s.Field<string>(RecipientsVM.Prop.RecipientDesc.Name).Trim() == dr[RecipientsVM.Prop.RecipientDesc.Name].ToString().Trim() && s.Field<string>(RecipientsVM.Prop.MailAddress.Name).Trim() == dr[RecipientsVM.Prop.MailAddress.Name].ToString().Trim()
                                                         select s;
                        recipientsVM.lstMailNotification = new List<MailNotificationsVM>();
                        foreach (DataRow dr2 in (schedules.CopyToDataTable<DataRow>()).Rows)
                        {
                            mailNotificationsVM = new MailNotificationsVM
                            {
                                Subject = dr2[MailNotificationsVM.Prop.Subject.Name].ToString(),
                                ScheduleStartDate = (DateTime.Parse(dr2[SchedulesVM.Prop.StartDate.Name].ToString())).ToString(Global.DateFormatLongMonth),
                                ScheduleStartTime = (DateTime.Parse(dr2[SchedulesVM.Prop.StartDate.Name].ToString())).ToString(Global.ShortTimeFormat)
                            };

                            recipientsVM.lstMailNotification.Add(mailNotificationsVM);
                        }

                        paramToSend = new Dictionary<string, string>();
                        paramToSend.Add(paramRecipient, recipientsVM.RecipientDesc);
                        paramToSend.Add(paramDateDay, recipientsVM.ScheduleDate.ToString(Global.DateWithDay));
                        string ValueTableSchedule = "<table style=\"font-family: arial, sans-serif;\"><tr><th style=\" border: 1px solid #dddddd;text-align: left; padding:8px;\">Tanggal</th><th style=\" border: 1px solid #dddddd;text-align:left;padding:8px;\">Jam</th><th style=\" border: 1px solid #dddddd;text-align: left; padding:8px;\">Subject</th></tr>";
                        foreach (MailNotificationsVM mnt in recipientsVM.lstMailNotification.OrderBy(x => DateTime.Parse(x.ScheduleStartDate + " " + x.ScheduleStartTime)))
                        {
                            ValueTableSchedule += "<tr>";
                            ValueTableSchedule += "<td style=\" border: 1px solid #dddddd;text-align: left;padding:8px;\">" + mnt.ScheduleStartDate + "</td>";
                            ValueTableSchedule += "<td style=\" border: 1px solid #dddddd;text-align: left;padding:8px;\">" + mnt.ScheduleStartTime + "</td>";
                            ValueTableSchedule += "<td style=\" border: 1px solid #dddddd;text-align: left;padding:8px;\">" + mnt.Subject + "</td>";
                            ValueTableSchedule += "</tr>";
                        }
                        ValueTableSchedule += "</table>";
                        paramToSend.Add(paramListSchedule, ValueTableSchedule);
                        ContentBody = Global.ParseParameter(ConfigContentBody, paramToSend, new List<RecipientsVM> { recipientsVM }, ref message_);
                        successSent = BaseController_.SendMailScheduller(paramSubjectReminder, recipientsVM.MailAddress, ContentBody, ref message_);
                        if (!successSent)
                        {
                            throw new Exception(String.Format("Failed Sent Mail to {0} Row Number {1} Caused by {2}", recipientsVM.MailAddress, sentCounter, message_));
                        }
                        ListSentItems.Add(recipientsVM);
                    }


                    message_ = string.Format("Success sent {0} recipient{1} [Recipient{1} : {2}].", sentCounter, sentCounter > 1 ? "s" : "", string.Join(";", ListSentItems.Select(x => x.MailAddress)));
                    successSent = true;
                }
                else if (objRecipient.Success && objRecipient.AffectedRows == 0)
                {
                    successSent = true;
                    message_ = "No Schedule";
                }
                else
                {
                    throw new Exception(objRecipient.Message + " " + recipientsVM.MailAddress);
                }

            }
            catch (Exception ex)
            {
                message_ = "Error : " + ex.Message;
                resultReturn.Success = false;
                resultReturn.Message = message_;
                string ErrorContentBody = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                                            <html xmlns=""http://www.w3.org/1999/xhtml"">
                                            <head>
                                            <meta name=""viewport"" content=""width=device-width"" />
                                            <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
                                            <title>Alerts e.g. approaching your limit</title>
                                            <style>
                                                 /* ------------------------------------- GLOBAL ------------------------------------- */ * { margin: 0; padding: 0; font-family: ""Helvetica Neue"", ""Helvetica"", Helvetica, Arial, sans-serif; box-sizing: border-box; font-size: 14px; } img { max-width: 100%; } body { -webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; width: 100% !important; height: 100%; line-height: 1.6; } /* Let's make sure all tables have defaults */ table td { vertical-align: top; } /* ------------------------------------- BODY & CONTAINER ------------------------------------- */ body { background-color: #f6f6f6; } .body-wrap { background-color: #f6f6f6; width: 100%; } .container { display: block !important; max-width: 600px !important; margin: 0 auto !important; /* makes it centered */ clear: both !important; } .content { max-width: 600px; margin: 0 auto; display: block; padding: 20px; } /* ------------------------------------- HEADER, FOOTER, MAIN ------------------------------------- */ .main { background: #fff; border: 1px solid #e9e9e9; border-radius: 3px; } .content-wrap { padding: 20px; } .content-block { padding: 0 0 20px; } .header { width: 100%; margin-bottom: 20px; } .footer { width: 100%; clear: both; color: #999; padding: 20px; } .footer a { color: #999; } .footer p, .footer a, .footer unsubscribe, .footer td { font-size: 12px; } /* ------------------------------------- GRID AND COLUMNS ------------------------------------- */ .column-left { float: left; width: 50%; } .column-right { float: left; width: 50%; } /* ------------------------------------- TYPOGRAPHY ------------------------------------- */ h1, h2, h3 { font-family: ""Helvetica Neue"", Helvetica, Arial, ""Lucida Grande"", sans-serif; color: #000; margin: 40px 0 0; line-height: 1.2; font-weight: 400; } h1 { font-size: 32px; font-weight: 500; } h2 { font-size: 24px; } h3 { font-size: 18px; } h4 { font-size: 14px; font-weight: 600; } p, ul, ol { margin-bottom: 10px; font-weight: normal; } p li, ul li, ol li { margin-left: 5px; list-style-position: inside; } /* ------------------------------------- LINKS & BUTTONS ------------------------------------- */ a { color: #348eda; text-decoration: underline; } .btn-primary { text-decoration: none; color: #FFF; background-color: #348eda; border: solid #348eda; border-width: 10px 20px; line-height: 2; font-weight: bold; text-align: center; cursor: pointer; display: inline-block; border-radius: 5px; text-transform: capitalize; } /* ------------------------------------- OTHER STYLES THAT MIGHT BE USEFUL ------------------------------------- */ .last { margin-bottom: 0; } .first { margin-top: 0; } .padding { padding: 10px 0; } .aligncenter { text-align: center; } .alignright { text-align: right; } .alignleft { text-align: left; } .clear { clear: both; } /* ------------------------------------- Alerts ------------------------------------- */ .alert { font-size: 16px; color: #fff; font-weight: 500; padding: 20px; text-align: center; border-radius: 3px 3px 0 0; } .alert a { color: #fff; text-decoration: none; font-weight: 500; font-size: 16px; } .alert.alert-warning { background: #ff9f00; } .alert.alert-bad { background: #d0021b; } .alert.alert-good { background: #68b90f; } /* ------------------------------------- INVOICE ------------------------------------- */ .invoice { margin: 40px auto; text-align: left; width: 80%; } .invoice td { padding: 5px 0; } .invoice .invoice-items { width: 100%; } .invoice .invoice-items td { border-top: #eee 1px solid; } .invoice .invoice-items .total td { border-top: 2px solid #333; border-bottom: 2px solid #333; font-weight: 700; } /* ------------------------------------- RESPONSIVE AND MOBILE FRIENDLY STYLES ------------------------------------- */ @media only screen and (max-width: 640px) { h1, h2, h3, h4 { font-weight: 600 !important; margin: 20px 0 5px !important; } h1 { font-size: 22px !important; } h2 { font-size: 18px !important; } h3 { font-size: 16px !important; } .container { width: 100% !important; } .content, .content-wrapper { padding: 10px !important; } .invoice { width: 100% !important; } }       
                                            </style>
                                            </head>

                                            <body>

                                            <table class=""body-wrap"">
	                                            <tr>
		                                            <td></td>
		                                            <td class=""container"" width=""600"">
			                                            <div class=""content"">
				                                            <table class=""main"" width=""100%"" cellpadding=""0"" cellspacing=""0"">
					                                            <tr>
						                                            <td class=""alert alert-warning"">
							                                            Warning: Automatic Mail Scheduller is Error, please check.
						                                            </td>
					                                            </tr>
					                                            <tr>
						                                            <td class=""content-wrap"">
							                                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
								                                            <tr>
									                                            <td class=""content-block"">";
                ErrorContentBody += ex.Message;
                ErrorContentBody += @"</td>
								                                            </tr>
							                                            </table>
						                                            </td>
					                                            </tr>
				                                            </table>
				                                            <div class=""footer"">
				                                            </div></div>
		                                            </td>
		                                            <td></td>
	                                            </tr>
                                            </table>

                                            </body>
                                            </html>";
                //BaseController_.SendMailScheduller("Auto Scheduller Error", "revo.christie@sinarmasland.com;taufik.setyawan@sinarmasland.com", ErrorContentBody, ref message_);

                return resultReturn;
            }

            resultReturn.Success = successSent;
            resultReturn.Message = message_;
            return resultReturn;

        }
        private List<ConfigVM> GetConfig(string key1, string key2 = null, string key3 = null, string key4 = null)
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Key1.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key2.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key4.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc2.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc4.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(key1);
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            //if (!string.IsNullOrEmpty(key2)){
            //    m_lstFilter = new List<object>();
            //    m_lstFilter.Add(Operator.Equals);
            //    m_lstFilter.Add(key2);
            //    m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);
            //}
            if (!string.IsNullOrEmpty(key2))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(key2);
                m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);
            }
            if (!string.IsNullOrEmpty(key3))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(key3);
                m_objFilter.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);
            }
            if (!string.IsNullOrEmpty(key4))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(key4);
                m_objFilter.Add(ConfigVM.Prop.Key4.Map, m_lstFilter);
            }


            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
            if (m_objUConfigDA.Message == string.Empty)
            {
                m_lstConfigVM = (
                        from DataRow m_drUConfigDA in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key1 = m_drUConfigDA[ConfigVM.Prop.Key1.Name].ToString(),
                            Key2 = m_drUConfigDA[ConfigVM.Prop.Key2.Name].ToString(),
                            Key3 = m_drUConfigDA[ConfigVM.Prop.Key3.Name].ToString(),
                            Key4 = m_drUConfigDA[ConfigVM.Prop.Key4.Name].ToString(),
                            Desc1 = m_drUConfigDA[ConfigVM.Prop.Desc1.Name].ToString(),
                            Desc2 = m_drUConfigDA[ConfigVM.Prop.Desc2.Name].ToString(),
                            Desc3 = m_drUConfigDA[ConfigVM.Prop.Desc3.Name].ToString(),
                            Desc4 = m_drUConfigDA[ConfigVM.Prop.Desc4.Name].ToString()
                        }
                    ).ToList();
            }
            return m_lstConfigVM;
        }

    }

}
