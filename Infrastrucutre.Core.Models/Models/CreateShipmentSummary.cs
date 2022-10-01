using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.Models
{
    public class CreateShipmentSummary
    {
        [Key]
        public long Id { get; set; }
        public long ShipmentOrderId { get; set; }
        public string ShipmentId { get; set; }
        public string WarehouseRoot  { get; set; }
	    public string ShipmentTitle  { get; set; }
	    public string PortOfLoading  { get; set; }
	    public string PortOfArrival  { get; set; }
	    public string Currency  { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PaidDeposit { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public string ContainerSize { get; set; }
        public decimal TotalCVM { get; set; }
        public string FinalETD { get; set; }
        public DateTime? ETDDate { get; set; }
        public string ETDDateString { get; set; }
        public DateTime? ETADate { get; set; }
        public string ETADateString { get; set; }
        public string Comment { get; set; }
        public string CaseWorker { get; set; }
        public string BookingAgent { get; set; }
        public string ClearingAgent { get; set; }
        public string ShippingAgent { get; set; }
        public decimal FreightCharges { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
