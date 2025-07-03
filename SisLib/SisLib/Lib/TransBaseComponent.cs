using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using DevExpress.Blazor;
using SisData.Data;
using SisData.Model;
using SisData.Service;
using SisData.Dataaccess;
using DevExpress.DataAccess.ConnectionParameters;
using System.Reflection.Metadata;
using Microsoft.Extensions.Localization;
using Blazored.LocalStorage;
using System.Data;
using Microsoft.JSInterop;
using SisCom.Shared;
using SisLib.Ctrl;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices.JavaScript;
using DevExpress.Office.NumberConverters;
using SisFunc;
using DevExpress.CodeParser.Html;
using DevExpress.Data.Browsing;
using Microsoft.AspNetCore.Hosting;

namespace SisLib.Lib
{
    public class TransBaseComponent : ComponentBase, IDisposable
    {
        #region Parameter
        [Inject] public IOptionsManager myOption { get; set; }
        [Inject] public ISysVarManager mySysvar { get; set; }
        [Inject] public IDxWindowService myModal { get; set; }
        [Inject] public IDbManager myDb { get; set; }
        [Inject] public ISqldataacceess mySqldb { get; set; }
        [Inject] public ILocalStorageService myLcStore { get; set; }
        [Inject] public Statemanagerment myStateMN { get; set; }
        [Inject] public NavigationManager myNavi { get; set; }
        [Inject] public ICommandManager myCommand { get; set; }
        [Inject] public IJSRuntime JavacriptS { get; set; }
        [Inject] public XmlStringLocalizer Lap { get; set; }
        [Inject] IDialogService myDlSvc { get; set; }
        public bool? ResultDl { get; set; } = null;
        public string MessageDl { get; set; } = "";        
        [Parameter] public string Menu_id { get; set; }
        public string Ws_Id = string.Empty;
        public string Ma_ct = string.Empty;
        public string Nh_menu;
        public Command curCommand = new Command();
        public Dmct curDmct = new Dmct();
        public DataTable tbStatus = (DataTable)null;
        public DataSet DsTrans = (DataSet)null;
        public ActionTask curActionTask { get; set; } = ActionTask.None;
        public FrmBrowseComponent oBrowsePH;
        public FrmBrowseComponent oBrowseCT;
        public FrmBrowseComponent oBrowseGT;

        public string M_Phdbf = string.Empty;
        public string M_Ctdbf = string.Empty;
        public string M_Ctgtdbf = string.Empty;
        public string SqlTableKey = "stt_rec";
        public string SqlTableKeyValue = "";

        public DataTable TblPH = (DataTable)null;
        public DataTable TblCT = (DataTable)null;
        public DataTable TblGT = (DataTable)null;

        public string FbrowsePH = "";
        public string FbrowseCT = "";
        public string FbrowseGT = "";
        public string phFilter = "1=1";
        public string ctFilter = "1=1";
        public string ctgtFilter = "1=1";

        public DxPopup OptwRef;
        public DxPopup popFilter { get; set; }
        public bool ShowLoadding { get; set; } = false;
        public bool ShowLoaddingVoucher { get; set; } = true;
        public bool popFilterVisible { get; set; } = false;
        public bool IsShowFrm { get; set; } = false;
        public bool IsShowImport { get; set; } = false;
        public Dictionary<string, object> Par { get; set; } = new Dictionary<string, object>();
        public Type FrmType { get; set; }
        public IList LSData;
        public string mPage = "Socthda";
        public string editPage = "frmSocthda";        
        
        public Users curUser;
        public bool IsLangEn = false;
        public string UserLang = "";
        public static string LanguageID = "";
        public DateTime Ngay_ct1 = DateTime.Now.Date;
        public DateTime Ngay_ct2 = DateTime.Now.Date;

        public string filterView = string.Empty;
        public string M_ma_nt0 = string.Empty;        
        public string M_MST_CHECK = string.Empty;
        public int M_User_Id = 0;
        public string M_ngay_lct = string.Empty;
        public string M_ong_ba = string.Empty;
        public int M_sl_ct0 = 0;
        
