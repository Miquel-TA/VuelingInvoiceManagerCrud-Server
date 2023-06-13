using System;
using System.Collections.Generic;
using System.ServiceModel;
using Vueling.Crosscutting.Models;

namespace Vueling.Api.WCF
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        int InsertInvoice(Invoice invoice);

        [OperationContract]
        bool DeleteInvoice(int invoiceToDelete);

        [OperationContract]
        List<Invoice> GetAllInvoices();
        
    }

}
