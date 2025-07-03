using Microsoft.AspNetCore.Mvc;

namespace SISERPSME.Controllers
{
    public class Qrkiemke : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public string GetStt()
        {
            return "";
        }
        [HttpGet]
        public IEnumerable<string> GetName(string id)
        {
            return new string[] { "value1", "value2", id };
        }
    }
}
