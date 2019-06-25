using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wine.Client;
using Wine.Models;

namespace Wine.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly WineClient _wineClient;

        public HomeController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _wineClient = new WineClient(_clientFactory);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ListClientsOrderByTotalValue()
        {
            var clients = GetClientsWithHistoryShop();

            if (clients == null)
                return NotFound();

            var clientOrder = clients.OrderByDescending(c => c.ShopHistory.Sum(sH => sH.TotalValue));

            return PartialView("_PartialListClientsOrderByTotalValue", clientOrder);
        }

        public IActionResult ShowClientWithBiggestShopValue()
        {
            var clientList = new List<Client.Client>();
            var clients = GetClientsWithHistoryShop();

            if (clients == null)
                return NotFound();

            clients.ForEach(c =>
            {
                var historyList = new List<ShopHistory>();

                c.ShopHistory.ForEach(sH =>
                {
                    if (sH.Date.Contains("2016"))
                        historyList.Add(sH);
                });


                clientList.Add(new Client.Client
                {
                    Document = c.Document,
                    Id = c.Id,
                    Name = c.Name,
                    ShopHistory = historyList
                });
            });

            var clientWithBiggestShopValue = clientList.OrderByDescending(c => c.ShopHistory.Max(sH => sH.TotalValue)).FirstOrDefault();

            return PartialView("_PartialShowClientWithBiggestShopValue", clientWithBiggestShopValue);
        }

        public IActionResult ListFaithfulClients()
        {
            var clients = GetClientsWithHistoryShop();

            if (clients == null)
                return NotFound();

            var clientOrder = clients.OrderByDescending(c => c.ShopHistory.Count()).Take(3).ToList();

            return PartialView("_PartialListFaithfulClients", clientOrder);
        }

        public IActionResult RecommendWine(string document)
        {
            document = document.Remove(11, 1).Insert(11, ".");
            var client = GetClientsWithHistoryShop().Find(c => c.Document == document);

            if (client == null)
                return PartialView("_PartialRecommendWine", new ClientItemViewModel { NotFound = true });

            var viewModel = new ClientItemViewModel
            {
                Document = client.Document,
                Name = client.Name
            };

            var repeatedItems = client.ShopHistory.SelectMany(sH => sH.Items).GroupBy(i => i.Code)
                .Where(c => c.Count() > 1)
                .Select(y => new ItemModel { Code = y.Key, Counter = y.Count() });

            var reccomended = repeatedItems.OrderByDescending(c => c.Counter).ElementAtOrDefault(1);

            if (reccomended != null)
            {
                viewModel.Item = FindItemByCode(reccomended.Code);
            }
            else
            {
                repeatedItems = client.ShopHistory.SelectMany(sH => sH.Items).GroupBy(i => i.Product)
                .Where(c => c.Count() > 1)
                .Select(y => new ItemModel { Product = y.Key, Counter = y.Count() });

                reccomended = repeatedItems.OrderByDescending(c => c.Counter).ElementAtOrDefault(1);

                viewModel.Item = FindItemByProduct(reccomended.Product);
            }

            return PartialView("_PartialRecommendWine", viewModel);
        }

        private Items FindItemByCode(string code)
        {
            var historyShop = GetShopHistories();

            if (historyShop == null)
                return null;

            var item = historyShop.SelectMany(i => i.Items).Where(i => i.Code == code).FirstOrDefault();

            return item;
        }

        private Items FindItemByProduct(string product)
        {
            var historyShop = GetShopHistories();

            if (historyShop == null)
                return null;

            var item = historyShop.SelectMany(i => i.Items).Where(i => i.Product == product).FirstOrDefault();

            return item;
        }

        private List<Client.Client> GetClientsWithHistoryShop()
        {
            var clients = GetRefactoredClients();

            if (clients == null)
                return null;

            return clients.Select(c => new Client.Client
            {
                Document = c.Document,
                Id = c.Id,
                Name = c.Name,
                ShopHistory = GetShopHistories().Where(sH =>
                sH.Client.Substring(sH.Client.Length - Math.Min(2, sH.Client.Length)) ==
                c.Document.Substring(c.Document.Length - Math.Min(2, c.Document.Length)))
                        .ToList()
            }).ToList();
        }

        private List<Client.Client> GetRefactoredClients()
        {
            var clients = new List<Client.Client>();

            var clientsResult = _wineClient.GetClients().Result;

            if (clientsResult == null)
                return null;

            clientsResult.ForEach(c =>
            {
                var document = c.Document.Replace("-", ".");
                clients.Add(new Client.Client
                {
                    Id = c.Id,
                    Document = document,
                    Name = c.Name
                });
            });

            return clients;
        }

        private List<ShopHistory> GetShopHistories()
        {
            var historyShop = _wineClient.GetShopHistory();

            if (historyShop == null)
                return null;

            return historyShop.Result;
        }
    }
}
