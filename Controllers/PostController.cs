using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController] //Pra nao ter uma rota base/padrao
    public class PostController : ControllerBase
    {
        [HttpGet("v1/posts")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context,
            [FromQuery] int page = 0, //from query string
            [FromQuery] int pageSize = 25)
        {

            try
            {
                var count = await context.Posts
                                    .AsNoTracking()
                                    .CountAsync();

                var posts = await context.Posts
                    .AsNoTracking() //usado quando soh quero ler, nao quero modificar nada
                    .Include(post => post.Category) //preciso incluir esses dois categoria e author
                    .Include(post => post.Author)
                    .Select(post =>
                    new ListPostsViewModel
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Slug = post.Slug,
                        LastUpdateDate = post.LastUpdateDate,
                        Category = post.Category.Name,
                        Author = $"{post.Author.Name} ({post.Author.Email})",
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(post => post.LastUpdateDate) //Do mais atual, do ultimo modificado
                    .ToListAsync(); //essa eh a query que vai consultar no banco, por isso o select tem que ser feito antes de chama o ToListAsync


                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<List<Post>>("Falha interna no servidor"));
            }


        }


        [HttpGet("v1/posts/{id:int}")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] int id)
        {

            try
            {

                var post = await context.Posts
                    .AsNoTracking() //usado quando soh quero ler, nao quero modificar nada
                    .Include(post => post.Author)
                    .ThenInclude(x => x.Roles) //Um post tem um autor, e um autor tem muitos roles, (then Include server pra pegar um outro noh)
                                               //ThenInclude ira gerar um subSelect no banco, estar consciente desse preco
                    .Include(post => post.Category) //preciso incluir esses dois categoria e author
                    .FirstOrDefaultAsync(post => post.Id == id);


                if (post == null)
                    return NotFound(new ResultViewModel<Post>("Content not found"));

                return Ok(new ResultViewModel<Post>(post));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<Post>("Falha interna no servidor"));
            }


        }


        [HttpGet("v1/posts/category/{category}")]
        public async Task<IActionResult> GetByCategoryAsync(
           [FromRoute] string category,
           [FromServices] BlogDataContext context,
           [FromQuery] int page = 0, //from query string
           [FromQuery] int pageSize = 25)
        {

            try
            {
                var count = await context.Posts
                                    .AsNoTracking()
                                    .CountAsync();

                var posts = await context.Posts
                    .AsNoTracking() //usado quando soh quero ler, nao quero modificar nada
                    .Include(post => post.Category) //preciso incluir esses dois categoria e author
                    .Include(post => post.Author)
                    .Where(post => post.Category.Slug == category)
                    .Select(post =>
                    new ListPostsViewModel
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Slug = post.Slug,
                        LastUpdateDate = post.LastUpdateDate,
                        Category = post.Category.Name,
                        Author = $"{post.Author.Name} ({post.Author.Email})",
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(post => post.LastUpdateDate) //Do mais atual, do ultimo modificado
                    .ToListAsync(); //essa eh a query que vai consultar no banco, por isso o select tem que ser feito antes de chama o ToListAsync
                    //ToList eh a materializacao da query no banco

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<List<Post>>("Falha interna no servidor"));
            }


        }
    }
}
