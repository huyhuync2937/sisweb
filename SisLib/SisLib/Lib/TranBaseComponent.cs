using System;
using System.Data;
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
using System.Reflection.Metadata;
using Microsoft.Extensions.Localization;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using SisCom.Shared;
using SisLib.Ctrl;
using DevExpress.XtraRichEdit.Import.Doc;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices.JavaScript;
using System.Globalization;
using System.Reactive.Joins;
using Microsoft.SqlServer.Server;
using Microsoft.AspNetCore.Hosting;
using DevExpress.CodeParser;

namespace SisLib.Lib
{
    public class TranBaseComponent : ComponentBase, IDisposable
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
        [Inject] public IWebHostEnvironment myWebhost { get; set; }
        [Inject] public IJSRuntime JavacriptS { get; set; }
        [Inject] IDialogService myDlSvc { get; set; }
        [Inject] public XmlStringLocalizer Lap { get; set; }
        [Parameter] public string Menu_id { get; set; }
        [Parameter] public string Stt_rec { get; set; }
        [Parameter] public DxWindowModel DxWindowmodel { get; set; }
        [Parameter] public string Ma_ct { get; set; }
        [Parameter] public ActionTask curActionTask { get; set; }
        [CascadingParameter(Name = "Ma_qs")] public string Ma_qs { get; set; }
        [CascadingParameter(Name = "Ma_dvcs")] public string Ma_dvcs { get; set; }
        [Parameter] public EventCallback<bool> Closeform { get; set; }
        public bool? ResultDl { get; set; } = null;
        public string MessageDl { get; set; } = "";        

        public string Ws_Id = string.Empty;
        public string curStt_Rec = string.Empty;
        public int Max_Col = 20;

        public string Nh_menu;
        public Command curCommand = new Command();
        public Dmct curDmct = new Dmct();
        public Dmqs curQuyenso;
        public List<Dmtk> LSSOCT = new List<Dmtk>();
        public DataTable tbStatus = (DataTable)null;
       
        public DataSet DsTrans = (DataSet)null;
        public List<FreeCodeInfo> LsFreeCodeInfo = new List<FreeCodeInfo>();
        public List<FreeCodeInfo> LsFreeCodeInfo2 = new List<FreeCodeInfo>();
        public BiDirectionalDictionary<int, string> CtOrder = new BiDirectionalDictionary<int, string>(); //Sắp xếp cột cho trường tự do trên Tab chi tiết
        public BiDirectionalDictionary<int, string> CtgtOrder = new BiDirectionalDictionary<int, string>();//Sắp xếp cột cho trường tự do trên Tab thuế
        public bool PopupVisible { get; set; } = true;
        public bool IsShowVoucher { get; set; } = true;
        public IGrid GridCT { get; set; }
        public IGrid GridCTGT { get; set; }

        public string grd_Css = "grd-detail ";
        private bool _allowEditMode;
        public bool AllowEditMode {
            get => _allowEditMode;
            set
            {                
                _allowEditMode = value;
                grd_Css = _allowEditMode ? "grd-detail " : "grd-detail disabled-grid";
            }
        }
       
        public string M_Phdbf = string.Empty;
        public string M_Ctdbf = string.Empty;
        public string M_Ctgtdbf = string.Empty;
        

        public DataTable TblPH = (DataTable)null;
        public DataTable TblCT = (DataTable)null;
        public DataTable TblGT = (DataTable)null;

        public DxPopup OptwRef; 
        public Users curUser;
        public bool IsLangEn = false;
        public string UserLang = "";

        public bool NotMaNt0Voucher { get; set; }
        public string M_ma_nt0 = string.Empty;        
        public string M_MST_CHECK = string.Empty;
        public int M_User_Id = 0;
        public string M_ngay_lct = string.Empty;
        public string M_ong_ba = string.Empty;
        public int M_sl_ct0 = 0;
        public int M_IN_HOI_CK = 0;
        public int M_CHK_HD_VAO = 0;
        public string hd_thue = "1";
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
        public object SelectedDataItemCT { get; set; }
        public object SelectedDataItemCTGT { get; set; }
        public int ActiveTabIndex { get; set; } = 1;
        public SizeMode? Itemsizemode { get; set; } = SizeMode.Medium;
        public GridEditNewRowPosition NewItemRowPosition => GridEditNewRowPosition.FixedOnTop;        
        public Dictionary<string, int> tblLength = new Dictionary<string, int>();

