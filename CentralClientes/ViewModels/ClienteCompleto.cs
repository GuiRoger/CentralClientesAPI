namespace CentralClientes.ViewModels
{
    public class ClienteCompleto
    {

        public int? Id { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string NumeroTel { get; set; }
        public int SaldoPontos { get; set; }
        public string Rua { get; set; }
        public int Numero { get; set; }
        public string Cep { get; set; }


    }
}
