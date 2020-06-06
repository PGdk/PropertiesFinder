using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        public ActionResult<Info> GetInfo()
        {
            return new Info()
            {
                ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=PawelN170633;Integrated Security=True",
                IntegrationName = "ZnajdzTo",
                StudentName = "Paweł",
                StudentIndex = 170633
            };
        }
    }

}
