using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AccountController(
            ITokenService tokenService)
        {
            _tokenService = tokenService;
        }


        [HttpPost("v1/accounts/")]
        public async Task<IActionResult> PostAsync(
            [FromBody] RegisterViewModel model,
            [FromServices] BlogDataContext context,
            [FromServices] IEmailService emailService) //Contexto de dados
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<User>(ModelState.GetErrors()));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-")
                                  .Replace(".", "-")
            };

            //gerar senha e logo apos hashear a senha
            var password = PasswordGenerator.Generate(25);

            user.PasswordHash = PasswordHasher.Hash(password);

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                //emailService.Send(user.Name, user.Email, 
                //    "Bem vindo ao blog", $"Your password is <strong>{password}</strong>");

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    password
                }));

            }
            catch (DbUpdateException ex)
            {
                return StatusCode(400,
                    new ResultViewModel<User>($"This email already exist: {ex.Message}"));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<User>("Falha interna no servidor"));
            }
        }


        [AllowAnonymous] //As this controller has the Authorize attribute,
                         //in order to give access to the Login I need to use the AllowAnonymous attribute
        [HttpPost("v1/accounts/login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginViewModel model,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await context.Users
                              .AsNoTracking()
                              .Include(user => user.Roles) //Inclur os roles pra gerar os claims
                              .FirstOrDefaultAsync(user => user.Email == model.Email);

            if (user == null)
                return StatusCode(401, new ResultViewModel<string>("Invalid user"));

            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, new ResultViewModel<string>("Invalid user"));


            try
            {
                var token = _tokenService.GenerateToken(user);

                return Ok(new ResultViewModel<string>(token, null));
            }
            catch
            {
                return StatusCode(500,
                   new ResultViewModel<User>("Falha interna no servidor"));
            }

        }


        [HttpPost("v1/accounts/upload-image")]
        public async Task<IActionResult> UploadImage(
            [FromBody] UploadImageViewModel model,
            [FromServices] BlogDataContext context)
        {
            var fileName = $"{Guid.NewGuid()}.jpg";
            var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(model.Base64Image, "");
            var bytes = Convert.FromBase64String(data);

            try
            {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<string>("Falha Interna no servidor"));
            }

            var user = await context.Users
                                    .FirstOrDefaultAsync(user => user.Email == User.Identity.Name);

            if (user == null)
                return NotFound(new ResultViewModel<User>("User not found"));


            user.Image = $"https://localhost:7296/images/{fileName}";

            try
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();   

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<string>("Falha interna no servidor"));
            }

            return Ok(new ResultViewModel<string>("Imagem alterada com sucesso", null));
        }

        //[Authorize(Roles="user")] //Verify if the user is logged-in
        //[HttpGet("v1/user")]
        //public IActionResult GetUser()
        //    => Ok(User.Identity.Name); //nome do usuario logado

        //[Authorize(Roles ="author")]
        //[Authorize(Roles = "admin")]
        //[HttpGet("v1/author")]
        //public IActionResult GetAuthor()
        //    => Ok(User.Identity.Name); //nome do usuario logado

        //[Authorize(Roles ="admin")]
        //[HttpGet("v1/admin")]
        //public IActionResult GetAdmin()
        //    => Ok(User.Identity.Name); //nome do usuario logado
    }
}
