using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;



namespace CrawlWebSite
{
    public class MongoConn : IDisposable
    {
        private MongoClient client;
        private IMongoDatabase database;

        public MongoConn()
        {
            client = new MongoClient();
            database = client.GetDatabase("spider");
        }

        internal bool HasDescription(string host)
        {
            var coll = database.GetCollection<BsonDocument>("UrlSummary");
            var document = coll.FindSync(Builders<BsonDocument>.Filter.Eq("url", host));
            while (document != null && document.MoveNext())
            {
                var res = document.Current.Count();
                return res > 0;
            }
            return false;
        }

        public void UpsertUrlToHost(string tableName, string url, int value)
        {
            Console.WriteLine("Add url to host: {0}", url);
            var collection = database.GetCollection<BsonDocument>(tableName);
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("url", url);
            var update = Builders<BsonDocument>.Update.Set("value", value);
            collection.UpdateOne(filter, update, new UpdateOptions() { IsUpsert = true });
        }

        public IEnumerable<string> MoveNextTable()
        {
            var cursor = database.ListCollections(new ListCollectionsOptions());

            while (cursor.MoveNext())
            {
                foreach (var cur in cursor.Current)
                {
                    yield return cur.GetValue("name").AsString;
                }
            }
        }

        public string MoveNextRecord(string tableName)
        {
            var coll = database.GetCollection<BsonDocument>(tableName);

            var document = coll.FindOneAndUpdate<BsonDocument>(Builders<BsonDocument>.Filter.Eq("value", 0), Builders<BsonDocument>.Update.Set("value", 1));
            if (document == null)
            {
                return null;
            }
            return document.GetValue("url").AsString;
        }

        public string PopUrl(string tableName)
        {
            Console.WriteLine("Pop url: {0}", tableName);
            var collection = database.GetCollection<BsonDocument>(tableName);
            var document = collection.FindOneAndDelete<BsonDocument>(Builders<BsonDocument>.Filter.Empty);
            return document.GetValue("Url").AsString;
        }

        public void UpsertUrl(string host, string description)
        {
            var collection = database.GetCollection<BsonDocument>("UrlSummary");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("host", host);
            var update = Builders<BsonDocument>.Update.Set("host", host).Set("description", description);

            collection.UpdateOne(filter, update, new UpdateOptions() { IsUpsert = true });
        }

        public void Dispose()
        {

        }
    }

    public class Web
    {
        public string Url { set; get; }

        /// <summary>
        /// 0 ->Need to crawl , 1 Success, 2 fail
        /// </summary>
        public int Status { set; get; }
    }

}
