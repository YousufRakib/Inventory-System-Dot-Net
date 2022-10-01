using Infrastrucutre.Core.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.ViewModels
{
    public class ForecastingItemViewModel
    {
        public List<ForecastingItem> ForecastingItems { get; set; }
        public ForecastingItemSummary ForecastingItemSummary { get; set; }
    }
}
