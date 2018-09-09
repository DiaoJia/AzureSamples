using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq.Expressions;

namespace CosmosDBSample02
{
    public static class DocumentDBRepository<T> where T : class
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static readonly string CollectionId = ConfigurationManager.AppSettings["collection"];
        private static readonly string Endpoint = ConfigurationManager.AppSettings["endpoint"];
        private static readonly string AuthKey = ConfigurationManager.AppSettings["authKey"];

        private static DocumentClient client;

        public static async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T,bool>> predicate)
        {
            var collectionLink = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
            var feedOptions = new FeedOptions { MaxItemCount = -1,EnableCrossPartitionQuery = true };
            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(collectionLink, feedOptions)
                .Where(predicate)
                .AsDocumentQuery();
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());                
            }

            return results;
        }


        public static void Initialize()
        {
            client = new DocumentClient(new Uri(Endpoint), AuthKey);
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }                
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId,CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection()
                        {
                            Id = CollectionId
                        },
                        new RequestOptions { OfferThroughput = 400 }
                        );
                }
                throw;
            }
        }

    }
}
