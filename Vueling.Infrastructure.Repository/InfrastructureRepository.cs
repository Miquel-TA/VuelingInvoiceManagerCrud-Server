using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Vueling.Crosscutting.Models;

namespace Vueling.Infrastructure.Repository
{
    public class InfrastructureRepository
    {
        private readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        public List<Invoice> GetAllInvoices()
        {
            var invoicesDict = new Dictionary<int, Invoice>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                /*
                SqlCommand command = new SqlCommand(@"
                SELECT 
                    invoice.ID,
                    invoice.date,
                    invoice.order_number,
                    invoice.expiry_date,
                    invoice.subtotal_price,
                    invoice.discount,
                    invoice.tax_percentage,
                    invoice.total_price,
                    entityFrom.ID AS EntityFromId,
                    entityFrom.name AS EntityFromName,
                    entityFrom.address AS EntityFromAddress,
                    entityFrom.phone_number AS EntityFromPhone,
                    entityFrom.email AS EntityFromEmail,
                    entityTo.ID AS EntityToId,
                    entityTo.name AS EntityToName,
                    entityTo.address AS EntityToAddress,
                    entityTo.phone_number AS EntityToPhone,
                    entityTo.email AS EntityToEmail,
                    product.ID AS ProductId,
                    product.description AS ProductDescription
                FROM invoice 
                INNER JOIN entity entityFrom ON invoice.entity_from = entityFrom.ID
                INNER JOIN entity entityTo ON invoice.entity_to = entityTo.ID
                INNER JOIN invoice_product invoiceProduct ON invoice.ID = invoiceProduct.invoice_id
                INNER JOIN product ON invoiceProduct.product_id = product.ID",
                connection);
                */

                SqlCommand command = new SqlCommand("SELECT * FROM InvoiceView", connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var invoiceId = reader.GetInt32(0);
                        Invoice invoice;

                        // If the invoice is already in the dictionary, use that, otherwise create a new one
                        if (!invoicesDict.TryGetValue(invoiceId, out invoice))
                        {
                            invoice = new Invoice
                            {
                                ID = invoiceId,
                                Date = reader.GetDateTime(1),
                                OrderNumber = reader.GetString(2),
                                ExpiryDate = reader.GetDateTime(3),
                                SubtotalPrice = reader.GetDecimal(4),
                                Discount = reader.GetDecimal(5),
                                TaxPercentage = reader.GetDecimal(6),
                                TotalPrice = reader.GetDecimal(7),
                                EntityFrom = new Entity
                                {
                                    ID = reader.GetInt32(8),
                                    Name = reader.GetString(9),
                                    Address = reader.GetString(10),
                                    PhoneNumber = reader.GetString(11),
                                    Email = reader.GetString(12)
                                },
                                EntityTo = new Entity
                                {
                                    ID = reader.GetInt32(13),
                                    Name = reader.GetString(14),
                                    Address = reader.GetString(15),
                                    PhoneNumber = reader.GetString(16),
                                    Email = reader.GetString(17)
                                },
                                Products = new List<Product>()
                            };

                            invoicesDict.Add(invoiceId, invoice);
                        }

                        // Add the product to the existing invoice
                        invoice.Products.Add(new Product
                        {
                            ID = reader.GetInt32(18),
                            Description = reader.GetString(19)
                        });
                    }
                }
            }

