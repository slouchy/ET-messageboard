using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;

namespace MessageBoard.Tools
{
    public class DBTool
    {
        /// <summary>
        /// 回傳值的 DB 操作
        /// <para>Item1(Bool) 狀態</para>
        /// <para>Item2(string) 訊息</para>
        /// <para>Item3(DataTable) 資料內容</para>
        /// </summary>
        /// <param name="conn">DB 連線字串</param>
        /// <param name="sql">sql 語法</param>
        /// <param name="ld">傳遞參數</param>
        /// <returns></returns>
        public Tuple<bool, string, DataTable> ExcuteDataTable(string conn, string sql, ListDictionary ld = null)
        {
            bool isSQLCorrect = false;
            string returnMsg = "";
            DataTable returnDT = new DataTable();
            using (SqlConnection connection = new SqlConnection(conn))
            {
                try
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(sql, connection))
                    {
                        adapter.SelectCommand.Parameters.Clear();
                        if (ld != null)
                        {
                            foreach (DictionaryEntry DE in ld)
                            {
                                adapter.SelectCommand.Parameters.AddWithValue((string)DE.Key, DE.Value);
                            }
                        }

                        adapter.Fill(returnDT);
                    }
                    returnMsg = "OK";
                    isSQLCorrect = true;
                }
                catch (Exception err)
                {
                    if (HttpContext.Current.IsDebuggingEnabled)
                    {
                        returnMsg = "ERROR:" + err.Message + "[換行]" + err.StackTrace.Replace("\r\n", "[換行]").Replace("\\", "/");
                    }
                    else
                    {
                        returnMsg = "資料庫發生錯誤";
                    }
                }
            }
            return Tuple.Create(isSQLCorrect, returnMsg, returnDT);
        }

        /// <summary>
        /// 不回傳值的 DB 操作
        /// <para>Item1(Bool)狀態，Item2(string)訊息</para>
        /// </summary>
        /// <param name="conn">DB 連線字串</param>
        /// <param name="sqlStr">SQL語法</param>
        /// <param name="ld">Parameter參數(option)</param>
        /// <returns></returns>
        public Tuple<bool, string> ExcuteSqlTransaction(string conn, string sqlStr, ListDictionary ld = null)
        {
            bool result = false;
            string sqlMsg = "";
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection connection = new SqlConnection(conn))
                    {
                        connection.Open();
                        using (SqlCommand cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = sqlStr;
                            //設置參數
                            cmd.Parameters.Clear();
                            if (ld != null)
                            {
                                foreach (DictionaryEntry DE in ld)
                                {
                                    cmd.Parameters.AddWithValue((string)DE.Key, (object)DE.Value ?? DBNull.Value);
                                }
                            }

                            //回傳訊息設置
                            cmd.ExecuteNonQuery();
                            //完成交易
                            scope.Complete();
                            result = true;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                if (HttpContext.Current.IsDebuggingEnabled)
                {
                    sqlMsg = err.Message;
                }
                else
                {
                    sqlMsg = "資料庫發生錯誤";
                }
            }

            return Tuple.Create(result, sqlMsg);
        }
    }
}