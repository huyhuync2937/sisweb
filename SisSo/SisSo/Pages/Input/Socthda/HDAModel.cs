using SisSo.Model;

namespace SisSo.Pages.Input.Socthda
{
    public class HDAModel
    {
        public string Tieu_de1 { get; set; }
        public string Tieu_de2 { get; set; }
        public byte[] Print_logo { get; set; }
        public List<Ph81> LSPh { get; set; }
        public List<Ct81> LSCt { get; set; }
    }
}
