using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Vueling.Crosscutting.Models;

namespace Vueling.Infrastructure.Repository
{
    public class Repository
    {
        private readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;


        public int Insert(Invoice invoice)
        {
            return InsertInvoice(invoice);
        }
        public List<Invoice> GetAll()
        {
            return GetAllInvoices();
        }
        public bool Delete(int invoiceId)
        {
            return DeleteInvoice(invoiceId);
        }


        private int InsertInvoice(Invoice invoice)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var entityFromId = CreateEntityFrom(connection, invoice);

                var entityToId = CreateEntityTo(connection, invoice);

                var invoiceId = CreateInvoice(connection, invoice, entityFromId, entityToId);

                CreateProducts(connection, invoice, invoiceId);

                return invoiceId;
            }
        }

        private bool DeleteInvoice(int invoiceId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(@"DELETE FROM invoice_product WHERE invoice_id = @InvoiceId;
                                                    DELETE FROM invoice WHERE ID = @InvoiceId;",
                                                        connection);
                command.Parameters.AddWithValue("@InvoiceId", invoiceId);

                return (command.ExecuteNonQuery() > 0);
            }
        }

        private List<Invoice> GetAllInvoices()
        {
            var invoicesDict = new Dictionary<int, Invoice>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

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
                            invoice = GetInvoieFromReader(invoiceId, reader);

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

        private Invoice GetInvoieFromReader(int invoiceId, SqlDataReader reader)
        {
            return new Invoice
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
        }

        private void CreateProducts(SqlConnection connection, Invoice invoice, int invoiceId)
        {
            foreach (var product in invoice.Products)
            {
                // Add Products
                var productCommand = new SqlCommand(@"INSERT INTO product (description)
                                                    VALUES (@ProductDescription);
                                                    SELECT SCOPE_IDENTITY();", connection);
                productCommand.Parameters.AddWithValue("@ProductDescription", product.Description);
                var productId = Convert.ToInt32(productCommand.ExecuteScalar());

                // Add many-to-many Invoice-Product
                var invoiceProductCommand = new SqlCommand(@"INSERT INTO invoice_product (invoice_id, product_id)
                                                            VALUES (@InvoiceId, @ProductId);", connection);
                invoiceProductCommand.Parameters.AddWithValue("@InvoiceId", invoiceId);
                invoiceProductCommand.Parameters.AddWithValue("@ProductId", productId);
                invoiceProductCommand.ExecuteNonQuery();
            }
        }

        private int CreateInvoice(SqlConnection connection, Invoice invoice, int entityFromId, int entityToId)
        {
            var query = new SqlCommand(@"INSERT INTO invoice (date, order_number, expiry_date, subtotal_price, 
                                                discount, tax_percentage, total_price, entity_from, entity_to)
                                                VALUES (@Date, @OrderNumber, @ExpiryDate, @SubtotalPrice, 
                                                @Discount, @TaxPercentage, @TotalPrice, @EntityFrom, @EntityTo);
                                                SELECT SCOPE_IDENTITY();", connection);
            query.Parameters.AddWithValue("@Date", invoice.Date);
            query.Parameters.AddWithValue("@OrderNumber", invoice.OrderNumber);
            query.Parameters.AddWithValue("@ExpiryDate", invoice.ExpiryDate);
            query.Parameters.AddWithValue("@SubtotalPrice", invoice.SubtotalPrice);
            query.Parameters.AddWithValue("@Discount", invoice.Discount);
            query.Parameters.AddWithValue("@TaxPercentage", invoice.TaxPercentage);
            query.Parameters.AddWithValue("@TotalPrice", invoice.TotalPrice);
            query.Parameters.AddWithValue("@EntityFrom", entityFromId);
            query.Parameters.AddWithValue("@EntityTo", entityToId);
            return Convert.ToInt32(query.ExecuteScalar());
        }

        private int CreateEntityTo(SqlConnection connection, Invoice invoice)
        {
            var query = new SqlCommand(@"INSERT INTO entity (name, address, phone_number, email)
                                                VALUES (@EntityName, @EntityAddress, @EntityPhone, @EntityEmail);
                                                SELECT SCOPE_IDENTITY();;", connection);
            query.Parameters.AddWithValue("@EntityName", invoice.EntityTo.Name);
            query.Parameters.AddWithValue("@EntityAddress", invoice.EntityTo.Address);
            query.Parameters.AddWithValue("@EntityPhone", invoice.EntityTo.PhoneNumber);
            query.Parameters.AddWithValue("@EntityEmail", invoice.EntityTo.Email);
            return Convert.ToInt32(query.ExecuteScalar());
        }

        private int CreateEntityFrom(SqlConnection connection, Invoice invoice)
        {
            var query = new SqlCommand(@"INSERT INTO entity (name, address, phone_number, email)
                                                    VALUES (@EntityName, @EntityAddress, @EntityPhone, @EntityEmail);
                                                    SELECT SCOPE_IDENTITY();", connection);
            query.Parameters.AddWithValue("@EntityName", invoice.EntityFrom.Name);
            query.Parameters.AddWithValue("@EntityAddress", invoice.EntityFrom.Address);
            query.Parameters.AddWithValue("@EntityPhone", invoice.EntityFrom.PhoneNumber);
            query.Parameters.AddWithValue("@EntityEmail", invoice.EntityFrom.Email);
            return Convert.ToInt32(query.ExecuteScalar());
        }


    }
}