        public string M_MA_THUE = "";
        public string M_WS_ID = "";
        public string M_IP_SL = "";
        public string M_IP_GIA = "";
        public string M_IP_GIA_NT = "";
        public string M_IP_TIEN = "";
        public string M_IP_TIEN_NT = "";
        public string M_IP_TY_GIA = "";
        public string M_IP_TY_GIAF = "";

        public int M_ROUND;
        public int M_ROUND_NT;
        public int M_ROUND_GIA;
        public int M_ROUND_GIA_NT;
        public string[] Check_Valid_Store;
        public string[] Post_store;
        public string[] Process_store;
        public string Deletemessage { get; set; }       
        public object SelectedItem { get; set; }
        public int iRow = 0;
        public int pageSize = 20;
        public bool IsPageSize = true;

        private DotNetObjectReference<TransBaseComponent>? _dotNetHelper;
        #endregion
        public virtual void CurItem(object obj)
        {
            if (obj != null)
            {
                SelectedItem = obj;
                DataRowView rview = (DataRowView)SelectedItem;
                SqlTableKeyValue = rview[SqlTableKey].ToString().Trim();
                if(TblCT != null)
                    TblCT.DefaultView.RowFilter = SqlTableKey + "='" + SqlTableKeyValue + "'";
                if(TblGT != null)
                {
                    TblGT.DefaultView.RowFilter = SqlTableKey + "='" + SqlTableKeyValue + "'";
                }
                iRow = TblPH.DefaultView.Cast<DataRowView>()
                      .ToList()
                      .FindIndex(row => row == rview);                
            }
            else
            {
                if (TblCT != null) TblCT.DefaultView.RowFilter = SqlTableKey + "=''";
                if (TblGT != null) TblGT.DefaultView.RowFilter = SqlTableKey + "=''";
            }
        }
        protected override void OnInitialized()
        {
            if (String.IsNullOrEmpty(Menu_id))
                Menu_id = "we.sm.23";
            else
            {
                Menu_id = Menu_id.Replace("_", ".");
            }            
            curUser = myStateMN.GetCurUser();
            Ngay_ct1 = myStateMN.GetStartDate();
            Ngay_ct2 = myStateMN.GetEndDate();

            base.OnInitialized();
            
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Language();                
                await PageLoad();
                if (curCommand == null)
                {
                    myStateMN.SetToolBar(new MyToolbar("toolbar", false));
                    return;
                }
                await Lap.LoadLocalizationAsync(String.Format("{0}-{1}", String.IsNullOrEmpty(LanguageID) ? curCommand.Ma_phan_he.Trim().ToLower() : LanguageID, UserLang.Trim()));
                string Menuid = Menu_id.Replace("_", ".");
                if (curUser.Is_admin == 1 || curUser.CheckRight(Menuid, "VIEW"))
                {
                    myStateMN.SetToolBar(new MyToolbar("toolbar", true));
                    myStateMN.SetToolBar(new MyToolbar("New", true));
                    myStateMN.SetToolBar(new MyToolbar("Edit", true));                    
                    myStateMN.SetToolBar(new MyToolbar("Delete", true));
                    myStateMN.SetToolBar(new MyToolbar("View", true));
                    myStateMN.SetToolBar(new MyToolbar("Print", true));
                    if (IsShowImport) myStateMN.SetToolBar(new MyToolbar("Import", true));
                    myStateMN.SetToolBar(new MyToolbar("Export", true));
                    myStateMN.SetToolBar(new MyToolbar("Search", true));
                    myStateMN.SetToolBar(new MyToolbar("Quit", false));
                }
                else
                {
                    myStateMN.SetToolBar(new MyToolbar("toolbar", false));
                }

                myStateMN.OnActionTaskChanged += OnActionTaskchanged;
                myStateMN.OnLangguaChanged += OnLangguachange;
                _dotNetHelper = DotNetObjectReference.Create(this);
                await JavacriptS.InvokeVoidAsync("blazorKeyListener.init", _dotNetHelper);
                ShowLoaddingVoucher = false;
                StateHasChanged();
            }
        }
        public virtual async Task PageLoad()
        {
            M_MST_CHECK = myOption.GetOptionsValue("M_MST_CHECK").ToString().Trim();
            M_MA_THUE = myOption.GetOptionsValue("M_MA_THUE").ToString();
            Ws_Id = myOption.GetOptionsValue("WS_ID").ToString().Trim();
            M_ma_nt0 = myOption.GetOptionsValue("M_MA_NT0").ToString().Trim();

            curCommand = await myCommand.GetSubMenuBymenuId(Menu_id.Replace("_", "."));
            if (curCommand != null)
            {
                mPage = curCommand.Mobile_page;
                Nh_menu = String.IsNullOrEmpty(curCommand.Nh_menu)?"":curCommand.Nh_menu.Trim();
                Ma_ct = curCommand.Ma_ct.Trim();

                string[] strBrowse = (IsLangEn? curCommand.Ebrowse1: curCommand.Vbrowse1).Split('|');
                FbrowsePH = String.IsNullOrEmpty(strBrowse[0])? @"stt_rec:110:h=stt rec;ngay_ct:80:h=Ngày c.từ:FL:D;so_ct:85:h=Số hđ:FL:HR:LINKCT" : strBrowse[0];
                FbrowseCT = strBrowse.Length > 1?strBrowse[1]: @"stt_rec:110:h=stt rec;stt_rec0:110:h=stt rec0";
                FbrowseGT = strBrowse.Length > 2 ? strBrowse[2] : @"stt_rec:110:h=stt rec;stt_rec0:110:h=stt rec0";

                //Load dmct
                string strsql = "Select * from dmct where ma_ct = '" + Ma_ct + "'";
                DataSet dsdmct = myDb.LoadDataSet(strsql);
                if(dsdmct.Tables.Count > 0)
                {                    
                    curDmct = myDb.ConvertDataRow<Dmct>(dsdmct.Tables[0].Rows[0]);
                    M_Phdbf = curDmct.M_phdbf;
                    M_Ctdbf = curDmct.M_ctdbf;
                    M_Ctgtdbf = curDmct.M_ctgtdbf;
                    Process_store = curDmct.Process_Store.Trim().Split('|');
                    Post_store = curDmct.Post_store.Trim().Split('|');
                    Check_Valid_Store = curDmct.Check_valid_store.Trim().Split('|');
                    pageSize = curDmct.M_sl_ct0>0?curDmct.M_sl_ct0: pageSize;
                    IsPageSize = pageSize > curDmct.M_sl_ct0;
                }
                else
                {
                    myStateMN.SetNewThongbao(String.Format(Lap["No voucher {0}"],Ma_ct) + " !");
                    return;
                }
                await Loaddata();
                //string strph = SisFunc.Converts.DataTable2Model(DsTrans.Tables[0], "Ph82");
                //string strct = SisFunc.Converts.DataTable2Model(DsTrans.Tables[1], "Ct82");
                
            }
            else
            {
                myStateMN.SetNewThongbao(Lap["No Command"] + " !");
                return;
            }            
        }
        public virtual async Task Loaddata(bool isStartup = true)
        {
            string _ma_dvcs = myStateMN.GetMa_Dvcs();
            string strFilter = (String.IsNullOrEmpty(phFilter)?"1=1": phFilter) + (_ma_dvcs.ToUpper().Trim().Equals("ALL") ? "" : " AND ma_dvcs like ''" + _ma_dvcs.Trim() + "%''");
            string strsql = "Exec " + Process_store[0].Trim() + " @ma_ct='" + Ma_ct + "',@sPhFilter ='" + strFilter + "',@sCtFilter='" + ctFilter + "',@sGtFilter='" + ctgtFilter + "',@sl_ct=" + (isStartup?curDmct.M_sl_ct0.ToString().Trim():"-1");
            DsTrans = myDb.LoadDataSet(strsql);
            TblPH = DsTrans.Tables.Count > 0 ? DsTrans.Tables[0] : (DataTable)null;
            TblCT = DsTrans.Tables.Count > 1 ? DsTrans.Tables[1] : (DataTable)null;
            TblGT = DsTrans.Tables.Count > 2 ? DsTrans.Tables[2] : (DataTable)null;
            pageSize = curDmct.M_sl_ct0 > 0 ? curDmct.M_sl_ct0 : pageSize;
            IsPageSize = (TblPH.Rows.Count > pageSize?true:false);

            if (DsTrans.Tables.Count <= 0)
            {
                myStateMN.SetNewThongbao(String.Format(Lap["No data {0}"], Ma_ct) + " !");
                return;
            }

            if (TblCT != null)
            {
                TblCT.DefaultView.Sort = "stt_rec0 asc";
                if (TblPH.DefaultView.Count > 0)
                    TblCT.DefaultView.RowFilter = SqlTableKey + "='" + TblPH.DefaultView[0][SqlTableKey].ToString().Trim() + "'";
            }
            if (TblGT != null)
            {
                TblGT.DefaultView.Sort = "stt_rec0 asc";
                if (TblGT.DefaultView.Count > 0 && TblPH.DefaultView.Count > 0)
                {
                    TblGT.DefaultView.RowFilter = SqlTableKey + "='" + TblPH.DefaultView[0][SqlTableKey].ToString().Trim() + "'";
                }
            }
            if (TblPH != null)
                TblPH.DefaultView.Sort = "ngay_ct desc, so_ct desc";
        }
        public virtual void OnActionTaskchanged(ActionTask ac)
        {
            switch (ac)
            {
                case ActionTask.New:
                    myStateMN.SetCurrentActionTask(ActionTask.New);
                    V_Moi();
                    break;
                case ActionTask.Copy:
                    myStateMN.SetCurrentActionTask(ActionTask.Copy);
                    V_Copy();
                    break;
                case ActionTask.Edit:
                    myStateMN.SetCurrentActionTask(ActionTask.Edit);
                    V_Sua();
                    break;
                case ActionTask.View:
                    myStateMN.SetCurrentActionTask(ActionTask.View);
                    V_Xem();
                    break;
                case ActionTask.Search:
                    myStateMN.SetCurrentActionTask(ActionTask.Search);
                    V_Tim();
                    break;
                case ActionTask.Print:
                    myStateMN.SetCurrentActionTask(ActionTask.Print);
                    V_In();
                    break;
                case ActionTask.Delete:
                    V_Xoa();
                    break; 
                case ActionTask.Export:
                    ExportData(1);
                    break;
                case ActionTask.Export2:
                    ExportData(2);
                    break;
                case ActionTask.Export3:
                    ExportData(3);
                    break;
            }
        }
        public virtual void V_Moi()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {
                DataRowView newData = (SelectedItem as DataRowView);
                SqlTableKeyValue = newData[SqlTableKey].ToString().Trim();
            }
            else
            {
                SqlTableKeyValue = "";
            }    
            Par.Clear();
            Par.Add("Stt_rec", (object)SqlTableKeyValue);
            Par.Add("Ma_ct", (object)Ma_ct);
            Par.Add("Menu_id", (object)Menu_id);
            Par.Add("curActionTask", (object)ActionTask.New);
            Par.Add("Closeform", EventCallback.Factory.Create<bool>(this, CloseForm));

