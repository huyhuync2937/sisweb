using SisSo.Model;

namespace SisSo.Pages.Reports
{
    public class HDBanleModel
    {
        public string Tieu_de1 { get; set; }
        public string Tieu_de2 { get; set; }
        public byte[] Print_logo { get; set; }
        public List<Ph83> LSPh { get; set; }
        public List<Ct83> LSCt { get; set; }
    }
}
