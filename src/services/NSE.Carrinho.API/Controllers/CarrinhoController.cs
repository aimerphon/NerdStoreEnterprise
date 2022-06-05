using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSE.Carrinho.API.Data;
using NSE.Carrinho.API.Models;
using NSE.WebApi.Core.Controllers;
using NSE.WebApi.Core.Usuario;
using System;
using System.Threading.Tasks;

namespace NSE.Carrinho.API.Controllers
{
    [Authorize]
    public class CarrinhoController : MainController
    {
        private readonly IAspNetUser _user;
        private readonly CarrinhoContext _context;

        public CarrinhoController(IAspNetUser user, 
            CarrinhoContext context)
        {
            _user = user;
            _context = context;
        }

        [HttpGet("carrinho")]
        public async Task<CarrinhoCliente> ObterCarrinho()
        {
            return await ObterCarrinhoCliente() ?? new CarrinhoCliente();
        }

        [HttpPost("carrinho")]
        public async Task<ActionResult> AdicionarItemCarrinho(CarrinhoItem carrinhoItem)
        {
            var carrinho = await ObterCarrinhoCliente();

            if (carrinho == null)
                ManipularNovoCarrinho(carrinhoItem);
            else
                ManipularCarrinhoExistente(carrinho, carrinhoItem);

            if (!OperacaoValida()) return CustomResponse();

            var result = await _context.SaveChangesAsync();
            if (result <= 0) AdicionarErroProcessamento("Não foi possível persistir os dados no banco");

            return CustomResponse();
        }

        private void ManipularNovoCarrinho(CarrinhoItem carrinhoItem)
        {
            var carrinhoCliente = new CarrinhoCliente(_user.GetUserId());

            carrinhoCliente.AdicionarItem(carrinhoItem);

            _context.CarrinhoClientes.Add(carrinhoCliente);
        }

        private void ManipularCarrinhoExistente(CarrinhoCliente carrinhoCliente, CarrinhoItem item)
        {
            var produtoItemExistente = carrinhoCliente.CarrinhoItemExistente(item);

            carrinhoCliente.AdicionarItem(item);

            if (produtoItemExistente)
            {
                _context.CarrinhoItems.Update(carrinhoCliente.ObterPorProdutoId(item.ProdutoId));
            }
            else
            {
                _context.CarrinhoItems.Add(item);
            }

            _context.CarrinhoClientes.Update(carrinhoCliente);
        }

        [HttpPut("carrinho/{produtoId:guid}")]
        public async Task<ActionResult> AtualizarItemCarrinho(Guid produtoId, CarrinhoItem carrinhoItem)
        {
            return CustomResponse();
        }

        [HttpDelete("carrinho/{produtoId:guid}")]
        public async Task<ActionResult> RemoverItemCarrinho(Guid produtoId)
        {
            return CustomResponse();
        }

        private async Task<CarrinhoCliente> ObterCarrinhoCliente()
        {
            return await _context.CarrinhoClientes
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.ClienteId == _user.GetUserId());
        }
    }
}