using System;
using System.Collections.Generic;
using Vueling.Business.Logic;
using Vueling.Crosscutting.Models;

namespace Vueling.Api.WCF
{
    public class Service1 : IService1
    {
        private readonly BusinessLogic BusinessLogic = new BusinessLogic();

        public List<Invoice> GetAllInvoices()
        {
            return BusinessLogic.GetAllInvoices();
        }

        public int AddInvoice(Invoice invoice)
        {
            return BusinessLogic.AddInvoice(invoice);
        }

        public bool RemoveInvoice(int invoiceToDelete)
        {
            return BusinessLogic.DeleteInvoice(invoiceToDelete);
        }
    }
}
