using NSE.WebApp.MVC.Extensions;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NSE.WebApp.MVC.Services
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
            switch ((int)responseMessage.StatusCode)
            {
                case 401:
                case 403:
                case 404:
                case 500:
                    throw new CustomHttpRequestException(responseMessage.StatusCode);
                case 400:
                    return false;
            }

            responseMessage.EnsureSuccessStatusCode();

            return true;
        }

        protected async Task<TRetorno> ObterRetorno<TRetorno>(HttpResponseMessage response)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<TRetorno>(await response.Content.ReadAsStringAsync(), options);
        }
    }
}