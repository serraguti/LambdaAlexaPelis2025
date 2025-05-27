using LambdaAlexaPelis.Data;
using LambdaAlexaPelis.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaAlexaPelis.Repositories
{
    public class RepositoryPeliculas
    {
        private PeliculasContext context;
        public RepositoryPeliculas(PeliculasContext context)
        {
            this.context = context;
        }

        public async Task<List<Pelicula>> GetPeliculasAsync()
        {
            return await this.context.Peliculas.ToListAsync();
        }

        public async Task<Pelicula> FindPeliculaAsync(int idpelicula)
        {
            return await this.context.Peliculas
                .FirstOrDefaultAsync(x => x.IdPelicula == idpelicula);
        }
    }
}
