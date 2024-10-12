using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.Extensions
{
    public static class ModelStateExtensions
    {

        public static List<string> GetErrors(this ModelStateDictionary modelState)
        {
            return modelState.Values
                             .SelectMany(x => x.Errors)
                             .Select(error => error.ErrorMessage)
                             .ToList();


            //var result = new List<string>(); 
            
            //foreach(var value in modelState.Values)
            //{
            //    foreach (var error in value.Errors)
            //    {
            //        result.Add(error.ErrorMessage);
            //    }
            //}

            //return result;
        }
    }
}
