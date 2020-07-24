using System;
using System.ComponentModel.DataAnnotations;

namespace loggingProvider.Entidades
{
    public abstract class Entidade
    {
        protected Entidade()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public virtual Guid Id { get; set; }
    }
}
