using Infrastrucutre.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.DataAccess.Interfaces
{
    public interface IItemPriceRepository
    {
        bool AddItemPrice(ItemPrice s);
        List<ItemPriceViewModel> GetItemPrices();
        bool RemoveItemPrice(int id);
        ItemPrice GetItemPriceByID(int ItemPriceID);
        bool UpdateItemPrice(ItemPrice s);
        List<ItemPriceViewModel> GetItemPricesHistory(int id, out int rowCount, int jtStartIndex = 0, int jtPageSize = 0);
    }
}
