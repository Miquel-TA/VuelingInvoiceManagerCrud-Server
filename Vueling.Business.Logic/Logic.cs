using System;
using System.Collections.Generic;
using System.Transactions;
using Vueling.Crosscutting.Models;
using Vueling.Infrastructure.Repository;

namespace Vueling.Business.Logic
{
    public class Logic
    {
        private readonly Repository Repository = new Repository();

        public bool Delete(int invoiceID)
        {
            bool deleted = false;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    deleted = Repository.Delete(invoiceID);
                    if (deleted)
                    {
                        Log4Net.Info("Deleted Invoice: " + invoiceID.ToString());
                        scope.Complete();
                    }
                    else
                    {
                        Log4Net.Info("Couldn't delete invoice: " + invoiceID.ToString());
                    }
                    return deleted;
                }
            }
            catch (Exception ex)
            {
                Log4Net.Error(ex.Message);
                Log4Net.Error(ex.StackTrace);
                return deleted;
            }
        }

        public int Insert(Invoice invoice)
        {
            int invoiceId = 0;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    invoiceId = Repository.Insert(invoice);
                    Log4Net.Info("Inserted Invoice: " + invoice.ToString());
                    scope.Complete();
                    return invoiceId;
                }
            }
            catch (Exception ex)
            {
                Log4Net.Error(ex.Message);
                Log4Net.Error(ex.StackTrace);
                return invoiceId;
            }
        }

        public List<Invoice> GetAll()
        {
            List<Invoice> invoices = new List<Invoice>();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    invoices = Repository.GetAll();
                    Log4Net.Info("Database returned " + invoices.Count + " Invoices.");
                    scope.Complete();
                    return invoices;
                }
            }
            catch (Exception ex)
            {
                Log4Net.Error(ex.Message);
                Log4Net.Error(ex.StackTrace);
                return invoices;
            }
        }

    }
}
