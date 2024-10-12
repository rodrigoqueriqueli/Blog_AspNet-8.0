using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Attributes
{

    //atributo que sera usado pra interceptar a requisicao e ver se a key passada conhecide com a minha key 
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {

        //filtro de acao, enquando acao estiver sendo executa, vou poder interceptar e validar
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Query string...esta na rota passada, vai tentar obter o valor da key dessa rota
            if (!context.HttpContext.Request.Query.TryGetValue(Configuration.ApiKeyName, out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "ApiKey not found"
                };

                return;
            }

            if (!Configuration.ApiKey.Equals(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 403,
                    Content = "Not authorized"
                };

                return;
            }

            await next();
        }
    }
}
