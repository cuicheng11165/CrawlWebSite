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

        static string GetPrimaryDomain(string url)
        {
            Uri uri = new Uri(url);
            var domains = uri.Host.Split('.');
            string domain;
            if (domains.Length >= 3)
            {
                if (string.Equals(domains[domains.Length - 2], "com", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(domains[domains.Length - 2], "edu", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(domains[domains.Length - 2], "gov", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    domain = string.Format("{0}.{1}.{2}", domains[domains.Length - 3], domains[domains.Length - 2], domains[domains.Length - 1]);
                }
                else
                {
                    domain = string.Format("{0}.{1}", domains[domains.Length - 2], domains[domains.Length - 1]);
                }
            }
            else
            {
                domain = uri.Host;
            }
            return domain;
        }

        const string sqlconnectionstring = @"Data Source=.\s2012;Initial Catalog=WebCrawler;Integrated Security=true";

        public static void InsertToFailedWeb(string url, string errorMessage)
        {
            using (SqlConnection conn = new SqlConnection(sqlconnectionstring))
            {
                conn.Open();

                var tran = conn.BeginTransaction();
                var cmd = conn.CreateCommand();
                cmd.Transaction = tran;

                cmd.CommandText = @"begin tran
if not exists (select * from FailedWeb with (updlock,serializable) where PrimaryDomain=@v1 and FailedUrl = @v2)
begin
   insert into FailedWeb (PrimaryDomain,FailedUrl,ErrorMessage)
   values (@v1,@v2,@v3)
end
commit tran";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@v1", GetPrimaryDomain(url));
                cmd.Parameters.AddWithValue("@v2", url);
                cmd.Parameters.AddWithValue("@v3", errorMessage);
                cmd.ExecuteNonQuery();
                tran.Commit();

            }
        }

        internal static bool TrySelectFromFailedWeb(string url)
        {
            using (SqlConnection conn = new SqlConnection(sqlconnectionstring))
            {
                conn.Open();
                var cmd = conn.CreateCommand();


                cmd.CommandText = "select * from FailedWeb where PrimaryDomain=@v1 and FailedUrl=@v2";
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@v1", GetPrimaryDomain(url));
                cmd.Parameters.AddWithValue("@v2", url);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        internal static bool HasSuccessfulDomain(string newUrl)
        {
            using (SqlConnection conn = new SqlConnection(sqlconnectionstring))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                Uri uri = new Uri(newUrl);
                var domains = uri.Host.Split('.');
                cmd.CommandText = "select count(*) from SuccessfulWeb where PrimaryDomain=@v1";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@v1", string.Format("{0}.{1}", domains[domains.Length - 2], domains[domains.Length - 1]));
                var rowcount = (int)cmd.ExecuteScalar();

                return rowcount > 0;
            }
        }

        internal static void InserToSuccessfulWeb(string url, string keyword)
        {

            using (SqlConnection conn = new SqlConnection(sqlconnectionstring))
            {
                conn.Open();

                var tran = conn.BeginTransaction();
                var cmd = conn.CreateCommand();
                cmd.Transaction = tran;

                cmd.CommandText = @"begin tran
if not exists(select * from SuccessfulWeb with (updlock, serializable) where PrimaryDomain=@v1 and Url = @v2)
begin
   insert into SuccessfulWeb (PrimaryDomain, Url, Keyword)
   values(@v1, @v2, @v3)
end
commit tran";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@v1", GetPrimaryDomain(url));
                cmd.Parameters.AddWithValue("@v2", url);
                cmd.Parameters.AddWithValue("@v3", keyword);
                cmd.ExecuteNonQuery();
                tran.Commit();
            }

        }

        internal static bool TrySelectFromSuccessfulWeb(string url)
        {
            using (SqlConnection conn = new SqlConnection(sqlconnectionstring))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from SuccessfulWeb where PrimaryDomain=@v1 and Url=@v2";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@v1", GetPrimaryDomain(url));
                cmd.Parameters.AddWithValue("@v2", url);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    return true;
                }
                return false;
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
                            result = reader.GetString(1);
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
            using (SqlConnection conn = new SqlConnection(sqlconnectionstring))
            {
                conn.Open();

                var tran = conn.BeginTransaction();
                var cmd = conn.CreateCommand();
                cmd.Transaction = tran;

                foreach (var item in urls)
                {
                    cmd.CommandText = @"begin tran
if not exists(select * from CacheWeb with (updlock, serializable) where PrimaryDomain = @v1 and Url = @v2)
begin
   insert into CacheWeb (PrimaryDomain, Url)
   values(@v1, @v2)
end
commit tran";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@v1", GetPrimaryDomain(item));
                    cmd.Parameters.AddWithValue("@v2", item);
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();

            }
        }
    }
}
