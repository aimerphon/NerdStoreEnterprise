using NSE.Core.Communication;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSE.Bff.Compras.Services
{
    public abstract class Service
    {
        private readonly JsonSerializerOptions _options;

        protected Service()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected StringContent ObterConteudo(object dado)
        {
            return new StringContent(
                JsonSerializer.Serialize(dado),
                Encoding.UTF8,
                "application/json"
                );
        }
        protected bool TratarErrosResponse(HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == HttpStatusCode.BadRequest) return false;

            responseMessage.EnsureSuccessStatusCode();

            return true;
        }

        protected async Task<TRetorno> ObterRetorno<TRetorno>(HttpResponseMessage response)
        {

            return JsonSerializer.Deserialize<TRetorno>(await response.Content.ReadAsStringAsync(), _options);
        }

        protected ResponseResult RetornoOk()
        {
            return new ResponseResult();
        }
    }
}