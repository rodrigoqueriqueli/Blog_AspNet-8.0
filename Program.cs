using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

ConfigureAuthentication(builder);

ConfigureMvc(builder);

ConfigureServices(builder);


//adding swagger
builder.Services.AddEndpointsApiExplorer();
//responsible for generating code of swagger interface (HTML)
builder.Services.AddSwaggerGen();



//Parecido com o Transient, mas vai durar por requisicao..reaproveita a dependencias existentes ao longo da requisicao
//builder.Services.AddScoped<ITokenService, TokenService>(); 


//Dependencia continuara existindo ao longo da execucao da aplicacao
//builder.Services.AddSingleton<ITokenService, TokenService>();

var app = builder.Build();

LoadConfiguration(app);

//Forcing HTTPS
app.UseHttpsRedirection(); //redirecting http to https 


//configurando aplicacao pra utilizar autenticacao e autorizacao
app.UseAuthentication(); //quem vc eh
app.UseAuthorization(); //o que vc pode fazer

//mapeando os controllers
app.MapControllers();

//Pra servir arquivos estaticos chamar
app.UseStaticFiles(); //sempre vai procurar dentro do diretorio wwwrot

//To compress APIs response
app.UseResponseCompression();



if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Development evnironment");
    app.UseSwagger();
    app.UseSwaggerUI();
}

//if (app.Environment.IsProduction())
//    Console.WriteLine("Production evnironment");



app.Run();








void LoadConfiguration(WebApplication app)
{
    //app.Configuration.GetSection() -> Pra pegar um noh/uma sessao especifico do config.json
    //app.Configuration.GetValue<string>("JwtKey"); -> Pegar o valor de uma key especifica  do config.json
    //app.Configuration.GetConnectionString();

    Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");
    Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
    Configuration.ApiKey = app.Configuration.GetValue<string>("ApiKey");

    var smtp = new Configuration.SmtpConfiguration();
    app.Configuration.GetSection("Smtp").Bind(smtp);
    Configuration.Smtp = smtp;
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    //Indicando para aplicacao como sera feita a autenticacao do token
    //Desencriptacao com a chave secreta 
    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

    //adicionando autenticacao a app
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        //parametros do token
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, //validar a chave de assinatura
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
        };

    }); //o AddJwtBearer eh pra indicar como ele vai desencriptar o token
}

void ConfigureMvc(WebApplicationBuilder builder)
{
    //pra dar suporte ao mvc
    builder.Services.AddMemoryCache(); //utilizando suporte a cache em memoria, pra nao ter que descer no banco toda vez

    //Config para comprimir resposta enviadas de retorno das APIs
    builder.Services.AddResponseCompression(options =>
    {
        options.Providers.Add<GzipCompressionProvider>();
    });
    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Optimal; //Optimal melhor compressao possivel
    });

    builder.Services
           .AddControllers()
           .ConfigureApiBehaviorOptions(options =>
           {
               //flag que suprime/enibe opcao de validacao automatico do model state do asp.net
               options.SuppressModelStateInvalidFilter = true;
           })
           .AddJsonOptions(jsonOptions =>
           {
               jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //ignorar cyclo subsequents, evitar o loop de referencia circular, soh vai ate o primeiro noh
               jsonOptions.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
           });

}

void ConfigureServices(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    //pra deixar nosso BlogDataContext para toda nossa aplicacao
    builder.Services.AddDbContext<BlogDataContext>(ContextOptionsBuilder =>
    {
        ContextOptionsBuilder.UseSqlServer(connectionString);
    }); //Se tiver usando o DbContext sempre use via AddDbContext


    //Sempre criara uma nova instancia
    builder.Services.AddTransient<ITokenService, TokenService>();
    builder.Services.AddTransient<IEmailService, EmailService>();
}