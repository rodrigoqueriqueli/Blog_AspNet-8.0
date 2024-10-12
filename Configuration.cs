namespace Blog;


public static class Configuration
{
    //TOKEN - sera mandado pro cliente
    //formato do TOKEN a ser usado aqui eh o JWT (Json Web Token)
    //A chave ZmVkYWY3ZDg4NjNiNDhlMTk3YjkyODdkNDkyYjcwOGU= ficara no servidor, e quem tiver acesso a ela, conseguiria descriptar e editar o token
    public static string JwtKey = "ZmVkYWY3ZDg4NjNiNDhlMTk3YjkyODdkNDkyYjcwOGU=";


    //chave pra ser acessada sem necessidade de um usuario criado (exemplo do Robo - Console Application)
    //que vai ler o blog todo dia de manha, atraves dessa Key, o Robo pode acessar as APIs decoradas com o atributo: sem necessidade de estar logado
    public static string ApiKeyName = "api_key"; //nome do parametro passado, vai buscar por esse param na requisicao...se o param tiver na requisicao, entendo que ele esta autenticado
    public static string ApiKey = "curso_api_IlTevUM/z0ey3NwCV/unWg==";

    public static SmtpConfiguration Smtp = new();

    /// <summary>
    /// Class for Email service
    /// </summary>
    public class SmtpConfiguration
    {
        public string Host { get; set; } //gmail.com for instance
        public int Port { get; set; } = 25;
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
