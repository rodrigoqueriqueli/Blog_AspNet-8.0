using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        //convencao de nome da rota, sempre no minusculo e sempre no plural..
        //nome composto: user-roles ao inves de userRoles
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync(
            [FromServices] BlogDataContext context,
            [FromServices] IMemoryCache cache)
        {
            try
            {
                //var categories = await context.Categories.ToListAsync();

                //GetOrCreate - vai tentar obter e se nao existir vai criar
                //Todo cache tem uma chave
                var categories = cache.GetOrCreate("CategoriesCache", entry =>
                {
                    //vai criar a chave e atribuir pra que ele seja valido por 1 hora
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return GetCategories(context);
                });

                if (categories == null)
                    return NotFound();

                //o retorno deve ser concreto (nao retornar uma tarefa dentro de uma tarefa).
                //Nao se deve retornar um task dentro de um task
                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("Falha interna no servidor"));
            }
        }

        private List<Category> GetCategories(BlogDataContext context)
        {
            return context.Categories.ToList(); 
        }

        [HttpGet("v2/categories")]
        public IActionResult Get2(
        [FromServices] BlogDataContext context)
        {
            var categories = context.Categories.ToList();

            if (categories == null)
                return NotFound();

            return Ok(categories);
        }


        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories
                                            .FirstOrDefaultAsync(category => category.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>("Content not found"));

                //o retorno deve ser concreto (nao retornar uma tarefa dentro de uma tarefa).
                //Nao se deve retornar um task dentro de um task
                return Ok(new ResultViewModel<Category>(category));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>("Falha interna no servidor"));
            }
        }


        //retornar created eh soh para o Post
        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] EditorCategoryViewModel categoryViewModel,
            [FromServices] BlogDataContext context)
        {
            //validacao automatica do asp.net para algum campo do body que nao seja mandando por exemplo
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            try
            {
                var category = new Category
                {
                    Id = 0,
                    Name = categoryViewModel.Name,
                    Slug = categoryViewModel.Slug.ToLower(),
                };
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>($"Nao foi possivel incluir a categoria: {ex.Message}"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>("Falha interna no servidor"));
            }
        }


        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel categoryViewModel,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var categoryOnDb = await context
                                .Categories
                                .FirstOrDefaultAsync(category => category.Id == id);

                if (categoryOnDb == null)
                    return NotFound(new ResultViewModel<Category>("Content not found"));

                categoryOnDb.Name = categoryViewModel.Name;
                categoryOnDb.Slug = categoryViewModel.Slug;

                context.Categories.Update(categoryOnDb);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(categoryOnDb));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>($"Nao foi possivel alterar a categoria: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("Falha interna no servidor"));
            }

        }


        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var categoryOnDb = await context
                               .Categories
                               .FirstOrDefaultAsync(category => category.Id == id);

                if (categoryOnDb == null)
                    return NotFound(new ResultViewModel<Category>("Content not found"));

                context.Categories.Remove(categoryOnDb);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(categoryOnDb));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>($"Nao foi possivel deletar a categoria: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("Falha interna no servidor"));
            }

        }
    }
}
