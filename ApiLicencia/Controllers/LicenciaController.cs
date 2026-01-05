using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiLicencia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenciaController : ControllerBase
    {
        private readonly IConfiguration _config;

        public LicenciaController(IConfiguration config)
        {
            _config = config;
        }


        [HttpGet()]
        public IActionResult ObtenerLicencia()
        {
            var licenciaActiva = Convert.ToInt32(_config["activaLicencia"]) == 1 ? "OK" : "ERROR";
            return Ok(licenciaActiva);
        }
    }
}
