using Infrastrucutre.Core.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.ViewModels
{
    public class CreateShipmentViewModel
    {
        public List<CreateShipment> CreateShipments { get; set; }
        public CreateShipmentSummary CreateShipmentSummary { get; set; }
    }
}
