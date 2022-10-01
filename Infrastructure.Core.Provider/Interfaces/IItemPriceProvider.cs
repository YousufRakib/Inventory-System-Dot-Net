using Infrastrucutre.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Core.Provider.Interfaces
{
    public interface IItemPriceProvider
    {
        bool AddItemPrice(ItemPrice s);
        List<ItemPriceViewModel> GetItemPrices();
        List<ItemPriceViewModel> GetItemPricesHistory(int id, out int rowCount, int jtStartIndex = 0, int jtPageSize = 0);
        bool RemoveItemPrice(int id);
        ItemPrice GetItemPriceByID(int ItemPriceID);
        bool UpdateItemPrice(ItemPrice s);
    }
}
