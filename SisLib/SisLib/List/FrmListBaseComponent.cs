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
using SisFunc;

namespace SisLib.List
{
    public class FrmListBaseComponent : ComponentBase, IDisposable
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
        [Inject] public XmlStringLocalizer Lap { get; set; }
        [Parameter] public string Ma_ma { get; set; }
        [Parameter] public string Menu_id { get; set; }
        [Parameter] public ActionTask curActionTask { get; set; } = ActionTask.None;
        [Parameter] public DxWindowModel DxWindowmodel { get; set; }
        [Parameter] public bool? ShowAdd { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        public SizeMode Itemsizemode { get; set; } = SizeMode.Medium;
        public bool AllowEditMode;
        public bool AllowEditMa;
        public Command curCommand = new Command();
        public Dmdm CurDmdm = new Dmdm();
        public Users curUser;
        public bool IsLangEn = false;
        public string UserLang = "";
        public Dictionary<string, int> tblLength = new Dictionary<string, int>();
        public string ParentPage;
        public string sqlTableName = "dmkh";
        public string sqlTableView = "v_dmkh";
        public string SqlTableKey = "ma_kh";
        public string CheckSqlTableKey = "ma_kh";
        public string CheckSqlTableKeyValue = "";
        public string Transform = "";
        public string Title = "";
        public string SqlTableKeyValue = "";
        public string SqlTableKeyValueOld = "";
        public static string LanguageID = "";
        public List<Dmtk> LSStatus { get; set; }
        protected override void OnInitialized()
        {
            string VcSize = myOption.GetOptionsValue("M_SIZE_MODE").Trim();
            if (VcSize != null)
            {
                Itemsizemode = VcSize == "1" ? SizeMode.Small : VcSize == "3" ? SizeMode.Large : SizeMode.Medium;
            }
            LSStatus = new List<Dmtk>() {
                new Dmtk() {Ma= "1",Ten= Lap["Using"] },
                new Dmtk() {Ma= "0",Ten= Lap["Stopping"] },};
            if (curActionTask == ActionTask.None)
                curActionTask = myStateMN.GetCurrentActionTask();

            curUser = myStateMN.GetCurUser();
            tblLength = myDb.GetTableLength(sqlTableName);

            base.OnInitialized();
            myStateMN.OnLangguaChanged += OnLangguachange;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Language();
                await Lap.LoadLocalizationAsync(String.Format("{0}-{1}", String.IsNullOrEmpty(LanguageID) ?curCommand.Ma_phan_he.Trim().ToLower(): LanguageID, UserLang.Trim()));
            }
        }
        public virtual async Task PageLoad()
        {
            curCommand = await myCommand.GetSubMenuBymenuId(Menu_id.Replace("_","."));
            if (curCommand != null)
            {
                ParentPage = curCommand.Mobile_page.Trim() + @"/" + curCommand.Menu_id.Replace(".","_");
                string sql = "select * from dmdm where ma_dm = '" + curCommand.Store_proc + "'";
                DataSet ds = myDb.LoadDataSet(sql);
                List<Dmdm> lsdm = myDb.ConvertDataTable<Dmdm>(ds.Tables[0]);
                if (lsdm != null && lsdm.Count > 0)
                {
                    CurDmdm = lsdm[0];
                    if (CurDmdm != null)
                    {
                        sqlTableName = CurDmdm.Table_name.Trim();
                        sqlTableView = CurDmdm.table_view.Trim();
                        CheckSqlTableKey = CurDmdm.Key.Trim();
                        SqlTableKey = CurDmdm.Key.Trim().Split(";").LastOrDefault();
                        if (!string.IsNullOrEmpty(CurDmdm.Transform))
                            Transform = CurDmdm.Transform;
                        Title = IsLangEn ? CurDmdm.Title2 : CurDmdm.Title;
                        if (!string.IsNullOrEmpty(Title))
                            Title = Title.Split(".")[0];
                    }
                }                
            }
        }
        public string GetNewSttDM()
        {
            if (myOption.GetOptionsValue("M_AUTO_LIST_NUM").Trim().Equals("1") && CurDmdm.increase_type == 2)
            {
                string str = myDb.IncreaseCode(Ma_ma, SqlTableKey, sqlTableName);
                if (!string.IsNullOrEmpty(str) && str.Length <= tblLength[SqlTableKey])
                    return str;
                else
                {
                    return myDb.GetNewMadm(sqlTableName);
                }
            }
            return "";
        }
        public async void OnLangguachange(MyLang lang)
        {
            if (lang.IsnewLang)
            {
                await Language();
            }
        }
        public async Task Language()
        {
            try
            {
                IsLangEn = false;
                UserLang = await myLcStore.GetItemAsync<string>("userlang");
                if (!string.IsNullOrEmpty(UserLang))
                {
                    if (!UserLang.Equals("Vi"))
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
        public virtual bool CheckValidCode()
        {
            string New_id = SqlTableKeyValue.Trim();
            string Old_id = SqlTableKeyValueOld.Trim();
            if (New_id == Old_id)
            {
                return true;
            }
            if (String.IsNullOrEmpty(New_id))
            {
                myStateMN.SetNewThongbao(Lap["Empty code"] + "!");
                return false;
            }
            string str = CtlFunc.CheckInValidCode(myStateMN, New_id.Trim());
            if (str != "")
            {
                string _mes = Lap["The code cannot contain characters"] + String.Format(" [{0}]", str) + "!";
                myStateMN.SetNewThongbao(_mes);
                return false;
            }

            DataSet _ds = myDb.LoadDataSet("exec dbo.CheckExistListId @ma_dm, @value", "ma_dm;value", curCommand.Store_proc.Trim() + ";" + New_id);

            if (_ds.Tables[0].Rows.Count > 0)
            {
                if (_ds.Tables[0].Rows[0][0].ToString().Trim() == "1")
                {
                    string _mes = Lap["New code is the same as the code already in the list"] + " !";
                    myStateMN.SetNewThongbao(_mes);
                    return false;
                }
            }

            DataSet _ds1 = myDb.LoadDataSet("exec dbo.CheckLong @ma_dm, @newvalue, @oldvalue", "ma_dm;newvalue;oldvalue", curCommand.Store_proc.Trim() + ";" + New_id + ";" + Ma_ma);
            if (_ds1.Tables[0].Rows.Count != 0)
            {
                myStateMN.SetNewThongbao(Lap["Nested code"] + " !");
                return false;
            }
            return true;
        }
        public void HideVoucher()
        {
            if (DxWindowmodel != null)
            {
                myModal.CloseModal(DxWindowmodel);
            }

            if (ShowAdd == true)
            {
                OnClose.InvokeAsync();
            }
            else
                myNavi.NavigateTo(ParentPage);
        }
        public void Dispose()
        {
            myStateMN.OnLangguaChanged -= OnLangguachange;
        }
    }
}
