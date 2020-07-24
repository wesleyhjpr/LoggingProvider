using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace loggingProvider.Entidades
{
    [Table("LogEvento")]
    public class LogEvento : Entidade
    {
        public string Category { get; set; }
        public int EventId { get; set; }
        public string LogLevel { get; set; }
        public string Message { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
