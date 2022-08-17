using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralClientes.Domain
{
    public class Telefone
    {
  
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int DDD { get; set; }
        public int Numero { get; set; }

    }
}
