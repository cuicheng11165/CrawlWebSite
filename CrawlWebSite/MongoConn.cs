using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;



namespace CrawlWebSite
{
    //public class MongoConn : IDisposable
    //{
    //    private MongoClient client;
    //    private IMongoDatabase database;

    //    public MongoConn()
    //    {
    //        client = new MongoClient();
    //        database = client.GetDatabase("spider");
    //    }

    //    public bool HasDescription(string host)
    //    {
    //        var coll = database.GetCollection<BsonDocument>("UrlSummary");

    //        var document = coll.FindSync(Builders<BsonDocument>.Filter.Eq("host", host));

    //        while (document != null && document.MoveNext())
    //        {
    //            var res = document.Current.Count();
    //            return res > 0;
    //        }
    //        return false;
    //    }

    //    public void UpsertUrlToHost(string tableName, string url, int value)
    //    {
    //        //Console.WriteLine("Add url to host: {0}", url);
    //        var collection = database.GetCollection<BsonDocument>(tableName);
    //        var builder = Builders<BsonDocument>.Filter;
    //        var filter = builder.Eq("url", url);
    //        var findValue = collection.FindSync(filter);
    //        if (findValue != null && findValue.MoveNext() && findValue.Current.Count() > 0)
    //        {
    //            return;
    //        }
    //        var update = Builders<BsonDocument>.Update.Set("value", value);
    //        collection.UpdateOne(filter, update, new UpdateOptions() { IsUpsert = true });
    //    }

    //    public IEnumerable<string> MoveNextTable()
    //    {
    //        var cursor = database.ListCollections(new ListCollectionsOptions());

    //        while (cursor.MoveNext())
    //        {
    //            foreach (var cur in cursor.Current)
    //            {
    //                yield return cur.GetValue("name").AsString;
    //            }
    //        }
    //    }

    //    public string MoveNextRecord(string tableName)
    //    {
    //        var coll = database.GetCollection<BsonDocument>(tableName);

    //        var document = coll.FindOneAndUpdate<BsonDocument>(Builders<BsonDocument>.Filter.Eq("value", 0), Builders<BsonDocument>.Update.Set("value", 1));
    //        if (document == null)
    //        {
    //            return null;
    //        }
    //        var res = document.GetValue("url").AsString;

    //        return res;
    //    }

    //    public string PopUrl(string tableName)
    //    {
    //        Console.WriteLine("Pop url: {0}", tableName);
    //        var collection = database.GetCollection<BsonDocument>(tableName);
    //        var document = collection.FindOneAndDelete<BsonDocument>(Builders<BsonDocument>.Filter.Empty);
    //        return document.GetValue("Url").AsString;
    //    }

    //    public void UpsertUrl(string host, string description)
    //    {
    //        var collection = database.GetCollection<BsonDocument>("UrlSummary");
    //        var builder = Builders<BsonDocument>.Filter;
    //        var filter = builder.Eq("host", host);
    //        var update = Builders<BsonDocument>.Update.Set("description", description);

    //        collection.UpdateOne(filter, update, new UpdateOptions() { IsUpsert = true });
    //    }

    //    public void Dispose()
    //    {

    //    }
    //}

    public class MongoConn : IDisposable
    {
        private string connection = "";

        public MongoConn()
        {

        }

        public bool HasDescription(string host)
        {
            using (var conn = new SqlConnection(SQLQuery.connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = SQLQuery.HasDescription;
                cmd.Parameters.AddWithValue("@Url", host);
                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    return true;
                }
            }
            return false;
        }

        public void UpsertUrlToHost(string tableName, string url, string title, string keywords, int status)
        {
            using (var conn = new SqlConnection(SQLQuery.connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(SQLQuery.InsertUrl, tableName);
                cmd.Parameters.AddWithValue("@Url", url);
                cmd.Parameters.AddWithValue("@TableName", tableName);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Keywords", keywords);
                cmd.Parameters.AddWithValue("@Status", status);
                var result = cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<string> MoveNextTable()
        {
            using (var conn = new SqlConnection(SQLQuery.connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = SQLQuery.SelectTables;
                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    yield return result[0].ToString();
                }
            }
        }

        public static HashSet<string> TableNames = new HashSet<string>();

        public string MoveNextRecord(string tableName)
        {
            using (var conn = new SqlConnection(SQLQuery.connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(SQLQuery.SelectUrl, tableName);
                cmd.Parameters.AddWithValue("@TableName", tableName);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return result.ToString();
                }
                return "";
            }
        }

        public string MoveNextDescription()
        {
            using (var conn = new SqlConnection(SQLQuery.connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(SQLQuery.SelectDescription);
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return "http://" + result.ToString();
                }
                return "";
            }
        }

        //public string PopUrl(string tableName)
        //{
        //    Console.WriteLine("Pop url: {0}", tableName);
        //    var collection = database.GetCollection<BsonDocument>(tableName);
        //    var document = collection.FindOneAndDelete<BsonDocument>(Builders<BsonDocument>.Filter.Empty);
        //    return document.GetValue("Url").AsString;
        //}

        public void UpsertUrlDescription(string host, string title, string description)
        {
            using (var conn = new SqlConnection(SQLQuery.connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = SQLQuery.UpsertDescription;
                cmd.Parameters.AddWithValue("@Url", host);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Keywords", description);
                cmd.Parameters.AddWithValue("@Status", 1);
                var result = cmd.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {

        }

        public void CreateTable(string tableName)
        {
            using (var conn = new SqlConnection(SQLQuery.connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = string.Format(SQLQuery.CreateTable);
                cmd.Parameters.AddWithValue("@TableName", tableName);

                var result = cmd.ExecuteNonQuery();
            }
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
