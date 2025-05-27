using Amazon.Lambda.Core;
using LambdaAlexaPelis.Data;
using LambdaAlexaPelis.Models;
using LambdaAlexaPelis.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaAlexaPelis;

public class Function
{
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

    public async Task<Pelicula> FunctionHandler(ILambdaContext context)
    {
        var repo = ServiceProvider.GetService<RepositoryPeliculas>();
        Pelicula peli =
            await repo.FindPeliculaAsync(1);
        return peli;
    }
}
