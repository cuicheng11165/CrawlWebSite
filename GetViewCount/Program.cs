using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GetViewCount
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = ".\\s2012";
            builder.InitialCatalog = "aliyundbsource";
            builder.IntegratedSecurity = true;
            using (SqlConnection sql = new SqlConnection(builder.ToString()))
            {
                sql.Open();

                using (var cmd = sql.CreateCommand())
                {
                    cmd.CommandText = @"select top 20000 DocStream.DocId,DocumentStream 
  from DocStream (nolock)
  left join ViewComment (nolock)
  on
  DocStream.DocId = ViewComment.DocId
   where ViewCount is null";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var docid = reader.GetGuid(0);
                            var html = reader.GetString(1);

                            var doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(html);

                            var node = doc.DocumentNode.SelectSingleNode("//p[@class='postfoot']/text()");

                            if (node != null)
                            {


                                var array = node.InnerText.Trim().Split();


                                if (array.Length == 7)
                                {
                                    string name = array[4];
                                    var start_read = array[5].IndexOf("(");
                                    if (start_read > 0)
                                    {
                                        string read = array[5].Substring(start_read + 1, array[5].IndexOf(")") - start_read - 1);

                                        string readelement = null;
                                        if (array[6].IndexOf("(") > 0)
                                        {
                                            readelement = array[6];
                                        }
                                        else
                                        {
                                            readelement = array.FirstOrDefault(ele => ele.IndexOf("(") > 0);
                                        }
                                        if (readelement != null)
                                        {
                                            var start_comment = readelement.IndexOf("(");
                                            string comment = readelement.Substring(start_comment + 1, readelement.IndexOf(")") - start_comment - 1);
                                            InsertToResult(docid, Convert.ToInt32(read));
                                            continue;
                                        }
                                    }
                                }

                            }
                            if (html.IndexOf("post_view_count") > 0)
                            {
                                var blogstartid = html.IndexOf("cb_blogId=");
                                var pos = "cb_blogId=".Length;
                                int result = 0;
                                if (blogstartid > 0)
                                {
                                    var end = html.IndexOf(",", blogstartid);
                                    var blogid = html.Substring(blogstartid + pos, end - blogstartid - pos);

                                    WebClient client = new WebClient();
                                    //client.BaseAddress =;
                                    result = Convert.ToInt32(client.DownloadString("http://www.cnblogs.com/mvc/blog/ViewCountCommentCout.aspx?postId=" + blogid));
                                    if (result != 0)
                                    {
                                        InsertToResult(docid, result);
                                        continue;
                                    }

                                }

                                var entryId = html.IndexOf("cb_entryId=");
                                var posentry = "cb_entryId=".Length;

                                if (entryId > 0)
                                {
                                    var endentry = html.IndexOf(",", entryId);
                                    var blogid = html.Substring(entryId + posentry, endentry - entryId - posentry);

                                    WebClient client = new WebClient();
                                    //client.BaseAddress =;
                                    var resultentry = Convert.ToInt32(client.DownloadString("http://www.cnblogs.com/mvc/blog/ViewCountCommentCout.aspx?postId=" + blogid));
                                    if (resultentry != 0)
                                    {
                                        InsertToResult(docid, resultentry);
                                        continue;
                                    }

                                }

                            }

                            if (html.IndexOf("阅读:") > 0)
                            {
                                var blogstartid = html.IndexOf("阅读:");
                                var pos = "阅读:".Length;
                                int result = 0;
                                if (blogstartid > 0)
                                {
                                    var end = html.IndexOf(" ", blogstartid);
                                    var blogid = html.Substring(blogstartid + pos, end - blogstartid - pos);

                                    result = Convert.ToInt32(blogid);
                                    InsertToResult(docid, result);
                                    continue;

                                }
                            }

                            InsertToResult(docid, 0);


                        }
                    }

                }

            }


        }


        public static void InsertToResult(Guid docId, int viewcount)
        {
            Console.WriteLine("Viewcount {0}", viewcount);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = ".\\s2012";
            builder.InitialCatalog = "aliyundbsource";
            builder.IntegratedSecurity = true;
            using (SqlConnection sql = new SqlConnection(builder.ToString()))
            {
                sql.Open();

                bool needupdate = false;

                using (var cmd = sql.CreateCommand())
                {
                    cmd.CommandText = @"select * from [dbo].[ViewComment] where DocId=@v1";
                    cmd.Parameters.AddWithValue("@v1", docId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var existingCount = reader.GetInt32(1);
                            if (existingCount > 0)
                            {
                                return;
                            }
                            needupdate = true;
                        }
                    }
                }

                if (needupdate)
                {
                    using (var cmd = sql.CreateCommand())
                    {
                        cmd.CommandText = @"Update [dbo].[ViewComment] set ViewCount=@v2 where DocId=@v1 ";
                        cmd.Parameters.AddWithValue("@v1", docId);
                        cmd.Parameters.AddWithValue("@v2", viewcount);
                        cmd.ExecuteNonQuery();
                    }

                    Console.WriteLine("Update {0} to count {1}", docId, viewcount);
                }
                else
                {
                    using (var cmd = sql.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO [dbo].[ViewComment] ([DocId],[ViewCount],[CommentCount],[Tag]) VALUES (@v1,@v2,@v3,@v4)";
                        cmd.Parameters.AddWithValue("@v1", docId);
                        cmd.Parameters.AddWithValue("@v2", viewcount);
                        cmd.Parameters.AddWithValue("@v3", 0);
                        cmd.Parameters.AddWithValue("@v4", "");
                        cmd.ExecuteNonQuery();
                    }
                }

            }
        }
    }
}
