using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wine.Client
{
    public class WineClient
    {
        private readonly IHttpClientFactory _clientFactory;
        public WineClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<List<Client>> GetClients()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "598b16291100004705515ec5");

            var client = _clientFactory.CreateClient("wine");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<List<Client>>();

            return null;
        }

        public async Task<List<ShopHistory>> GetShopHistory()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "598b16861100004905515ec7");

            var client = _clientFactory.CreateClient("wine");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<List<ShopHistory>>();

            return null;
        }

    }

    public class Client
    {
        public int Id { get; set; }
        [JsonProperty(PropertyName = "nome")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "cpf")]
        public string Document { get; set; }
        public List<ShopHistory> ShopHistory { get; set; }
    }

    public class ShopHistory
    {
        [JsonProperty(PropertyName = "codigo")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "data")]
        public string Date { get; set; }
        [JsonProperty(PropertyName = "cliente")]
        public string Client { get; set; }
        [JsonProperty(PropertyName = "itens")]
        public List<Items> Items { get; set; }
        [JsonProperty(PropertyName = "valorTotal")]
        public double TotalValue { get; set; }
    }

    public class Items
    {
        [JsonProperty(PropertyName = "produto")]
        public string Product { get; set; }
        [JsonProperty(PropertyName = "variedade")]
        public string Variety { get; set; }
        [JsonProperty(PropertyName = "pais")]
        public string Country { get; set; }
        [JsonProperty(PropertyName = "categoria")]
        public string Category { get; set; }
        [JsonProperty(PropertyName = "safra")]
        public string Crop  { get; set; }
        [JsonProperty(PropertyName = "preco")]
        public double Price { get; set; }
        [JsonProperty(PropertyName = "codigo")]
        public string Code { get; set; }

    }

    public class ClientsShopHistory
    {
        public Client Client { get; set; }
        public ShopHistory ShopHistory { get; set; }
    }
}
