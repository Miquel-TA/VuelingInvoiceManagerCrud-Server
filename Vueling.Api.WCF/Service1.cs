using System;
using System.Collections.Generic;
using Vueling.Business.Logic;
using Vueling.Crosscutting.Models;

namespace Vueling.Api.WCF
{
    public class Service1 : IService1
    {
        private readonly Logic BusinessLogic = new Logic();

        public List<Invoice> GetAllInvoices()
        {
            return BusinessLogic.GetAll();
        }

        public int InsertInvoice(Invoice invoice)
        {
            return BusinessLogic.Insert(invoice);
        }

        public bool DeleteInvoice(int invoiceToDelete)
        {
            return BusinessLogic.Delete(invoiceToDelete);
        }
    }
}
