using Microsoft.AspNetCore.Mvc; 
using Hotebedscache.Service.Interfaces;

namespace Hotebedscache.Api.Controllers
{
    [Route("api/master")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private readonly IMasterServices _masterService;
        public MasterController(IMasterServices masterServices)
        {
            _masterService = masterServices;
        } 
    }
}
