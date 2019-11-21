using com.SML.BIGTRONS.DataAccess;
using com.SML.BIGTRONS.Models;
using com.SML.BIGTRONS.ViewModels;
using com.SML.Lib.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace com.SML.BIGTRONS.Controllers
{
    public class ImageCatalogController : Controller
    {
        // GET: ImageCatalog
        public ActionResult Index(string itemID, string vendorID)
        {
            List<ItemShowCaseVM> m_listShowCase = new List<ItemShowCaseVM>();
            m_listShowCase = GetShowCase(itemID, vendorID);

            return View();
        }
        private List<ItemShowCaseVM> GetShowCase(string itemID, string vendorID)
        {
            ItemShowCaseVM m_objShowCaseVM = new ItemShowCaseVM();
            List<ItemShowCaseVM> m_lstShowCase = new List<ItemShowCaseVM>();
            MItemShowCaseDA m_objShoCaseDA = new MItemShowCaseDA();
            m_objShoCaseDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemShowCaseVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemShowCaseVM.Prop.Filename.MapAlias);
            m_lstSelect.Add(ItemShowCaseVM.Prop.ContentType.MapAlias);
            m_lstSelect.Add(ItemShowCaseVM.Prop.RawData.MapAlias);
            m_lstSelect.Add(ItemShowCaseVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(ItemShowCaseVM.Prop.ShowCaseID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            if (!string.IsNullOrEmpty(vendorID))
            {
                List<object> m_lstFilter = new List<object>();
                m_lstKey.Add(vendorID);
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(vendorID);
                m_objFilter.Add(ItemShowCaseVM.Prop.Map("VendorID", false), m_lstFilter);
            }

            if (!string.IsNullOrEmpty(itemID))
            {
                List<object> m_lstFilter = new List<object>();
                m_lstKey.Add(itemID);
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(itemID);
                m_objFilter.Add(ItemShowCaseVM.Prop.Map("ItemID", false), m_lstFilter);
            }
            
            List<ItemShowCaseVM> listItemShowCase = new List<ItemShowCaseVM>();
            Dictionary<int, DataSet> m_dicDItemShowCase = m_objShoCaseDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

            ItemShowCaseVM m_itemShowCaseVM;

            if (m_dicDItemShowCase[0].Tables[0].Rows.Count == 0)
            {
                m_itemShowCaseVM = new ItemShowCaseVM();
                m_itemShowCaseVM.RawData = Url.Content("/Content/Images/thumbs/noimage.jpg");
                m_itemShowCaseVM.ListItemVM = GetItemList(itemID.ToString());
                m_itemShowCaseVM.ListItemDetailsVM = GetItemDetailList(itemID.ToString());
                listItemShowCase.Add(m_itemShowCaseVM);
            }
            else
            {
                foreach (DataRow d_itemShowCase in m_dicDItemShowCase[0].Tables[0].Rows)
                {
                    m_itemShowCaseVM = new ItemShowCaseVM();
                    m_itemShowCaseVM.ShowCaseID = d_itemShowCase[ItemShowCaseVM.Prop.ShowCaseID.Name].ToString();
                    m_itemShowCaseVM.ItemID = d_itemShowCase[ItemShowCaseVM.Prop.ItemID.Name].ToString();
                    m_itemShowCaseVM.Filename = d_itemShowCase[ItemShowCaseVM.Prop.Filename.Name].ToString();
                    m_itemShowCaseVM.ContentType = d_itemShowCase[ItemShowCaseVM.Prop.ContentType.Name].ToString();
                    m_itemShowCaseVM.RawData = "data:image/png;base64, " + d_itemShowCase[ItemShowCaseVM.Prop.RawData.Name].ToString();
                    m_itemShowCaseVM.VendorID = d_itemShowCase[ItemShowCaseVM.Prop.VendorID.Name].ToString();
                    m_itemShowCaseVM.ListItemVM = GetItemList(d_itemShowCase[ItemShowCaseVM.Prop.ItemID.Name].ToString());
                    m_itemShowCaseVM.ListItemDetailsVM = GetItemDetailList(d_itemShowCase[ItemShowCaseVM.Prop.ItemID.Name].ToString());
                    listItemShowCase.Add(m_itemShowCaseVM);
                }
            }
            ViewBag.ImageData = listItemShowCase;
            return listItemShowCase;
        }
        private List<ItemVM> GetItemList(string itemID)
        {
            List<ItemVM> lst_itemVM = new List<ItemVM>();
            MItem m_objItem = new MItem();
            MItemDA m_objItemDA = new MItemDA();
            m_objItemDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(itemID);
            m_objFilter.Add(ItemVM.Prop.Map("ItemID", false), m_lstFilter);

            Dictionary<int, DataSet> m_dicDItem = m_objItemDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

            ItemVM m_objItemVM;

            foreach (DataRow d_item in m_dicDItem[0].Tables[0].Rows)
            {
                m_objItemVM = new ItemVM();
                m_objItemVM.ItemID = d_item[ItemVM.Prop.ItemID.Name].ToString();
                m_objItemVM.ItemDesc = d_item[ItemVM.Prop.ItemDesc.Name].ToString();
                lst_itemVM.Add(m_objItemVM);
            }
            return lst_itemVM;
        }
        private List<ItemDetailVM> GetItemDetailList(string itemID)
        {
            List<ItemDetailVM> lst_itemDetailVM = new List<ItemDetailVM>();
            DItemDetails m_objItemDetail = new DItemDetails();
            DItemDetailsDA m_objItemDetailDA = new DItemDetailsDA();
            m_objItemDetailDA.ConnectionStringName = Global.ConnStrConfigName;
            string where = "";

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemDetailVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemDetailVM.Prop.ItemDetailDesc.MapAlias);

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            where += String.Format("DItemDetails.ItemID = '{0}' AND DItemDetails.ItemDetailTypeID = 'DEC'", itemID.ToString());
            m_objFilter.Add(where, m_lstFilter);

            Dictionary<int, DataSet> m_dicDItemDetail = m_objItemDetailDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

            ItemDetailVM m_objItemDetailVM;

            foreach (DataRow d_item in m_dicDItemDetail[0].Tables[0].Rows)
            {
                m_objItemDetailVM = new ItemDetailVM();
                m_objItemDetailVM.ItemID = d_item[ItemDetailVM.Prop.ItemID.Name].ToString();
                m_objItemDetailVM.ItemDetailDesc = d_item[ItemDetailVM.Prop.ItemDetailDesc.Name].ToString();
                lst_itemDetailVM.Add(m_objItemDetailVM);
            }
            return lst_itemDetailVM;
        }
    }
}