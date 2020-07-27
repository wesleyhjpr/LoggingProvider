using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace loggingProvider.Entidades
{
    [Table("Log")]
    public class Log : Entidade
    {
        public string Categoria { get; set; }
        public int IdEvento { get; set; }
        public string LogLevel { get; set; }
        public string Mensagem { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
