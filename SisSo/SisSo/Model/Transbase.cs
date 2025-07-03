using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisData.Data;
using SisData.Service;
using SisData.Dataaccess;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Components;
using System.Drawing.Text;
using Microsoft.Extensions.DependencyInjection;
using Blazored.LocalStorage;
using SisData.Model;
using System.Globalization;
using Microsoft.VisualBasic.FileIO;

namespace SisSo.Model
{
    public class Transbase
    {
        public string Ws_Id = string.Empty;
        public string Ma_ct = string.Empty;
        public string Menu_id = string.Empty;
        public string Editing_Stt_Rec = string.Empty;
        public string M_Phdbf = string.Empty;
        public string M_Ctdbf = string.Empty;
        public string M_Ctgtdbf = string.Empty;

        public Command CommandInfo = null;
        public DataRow DmctInfo = (DataRow)null;
        public DataRow Ma_ct_info;
        public DataTable tbStatus = (DataTable)null;
        public DataSet DsTrans = (DataSet)null;

        public Users CurUser;
        public string Phbrowse = string.Empty;
        public string Ctbrowse = string.Empty;
        public string filterId = string.Empty;
        public string filterView = string.Empty;
        public string M_ma_nt0 = string.Empty;
        public string M_LAN = string.Empty;
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
        public string[] Process_Store;

        [Inject]
        public IOptionsManager myopt { get; set; }
        [Inject]
        public Statemanagerment mystate { get; set; }
        IDbManager dbManager;
        public Transbase(IDbManager _dbManager)
        {
            dbManager = _dbManager;
        }
        public Transbase(string pma_ct, string pmenu_id, Statemanagerment _mystate, IDbManager _dbManager = null)
        {
            if (_dbManager != null)
                dbManager = _dbManager;
            
            mystate = _mystate;
            Ma_ct = pma_ct;

            if (pma_ct == null)
                return;

            CultureInfo cutr = CultureInfo.DefaultThreadCurrentCulture;
            string UserLang = cutr.Name.ToUpper();
            M_LAN = UserLang.StartsWith("V") ? "V" : "E";

            IDbManager dbManager1 = mystate.GetDbManager();
            myopt = mystate.GetOptions();
            LoadOptions();
            CurUser = mystate.GetCurUser();
            if(String.IsNullOrEmpty(pmenu_id))
            {
                string sqlcmd = "is_mobile = 2 AND ma_ct = '"+ Ma_ct + "'";
                pmenu_id = dbManager1.GetValue("command","min(menu_id)",sqlcmd).ToString().Trim();
                Menu_id = pmenu_id;
            }    

            ICommandManager Commands = mystate.GetCommand();
            Task<Command> task = Task.Run(async () => await Commands.GetSubMenuBymenuId(pmenu_id));
            CommandInfo = task.Result;

            if (!String.IsNullOrEmpty(CommandInfo.Ma_ct))
                Ma_ct = CommandInfo.Ma_ct;

            string[] brows = (M_LAN == "V" ? CommandInfo.Vbrowse1.Trim().Split('|') : CommandInfo.Ebrowse1.Trim().Split('|'));
            Phbrowse = (brows.Length > 0 ? brows[0] : "");
            Ctbrowse = (brows.Length > 1 ? brows[1] : "");
            if (String.IsNullOrEmpty(Phbrowse))
                Phbrowse = @"stt_rec:110:h=stt rec;ngay_ct:80:h=Ngày c.từ:FL:D;so_ct:85:h=Số hđ:FL:HR:LINKCT";
            if (String.IsNullOrEmpty(Ctbrowse))
                Ctbrowse = @"stt_rec:110:h=stt rec;stt_rec0:110:h=stt rec0";

            string strsql = "Select * from dmct where ma_ct = '" + Ma_ct + "'";
            DataSet dsdmct = dbManager1.LoadDataSet(strsql);
            DmctInfo = dsdmct.Tables[0].Rows[0];
            //string modelstr = SisFunc.Converts.DataTable2Model(dsdmct.Tables[0], "Dmct");
            if (DmctInfo != null)
            {
                M_Phdbf = DmctInfo["m_phdbf"].ToString().Trim();
                M_Ctdbf = DmctInfo["m_ctdbf"].ToString().Trim();
                M_Ctgtdbf = DmctInfo["m_ctgtdbf"].ToString().Trim();
            } 
            else
            {
                mystate.SetNewThongbao("Không tìm thấy mã chứng từ " + Ma_ct + " trong bảng dmct");
                return;
            }    

            Process_Store = DmctInfo["process_Store"].ToString().Trim().Split('|');
            string _ma_dvcs = mystate.GetMa_Dvcs();
            string phFilter = _ma_dvcs.ToUpper().Trim().Equals("ALL") ? "1=1" : "ma_dvcs = ''" + mystate.GetMa_Dvcs() + "''";
            strsql = "Exec " + Process_Store[0].Trim() + " @ma_ct='" + Ma_ct + "',@sPhFilter ='" + phFilter + "',@sCtFilter='1=1',@sGtFilter='1=1'";
            DsTrans = dbManager1.LoadDataSet(strsql);
        }
        public Dmqs GetNewSo_ct(string ma_qs)
        {
            Dmqs qs = new Dmqs();
            string sql = string.Format("exec [GetNewSoct#2] '{0}'", ma_qs);
            DataSet ds = dbManager.LoadDataSet(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                qs.Ma_qs = ds.Tables[0].Rows[0][0].ToString();
                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0][1].ToString()))
                    qs.So_ct = Convert.ToInt32(ds.Tables[0].Rows[0][1].ToString());
            }
            return qs;
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
            await dbManager.InsertOneRow<Cthhd>(_cthhd, "cthhd");
            return true;
        }
        public string GetStt_rec(string ma_ct, string wid)
        {
            string stt_rec = "";
            string sql = string.Format("exec [GetSttRec] '{0}', '{1}'", ma_ct, wid);
            DataSet ds = dbManager.LoadDataSet(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                stt_rec = ds.Tables[0].Rows[0][0].ToString();
            }
            return stt_rec;
        }
        public string GetMant(string ma_ct)
        {
            string Ma_nt = "";
            string sql = string.Format("Select ma_nt from dmct where ma_ct = '{0}'", ma_ct);
            DataSet ds = dbManager.LoadDataSet(sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                Ma_nt = ds.Tables[0].Rows[0][0].ToString();
            }
            return Ma_nt;
        }
        void LoadOptions()
        {
            M_ma_nt0 = myopt.GetOptionsValue("M_MA_NT0");
            M_MA_THUE = myopt.GetOptionsValue("M_MA_THUE");
            M_WS_ID = myopt.GetOptionsValue("M_WS_ID").Trim().ToUpper();
            M_IP_SL = myopt.GetOptionsValue("M_IP_SL");
            M_IP_GIA = myopt.GetOptionsValue("M_IP_GIA");
            M_IP_GIA_NT = myopt.GetOptionsValue("M_IP_GIA_NT");
            M_IP_TIEN = myopt.GetOptionsValue("M_IP_TIEN");
            M_IP_TIEN_NT = myopt.GetOptionsValue("M_IP_TIEN_NT");
            M_IP_TY_GIA = myopt.GetOptionsValue("M_IP_TY_GIA");
            M_IP_TY_GIAF = myopt.GetOptionsValue("M_IP_TY_GIAF");            
        }
    }
}
