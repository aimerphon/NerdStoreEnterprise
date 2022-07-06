using Dapper;
using NSE.Pedidos.API.Application.DTO;
using NSE.Pedidos.Domain.Pedidos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Pedidos.API.Application.Queries
{
    public interface IPedidoQueries
    {
        Task<PedidoDTO> ObterUltimoPedido(Guid clienteId);
        Task<IEnumerable<PedidoDTO>> ObterListaPorClienteId(Guid clienteId);
        Task<PedidoDTO> ObterPedidosAutorizados();
    }

    public class PedidoQueries : IPedidoQueries
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoQueries(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<PedidoDTO> ObterUltimoPedido(Guid clienteId)
        {
            var dataInicio = DateTime.Now.AddMinutes(-5);
            var dataFim = DateTime.Now.AddMinutes(1);

            const string sql = @"SELECT
                                P.ID AS 'ProdutoId', P.CODIGO, P.VOUCHERUTILIZADO, P.DESCONTO, P.VALORTOTAL,P.PEDIDOSTATUS,
                                P.LOGRADOURO,P.NUMERO, P.BAIRRO, P.Endereco_Cep CEP, P.COMPLEMENTO, P.CIDADE, P.ESTADO,
                                PIT.ID AS 'ProdutoItemId',PIT.PRODUTONOME, PIT.QUANTIDADE, PIT.PRODUTOIMAGEM, PIT.VALORUNITARIO 
                                FROM NSE.PEDIDOS P 
                                INNER JOIN NSE.PEDIDOITEMS PIT ON P.ID = PIT.PEDIDOID 
                                WHERE P.CLIENTEID = @clienteId 
                                AND P.DATACADASTRO between @dataInicio and @dataFim
                                AND P.PEDIDOSTATUS = 1 
                                ORDER BY P.DATACADASTRO DESC";

            var pedido = await _pedidoRepository.ObterConexao()
                .QueryAsync<dynamic>(sql, new { clienteId , dataInicio, dataFim });

            return MapearPedido(pedido);
        }

        public async Task<IEnumerable<PedidoDTO>> ObterListaPorClienteId(Guid clienteId)
        {
            var pedidos = await _pedidoRepository.ObterListaPorClienteId(clienteId);

            return pedidos.Select(PedidoDTO.ParaPedidoDTO);
        }

        public async Task<PedidoDTO> ObterPedidosAutorizados()
        {
            const string sql = @"SELECT TOP 1
                                 P.ID as 'PedidoId', P.ID, P.CLIENTEID,
                                 PI.ID as 'PedidoItemId', PI.ID, PI.PRODUTOID, PI.QUANTIDADE
                                 FROM nse.PEDIDOS P
                                 INNER JOIN nse.PEDIDOITEMS PI ON P.ID = PI.PEDIDOID
                                 WHERE P.PEDIDOSTATUS = 1
                                 ORDER BY P.DATACADASTRO";

            // Utilizacao do lookup para manter o estado a cada ciclo de registro retornado
            var lookup = new Dictionary<Guid, PedidoDTO>();

            await _pedidoRepository.ObterConexao().QueryAsync<PedidoDTO, PedidoItemDTO, PedidoDTO>(sql,
                (p, pi) =>
                {
                    if (!lookup.TryGetValue(p.Id, out var pedidoDTO))
                        lookup.Add(p.Id, pedidoDTO = p);

                    pedidoDTO.PedidoItems ??= new List<PedidoItemDTO>();
                    pedidoDTO.PedidoItems.Add(pi);

                    return pedidoDTO;

                }, splitOn: "PedidoId,PedidoItemId");

            // Obtendo dados o lookup
            return lookup.Values.OrderBy(p => p.Data).FirstOrDefault();
        }

        private PedidoDTO MapearPedido(dynamic result)
        {
            var pedido = new PedidoDTO
            {
                Codigo = result?.First()?.CODIGO,
                Status = result?.First()?.PEDIDOSTATUS,
                ValorTotal = result?.First()?.VALORTOTAL,
                Desconto = result?.First()?.DESCONTO,
                VoucherUtilizado = result?.First()?.VOUCHERUTILIZADO,

                PedidoItems = new List<PedidoItemDTO>(),
                Endereco = new EnderecoDTO
                {
                    Logradouro = result?.First()?.LOGRADOURO,
                    Bairro = result?.First()?.BAIRRO,
                    Cep = result?.First()?.CEP,
                    Cidade = result?.First()?.CIDADE,
                    Complemento = result?.First()?.COMPLEMENTO,
                    Estado = result?.First()?.ESTADO,
                    Numero = result?.First()?.NUMERO
                }
            };

            foreach (var item in result)
            {
                var pedidoItem = new PedidoItemDTO
                {
                    Nome = item.PRODUTONOME,
                    Valor = item.VALORUNITARIO,
                    Quantidade = item.QUANTIDADE,
                    Imagem = item.PRODUTOIMAGEM
                };

                pedido.PedidoItems.Add(pedidoItem);
            }

            return pedido;
        }
    }
}