using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Blog.ViewModels
{
    //Todo resultado das apis retorna isso
    //Padronizando resultado para as apis
    public class ResultViewModel<T>
    {
        public T Data { get; private set; }

        public List<string> Errors { get; private set; } = new();


        public ResultViewModel(T data, List<string> errors)
        {
            Data = data;
            Errors = errors;
        }

        //recebe o dados se der certo
        public ResultViewModel(T data)
        {
            Data = data;
        }

        //recebe o dados quando der errado
        public ResultViewModel(List<string> errors)
        {
            Errors = errors;
        }

        public ResultViewModel(string error)
        {
            Errors.Add(error);
        }

    }
}
