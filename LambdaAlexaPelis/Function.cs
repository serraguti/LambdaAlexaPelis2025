using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using LambdaAlexaPelis.Data;
using LambdaAlexaPelis.Models;
using LambdaAlexaPelis.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
//[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaAlexaPelis;

public class Function
{
    ILambdaLogger log;

    private static ServiceProvider ServiceProvider { get; set; }

    public Function()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }
    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<RepositoryPeliculas>();
        string connectionString = @"Data Source=sqltajamarpgs.database.windows.net;Initial Catalog=AZURETAJAMAR;Persist Security Info=True;User ID=adminsql;Password=Admin123;Trust Server Certificate=True";
        services.AddDbContext<PeliculasContext>
            (options => options.UseSqlServer(connectionString));
    }

    public async Task<SkillResponse>
            FunctionHandler(SkillRequest input, ILambdaContext context)
    {
        var repo = ServiceProvider.GetService<RepositoryPeliculas>();
        SkillResponse response = new SkillResponse();
        response.Response = new ResponseBody();
        response.Response.ShouldEndSession = false;
        IOutputSpeech innerResponse = null;
        this.log = context.Logger;
        log.LogLine($"Skill Request Object:"
            + JsonConvert.SerializeObject(input));
        if (input.GetRequestType() == typeof(LaunchRequest))
        {
            innerResponse = new PlainTextOutputSpeech();
            (innerResponse as PlainTextOutputSpeech).Text =
                "Soy tu Alexa privado.  Pideme una película...¿Qué número de Pelicula quieres?";
        }
        else if (input.GetRequestType() == typeof(IntentRequest))
        {
            var intentRequest = (IntentRequest)input.Request;
            if (intentRequest.Intent.Name == "preguntapelis")
            {
                log.LogLine("PIDIENDO DATOS!!!!!!!");
                string slotJson = JsonConvert.SerializeObject
                    (intentRequest.Intent.Slots);
                int idpelicula = GetSlotValue(slotJson);
                log.LogLine($"Id Peli: " + idpelicula);
                log.LogLine($"Slots peli: " + slotJson);
                Pelicula peli = await repo.FindPeliculaAsync(idpelicula);
                if (peli != null)
                {
                    innerResponse = new PlainTextOutputSpeech();
                    (innerResponse as PlainTextOutputSpeech).Text =
                        peli.Argumento;
                }
                else
                {
                    innerResponse = new PlainTextOutputSpeech();
                    (innerResponse as PlainTextOutputSpeech).Text =
                        "No he encontrado tu Peli " + idpelicula;
                }
            }
            else
            {
                innerResponse = new PlainTextOutputSpeech();
                (innerResponse as PlainTextOutputSpeech).Text =
                    "Ni idea de lo que me hablas";
            }
        }
        else
        {
            innerResponse = new PlainTextOutputSpeech();
            (innerResponse as PlainTextOutputSpeech).Text =
                "Ni idea de lo que me hablas, en else";
        }

        response.Response.OutputSpeech = innerResponse;
        response.Version = "1.0";
        return response;
    }

    private int GetSlotValue(string dataJson)
    {
        var jsonObject = JObject.Parse(dataJson);
        var data = (JObject)jsonObject["idpelicula"];
        var nombre = (string)data["name"];
        var id = (string)data["value"];
        return int.Parse(id);
    }

    private string GetSlotValueString(string dataJson)
    {
        try
        {
            var jsonObject = JObject.Parse(dataJson);
            var data = (JObject)jsonObject["idpelicula"];
            log.LogLine($"Data " + data);
            var nombre = (string)data["name"];
            log.LogLine($"nombre " + nombre);
            var id = (string)data["value"];
            log.LogLine($"Id " + id);
            return "Nombre " + nombre + ", Id: " + id;
        }
        catch (Exception ex)
        {
            log.LogLine($"Error Gordo... " + ex);
            return ex.ToString();
        }
    }
}
