using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace CosmosDBAzure
{
    class Program
    {
        static string DatabaseName = "maindb"; // Nome do Banco
        static string CollectionName = "employee"; // Nome da coleção "Tabela"
        static DocumentClient dc;

        static string endpoint = "https://YOURCOSMOSDB.documents.azure.com:443/";
        static string key = "YOURKEY";
        static void Main(string[] args)
        {
            dc = new DocumentClient(new Uri(endpoint), key);
            Update("Raidy", "Machado"); // Inserção de Dados
            //Read(); // Consulta de dados

        }

        static void Create(string first, string last)
        {
            EmployeeEntity employeeEntity = new EmployeeEntity();
            employeeEntity.FirstName = first;
            employeeEntity.LastName = last;

            //Será salvo EM JSON no banco NoSQL
            var result = dc.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName),
                employeeEntity).GetAwaiter().GetResult();

            Console.WriteLine(result);
        }

        static void Read()
        {
            FeedOptions feedOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
            
            IQueryable<EmployeeEntity> query = dc.CreateDocumentQuery<EmployeeEntity>(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), feedOptions);

            query.ToList().ForEach(i =>
            {
                Console.WriteLine(i);
            });
        }

        static void Update(string first, string last)
        {
            FeedOptions feedOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };
           
            EmployeeEntity query = dc.CreateDocumentQuery<EmployeeEntity>(
            UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), feedOptions).FirstOrDefault(x => x.FirstName == "Raidy");

            query.FirstName = first;
            query.LastName = last;

            var result = dc.ReplaceDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName),
                query).GetAwaiter().GetResult();

            if (result.StatusCode == HttpStatusCode.OK)
                    Console.WriteLine("Updated");
        }
    }

    public class EmployeeEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
