using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralClientes.Domain
{
    public class Endereco
    { 
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Rua { get; set; }
        public int Numero { get; set; }  
        public string Cep { get; set; }

    }
}
