using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coiner.Business.Heplers
{
    public class NotifyHub : Hub
    {
        // Groups Mehtod

        public void JoinGroup(string grouptName)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, grouptName);
        }

        public void LeaveGroup(string grouptName)
        {
            Groups.RemoveFromGroupAsync(Context.ConnectionId, grouptName);
        }

        public void SendProductToGroup(Product product)
        {
            Clients.Group(product.ProductName).SendAsync("GetProductFromGroup", product);
        }

        public void ProductOfferSellGroup(ProductOffer[] productOfferSell, string productName)
        {
            var Object = new { productOfferSell, productName };
            Clients.Group(productName).SendAsync("GetProductOfferSellGroup", Object);
        }

        public void ProductOfferBuyGroup(ProductOffer[] productOfferBuy, string productName)
        {
            var Object = new { productOfferBuy, productName };
            Clients.Group(productName).SendAsync("GetProductOfferBuyGroup", Object);
        }

        public async Task SendStatsToGroup(List<decimal> stats, string productName)
        {
            await Clients.Group(productName).SendAsync("RefreshProductStats", stats);
        }

        public async Task RefreshProductsListingPage(Product product)
        {
            await Clients.Group("ProductsListingPage").SendAsync("OnRefreshProductsListingPage", product);
        }
    }
}
