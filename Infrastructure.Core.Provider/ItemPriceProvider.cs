using Infrastructure.Core.Provider.Interfaces;
using Infrastrucutre.Core.DataAccess.Interfaces;
using Infrastrucutre.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Core.Provider
{
    public class ItemPriceProvider : IItemPriceProvider
    {
        IItemPriceRepository _ItemPriceRepository;

        public ItemPriceProvider(IItemPriceRepository _ItemPriceRepository)
        {
            this._ItemPriceRepository = _ItemPriceRepository;
        }

        public List<ItemPriceViewModel> GetItemPrices()
        {
            return _ItemPriceRepository.GetItemPrices();
        }

        public List<ItemPriceViewModel> GetItemPricesHistory(int id, out int rowCount, int jtStartIndex = 0, int jtPageSize = 0)
        {
            return _ItemPriceRepository.GetItemPricesHistory(id,out rowCount,jtStartIndex,jtPageSize);
        }

        public bool RemoveItemPrice(int id)
        {
            return _ItemPriceRepository.RemoveItemPrice(id);
        }

        public bool AddItemPrice(ItemPrice s)
        {
            s.Price = ((s.Vat / 100) * s.OriginalPrice) + s.OriginalPrice;
            return _ItemPriceRepository.AddItemPrice(s);
        }


        public ItemPrice GetItemPriceByID(int ItemPriceID)
        {
            return _ItemPriceRepository.GetItemPriceByID(ItemPriceID);
        }


        public bool UpdateItemPrice(ItemPrice s)
        {
            s.Price = ((s.Vat / 100) * s.OriginalPrice) + s.OriginalPrice;
            return _ItemPriceRepository.UpdateItemPrice(s);
        }
    }
}
