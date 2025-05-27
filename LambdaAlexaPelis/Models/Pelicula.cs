using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaAlexaPelis.Models
{
    [Table("pelismysql")]
    public class Pelicula
    {
        [Key]
        [Column("IdPelicula")]
        public int IdPelicula { get; set; }
        [Column("Genero")]
        public string Genero { get; set; }
        [Column("Titulo")]
        public string Titulo { get; set; }
        [Column("Argumento")]
        public string Argumento { get; set; }
        [Column("Actores")]
        public string Actores { get; set; }
        [Column("Precio")]
        public int Precio { get; set; }
        [Column("Youtube")]
        public string YouTube { get; set; }
    }
}
