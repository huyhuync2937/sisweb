using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisSo.Pages.SaleRepo.Soth1
{ 
    [Serializable]
    public class Soth1Model
    {       
        public DateTime Ngay_ct1 { get; set; }
        public DateTime Ngay_ct2 { get; set; }
        public string Ma_ma { get; set; }
        public int LoaiBoCT { get; set; }
        public List<Soth2> LsData { get; set; }
        public Soth1Model()
        {
            LsData = new List<Soth2>();
        }
    }
    public class Soth2
    {
        public Int64 Stt { get; set; }
        public String Ma_vt { get; set; }
        public String Ma_vt_ten_vt { get; set; }
        public String Ma_vt_dvt { get; set; }
        public Decimal So_luong { get; set; }
        public Decimal Gia2 { get; set; }
        public Decimal Tien2 { get; set; }
        public Decimal Ck { get; set; }
        public Decimal Tien_sau_ck { get; set; }
        public Decimal Thue { get; set; }
        public Decimal Pt { get; set; }
        public Decimal Gia { get; set; }
        public Decimal Tien { get; set; }
        public Decimal Sl_hbtl { get; set; }
        public Decimal Tien_hbtl { get; set; }
        public Decimal Tien_ck_tl { get; set; }
        public Decimal Tien_thue_tl { get; set; }
        public Decimal Tien_von_tl { get; set; }
        public Decimal Tien_giam_gia { get; set; }
        public Decimal Tien_thue_giam_gia { get; set; }
        public Decimal Sl_thuan { get; set; }
        public Decimal Dt_thuan { get; set; }
        public Decimal Tien_thue_thuan { get; set; }
        public Decimal Tien_von_thuan { get; set; }
        public Decimal Lai { get; set; }
        public Decimal Ty_le2 { get; set; }
        public Decimal Ty_le { get; set; }
        public String Ma_vt_nh_vt1 { get; set; }
        public String Ma_vt_nh_vt2 { get; set; }
        public String Ma_vt_nh_vt3 { get; set; }
        public String Ten_vt { get; set; }
        public String Ten_vt2 { get; set; }
        public String Dvt { get; set; }
        public Decimal Tien_thue_thuan_nt { get; set; }
        public Decimal Pt_not_ck_nt { get; set; }
        public Decimal Dt_thuan_nt { get; set; }
        public Decimal Tien_nt { get; set; }
        public Decimal Lai_nt { get; set; }
        public Decimal Tien_von_thuan_nt { get; set; }
        public Decimal Tien_von_tl_nt { get; set; }
    }
}
