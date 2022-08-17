using CentralClientes.Domain;
using CentralClientes.Infra;
using CentralClientes.Models;
using CentralClientes.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CentralClientes.Negocio
{
    public class ClienteNegocios
    {
        public CentralClienteDbContext _context;
        public ClienteNegocios(CentralClienteDbContext context)
        {
            _context = context;
        }

        #region Cadastrar Usuarios
        public async Task<RetornoPadrao> CadastrarCliente(CadCliente client)
        {
            var rp = new RetornoPadrao();
            using (var transacao = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    client.NumeroTel = client.NumeroTel.Trim().Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "");
                    client.CPF = client.CPF.Trim().Replace("-", "").Replace(".", "");
                    client.Cep = client.Cep.Trim().Replace("-", "");
                    var validacao = ValidaDados(client);


                    if (validacao.Status)
                    {
                        var dadosCliente = new Cliente()
                        {
                            Nome = client.Nome,
                            CPF = client.CPF,
                            DataCadastro = DateTime.Now

                        };

                        await _context.Clientes.AddAsync(dadosCliente);
                        await _context.SaveChangesAsync();


                        var dadosConta = new Conta()
                        {
                            ClienteId = dadosCliente.Id,
                            SaldoPontos = 0, //Saldo inicial
                        };
                        await _context.Contas.AddAsync(dadosConta);
                        await _context.SaveChangesAsync();

                        var dadosEndereco = new Endereco()
                        {
                            ClienteId = dadosCliente.Id,
                            Rua = client.Rua,
                            Numero = client.Numero,
                            Cep = client.Cep
                        };
                        await _context.Enderecos.AddAsync(dadosEndereco);
                        await _context.SaveChangesAsync();

                        var dadosTelefone = new Telefone()
                        {
                            ClienteId = dadosCliente.Id,
                            DDD = Convert.ToInt32(client.NumeroTel.Substring(0, 2)),
                            Numero = Convert.ToInt32(client.NumeroTel.Substring(2)),
                        };
                        await _context.Telefones.AddAsync(dadosTelefone);
                        await _context.SaveChangesAsync();
                        rp.Mensagem = "Usuário Cadastrado com sucesso.";
                        rp.Status = true;
                        transacao.Commit();
                    }
                    else
                    {

                        rp.Mensagem = validacao.Mensagem;
                        rp.Status = validacao.Status;
                        return rp;
                    }


                }
                catch (Exception ex)
                {
                    rp.Mensagem = ex.Message;
                    rp.Status = false;

                    transacao.Rollback();
                }
            }
            return rp;
        }

        #endregion

        #region Listar Todos os Clientes
        public async Task<List<ClienteCompleto>> ListarClientes()
        {
            var tableClients = await _context.Clientes.ToListAsync();
            var listaClienteCompleto = new List<ClienteCompleto>();
            var tabelaContas = await _context.Contas.Where(cont => tableClients.Select(g => g.Id).Contains(cont.ClienteId)).ToListAsync();
            var tabelaEnderecos = await _context.Enderecos.Where(end => tableClients.Select(g => g.Id).Contains(end.ClienteId)).ToListAsync();
            var tabelaTelefone = await _context.Telefones.Where(tel => tableClients.Select(g => g.Id).Contains(tel.ClienteId)).ToListAsync();


            foreach (var item in tableClients)
            {
                var telefone = tabelaTelefone.FirstOrDefault(tel => tel.ClienteId == item.Id);
                var endereco = tabelaEnderecos.FirstOrDefault(end => end.ClienteId == item.Id);
                var fullClient = new ClienteCompleto();
                fullClient.Id = item.Id;
                fullClient.Nome = item.Nome;
                fullClient.CPF = item.CPF;
                fullClient.DataCadastro = item.DataCadastro;
                fullClient.DataAlteracao = item.DataAlteracao;
                fullClient.SaldoPontos = tabelaContas.FirstOrDefault(cont => cont.ClienteId == item.Id).SaldoPontos;
                fullClient.Cep = endereco.Cep;
                fullClient.Rua = endereco.Rua;
                fullClient.Numero = endereco.Numero;
                fullClient.NumeroTel = telefone.DDD.ToString() + telefone.Numero.ToString();


                listaClienteCompleto.Add(fullClient);
            }

            return listaClienteCompleto;
        }
        #endregion

        #region Buscar Clientes na Base
        public async Task<ClienteCompleto> BuscaCliente(int Id)
        {
            var ExistClient = await _context.Clientes.FirstOrDefaultAsync(clt => clt.Id == Id);
            if (ExistClient != null)
            {
                var ContasDoCliente = await _context.Contas.FirstOrDefaultAsync(cont => cont.ClienteId == ExistClient.Id);
                var EnderecosDoCliente = await _context.Enderecos.FirstOrDefaultAsync(end => end.ClienteId == ExistClient.Id);
                var TelefonesDoCliente = await _context.Telefones.FirstOrDefaultAsync(tel => tel.ClienteId == ExistClient.Id);

                var fullClient = new ClienteCompleto();
                fullClient.Id = ExistClient.Id;
                fullClient.Nome = ExistClient.Nome;
                fullClient.CPF = ExistClient.CPF;
                fullClient.DataCadastro = ExistClient.DataCadastro;
                fullClient.DataAlteracao = ExistClient.DataAlteracao;
                fullClient.SaldoPontos = ContasDoCliente.SaldoPontos;
                fullClient.Cep = EnderecosDoCliente.Cep;
                fullClient.Rua = EnderecosDoCliente.Rua;
                fullClient.Numero = EnderecosDoCliente.Numero;
                fullClient.NumeroTel = TelefonesDoCliente.DDD.ToString() + TelefonesDoCliente.Numero.ToString();

                return fullClient;
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region Atualizar Dados do Cliente 
        public async Task<RetornoPadrao> AtualizarCliente(CadCliente dadosAtualizados)
        {
            var rp = new RetornoPadrao();
            using (var transacao = await _context.Database.BeginTransactionAsync())
            {


                try
                {
                    dadosAtualizados.NumeroTel = dadosAtualizados.NumeroTel.Trim().Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "");
                    dadosAtualizados.CPF = dadosAtualizados.CPF.Replace("-", "").Replace(".", "");
                    dadosAtualizados.Cep = dadosAtualizados.Cep.Trim().Replace("-", "");
                    var validacao = ValidaDados(dadosAtualizados);


                    if (validacao.Status)
                    {
                        var dadosCliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == dadosAtualizados.Id);
                        var EnderecoCliente = await _context.Enderecos.FirstOrDefaultAsync(end => end.ClienteId == dadosCliente.Id);
                        var TelefoneCliente = await _context.Telefones.FirstOrDefaultAsync(tel => tel.ClienteId == dadosCliente.Id);

                        dadosCliente.Nome = dadosAtualizados.Nome;
                        dadosCliente.CPF = dadosAtualizados.CPF;
                        dadosCliente.DataAlteracao = DateTime.Now;

                        EnderecoCliente.Rua = dadosAtualizados.Rua;
                        EnderecoCliente.Cep = dadosAtualizados.Cep;
                        EnderecoCliente.Numero = dadosAtualizados.Numero;

                        TelefoneCliente.Numero = Convert.ToInt32(dadosAtualizados.NumeroTel.Substring(2));
                        TelefoneCliente.DDD = Convert.ToInt32(dadosAtualizados.NumeroTel.Substring(0, 2));

                        await _context.SaveChangesAsync();


                        rp.Status = true;
                        rp.Mensagem = "Atualizado com sucesso.";

                        transacao.Commit();
                    }
                    else
                    {
                        rp.Mensagem = validacao.Mensagem;
                        rp.Status = validacao.Status;
                    }

                }
                catch (Exception ex)
                {
                    rp.Status = false;
                    rp.Mensagem = ex.Message;

                    transacao.Rollback();
                }
            }
            return rp;
        }
        #endregion

        #region Excluir Cliente

        public async Task<RetornoPadrao> DeletarCliente(int id)
        {
            var rp = new RetornoPadrao();
            using (var transacao = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var clientRem = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
                    if (clientRem != null)
                    {


                        var contaRem = await _context.Contas.FirstOrDefaultAsync(c => c.ClienteId == clientRem.Id);

                        _context.Contas.Remove(contaRem);
                        await _context.SaveChangesAsync();

                        var endeRem = await _context.Enderecos.FirstOrDefaultAsync(e => e.ClienteId == clientRem.Id);
                        _context.Enderecos.Remove(endeRem);
                        await _context.SaveChangesAsync();

                        var telRem = await _context.Telefones.FirstOrDefaultAsync(e => e.ClienteId == clientRem.Id);
                        _context.Telefones.Remove(telRem);
                        await _context.SaveChangesAsync();

                        _context.Clientes.Remove(clientRem);
                        await _context.SaveChangesAsync();

                        rp.Status = true;
                        rp.Mensagem = "Deletado com sucesso.";
                        transacao.Commit();
                    }
                    else
                    {
                        rp.Mensagem = "Usuário não encontrado.";
                        rp.Status = false;
                    }

                }
                catch (Exception ex)
                {
                    rp.Status = false;
                    rp.Mensagem = ex.Message;
                    transacao.Rollback();
                }
            }
            return rp;
        }

        #endregion

        #region Adicionar Pontos ao cliente
        public async Task<RetornoPadrao> AdicionarPontos(int idCliente, int pontos)
        {
            var rp = new RetornoPadrao();
            if (pontos > 0)
            {
                try
                {
                    var existClient = await _context.Clientes.FirstOrDefaultAsync(g => g.Id == idCliente);
                    if (existClient != null)
                    {
                        var pontosCliente = await _context.Contas.FirstOrDefaultAsync(c => c.ClienteId == idCliente);

                        if (pontosCliente.SaldoPontos == 1000)
                        {
                            rp.Mensagem = "Cliente atingiu quantidade máxima de pontos.";
                            rp.Status = false;
                        }
                        else
                        {
                            var totalPontos = pontosCliente.SaldoPontos + pontos;
                            if (totalPontos > 1000)
                            {
                                rp.Mensagem = "Cliente não pode ter mais de  1000 pontos";
                                rp.Status = false;

                            }
                            else
                            {
                                pontosCliente.SaldoPontos = totalPontos;
                                await _context.SaveChangesAsync();
                                rp.Mensagem = "Pontos adicionados com sucesso. Saldo atual: " + pontosCliente.SaldoPontos;
                                rp.Status = true;
                            }
                        }
                    }
                    else
                    {
                        rp.Mensagem = "Cliente não encontrado.";
                        rp.Status = false;
                    }


                }
                catch (Exception ex)
                {
                    rp.Mensagem = ex.Message;
                    rp.Status = false;
                }
            }
            else
            {
                rp.Mensagem = "Não é possivel adicionar " + pontos + " pontos ao cliente";
                rp.Status = false;
            }


            return rp;
        }
        #endregion

        #region Retirar pontos do cliente
        public async Task<RetornoPadrao> RetiraPontos(int idCliente, int pontos)
        {
            var rp = new RetornoPadrao();
            if (pontos > 0)
            {
                try
                {

                    var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == idCliente);
                    if (cliente != null)
                    {

                        var pontosCliente = await _context.Contas.FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

                        pontosCliente.SaldoPontos = pontosCliente.SaldoPontos - pontos;
                        await _context.SaveChangesAsync();
                        rp.Mensagem = "Pontos retirados com sucesso. Saldo atual: " + pontosCliente.SaldoPontos;
                        rp.Status = true;
                    }
                    else
                    {
                        rp.Mensagem = "Cliente não encontrado.";
                        rp.Status = false;
                    }

                }
                catch (Exception ex)
                {
                    rp.Mensagem = ex.Message;
                    rp.Status = false;
                }

            }
            else
            {
                rp.Mensagem = "Não é possivel retirar " + pontos + " pontos do cliente";
                rp.Status = false;
            }
            return rp;
        }
        #endregion

        #region Valida Dados
        private RetornoPadrao ValidaDados(CadCliente client)
        {
            var rp = new RetornoPadrao();
            var existClient = new Cliente();
            var existTelefone = new Telefone();

            if (client.Id == 0)
            {
                existClient = _context.Clientes.FirstOrDefault(g => g.CPF == client.CPF);
                existTelefone = _context.Telefones.FirstOrDefault(g => g.Numero == Convert.ToInt32(client.NumeroTel.Substring(2)));

            }
            else
            {
                existClient = _context.Clientes.FirstOrDefault(g => g.CPF == client.CPF && g.Id != client.Id);
                existTelefone = _context.Telefones.FirstOrDefault(g => g.Numero == Convert.ToInt32(client.NumeroTel.Substring(2)) && g.Id != client.Id);
            }



            if (existClient != null)
            {
                rp.Mensagem = "Cliente já cadastrado com esse CPF.";
                rp.Status = false;
            }
            else if (existTelefone != null)
            {
                rp.Mensagem = "Cliente já cadastrado com esse Telefone.";
                rp.Status = false;
            }
            else if (client.CPF.Length != 11)
            {
                rp.Mensagem = "CPF invalido.";
                rp.Status = false;
            }
            else if (client.NumeroTel.Length != 11)
            {
                rp.Mensagem = "Telefone invalido. É Necessário informar o DDD e o Numero de telefone juntos.";
                rp.Status = false;
            }
            else
            {
                rp.Mensagem = "Tudo valido.";
                rp.Status = true;
            }

            return rp;
        }
        #endregion

    }
}
