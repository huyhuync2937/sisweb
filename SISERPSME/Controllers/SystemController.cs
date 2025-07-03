using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SISERPSME.Controllers
{
    [ApiController]
    [Route("api/system")]
    public class SystemController : ControllerBase
    {
        [HttpGet("hostname")]
        public IActionResult GetHostName()
        {
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;

            if (remoteIpAddress == null)
                return BadRequest("NOIP");

            try
            {
                var hostName = Dns.GetHostEntry(remoteIpAddress)?.HostName ?? "NOXD";
                return Ok(hostName);
            }
            catch
            {
                return BadRequest("NONAME");
            }
        }
    }
}