            return invoicesDict.Values.ToList();
        }


        public int AddInvoice(Invoice invoice)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                // Add From Entity
                var entityFromCommand = new SqlCommand(@"INSERT INTO entity (name, address, phone_number, email)
                                                    VALUES (@EntityName, @EntityAddress, @EntityPhone, @EntityEmail);
                                                    SELECT SCOPE_IDENTITY();", connection);
                entityFromCommand.Parameters.AddWithValue("@EntityName", invoice.EntityFrom.Name);
                entityFromCommand.Parameters.AddWithValue("@EntityAddress", invoice.EntityFrom.Address);
                entityFromCommand.Parameters.AddWithValue("@EntityPhone", invoice.EntityFrom.PhoneNumber);
                entityFromCommand.Parameters.AddWithValue("@EntityEmail", invoice.EntityFrom.Email);
                var entityFromId = Convert.ToInt32(entityFromCommand.ExecuteScalar());

                // Add To Entity
                var entityToCommand = new SqlCommand(@"INSERT INTO entity (name, address, phone_number, email)
                                                VALUES (@EntityName, @EntityAddress, @EntityPhone, @EntityEmail);
                                                SELECT SCOPE_IDENTITY();;", connection);
                entityToCommand.Parameters.AddWithValue("@EntityName", invoice.EntityTo.Name);
                entityToCommand.Parameters.AddWithValue("@EntityAddress", invoice.EntityTo.Address);
                entityToCommand.Parameters.AddWithValue("@EntityPhone", invoice.EntityTo.PhoneNumber);
                entityToCommand.Parameters.AddWithValue("@EntityEmail", invoice.EntityTo.Email);
                var entityToId = Convert.ToInt32(entityToCommand.ExecuteScalar());

                // Add Invoice
                var invoiceCommand = new SqlCommand(@"INSERT INTO invoice (date, order_number, expiry_date, subtotal_price, 
                                                discount, tax_percentage, total_price, entity_from, entity_to)
                                                VALUES (@Date, @OrderNumber, @ExpiryDate, @SubtotalPrice, 
                                                @Discount, @TaxPercentage, @TotalPrice, @EntityFrom, @EntityTo);
                                                SELECT SCOPE_IDENTITY();", connection);
                invoiceCommand.Parameters.AddWithValue("@Date", invoice.Date);
                invoiceCommand.Parameters.AddWithValue("@OrderNumber", invoice.OrderNumber);
                invoiceCommand.Parameters.AddWithValue("@ExpiryDate", invoice.ExpiryDate);
                invoiceCommand.Parameters.AddWithValue("@SubtotalPrice", invoice.SubtotalPrice);
                invoiceCommand.Parameters.AddWithValue("@Discount", invoice.Discount);
                invoiceCommand.Parameters.AddWithValue("@TaxPercentage", invoice.TaxPercentage);
                invoiceCommand.Parameters.AddWithValue("@TotalPrice", invoice.TotalPrice);
                invoiceCommand.Parameters.AddWithValue("@EntityFrom", entityFromId);
                invoiceCommand.Parameters.AddWithValue("@EntityTo", entityToId);
                var invoiceId = Convert.ToInt32(invoiceCommand.ExecuteScalar());

                // Add Products
                foreach (var product in invoice.Products)
                {
                    var productCommand = new SqlCommand(@"INSERT INTO product (description)
                                                    VALUES (@ProductDescription);
                                                    SELECT SCOPE_IDENTITY();", connection);
                    productCommand.Parameters.AddWithValue("@ProductDescription", product.Description);
                    var productId = Convert.ToInt32(productCommand.ExecuteScalar());

                    // Add Invoice-Product
                    var invoiceProductCommand = new SqlCommand(@"INSERT INTO invoice_product (invoice_id, product_id)
                                                            VALUES (@InvoiceId, @ProductId);", connection);
                    invoiceProductCommand.Parameters.AddWithValue("@InvoiceId", invoiceId);
                    invoiceProductCommand.Parameters.AddWithValue("@ProductId", productId);
                    invoiceProductCommand.ExecuteNonQuery();
                }
                return invoiceId;
            }
        }


        public void DeleteInvoice(int invoiceId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(@"DELETE FROM invoice_product WHERE invoice_id = @InvoiceId;
                                                    DELETE FROM invoice WHERE ID = @InvoiceId;",
                                                        connection);
                command.Parameters.AddWithValue("@InvoiceId", invoiceId);

                command.ExecuteNonQuery();
            }
        }
    }
}