        public int iRow = 0;
        public int pageSize = 30;
        public string Pagetitle = "";
        public string Message = "";
        public bool IsPageSize = true;
        public IList LSData;
        #endregion
        public virtual void CurItem(object obj)
        {
            if (obj != null)
            {
                SelectedItem = obj;
                DataRowView rview = (DataRowView)SelectedItem;
                
                iRow = TblPH.DefaultView.Cast<DataRowView>()
                      .ToList()
                      .FindIndex(row => row == rview);                
            }           
        }
        protected async override Task OnInitializedAsync()
        {
            if (String.IsNullOrEmpty(Menu_id))
                Menu_id = "we.sm.23";
            else
            {
                Menu_id = Menu_id.Replace("_", ".");
            }
            string VcSize = myOption.GetOptionsValue("M_SIZE_MODE").Trim();
            if (VcSize != null)
            {
                Itemsizemode = VcSize == "1" ? SizeMode.Small : VcSize == "3" ? SizeMode.Large : SizeMode.Medium;
            }
            curUser = myStateMN.GetCurUser();
            
            if (String.IsNullOrEmpty(Menu_id))
            {
                string sqlcmd = "is_mobile = 2 AND ma_ct = '" + Ma_ct + "'";
                Menu_id = myDb.GetValue("command", "min(menu_id)", sqlcmd).ToString().Trim();                
            }
            if (String.IsNullOrEmpty(Menu_id))
            {
                myStateMN.SetNewThongbao(Lap["No Command"] + " !");
                return;
            }

            curCommand = await myCommand.GetSubMenuBymenuId(Menu_id.Replace("_", "."));
            if (curCommand != null)
            {
                Nh_menu = String.IsNullOrEmpty(curCommand.Nh_menu) ? "" : curCommand.Nh_menu.Trim();
                Ma_ct = String.IsNullOrEmpty(Ma_ct) ?curCommand.Ma_ct.Trim(): Ma_ct;

                string strsql = "Select * from dmct where ma_ct = '" + Ma_ct + "'";
                DataSet dsdmct = myDb.LoadDataSet(strsql);
                curDmct = myDb.ConvertDataRow<Dmct>(dsdmct.Tables[0].Rows[0]);            
                if (curDmct != null)
                {
                    M_Phdbf = curDmct.M_phdbf.ToString().Trim();
                    M_Ctdbf = curDmct.M_ctdbf.ToString().Trim();
                    if (string.IsNullOrEmpty(curDmct.M_ctgtdbf) || curDmct.M_ctgtdbf.Equals(""))
                    {
                        Console.WriteLine("M_ctgtdbf is null or empty for Ma_ct: " + Ma_ct);
                    }
                    M_Ctgtdbf = curDmct.M_ctgtdbf.ToString().Trim();
                    if (curDmct.Process_Store == null)
                    {
                        Console.WriteLine("Process_Store is null for Ma_ct: " + Ma_ct);
                    }
                    Process_store = curDmct.Process_Store.Split('|');
                    Post_store = curDmct.Post_store.Trim().Split('|');
                    Check_Valid_Store = curDmct.Check_valid_store.Trim().Split('|');                
                }
                else
                {
                    myStateMN.SetNewThongbao(String.Format(Lap["No voucher {0}"], Ma_ct) + " !");
                    return;
                }
                tblLength = myDb.GetTableLength(M_Phdbf);
                string test = String.Format("select ma_ct,ma_dm,order2 as [order],width,carry,field_ct,post,status2 as [status],enabledstatus,loai_dmctct,caption2 as caption from dbo.s_freecodefieldoftrans where isnull(field_ct,'') <> '' AND loai_dmctct <> 0 AND status2 =1 AND ma_ct = '{0}' order by [order2]", Ma_ct);
                DataTable a =  myDb.GetTable(String.Format("select ma_ct,ma_dm,[order],width,carry,field_ct,post,[status],enabledstatus,loai_dmctct,caption from dbo.s_freecodefieldoftrans where isnull(field_ct,'') <> '' AND loai_dmctct <> 0 AND status =1 AND ma_ct = '{0}' order by [order]", Ma_ct));
                LsFreeCodeInfo = myDb.ConvertDataTable<FreeCodeInfo>(myDb.GetTable(String.Format("select ma_ct,ma_dm,[order],width,carry,field_ct,post,[status],enabledstatus,loai_dmctct,caption from dbo.s_freecodefieldoftrans where isnull(field_ct,'') <> '' AND loai_dmctct <> 0 AND status =1 AND ma_ct = '{0}' order by [order]", Ma_ct)));
                LsFreeCodeInfo2 = myDb.ConvertDataTable<FreeCodeInfo>(myDb.GetTable(String.Format("select ma_ct,ma_dm,order2 as [order],width,carry,field_ct,post,status2 as [status],enabledstatus,loai_dmctct,caption2 as caption from dbo.s_freecodefieldoftrans where isnull(field_ct,'') <> '' AND loai_dmctct <> 0 AND status2 =1 AND ma_ct = '{0}' order by [order2]", Ma_ct)));
                LsFreeCodeInfo.ForEach(x => { x.Field_ct = SisFunc.Converts.FirstUpper(x.Field_ct.Trim());x.sWidth = x.Width <= 0 ? "100px" : x.Width.ToString().Trim() + "px";});
                LsFreeCodeInfo2.ForEach(x => { x.Field_ct = SisFunc.Converts.FirstUpper(x.Field_ct.Trim()); x.sWidth = x.Width <= 0 ? "100px" : x.Width.ToString().Trim() + "px"; });

                for (int i = 0; i < Max_Col; i++)
                {
                    CtOrder.Add(i, i.ToString().Trim());
                    CtgtOrder.Add(i, i.ToString().Trim());
                }
                foreach (FreeCodeInfo row in LsFreeCodeInfo)
                {
                    int iOrder = row.Order;
                  
                    while (!String.IsNullOrEmpty(CtOrder.GetValue(iOrder)))
                    {
                        iOrder++;
                    }
                    if(CtOrder.GetKey(row.Field_ct.Trim()) <=0)
                    {
                        CtOrder.Add(iOrder, row.Field_ct.Trim());
                    }

                }
                foreach (FreeCodeInfo row in LsFreeCodeInfo)
                {
                    int iOrder = row.Order;                    
                    while (!String.IsNullOrEmpty(CtgtOrder.GetValue(iOrder)))
                    {
                        iOrder++;
                    }
                    if (CtgtOrder.GetKey(row.Field_ct.Trim()) <= 0)
                    {
                        CtgtOrder.Add(iOrder, row.Field_ct.Trim());
                    }

                }

                string _ma_dvcs = myStateMN.GetMa_Dvcs();
                string phFilter ="1=1" ;
                if((curActionTask == ActionTask.Edit || curActionTask == ActionTask.View) && !String.IsNullOrEmpty(Stt_rec))
                    phFilter = "stt_rec = ''" + Stt_rec + "''";
                else
                {
                    phFilter = "1=0";
                }    
                strsql = "Exec " + curDmct.Process_Store.Trim().Split('|')[0].Trim() + " @ma_ct='" + Ma_ct + "',@sPhFilter ='" + phFilter + "',@sCtFilter='1=1',@sGtFilter='1=1'";
                DsTrans = myDb.LoadDataSet(strsql);
            }
            else
            {
                myStateMN.SetNewThongbao(Lap["No Command"] + " !");
                return;
            }
            
            base.OnInitialized();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {                
                await Language();
                await Loaddata();
                await PageLoad();                
                string Menuid = Menu_id.Replace("_", ".");

                myStateMN.OnActionTaskChanged += OnActionTaskchanged;
                myStateMN.OnLangguaChanged += OnLangguachange;
            }
            await JavacriptS.InvokeVoidAsync("changeTextNewRowButton", Lap["Click here to add a new row"].ToString().Trim());
        }
        public virtual async Task Loaddata()
        { 
        }
        public virtual async Task PageLoad()
        {
            M_MST_CHECK = myOption.GetOptionsValue("M_MST_CHECK").ToString().Trim();
            M_MA_THUE = myOption.GetOptionsValue("M_MA_THUE").ToString();
            Ws_Id = myOption.GetOptionsValue("WS_ID").ToString().Trim();
            M_ma_nt0 = myOption.GetOptionsValue("M_MA_NT0").ToString().Trim();
            var a = DsTrans.Tables;
            TblPH = DsTrans.Tables.Count > 0 ? DsTrans.Tables[0] : (DataTable)null;
            TblCT = DsTrans.Tables.Count > 1 ? DsTrans.Tables[1] : (DataTable)null;
            TblGT = DsTrans.Tables.Count > 2 ? DsTrans.Tables[2] : (DataTable)null;
            if (TblCT != null)
            {
                TblCT.DefaultView.Sort = "stt_rec0 asc";
            }
            if (TblGT != null)
            {
                TblGT.DefaultView.Sort = "stt_rec0 asc";
            }
            if (TblPH != null)
                TblPH.DefaultView.Sort = "ngay_ct desc, so_ct desc";
            StateHasChanged();
        }
        public virtual async Task LoadVoucher()
        {
            string phFilter = "stt_rec = ''" + Stt_rec + "''";
            string strsql = "Exec " + curDmct.Process_Store.Trim().Split('|')[0].Trim() + " @ma_ct='" + Ma_ct + "',@sPhFilter ='" + phFilter + "',@sCtFilter='1=1',@sGtFilter='1=1'";
            DsTrans = myDb.LoadDataSet(strsql);
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
                case ActionTask.Print:
                    myStateMN.SetCurrentActionTask(ActionTask.Print);
                    V_In();
                    break;
                case ActionTask.Delete:
                    V_Xoa();
                    break;
            }
        }
        public virtual void V_Moi()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {
               
                
            }            
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
            
        }
        public virtual void V_Xem()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {
                DataRowView newData = (SelectedItem as DataRowView);
                StateHasChanged();
            }
            else
                myStateMN.SetNewThongbao(Lap["No data selected"]);
        }        
        public virtual async void V_Xoa()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview != null)
            {                
                MessageDl = String.Format(Lap["Do you want to delete :{0} ?"], rview["so_ct"].ToString().Trim());
                ResultDl = await myDlSvc.ConfirmAsync(new MessageBoxOptions()
                {
                    Text = MessageDl, Title = Lap["Confirm deletion of voucher"],
                    OkButtonText = Lap["OK"], CancelButtonText = Lap["Cancel"],
                    ShowIcon = true, ShowCloseButton = false,
                    RenderStyle = MessageBoxRenderStyle.Danger,
                });
                if(ResultDl == true)
                {
                    DeleteVoucher(curStt_Rec);
                }

                StateHasChanged();
            }
            else
                myStateMN.SetNewThongbao(Lap["No data selected"]);
        }
        public virtual async void V_In()
        {
            DataRowView rview = (DataRowView)SelectedItem;
            if (rview == null)
            {               
                myStateMN.SetNewThongbao(Lap["No data selected"]);
                return;
            }
           
            DataSet DataSource = CopyVoucherToDataSet(curStt_Rec);           
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
                foreach (DataRow row in tbl.Select("stt_rec='" + _stt_rec + "'"))
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
                foreach (DataRow row in tbl.Select("stt_rec='" + _stt_rec + "'"))
                {
                    tblNew.ImportRow(row);
                }
                dtsPrt.Tables.Add(tblNew);
            }
            return dtsPrt;
        }
        public DataRow GetRowTable(string table, string filter, string order)
        {
            string sql = "Select top 1 * from " + table + " where  " + filter + " Order by " + order;
            DataTable dataTable = new DataTable();
            try
            {
                DataSet _ds = myDb.LoadDataSet(sql);
                dataTable = _ds.Tables[0];
            }
            catch
            {

            }
            return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : (DataRow)null;
        }
        public async Task<bool> InsertCTHHD(string stt_rec, string ma_qs, string ma_ct, int so_ct)
        {
            Cthhd _cthhd = new Cthhd();
            _cthhd.Stt_rec = stt_rec;
            _cthhd.Ma_qs = ma_qs;
            _cthhd.Ma_ct = ma_ct;
            _cthhd.Ngay_ct = DateTime.Now.Date;
            _cthhd.So_luong = 1;
            _cthhd.So_ct = so_ct.ToString();
            await myDb.InsertOneRow<Cthhd>(_cthhd, "cthhd");
            return true;
        }
        public Dmqs GetNewSo_ct(string ma_qs)
        {
            Dmqs qs = new Dmqs();
            string sql = string.Format("exec [GetNewSoct#2] '{0}'", ma_qs);
            DataSet ds = myDb.LoadDataSet(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                qs.Ma_qs = ds.Tables[0].Rows[0][0].ToString();
                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0][1].ToString()))
                    qs.So_ct = Convert.ToInt32(ds.Tables[0].Rows[0][1].ToString());
            }
            return qs;
        }
        public string GetStt_rec()
        {
            string stt_rec = "";
            string format = "exec {0} '{1}', '{2}'";
            string sql = (Process_store == null || Process_store.Length <= 9) ? string.Format(format, (object)"GetSttRec", Ma_ct, M_WS_ID) : string.Format(format, (object)Process_store[9], Ma_ct, M_WS_ID);
            //sql = string.Format("exec [GetSttRec] '{0}', '{1}'", ma_ct, wid);
            DataSet ds = myDb.LoadDataSet(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                stt_rec = ds.Tables[0].Rows[0][0].ToString();
            }
            return stt_rec;
        }
        public string GetStt_rec(string ma_ct, string wid)
        {
            string stt_rec = "";
            string format = "exec {0} '{1}', '{2}'";
            string sql = (Process_store == null || Process_store.Length <= 9) ? string.Format(format, (object)"GetSttRec", ma_ct, wid) : string.Format(format, (object)Process_store[9], ma_ct, wid);
            //sql = string.Format("exec [GetSttRec] '{0}', '{1}'", ma_ct, wid);
            DataSet ds = myDb.LoadDataSet(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                stt_rec = ds.Tables[0].Rows[0][0].ToString();
            }
            return stt_rec;
        }
        public async Task<string> GetNewSoct(string ma_qs)
        {
            int M_AUTO_SOCT = int.Parse(myOption.GetOptionsValue("M_AUTO_SOCT"));
            string sql = M_AUTO_SOCT == 1 ? "SELECT transform, so_ct + 1 as so_ct FROM dmqs WHERE ma_qs = '" + ma_qs.Trim() + "'" : "EXEC  [GetNewSoct] '" + ma_qs.Trim() + "'";
            DataSet _ds = await myDb.LoadDataset(sql);
            DataTable table = _ds.Tables[0];
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                if (row[1] != null && row[1] != DBNull.Value)
                {
                    string str2 = row[1].ToString();
                    return string.Format(row[0].ToString(), Convert.ToDouble(str2));
                }
            }
            return "";
        }
        public bool CheckColumn(DataRowView view, string columnName)
        {
            foreach (DataColumn column in view.DataView.ToTable().Columns)
            {
                if (column.ColumnName.ToLower().Equals(columnName))
                {
                    return true;
                }
            }
            return false;
        }
        public string GetMant(string ma_ct)
        {
            string Ma_nt = "";
            string sql = string.Format("Select ma_nt from dmct where ma_ct = '{0}'", ma_ct);
            DataSet ds = myDb.LoadDataSet(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                Ma_nt = ds.Tables[0].Rows[0][0].ToString();
            }
            return Ma_nt;
        }
        public void LoadQuyenso()
        {
            string sql = String.Format("Select ma_qs, ma_cts, transform, expr1, expr2 from dmqs where ma_qs = '{0}' and ma_cts = '{1}'", Ma_qs, Ma_ct);
            DataSet ds = myDb.LoadDataSet(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                List<Dmqs> lsqs = myDb.ConvertDataTable<Dmqs>(ds.Tables[0]);
                if (lsqs != null && lsqs.Count > 0)
                    curQuyenso = lsqs[0];
            }
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
        public virtual void Grid_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            if (e.ElementType == GridElementType.DataRow)
            {
                e.CssClass = "grdrow";
            }
        }
        public virtual decimal GetRates(string _ma_nt, DateTime _ngay)
        {
            decimal _ty_giaf = 0;
            try
            {
                string sql = string.Format("Select [dbo].[GetRates]('{0}','{1}')", _ma_nt, _ngay.ToString("yyyyMMdd"));
                DataSet ds = myDb.LoadDataSet(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    _ty_giaf = Convert.ToDecimal(ds.Tables[0].Rows[0][0].ToString());
                }
                return _ty_giaf;
            }
            catch (Exception ex)
            {

            }
            return _ty_giaf;
        }
        public void Dispose()
        {
            myStateMN.OnActionTaskChanged -= OnActionTaskchanged;
            myStateMN.OnLangguaChanged -= OnLangguachange;
        }
    }
}
