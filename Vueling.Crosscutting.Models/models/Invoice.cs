using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Vueling.Crosscutting.Models
{
    [DataContract]
    public class Invoice
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string OrderNumber { get; set; }

        [DataMember]
        public DateTime ExpiryDate { get; set; }

        [DataMember]
        public decimal SubtotalPrice { get; set; }

        [DataMember]
        public decimal Discount { get; set; }

        [DataMember]
        public decimal TaxPercentage { get; set; }

        [DataMember]
        public decimal TotalPrice { get; set; }

        [DataMember]
        public Entity EntityFrom { get; set; }

        [DataMember]
        public Entity EntityTo { get; set; }

        [DataMember]
        public List<Product> Products { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"ID: {ID}");
            sb.AppendLine($"Date: {Date}");
            sb.AppendLine($"OrderNumber: {OrderNumber}");
            sb.AppendLine($"ExpiryDate: {ExpiryDate}");
            sb.AppendLine($"SubtotalPrice: {SubtotalPrice}");
            sb.AppendLine($"Discount: {Discount}");
            sb.AppendLine($"TaxPercentage: {TaxPercentage}");
            sb.AppendLine($"TotalPrice: {TotalPrice}");

            sb.AppendLine("EntityFrom:");
            sb.AppendLine(EntityFrom.ToString());

            sb.AppendLine("EntityTo:");
            sb.AppendLine(EntityTo.ToString());

            sb.AppendLine("Products:");
            foreach (var product in Products)
            {
                sb.AppendLine(product.ToString());
            }

            return sb.ToString();
        }
    }
}
