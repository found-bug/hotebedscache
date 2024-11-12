using Hotebedscache.Service;
using Hotebedscache.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Collections;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace Hotebedscache.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelAPIController : ControllerBase
    {
        private readonly IHotelServices _masterService;
        public HotelAPIController(IHotelServices masterService)
        {
            _masterService = masterService;
        }

        [HttpGet]
        [Route("get/hotel/data")]
        public async Task<IActionResult> getZipData()
        {
            try
            {
                await _masterService.callapi();
            }
            catch (Exception e1)
            {

                throw;
            }
            return Ok();
        }
    }
}
