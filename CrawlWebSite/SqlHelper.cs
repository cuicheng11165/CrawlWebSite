using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlWebSite
{
    class SqlHelper
    {
        const string sqlconnectionstring = @"Data Source=.\s2012;Initial Catalog=WebCrawler;Integrated Security=true";

        public static void InsertToFailedWeb(IEnumerable<string> urls)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlconnectionstring))
                {
                    conn.Open();

                    var tran = conn.BeginTransaction();
                    var cmd = conn.CreateCommand();
                    cmd.Transaction = tran;

                    foreach (var item in urls)
                    {
                        Uri uri = new Uri(item);
                        var domains = uri.Host.Split('.');
                        cmd.CommandText = "Insert into FailedWeb (PrimaryDomain,FailedUrl) values(@v1,@v2)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@v1", string.Format("{0}.{1}", domains[domains.Length - 2], domains[domains.Length - 1]));
                        cmd.Parameters.AddWithValue("@v2", item);
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();

                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static bool TrySelectFromFailedWeb(string url)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlconnectionstring))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    Uri uri = new Uri(url);
                    var domains = uri.Host.Split('.');
                    cmd.CommandText = "select * from SuccessfulWeb where PrimaryDomain=@v1 and Url=@v2";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@v1", string.Format("{0}.{1}", domains[domains.Length - 2], domains[domains.Length - 1]));
                    cmd.Parameters.AddWithValue("@v2", url);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static void InserToSuccessfulWeb(IDictionary<string, string> usedDictionary)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlconnectionstring))
                {
                    conn.Open();

                    var tran = conn.BeginTransaction();
                    var cmd = conn.CreateCommand();
                    cmd.Transaction = tran;

                    foreach (var item in usedDictionary)
                    {
                        Uri uri = new Uri(item.Key);
                        var domains = uri.Host.Split('.');
                        cmd.CommandText = "Insert into SuccessfulWeb (PrimaryDomain,Url,Keyword) values(@v1,@v2,@v3)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@v1", string.Format("{0}.{1}", domains[domains.Length - 2], domains[domains.Length - 1]));
                        cmd.Parameters.AddWithValue("@v2", item.Key);
                        cmd.Parameters.AddWithValue("@v3", item.Value);
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static bool TrySelectFromSuccessfulWeb(string url, out string result)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlconnectionstring))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    Uri uri = new Uri(url);
                    var domains = uri.Host.Split('.');
                    cmd.CommandText = "select * from SuccessfulWeb where PrimaryDomain=@v1 and Url=@v2";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@v1", string.Format("{0}.{1}", domains[domains.Length - 2], domains[domains.Length - 1]));
                    cmd.Parameters.AddWithValue("@v2", url);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        result = reader.GetString(2);
                        return true;
                    }
                    result = "";
                    return false;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static bool TryPopFromCacheWeb(out string result)
        {
            bool found = false;
            try
            {
                result = "";
                using (SqlConnection conn = new SqlConnection(sqlconnectionstring))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "select top 1 * from CacheWeb";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader.GetString(0);
                            found = true;
                        }
                    }
                    if (found)
                    {
                        var cmdDelete = conn.CreateCommand();
                        cmdDelete.CommandText = "delete from CacheWeb where Url =@v1";
                        cmdDelete.Parameters.AddWithValue("@v1", result);
                        cmdDelete.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return found;
        }

        internal static void InsertToCacheWeb(IEnumerable<string> urls)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(sqlconnectionstring))
                {
                    conn.Open();

                    var tran = conn.BeginTransaction();
                    var cmd = conn.CreateCommand();
                    cmd.Transaction = tran;

                    foreach (var item in urls)
                    {
                        Uri uri = new Uri(item);
                        var domains = uri.Host.Split('.');
                        cmd.CommandText = "Insert into CacheWeb (Url) values(@v1)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@v1", item);
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();

                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
