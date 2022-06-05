﻿using Microsoft.Extensions.Options;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
{
    public class CarrinhoService : Service, ICarrinhoService
    {
        private readonly HttpClient _httpClient;

        public CarrinhoService(HttpClient httpClient,
            IOptions<AppSettings> settings)
        {
            httpClient.BaseAddress = new Uri(settings.Value.CarrinhoUrl);

            _httpClient = httpClient;
        }

        public async Task<CarrinhoViewModel> ObterCarrinho()
        {
            var response = await _httpClient.GetAsync("/carrinho/");

            TratarErrosResponse(response);

            return await ObterRetorno<CarrinhoViewModel>(response);
        }

        public async Task<ResponseResult> AdicionarItemCarrinho(ItemProdutoViewModel produto)
        {
            var itemConteudo = ObterConteudo(produto);

            var response = await _httpClient.PostAsync("/carrinho/", itemConteudo);

            if (!TratarErrosResponse(response)) return await ObterRetorno<ResponseResult>(response);

            return RetornarOk();
        }

        public async Task<ResponseResult> AtualizarItemCarrinho(Guid produtoId, ItemProdutoViewModel produto)
        {
            var itemConteudo = ObterConteudo(produto);

            var response = await _httpClient.PutAsync($"/carrinho/{produtoId}", itemConteudo);

            if (!TratarErrosResponse(response)) return await ObterRetorno<ResponseResult>(response);

            return RetornarOk();
        }

        public async Task<ResponseResult> RemoverItemCarrinho(Guid produtoId)
        {
            var response = await _httpClient.DeleteAsync($"/carrinho/{produtoId}");

            if (!TratarErrosResponse(response)) return await ObterRetorno<ResponseResult>(response);

            return RetornarOk();
        }
    }
}