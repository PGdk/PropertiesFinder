using IntegrationApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IntegrationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class InfoController : ControllerBase
    {
        private readonly InfoViewModel _info;

        public InfoController(IOptions<InfoViewModel> viewModel) => _info = viewModel.Value;

        [HttpGet]
        public IActionResult Info() => Ok(_info);
    }
}