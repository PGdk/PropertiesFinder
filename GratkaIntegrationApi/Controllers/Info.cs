using DatabaseConnection;
using DatabaseConnection.Models;
using Microsoft.AspNetCore.Mvc;
namespace IntegrationApi.Controllers
{
    [Route("")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly DatabaseContext _context;
        public InfoController(DatabaseContext context)
        {
            _context = context;
        }
        [HttpGet]
        public Info Get()
        {
            return new Info()
            {
                ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=BartoszW137434;Integrated Security=True",
                IntegrationName = "Gratka",
                StudentIndex = 137434,
                StudentName = "BartoszW"
            };
        }
    }
}
