using SisSo.Pages.Input.Socthda;

namespace SisSo.Pages.Input.Sodmhdb
{
    public class HDBModel
    {
        public string Tieu_de1 { get; set; }
        public string Tieu_de2 { get; set; }
        public byte[] Print_logo { get; set; }
        public List<Dmhd> LSPh { get; set; }
        public List<Dmhdct> LSCt { get; set; }
    }
}
