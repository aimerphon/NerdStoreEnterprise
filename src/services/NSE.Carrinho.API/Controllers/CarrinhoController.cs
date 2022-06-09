using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSE.Carrinho.API.Data;
using NSE.Carrinho.API.Models;
using NSE.WebApi.Core.Controllers;
using NSE.WebApi.Core.Usuario;
using System;
using System.Linq;
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

            return await PersistirDados();
        }

        [HttpPut("carrinho/{produtoId:guid}")]
        public async Task<ActionResult> AtualizarItemCarrinho(Guid produtoId, CarrinhoItem carrinhoItem)
        {
            var carrinho = await ObterCarrinhoCliente();
            var itemCarrinho = await ObterItemCarrinhoValidado(produtoId, carrinho, carrinhoItem);

            if (itemCarrinho == null) return CustomResponse();

            carrinho.AtualizarUnidades(itemCarrinho, carrinhoItem.Quantidade);

            if (!OperacaoValida()) return CustomResponse();

            _context.CarrinhoItems.Update(itemCarrinho);
            _context.CarrinhoClientes.Update(carrinho);

            return await PersistirDados();
        }

        [HttpDelete("carrinho/{produtoId:guid}")]
        public async Task<ActionResult> RemoverItemCarrinho(Guid produtoId)
        {
            var carrinho = await ObterCarrinhoCliente();
            var itemCarrinho = await ObterItemCarrinhoValidado(produtoId, carrinho);

            if (itemCarrinho == null) return CustomResponse();

            ValidarCarrinho(carrinho);
            if (!OperacaoValida()) return CustomResponse();

            carrinho.RemoverItem(itemCarrinho);

            _context.CarrinhoItems.Remove(itemCarrinho);
            _context.CarrinhoClientes.Update(carrinho);

            return await PersistirDados();
        }

        private async Task<CarrinhoCliente> ObterCarrinhoCliente()
        {
            return await _context.CarrinhoClientes
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.ClienteId == _user.ObterUserId());
        }

        private void ManipularNovoCarrinho(CarrinhoItem carrinhoItem)
        {
            var carrinhoCliente = new CarrinhoCliente(_user.ObterUserId());

            carrinhoCliente.AdicionarItem(carrinhoItem);

            ValidarCarrinho(carrinhoCliente);
            _context.CarrinhoClientes.Add(carrinhoCliente);
        }

        private void ManipularCarrinhoExistente(CarrinhoCliente carrinhoCliente, CarrinhoItem item)
        {
            var produtoItemExistente = carrinhoCliente.CarrinhoItemExistente(item);

            carrinhoCliente.AdicionarItem(item);

            ValidarCarrinho(carrinhoCliente);

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

        private async Task<CarrinhoItem> ObterItemCarrinhoValidado(Guid produtoId,
            CarrinhoCliente carrinho, 
            CarrinhoItem item = null)
        {
            if (item != null && produtoId != item.ProdutoId)
            {
                AdicionarErroProcessamento("O item não corresponde ao informado");
                return null;
            }

            if (carrinho == null)
            {
                AdicionarErroProcessamento("Carrinho não encontrado");
                return null;
            }

            var itemCarrinho = await _context.CarrinhoItems
                .FirstOrDefaultAsync(i => i.CarrinhoId == carrinho.Id && i.ProdutoId == produtoId);

            if (itemCarrinho == null || !carrinho.CarrinhoItemExistente(itemCarrinho))
            {
                AdicionarErroProcessamento("O item não está no carrinho");
                return null;
            }

            return itemCarrinho;
        }

        private async Task<ActionResult> PersistirDados()
        {
            var result = await _context.SaveChangesAsync();
            if (result <= 0) AdicionarErroProcessamento("Não foi possível persistir os dados no banco");

            return CustomResponse();
        }

        private bool ValidarCarrinho(CarrinhoCliente carrinho)
        {
            if (carrinho.EhValido()) return true;

            carrinho.ValidationResult.Errors.ToList().ForEach(e => AdicionarErroProcessamento(e.ErrorMessage));

            return false;
        }
    }
}