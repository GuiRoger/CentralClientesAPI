using CentralClientes.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralClientes.Infra
{
    public class CentralClienteDbContext : DbContext
    {

        public CentralClienteDbContext(DbContextOptions<CentralClienteDbContext> opt) : base(opt)
        {

        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Telefone> Telefones { get; set; }

    }
}
