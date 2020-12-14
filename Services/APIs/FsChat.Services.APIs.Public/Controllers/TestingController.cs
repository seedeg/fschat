using System;
using System.Net.NetworkInformation;
using System.Web.Http;

namespace FsChat.Services.APIs.Public.Controllers
{
    // This controller is used to test whether the API is up and running
    [RoutePrefix("api/testing")]
    public class TestingController : ApiController
    {
        [HttpGet]
        [Route("date")]
        public IHttpActionResult GetDate()
        {
            return Ok(DateTime.UtcNow);
        }

        [HttpGet]
        [Route("unique-id")]
        public IHttpActionResult GetMacAddress()
        {
            string macAddress = string.Empty;
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    macAddress += networkInterface.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return Ok(macAddress);
        }
    }
}
