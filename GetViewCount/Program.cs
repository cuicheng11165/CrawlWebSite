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
                    cmd.CommandText = "select * from DocStream";
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
                                    string read = array[5].Substring(start_read + 1, array[5].IndexOf(")") - start_read - 1);


                                    var start_comment = array[6].IndexOf("(");
                                    string comment = array[6].Substring(start_comment + 1, array[6].IndexOf(")") - start_comment - 1);
                                    InsertToResult(docid, Convert.ToInt32(read));
                                    continue;
                                }

                            }
                            if (html.IndexOf("post_view_count") > 0)
                            {
                                var blogstartid = html.IndexOf("cb_blogId=");
                                var pos = "cb_blogId=".Length;
                                if (blogstartid > 0)
                                {
                                    var end = html.IndexOf(",", blogstartid);
                                    var blogid = html.Substring(blogstartid + pos, end - blogstartid - pos);

                                    WebClient client = new WebClient();
                                    //client.BaseAddress =;
                                    var result = client.DownloadString("http://www.cnblogs.com/mvc/blog/ViewCountCommentCout.aspx?postId=" + blogid);
                                    InsertToResult(docid, Convert.ToInt32(result));
                                }

                            }



                        }
                    }

                }

            }


        }


        public static void InsertToResult(Guid docId, int viewcount)
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
                    cmd.CommandText = @"select * from [dbo].[ViewComment] where DocId=@v1";
                    cmd.Parameters.AddWithValue("@v1", docId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return;
                        }
                    }
                }

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
