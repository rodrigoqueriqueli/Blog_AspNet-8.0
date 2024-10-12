using Blog.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    //por convencao o controller raiz sempre chama HomeController
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {

        [HttpGet("")] //health check, rota para indicar se api esta funcionando..se retornar 500 eh porque nao esta
        //Pra usar o ApiKey nao posso utilizar o atributo [Authorize]
        //[ApiKey] //Atributo para acessar api sem autenticacao via login
        public IActionResult Get()
        {
            return Ok();
        }


        [HttpGet("get2")] 
        public IActionResult Get2(
            [FromServices] IConfiguration config)
        {
            var environment = config.GetValue<string>("Env");
            return Ok(
                new
                {
                    environment = environment,
                });
        }
    }
}
