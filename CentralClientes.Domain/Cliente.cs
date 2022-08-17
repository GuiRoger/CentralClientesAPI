using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralClientes.Domain
{
    public class Cliente
    {    
        public int Id { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}
