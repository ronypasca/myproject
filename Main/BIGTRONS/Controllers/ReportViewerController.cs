using com.SML.BIGTRONS.Enum;
using com.SML.BIGTRONS.Models;
using com.SML.BIGTRONS.ViewModels;
using com.SML.Lib.Common;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using XMVC = Ext.Net.MVC;

namespace com.SML.BIGTRONS.Controllers
{
    public class ReportViewerController : BaseController
    {
        private string title = "ReportViewer";

        // GET: Report
        public ActionResult Index()
        {
            base.Initialize();
            return View();
        }
        //public ActionResult Form()
        //{
        //    Global.HasAccess = GetHasAccess();
        //    if (!Global.HasAccess.Read)
        //        return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

        //    return new XMVC.PartialViewResult
        //    {
        //        ClearContainer = true,
        //        ContainerId = General.EnumDesc(Params.MainContainer),
        //        RenderMode = RenderMode.AddTo,
        //        ViewName = "Bill/_Outstanding",
        //        WrapByScriptTag = false
        //    };
        //}
        public ActionResult LoadReport(string ReportViewName)
        {
            try
            {
                if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Print)))
                    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
                ReportViewName = (ReportViewName == null ? "" : JSON.Deserialize<string>(ReportViewName));
                if (Global.ViewExists(this, ReportViewName))
                    return new XMVC.PartialViewResult
                    {
                        ClearContainer = true,
                        ContainerId = General.EnumDesc(Params.PageContainer),
                        RenderMode = RenderMode.AddTo,
                        ViewName = ReportViewName,
                        WrapByScriptTag = false
                    };
            }
            catch (Exception ex)
            {
                Global.ShowError(title, ex.Message);
            }
            return this.Direct();
        }
        
        
    }
}