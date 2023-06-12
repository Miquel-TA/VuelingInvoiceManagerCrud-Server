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
        int AddInvoice(Invoice invoice);

        [OperationContract]
        bool RemoveInvoice(int invoiceToDelete);

        [OperationContract]
        List<Invoice> GetAllInvoices();
        
    }

}
