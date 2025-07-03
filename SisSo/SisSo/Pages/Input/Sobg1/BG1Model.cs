using SisSo.Pages.Input.Sodmhdb;

namespace SisSo.Pages.Input.Sobg1
{
    public class BG1Model
    {
        public string Tieu_de1 { get; set; }
        public string Tieu_de2 { get; set; }
        public byte[] Print_logo { get; set; }
        public List<Ph101> LSPh { get; set; }
        public List<Ct101> LSCt { get; set; }
    }
}