            IsShowFrm = true;
            StateHasChanged();           
        }
        public virtual void V_Copy()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {
                
            }
        }
        public virtual void V_Sua()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {
                SqlTableKeyValue = rview[SqlTableKey].ToString().Trim();
                //if (rview.Row.Table.Columns.Contains("ngay_ct") && rview.Row.Table.Rows.Count > 1 && !SisData.Lib.CheckValidNgayKs(mySysvar, myDb, new DateTime?(Convert.ToDateTime(rview["ngay_ct"])), Ma_ct))
                //{
                //    myStateMN.SetNewThongbao(Lap["Data is locked, cannot be edited!"]);
                //    return;
                //}
                if ((!(Ma_ct == "HDA") && !(Ma_ct == "HD1") || !(rview["stt_rec_pt"].ToString().Trim() != "")) && CheckPhanBo(rview, myDb, Post_store, Check_Valid_Store, SqlTableKeyValue) == 1)
                {
                    myStateMN.SetNewThongbao(Lap["Do not edit paid invoices!"]);
                    return;
                }

                Par.Clear();
                Par.Add("Stt_rec", (object)SqlTableKeyValue);
                Par.Add("Ma_ct", (object)Ma_ct);
                Par.Add("Menu_id", (object)Menu_id);
                Par.Add("curActionTask", (object)ActionTask.Edit);
                Par.Add("Closeform", EventCallback.Factory.Create<bool>(this, CloseForm));

                IsShowFrm = true;
                StateHasChanged();
            }
            else
            {
                myStateMN.SetNewThongbao(Lap["No data selected"]);
            }
        }
        public async void OnRDBClick(GridRowClickEventArgs e)
        {
            V_Xem();
        }
        public virtual void V_Xem()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {
                DataRowView newData = (SelectedItem as DataRowView);
                SqlTableKeyValue = newData[SqlTableKey].ToString().Trim();

                Par.Clear();
                Par.Add("Stt_rec", (object)SqlTableKeyValue);
                Par.Add("Ma_ct", (object)Ma_ct);
                Par.Add("Menu_id", (object)Menu_id);
                Par.Add("curActionTask", (object)ActionTask.View);
                Par.Add("Closeform", EventCallback.Factory.Create<bool>(this, CloseForm));

                IsShowFrm = true;
                StateHasChanged();
            }
            else
                myStateMN.SetNewThongbao(Lap["No data selected"]);
        }
        public virtual void V_Tim()
        {
            popFilterVisible = true;            
            StateHasChanged();
        }
        async void CloseForm(bool value)
        {
            if (value)
            {
                await Loaddata(false);
            }
            IsShowFrm = false;
            StateHasChanged();
        }
        public virtual async void V_Xoa()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {
                SqlTableKeyValue = rview[SqlTableKey].ToString().Trim();
                //if (rview.Row.Table.Columns.Contains("ngay_ct") && rview.Row.Table.Rows.Count > 1 && !SisData.Lib.CheckValidNgayKs(mySysvar, myDb, new DateTime?(Convert.ToDateTime(rview["ngay_ct"])), Ma_ct))
                //{
                //    myStateMN.SetNewThongbao(Lap["Data is locked, cannot be edited!"]);
                //    return;
                //}
                if ((!(Ma_ct == "HDA") && !(Ma_ct == "HD1") || !(rview["stt_rec_pt"].ToString().Trim() != "")) && CheckPhanBo(rview, myDb, Post_store, Check_Valid_Store, SqlTableKeyValue) == 1)
                {
                    myStateMN.SetNewThongbao(Lap["Do not edit paid invoices!"]);
                    return;
                }
                MessageDl = String.Format(Lap["Do you want to delete :{0} ?"], rview["so_ct"].ToString().Trim());
                ResultDl = await myDlSvc.ConfirmAsync(new MessageBoxOptions()
                {
                    Text = MessageDl,
                    Title = Lap["Confirm deletion of voucher"],
                    OkButtonText = Lap["OK"],
                    CancelButtonText = Lap["Cancel"],
                    ShowIcon = true,
                    ShowCloseButton = false,
                    RenderStyle = MessageBoxRenderStyle.Danger,
                });
                if (ResultDl == true)
                {
                    DeleteVoucher(SqlTableKeyValue);
                }

                StateHasChanged();
            }
            else
            {
                myStateMN.SetNewThongbao(Lap["No data selected"]);
            }
        }
        public virtual async void V_In()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview == null)
            {               
                myStateMN.SetNewThongbao(Lap["No data selected"]);
                return;
            }
           
            DataSet DataSource = CopyVoucherToDataSet(SqlTableKeyValue);
            //if (DataSource != null && DataSource.Tables.Count > 0)
            //{
            //    LSData = new List<HDBanleModel>();
            //    HDBanleModel _curModel = new HDBanleModel();                
            //    _curModel.LSPh = myDb.ConvertDataTable<Ph83>(DataSource.Tables[0]);
            //    _curModel.LSCt = myDb.ConvertDataTable<Ct83>(DataSource.Tables[1]);
            //    LSData.Add(_curModel);
            //}
            //ShowReport(LSData, typeof(HDBanleModel));           
        }
        public void ShowReport(IList LSItems, Type Itype)
        {
            Dictionary<string, object> Par = new Dictionary<string, object>();
            Par.Add("LSItems", (object)LSItems);
            Par.Add("Menu_id", (object)Menu_id);
            var aty = typeof(ReportOptionComponent<>).MakeGenericType(new[] { Itype });
            DxWindowModel model = new DxWindowModel(aty, Par, true, "BC");
            Par.Add("DxWindowmodel", model);
            myModal.Show(model);
        }
        public virtual async void OnLangguachange(MyLang lang)
        {
            if (lang.IsnewLang)
            {
                await Language();
            }
        }
        public virtual async Task Language()
        {
            try
            {
                IsLangEn = false;
                UserLang = await myLcStore.GetItemAsync<string>("userlang");
                if (!string.IsNullOrEmpty(UserLang))
                {
                    if (UserLang.Equals("En"))
                    {
                        IsLangEn = true;
                    }
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public virtual void DeleteVoucher(string _stt_rec)
        {
            string strdele = "DeleteVoucher";
            string strpara = "cMa_ct;stt_rec";
            string strparavalue = Ma_ct + ";" + _stt_rec;
            myDb.LoadStoreProcedure(strdele,strpara,strparavalue);
            foreach(DataTable tbl in DsTrans.Tables)
            {
                foreach (DataRow row in tbl.Select(SqlTableKey + "='" + _stt_rec + "'"))
                    tbl.Rows.Remove(row);
            }
            if(iRow > TblPH.Rows.Count-1)
            {
                iRow = TblPH.Rows.Count - 1;
            }
            if (iRow >= 0)
                CurItem(TblPH.DefaultView[iRow]);
        }
        public virtual DataSet CopyVoucherToDataSet(string _stt_rec)
        {
            DataSet dtsPrt = new DataSet();
            foreach (DataTable tbl in DsTrans.Tables)
            {
                DataTable tblNew = tbl.Clone();
                foreach (DataRow row in tbl.Select(SqlTableKey + "='" + _stt_rec + "'"))
                {
                    tblNew.ImportRow(row);
                }
                dtsPrt.Tables.Add(tblNew);
            }
            return dtsPrt;
        }
        public virtual async void ExportData(int type)
        {
            oBrowsePH.ExPortGrid(type);
            string ToastMessage = Lap["Export successfully"].ToString();
            myStateMN.SetNewThongbao(ToastMessage);
            StateHasChanged();
        }
        public virtual void LoadOptions()
        {
            M_ma_nt0 = myOption.GetOptionsValue("M_MA_NT0");
            M_MA_THUE = myOption.GetOptionsValue("M_MA_THUE");
            M_WS_ID = myOption.GetOptionsValue("M_WS_ID").Trim().ToUpper();
            M_IP_SL = myOption.GetOptionsValue("M_IP_SL");
            M_IP_GIA = myOption.GetOptionsValue("M_IP_GIA");
            M_IP_GIA_NT = myOption.GetOptionsValue("M_IP_GIA_NT");
            M_IP_TIEN = myOption.GetOptionsValue("M_IP_TIEN");
            M_IP_TIEN_NT = myOption.GetOptionsValue("M_IP_TIEN_NT");
            M_IP_TY_GIA = myOption.GetOptionsValue("M_IP_TY_GIA");
            M_IP_TY_GIAF = myOption.GetOptionsValue("M_IP_TY_GIAF");
        }
        public void BtnFilterCancel()
        {
            popFilterVisible = false;
            StateHasChanged();
        }
        [JSInvokable]
        public void OnKeyDown(int keyCode)
        {
            if (keyCode == 113)
            {
                myStateMN.SetCurrentActionTask(ActionTask.View);
                V_Xem();
            }
            if (keyCode == 114) // F3 có mã ASCII là 114
            {
                myStateMN.SetCurrentActionTask(ActionTask.Edit);
                V_Sua();
            }
            if (keyCode == 115)
            {
                myStateMN.SetCurrentActionTask(ActionTask.New);
                V_Moi();
            }
            if (keyCode == 118)
            {
                myStateMN.SetCurrentActionTask(ActionTask.Delete);
                V_In();
            }
            if (keyCode == 119)
            {
                myStateMN.SetCurrentActionTask(ActionTask.Delete);
                V_Xoa();
            }
        }
        public async ValueTask DisposeAsync()
        {
            if (_dotNetHelper is not null)
            {
                try
                {
                    await JavacriptS.InvokeVoidAsync("blazorKeyListener.remove", _dotNetHelper);
                    _dotNetHelper.Dispose();
                }
                catch{ }
            }
        }
        public static int CheckPhanBo(DataRowView drview,IDbManager myDb, string[] Post_store, string[] Check_Valid_Store, string stt_rec)
        {
            if (drview.Row.Table.Columns.Contains("stt_rec_hd") && !string.IsNullOrEmpty(drview["stt_rec_hd"].ToString().Trim()))
                return 0;
            int num = 0;
            if (drview.Row.Table.Columns.Contains("ma_ct"))
            {
                string upper = drview["ma_ct"].ToString().ToUpper();
                string str1 = "PN1;PN9;HD9;PN2;PN6;HD1;HD2;HD6;PC1;BN1;PT1;BC1;PNG;PNA;PNB;PNC;HDM;HDA;HDB;HDX;PXF;PNF;HD5;HDL";
                string str2 = "HD3;HD4;PN9;HD9;PC1;BN1;PT1;BC1;PXF";
                if (upper.Equals("PK1"))
                {
                    string format1 = "exec [dbo].{0} @stt_rec";
                    string strsql = "exec " + ((Post_store == null || Post_store.Length <= 0) ? string.Format(format1, (object)"InPostCttt20") : string.Format(format1, Post_store[0]));
                    strsql += " @stt_rec = '" + stt_rec + "'";
                    strsql += " @tablename = 'cttt20'";
                    int result = (int)myDb.GetValue(strsql);
                    
                    if (result > 0)
                    {
                        string format2 = "exec [dbo].{0} @stt_rec = '{1}', @tablename ='{2}'";
                        string chkVa_str = (Check_Valid_Store == null || Check_Valid_Store.Length <= 1) ? string.Format(format2, (object)"CheckEditVoucher", stt_rec, "cttt20") : string.Format(format2, (object)Check_Valid_Store[1], stt_rec, "cttt20");                        
                        num = (int)myDb.GetValue(chkVa_str);
                    }
                    else
                    {
                        string format2 = "exec [dbo].{0} @stt_rec = '{1}', @tablename ='{2}'";
                        string chkVa_str = (Check_Valid_Store == null || Check_Valid_Store.Length <= 2) ? string.Format(format2, (object)"CheckEditVouchertt", stt_rec, "cttt20") : string.Format(format2, (object)Check_Valid_Store[2], stt_rec, "cttt20");
                        num = (int)myDb.GetValue(chkVa_str);
                    }

                    if (num == 0)
                    {
                        string format2 = "exec [dbo].{0} @stt_rec = '{1}'";
                        string chkVa_str = (Post_store == null || Post_store.Length <= 1) ? string.Format(format2, (object)"InPostCttt30", stt_rec) : string.Format(format2, (object)Post_store[1], stt_rec);                        
                        if ((int)myDb.GetValue(chkVa_str) > 0)
                        {
                            string format3 = "exec [dbo].{0} @stt_rec = '{1}', @tablename ='{2}'";
                            string chk_str = (Check_Valid_Store == null || Check_Valid_Store.Length <= 1) ? string.Format(format3, (object)"CheckEditVoucher", stt_rec, "cttt30") : string.Format(format3, (object)Check_Valid_Store[1], stt_rec, "cttt30");                           
                            num = (int)myDb.GetValue(chk_str);
                        }
                        else
                        {
                            string format3 = "exec [dbo].{0} @stt_rec = '{1}', @tablename ='{2}'";
                            string chk_str = (Check_Valid_Store == null || Check_Valid_Store.Length <= 2) ? string.Format(format3, (object)"CheckEditVouchertt", stt_rec, "cttt30") : string.Format(format3, (object)Check_Valid_Store[2], stt_rec, "cttt30");
                            num = (int)myDb.GetValue(chk_str);
                        }
                    }
                }
                else
                {
                    if (str1.Contains(upper))
                    {
                        bool flag = true;
                        if (upper.Equals("HD6") || upper.Equals("PN6"))
                        {
                            if (drview.Row.Table.Columns.Contains("ma_gd") && drview["ma_gd"].ToString().Equals("2"))
                                flag = false;
                        }
                        else if (upper.Equals("PC1") || upper.Equals("BN1"))
                        {
                            if (drview.Row.Table.Columns.Contains("ma_gd") && !drview["ma_gd"].ToString().Equals("4"))
                                flag = false;
                        }
                        else if ((upper.Equals("PT1") || upper.Equals("BC1")) && (drview.Row.Table.Columns.Contains("ma_gd") && !drview["ma_gd"].ToString().Equals("4")))
                            flag = false;
                        if (flag)
                        {
                            string format = "exec [dbo].{0} @stt_rec = '{1}'";
                            string chvl_str = (Check_Valid_Store == null || Check_Valid_Store.Length <= 1) ? string.Format(format, (object)"CheckEditVoucher", stt_rec) : string.Format(format, (object)Check_Valid_Store[1], stt_rec);
                            num = (int)myDb.GetValue(chvl_str);
                        }
                    }
                    if (str2.Contains(upper) && num == 0)
                    {
                        bool flag = true;
                        if (upper.Equals("PC1") || upper.Equals("BN1"))
                        {
                            if (drview.Row.Table.Columns.Contains("ma_gd") && (drview["ma_gd"].ToString().Equals("1") || drview["ma_gd"].ToString().Equals("4")))
                                flag = false;
                        }
                        else if ((upper.Equals("PT1") || upper.Equals("BC1")) && drview.Row.Table.Columns.Contains("ma_gd") && (drview["ma_gd"].ToString().Equals("1") || drview["ma_gd"].ToString().Equals("4")))
                            flag = false;
                        if (flag)
                        {
                            string format = "exec [dbo].{0} @stt_rec = '{1}'";
                            string chvl_str = (Check_Valid_Store == null || Check_Valid_Store.Length <= 2) ? string.Format(format, (object)"CheckEditVouchertt", stt_rec) : string.Format(format, (object)Check_Valid_Store[2], stt_rec);
                            num = (int)myDb.GetValue(chvl_str);
                        }
                    }
                }
            }
            return num;
        }
        public async void Dispose()
        {
            if (_dotNetHelper is not null)
            {
                try
                {
                    await JavacriptS.InvokeVoidAsync("blazorKeyListener.remove", _dotNetHelper);
                    _dotNetHelper.Dispose();
                }
                catch { }
            }
            myStateMN.OnActionTaskChanged -= OnActionTaskchanged;
            myStateMN.OnLangguaChanged -= OnLangguachange;
        }
    }
}
