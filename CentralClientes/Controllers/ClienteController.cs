
using CentralClientes.Models;
using CentralClientes.Negocio;
using CentralClientes.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CentralClientes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {

        public ClienteNegocios _negocio;
        public ClienteController(ClienteNegocios negocio)
        {
            _negocio = negocio;
        }

        #region Cadastrar Cliente
        [HttpPost("CadastrarCliente")]
        public async Task<RetornoPadrao> CadastrarCliente([FromBody] CadCliente client)
        {
            return await _negocio.CadastrarCliente(client);

        }
        #endregion

        #region Listar Clientes

        [HttpGet("ListarClientes")]
        public async Task<List<ClienteCompleto>> ListarClientes()
        {
            return await _negocio.ListarClientes();
        }
        #endregion

        #region Buscar Cliente por ID
        [HttpGet("BuscaClientePorId/{idCliente}")]
        public async Task<ClienteCompleto> BuscaClientePorId(int idCliente)
        {

            return await _negocio.BuscaCliente(idCliente);

        }
        #endregion

        #region Atualizar cliente
        [HttpPost("AtualizarCliente")]
        public async Task<RetornoPadrao> AtualizarCliente([FromBody] CadCliente clienteNovo)
        {

            return await _negocio.AtualizarCliente(clienteNovo);


        }
        #endregion

        #region Deletar cliente por id

        [HttpDelete("DeletarCliente/{idCliente}")]
        public async Task<RetornoPadrao> DeletarCliente(int idCliente)
        {

            return await _negocio.DeletarCliente(idCliente);

        }
        #endregion

        #region Adicionar Pontos ao Cliente

        [HttpGet("AdicionarPontos/{idCliente}/{pontos}")]
        public async Task<RetornoPadrao> AdicionarPontos(int idCliente, int pontos)
        {
            return await _negocio.AdicionarPontos(idCliente, pontos);
        }
        #endregion

        #region Retira Pontos do Cliente

        [HttpGet("RetiraPontos/{idCliente}/{pontos}")]
        public async Task<RetornoPadrao> RetiraPontos(int idCliente, int pontos)
        {
            return await _negocio.RetiraPontos(idCliente, pontos);
        }
        #endregion

    }
}