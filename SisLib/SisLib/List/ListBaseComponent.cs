using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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
using System.Security.Cryptography.Xml;
using Microsoft.JSInterop;
using SisCom.Shared;
using System.Security.Claims;
using static DevExpress.Drawing.Printing.Internal.DXPageSizeInfo;
using Microsoft.AspNetCore.Components.Authorization;
using System.Globalization;
using System.ServiceModel.Channels;
using Microsoft.Extensions.Options;
using SisLib.Lib;

namespace SisLib.List
{
    public class ListBaseComponent : ComponentBase, IDisposable
    {
        [Inject] public IOptionsManager myOption { get; set; }
        [Inject] public IDxWindowService myModal { get; set; }
        [Inject] public IDbManager myDb { get; set; }
        [Inject] public ISqldataacceess mySqldb { get; set; }
        [Inject] public ILocalStorageService myLcStore { get; set; }
        [Inject] public Statemanagerment myStateMN { get; set; }
        [Inject] public NavigationManager myNavi { get; set; }
        [Inject] public ICommandManager myCommand { get; set; }
        [Inject] public IJSRuntime JavacriptS { get; set; }
        [Inject] public IDialogService myDlSvc { get; set; }        
        [Inject] public XmlStringLocalizer Lap { get; set; }
        [Parameter] public string Menu_id { get; set; }
        public string Nh_menu { get; set; }        
        public Type FrmType { get; set; }
        public bool IsShowFrm { get; set; } = false;
        public bool IsShowCopy { get; set; } = false;
        public bool IsShowImport { get; set; } = false;
        public Dictionary<string, object> Par { get; set; } = new Dictionary<string, object>();
        public bool Is_doi_ma { get; set; } = false;
        public ActionTask curActionTask { get; set; } = ActionTask.None;
        public FrmBrowseComponent oBrowse;        
        public string fbrowse = "";
        public int Page_size = 20;
        public DxPopup OptwRef;
        public bool IDVisible = false;

        public string mPage = "";
        public string editPage = "";
        public Command curCommand = new Command();
        public Dmdm CurDmdm = new Dmdm();
        public Users curUser;
        public bool IsLangEn = false;
        public string UserLang { get; set; } = "";

        public bool? ResultDl { get; set; } = null;
        public string MessageDl { get; set; } = "";
        public string Ten_item { get; set; }
        public DataTable TblData { get; set; }
        public object SelectedItem { get; set; }               

