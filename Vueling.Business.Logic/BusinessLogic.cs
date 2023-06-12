using System;
using System.Collections.Generic;
using System.Transactions;
using Vueling.Crosscutting.Models;
using Vueling.Infrastructure.Repository;

namespace Vueling.Business.Logic
{
    public class BusinessLogic
    {
        private readonly InfrastructureRepository Infrastructure = new InfrastructureRepository();

        public bool DeleteInvoice(int invoiceID)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Infrastructure.DeleteInvoice(invoiceID);
                    Logger.Log("Deleted Invoice: " + invoiceID.ToString(), Logger.Severity.Info);
                    scope.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, Logger.Severity.Error);
                Logger.Log(ex.StackTrace, Logger.Severity.Error);
                return false;
            }
        }

        public int AddInvoice(Invoice invoice)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    int invoiceId = Infrastructure.AddInvoice(invoice);
                    Logger.Log("Inserted Invoice: " + invoice.ToString(), Logger.Severity.Info);
                    scope.Complete();
                    return invoiceId;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, Logger.Severity.Error);
                Logger.Log(ex.StackTrace, Logger.Severity.Error);
                return -1;
            }
        }

        public List<Invoice> GetAllInvoices()
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<Invoice> invoicesReturned = Infrastructure.GetAllInvoices();
                    Logger.Log("Database returned " + invoicesReturned.Count + " Invoices.", Logger.Severity.Info);
                    scope.Complete();
                    return invoicesReturned;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, Logger.Severity.Error);
                Logger.Log(ex.StackTrace, Logger.Severity.Error);
                return new List<Invoice>();
            }
        }

    }
}
