using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using DevExpress.Blazor;
using DevExpress.Blazor.Reporting;
using DevExpress.XtraReports.UI;
using SisData.Data;
using SisData.Model;
using SisData.Service;
using SisData.Dataaccess;
using SisLib.Ctrl;
using DevExpress.DataAccess.ConnectionParameters;
using System.Reflection.Metadata;
using Microsoft.Extensions.Localization;
using Blazored.LocalStorage;
using System.Data;
using Microsoft.JSInterop;
using SisCom.Shared;
using static DevExpress.Drawing.Printing.Internal.DXPageSizeInfo;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections;
using DevExpress.CodeParser;
using System.Runtime.InteropServices.JavaScript;
using System.Drawing.Imaging;

namespace SisLib.Repo
{
    public class RepoBaseComponent : ComponentBase, IDisposable
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
        [Inject] public IDxWindowService Modal { get; set; }
        [Inject] public XmlStringLocalizer Lap { get; set; }
        [Parameter] public string Menu_id { get; set; }
        //[typeparam] public TItem { get; set; }
        public string Nh_menu { get; set; }
        public ActionTask curActionTask { get; set; } = ActionTask.None;
        public FrmBrowseComponent oBrowse;
        public string Dynamic_proc = "";
        public string Store_proc = "";
        public string Fbrowse = "";
        public static string LanguageID = "";
        public bool ShowLoadding { get; set; } = false;
        public bool PopupVisible { get; set; } = false;
        public bool ShowPrint { get; set; } = false;
        public DxPopup popFilter { get; set; }
        public DxPopup popPrint { get; set; }

        public DateTime Ngay_ct1 = DateTime.Now.Date;
        public DateTime Ngay_ct2 = DateTime.Now.Date;
        public Command curCommand = new Command();
        public string Ma_dvcs;
        public Users CurUser;
        public bool IsLangEn = false;
        public string UserLang { get; set; } = "";
        public DataTable TblData { get; set; }
        public object rptData { get; set; }       
        public object CurItem { get; set; }
        public int pageSize = 20;
        public virtual void SelectedItem(object obj)
        {
            CurItem = obj;
        }
        protected override void OnInitialized()
        {
            if (String.IsNullOrEmpty(Menu_id))
                Menu_id = "we.hr.3R";
            else
            {
                Menu_id = Menu_id.Replace("_", ".");
            }
            CurUser = myStateMN.GetCurUser();
            Ma_dvcs = myStateMN.GetMa_Dvcs();
            Ma_dvcs = (String.IsNullOrEmpty(Ma_dvcs) || Ma_dvcs.Trim() == "ALL") ? "" : Ma_dvcs.Trim();
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
                string Menuid = Menu_id.Replace("_", ".");
                if (CurUser.Is_admin == 1 || CurUser.CheckRight(Menuid, "VIEW"))
                {
                    ShowToolBar();
                }
                else
                {
                    myStateMN.SetToolBar(new MyToolbar("toolbar", false));
                }

                await Loaddata();
                await Language();

                myStateMN.OnActionTaskChanged += OnActionTaskchanged;
                myStateMN.OnLangguaChanged += OnLangguachange;
                await Lap.LoadLocalizationAsync(String.Format("{0}-{1}", String.IsNullOrEmpty(LanguageID) ? curCommand.Ma_phan_he.Trim().ToLower() : LanguageID, UserLang.Trim()));
            }
        }
        public virtual async Task ShowToolBar()
        { 
            myStateMN.SetToolBar(new MyToolbar("toolbar", true));
            myStateMN.SetToolBar(new MyToolbar("Search", true));
            myStateMN.SetToolBar(new MyToolbar("Print", true));
            myStateMN.SetToolBar(new MyToolbar("Export", true));
        }
        public virtual async Task PageLoad()
        {
            curCommand = await myCommand.GetSubMenuBymenuId(Menu_id.Replace("_", "."));
            if (curCommand != null)
            {
                Nh_menu = String.IsNullOrEmpty(curCommand.Nh_menu) ? "" : curCommand.Nh_menu.Trim();
                Store_proc = String.IsNullOrEmpty(curCommand.Store_proc) ? "" : curCommand.Store_proc.Trim();
                Fbrowse = IsLangEn ? curCommand.Ebrowse1 : curCommand.Vbrowse1;
            }
            StateHasChanged();
        }
        public virtual async Task Loaddata()
        {

        }
        public virtual void OnActionTaskchanged(ActionTask ac)
        {
            switch (ac)
            {
                case ActionTask.Search:
                    PopupVisible = true;
                    ShowLoadding = false;
                    myStateMN.SetCurrentActionTask(ActionTask.Search);
                    StateHasChanged();
                    break;
                case ActionTask.Print:
                    Print();
                    StateHasChanged();
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
        public void ShowReport(IList LSItems, Type Itype)
        {
            Dictionary<string, object> Par = new Dictionary<string, object>();
            Par.Add("LSItems", (object)LSItems);
            Par.Add("Menu_id", (object)Menu_id);
            var aty = typeof(ReportOptionComponent<>).MakeGenericType(new[] { Itype });            
            DxWindowModel model = new DxWindowModel(aty, Par, true, "BC");
            model.nWitdh = "80vw";
            Par.Add("DxWindowmodel", model);
            Modal.Show(model);
        }
        public virtual void Print()
        {
            ShowPrint = true;
        }
        public virtual async void ExportData(int type)
        {
            oBrowse.ExPortGrid(type);
            string ToastMessage = Lap["Export successfully"].ToString();
            myStateMN.SetNewThongbao(ToastMessage);
            StateHasChanged();
        }
        public void Dispose()
        {
            myStateMN.OnActionTaskChanged -= OnActionTaskchanged;
            myStateMN.OnLangguaChanged -= OnLangguachange;
        }
    }
}