        public static string sqlTableName = "";//dmkh
        public static string sqlTableView = "";//v_dmkh
        public static string SqlTableKey = "";//ma_kh
        public static string SqlItemName = "";//ten_kh
        public static string SqlTableKeyValue = "";
        public static string LanguageID = "";
        public DotNetObjectReference<ListBaseComponent>? _dotNetHelper;
        public virtual void CurItem(object obj)
        {
            SelectedItem = obj;
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
            SelectedItem = (object)null;
            base.OnInitialized();
            
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {                
                await Language();
                await PageLoad();
                await Lap.LoadLocalizationAsync(String.Format("{0}-{1}",String.IsNullOrEmpty(LanguageID)?curCommand.Ma_phan_he.Trim().ToLower(): LanguageID, UserLang.Trim()));
                string Menuid = Menu_id.Replace("_", ".");
                if (curUser.Is_admin == 1 || curUser.CheckRight(Menuid, "VIEW"))
                {
                    myStateMN.SetToolBar(new MyToolbar("toolbar", true));
                    myStateMN.SetToolBar(new MyToolbar("New", true,"F4"));
                    if(IsShowCopy) myStateMN.SetToolBar(new MyToolbar("Copy", true));
                    if (IsShowImport) myStateMN.SetToolBar(new MyToolbar("Import", true));
                    myStateMN.SetToolBar(new MyToolbar("Edit", true,"F3"));
                    myStateMN.SetToolBar(new MyToolbar("Save", false));
                    myStateMN.SetToolBar(new MyToolbar("Delete", true,"F8"));
                    myStateMN.SetToolBar(new MyToolbar("Change", Is_doi_ma));
                    myStateMN.SetToolBar(new MyToolbar("View", true,"F2"));
                    myStateMN.SetToolBar(new MyToolbar("Aproval", false));
                    myStateMN.SetToolBar(new MyToolbar("Print", false));
                    myStateMN.SetToolBar(new MyToolbar("Export", true));
                    myStateMN.SetToolBar(new MyToolbar("Search", false));
                    myStateMN.SetToolBar(new MyToolbar("Quit", false));
                }
                else
                {
                    myStateMN.SetToolBar(new MyToolbar("toolbar", false));
                }
                myStateMN.OnActionTaskChanged += OnActionTaskchanged;
                myStateMN.OnLangguaChanged += OnLangguachange;
                await Loaddata();
                await Language();
                _dotNetHelper = DotNetObjectReference.Create(this);                
            }
        }
        public virtual async Task PageLoad()
        {
            curCommand = await myCommand.GetSubMenuBymenuId(Menu_id.Replace("_", "."));
            if (curCommand != null)
            {
                mPage = curCommand.Mobile_page;
                Nh_menu = String.IsNullOrEmpty(curCommand.Nh_menu)?"":curCommand.Nh_menu.Trim();
                string sql = "select * from dmdm where ma_dm = '" + curCommand.Store_proc + "'";
                DataSet ds = await myDb.LoadDataset(sql);
                List<Dmdm> lsdm = myDb.ConvertDataTable<Dmdm>(ds.Tables[0]);
                if (lsdm != null && lsdm.Count > 0)
                {
                    CurDmdm = lsdm[0];
                    if (CurDmdm != null)
                    {
                        Is_doi_ma = (!String.IsNullOrEmpty(ds.Tables[0].Rows[0]["doi_ma"].ToString()) && !String.IsNullOrEmpty(ds.Tables[0].Rows[0]["doi_ma_table"].ToString())) ? true : false;
                        fbrowse = IsLangEn ? CurDmdm.Full_field2 : CurDmdm.Full_field;
                        Page_size = CurDmdm.Nrow > 0 ? CurDmdm.Nrow : int.Parse(myOption.GetOptionsValue("M_GRID_NROW"));
                        editPage = String.IsNullOrEmpty(CurDmdm.Form_edit)?editPage: CurDmdm.Form_edit.Split(";")[0].Trim();
                        sqlTableName = CurDmdm.Table_name.Trim();
                        sqlTableView = CurDmdm.table_view.Trim();
                        SqlTableKey = CurDmdm.Key.Trim();                        
                        if(CurDmdm.Field_search.Split(";").Length > 1)
                        {
                            SqlItemName = CurDmdm.Field_search.Split(";")[1];                            
                        }
                    }
                }
            }
            StateHasChanged();
        }
        public virtual async Task Loaddata()
        {
            string sql = "Select * from " + sqlTableView + (String.IsNullOrEmpty(CurDmdm.Order)?(" order by " + SqlTableKey) :(" order by " + CurDmdm.Order.Trim().Replace(";",",")));
            DataSet dsList = await myDb.LoadDataset(sql);
            TblData = dsList.Tables[0];
            //string strph = SisFunc.Converts.DataTable2Model(TblData, "Dmvt");
            if (TblData.DefaultView.Count > 0)
                SelectedItem = TblData.DefaultView[0];
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
                case ActionTask.Delete:
                    V_Xoa();
                    break;
                case ActionTask.Change:
                    ChangeID();
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
                string strkey = SqlTableKey.Split(";").LastOrDefault();
                if (!rview.DataView.Table.Columns.Contains(strkey))
                    return;
                if (!String.IsNullOrEmpty(rview[strkey].ToString()))
                {
                    if (editPage.ToUpper().StartsWith("FRM"))
                        myNavi.NavigateTo(editPage + "/" + Menu_id + "/" + rview[strkey].ToString());
                    else
                    {
                        Par.Clear();
                        Par.Add("Ma_ma", (object)rview[strkey].ToString());
                        Par.Add("Menu_id", (object)Menu_id);
                        Par.Add("ShowAdd", (object)true);
                        Par.Add("curActionTask", (object)ActionTask.New);
                        Par.Add("OnClose", EventCallback.Factory.Create(this, CloseForm));
                        IsShowFrm = true;
                        StateHasChanged();
                    }
                }
            }            
        }
        public virtual void V_Copy()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {
                string strkey = SqlTableKey.Split(";").LastOrDefault();
                if (!rview.DataView.Table.Columns.Contains(strkey))
                    return;
                if (!String.IsNullOrEmpty(rview[strkey].ToString()))
                {
                    if (editPage.ToUpper().StartsWith("FRM"))
                        myNavi.NavigateTo(editPage + "/" + Menu_id + "/" + rview[strkey].ToString());
                    else
                    {
                        Par.Clear();
                        Par.Add("Ma_ma", (object)rview[strkey].ToString());
                        Par.Add("Menu_id", (object)Menu_id);
                        Par.Add("ShowAdd", (object)true);
                        Par.Add("curActionTask", (object)ActionTask.Copy);
                        Par.Add("OnClose", EventCallback.Factory.Create(this, CloseForm));
                        IsShowFrm = true;
                        StateHasChanged();
                    }
                }
            }
        }
        public virtual void V_Sua()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {
                string strkey = SqlTableKey.Split(";").LastOrDefault();
                if (!rview.DataView.Table.Columns.Contains(strkey))
                    return;
                if (!String.IsNullOrEmpty(rview[strkey].ToString()))
                {
                    if (editPage.ToUpper().StartsWith("FRM"))
                        myNavi.NavigateTo(editPage + "/" + Menu_id + "/" + rview[strkey].ToString());
                    else
                    {
                        Par.Clear();                        
                        Par.Add("Ma_ma", (object)rview[strkey].ToString());
                        Par.Add("Menu_id", (object)Menu_id);
                        Par.Add("ShowAdd", (object)true);
                        Par.Add("curActionTask", (object)ActionTask.Edit);
                        Par.Add("OnClose", EventCallback.Factory.Create(this, CloseForm));
                        IsShowFrm = true;
                        StateHasChanged();
                    }    
                }
            }
            else
                myStateMN.SetNewThongbao(Lap["No data selected"]);
        }
        public virtual void V_Xem()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {
                string strkey = SqlTableKey.Split(";").LastOrDefault();
                if (!rview.DataView.Table.Columns.Contains(strkey))
                    return;
                if (!String.IsNullOrEmpty(rview[strkey].ToString()))
                {
                    if (editPage.ToUpper().StartsWith("FRM"))
                        myNavi.NavigateTo(editPage + "/" + Menu_id + "/" + rview[strkey].ToString());
                    else
                    {
                        Par.Clear();
                        Par.Add("Ma_ma", (object)rview[strkey].ToString());
                        Par.Add("Menu_id", (object)Menu_id);
                        Par.Add("ShowAdd", (object)true);
                        Par.Add("curActionTask", (object)ActionTask.View);
                        Par.Add("OnClose", EventCallback.Factory.Create(this, CloseForm));
                        IsShowFrm = true;
                        StateHasChanged();
                    }
                }
            }
            else
                myStateMN.SetNewThongbao(Lap["No data selected"]);
        }
        public virtual async void V_Xoa()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {
                if (!rview.DataView.Table.Columns.Contains(SqlTableKey.Split(";").LastOrDefault()))
                    return;
                if (!string.IsNullOrEmpty(rview[SqlTableKey.Split(";").LastOrDefault()].ToString()))
                    SqlTableKeyValue = rview[SqlTableKey.Split(";").LastOrDefault()].ToString().Trim();

                MessageDl = String.Format(Lap["Do you want to delete :{0} ?"], SqlTableKeyValue);               
                ResultDl = await myDlSvc.ConfirmAsync(new MessageBoxOptions()
                {
                    Text = MessageDl,
                    Title = Lap["Confirm deletion of list"],
                    OkButtonText = Lap["OK"],
                    CancelButtonText = Lap["Cancel"],
                    ShowIcon = true,
                    ShowCloseButton = false,
                    RenderStyle = MessageBoxRenderStyle.Danger,
                });
                if (ResultDl == true)
                {
                    Dongyxoa(true);
                    StateHasChanged();
                }                
            }
            else
                myStateMN.SetNewThongbao(Lap["No data selected"]);
        }
        public virtual async Task Dongyxoa(bool status)
        {
            if (status)
            {
                string sql = "Delete " + sqlTableName + " where " + SqlTableKey.Split(";").LastOrDefault() + "='" + SqlTableKeyValue + "'";
                await myDb.ExecuteSqlQueryAsync(sql);
                await Loaddata();
                string ToastMessage = String.Format(Lap["Delete successfully : {0}"], SqlTableKeyValue);
                myStateMN.SetNewThongbao(ToastMessage);                
            }
            MessageDl = "";
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
        public virtual void ChangeID()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {
                if (!string.IsNullOrEmpty(rview[SqlTableKey.Split(";").LastOrDefault()].ToString()))
                    SqlTableKeyValue = rview[SqlTableKey.Split(";").LastOrDefault()].ToString().Trim();
                if(!String.IsNullOrEmpty(SqlItemName) && rview.Row.Table.Columns.Contains(SqlItemName))
                    Ten_item = IsLangEn ? rview[SqlItemName.Trim() + "2"].ToString().Trim() : rview[SqlItemName].ToString().Trim();

                IDVisible = true;
                StateHasChanged();
            }
        }
        public virtual async void ChangeOK(string newID)
        {
            await Loaddata();
            StateHasChanged();
        }
        public virtual void HideVoucher()
        {
           
        }
        public virtual async void ExportData(int type)
        {
            oBrowse.ExPortGrid(type);
            string ToastMessage = Lap["Export successfully"].ToString();
            myStateMN.SetNewThongbao(ToastMessage);
            StateHasChanged();
        }
        public async void OnRDBClick(GridRowClickEventArgs e)
        {
            V_Xem();
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
            if (keyCode == 117)
            {
                ChangeID();
            }
            if (keyCode == 119)
            {
                myStateMN.SetCurrentActionTask(ActionTask.Delete);
                V_Xoa();
            }
        }
        public virtual async void CloseForm()
        {
            await Loaddata();
            IsShowFrm = false;
            StateHasChanged();
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            myStateMN.OnActionTaskChanged -= OnActionTaskchanged;
            myStateMN.OnLangguaChanged -= OnLangguachange;
        }
    }
}
