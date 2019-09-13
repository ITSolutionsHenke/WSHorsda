using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ZPF.AT;

namespace ZPF.SQL
{
   public class SQLServerEngine : IDBEngine
   {
      /// <summary>
      /// MaxUploadSize in MB
      /// </summary>
      public static uint MaxUploadSize = 10;

      SqlConnection _Connection = null;

      public SQLServerEngine()
      {
         Log.Write("", $"SQLServerEngine");
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      DBType IDBEngine.GetType()
      {
         return DBType.SQLServer;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public DbConnection Open(DBSQLViewModel sender, DBType dbType, string ConnectionString)
      {
         _Connection = new SqlConnection(ConnectionString);
         _Connection.Open();

         return _Connection;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public bool Close(DBSQLViewModel sender)
      {
         try
         {
            Log.Write("SQLServerEngine", $"Close {_Connection.ConnectionString}");

            _Connection.Close();
            _Connection.Dispose();
            _Connection = null;
         }
         catch (Exception ex)
         {
            Log.Write("SQLServerEngine", $"Close {ex.Message}");

            _Connection = null;

            return false;
         };

         return true;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public DbCommand NewCommand(DBSQLViewModel sender, string SQL)
      {
         DbCommand Result = null;
         Result = new SqlCommand(SQL, _Connection);
         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public DbDataReader NewReader(DBSQLViewModel sender, DbCommand dbCommand)
      {
         DbDataReader Result = null;
         Result = (dbCommand as SqlCommand).ExecuteReader();
         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public DbDataAdapter NewDataAdapter(DBSQLViewModel sender, string SQL)
      {
         DbDataAdapter Result = null;
         Result = new SqlDataAdapter(SQL, _Connection);
         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public DbDataAdapter NewDataAdapter(DBSQLViewModel sender, DbCommand dbCommand)
      {
         DbDataAdapter Result = null;
         Result = new SqlDataAdapter(dbCommand as SqlCommand);
         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public DataTable QuickQueryDataTable(DBSQLViewModel sender, string SQL, bool NoSchema = true)
      {
         if (!NoSchema)
         {
            throw new NotSupportedException();
         };

         return ConvertDataReader2DataTable(sender, SQL);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private DataTable ConvertDataReader2DataTable(DBSQLViewModel sender, string SQL)
      {
         if (false)
         {
            // Bug PK: QuickQueryViewUpdate
            try
            {
               // Create a new data adapter based on the specified query.
               using (DbDataAdapter dataAdapter = NewDataAdapter(sender, SQL))
               {
                  if (sender.CurrentTransaction != null)
                  {
                     dataAdapter.SelectCommand.Transaction = sender.CurrentTransaction as SqlTransaction;
                  };

                  // Create a command builder to generate SQL update, insert, and
                  // delete commands based on selectCommand. These are used to
                  // update the database.

                  // Populate a new data table and bind it to the BindingSource.
                  using (DataTable table = new DataTable())
                  {
                     table.Locale = System.Globalization.CultureInfo.InvariantCulture;

                     try
                     {
                        dataAdapter.Fill(table);
                        dataAdapter.Dispose();
                     }
                     catch (Exception ex)
                     {
                        sender.LastError = ex.Message;
                        sender.LastException = ex;

                        return null;
                     };

                     return table;
                  };
               };
            }
            catch (Exception ex)
            {
               sender.LastError = ex.Message;
               sender.LastException = ex;

               if (Debugger.IsAttached)
               {
                  Debugger.Break();
               };
               return null;
            };
         }
         else
         {
            try
            {
               DbCommand cmd = NewCommand(sender, SQL);

               if (sender.CurrentTransaction != null)
               {
                  cmd.Transaction = sender.CurrentTransaction as SqlTransaction;
               };

               DbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
               DataTable dtSchema = dr.GetSchemaTable();
               DataTable dt = new DataTable();

               // You can also use an ArrayList instead of List<> 
               List<DataColumn> listCols = new List<DataColumn>();

               if (dtSchema != null)
               {
                  foreach (DataRow drow in dtSchema.Rows)
                  {
                     string columnName = System.Convert.ToString(drow["ColumnName"]);
                     DataColumn column = new DataColumn(columnName, (Type)(drow["DataType"]));
                     column.Unique = (bool)drow["IsUnique"];
                     column.AllowDBNull = (bool)drow["AllowDBNull"];
                     column.AutoIncrement = (bool)drow["IsAutoIncrement"];

                     if (listCols.Where(x => x.ColumnName == columnName).Count() > 0)
                     {
                        column.ColumnName = column.ColumnName + listCols.Where(x => x.ColumnName == columnName).Count();
                     };

                     listCols.Add(column);
                     dt.Columns.Add(column);
                  };
               };

               // Read rows from DataReader and populate the DataTable 

               while (dr.Read())
               {
                  DataRow dataRow = dt.NewRow();
                  for (int i = 0; i < listCols.Count; i++)
                  {
                     dataRow[((DataColumn)listCols[i])] = dr[i];
                  }

                  dt.Rows.Add(dataRow);
               }

               return dt;
            }
            catch (Exception ex)
            {
               // handle error 
               Debug.WriteLine(ex.Message);
            };

            return null;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public DbTransaction Transaction(DBSQLViewModel sender, [CallerMemberName] string CallerName = null)
      {
         if (DB_SQL.DoTransactions)
         {
            return _Connection.BeginTransaction();
         }
         else
         {
            return null;
         };
      }

      public bool Commit(DBSQLViewModel sender, DbTransaction dbTransaction, [CallerMemberName] string CallerName = null)
      {
         if (DB_SQL.DoTransactions)
         {
            var t = (dbTransaction as SqlTransaction);
            t.Commit();

            return true;
         }
         else
         {
            return true;
         };
      }

      public bool Rollback(DBSQLViewModel sender, DbTransaction dbTransaction, [CallerMemberName] string CallerName = null)
      {
         if (DB_SQL.DoTransactions)
         {
            var t = (dbTransaction as SqlTransaction);

            try
            {
               t.Rollback();
            }
            catch (Exception ex)
            {
               Debug.WriteLine(ex.Message);
            };

            return true;
         }
         else
         {
            return true;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -   

      public bool UploadFile(DBSQLViewModel sender, byte[] buffer, string SQL)
      {
         bool Result = true;

         try
         {
            // Initialize SqlCommand object for insert.
            using (SqlCommand cmd = new SqlCommand(SQL, _Connection))
            {
               // We are passing the image byte data as SQL parameters.
               //cmd.Parameters.Add(new SqlParameter("@DocData", (object)buffer));

               SqlParameter dbParameter = new System.Data.SqlClient.SqlParameter("@DocData", SqlDbType.VarBinary);
               dbParameter.Size = (int)MaxUploadSize * 1024 * 1024;  // sets the maximum size, in bytes, of the data within the column.
               dbParameter.Value = (object)buffer;
               cmd.Parameters.Add(dbParameter);

               // Open connection and execute insert query.
               Result = (cmd.ExecuteNonQuery() == 1);
            };
         }
         catch (Exception ex)
         {
            sender.LastError = ex.Message;
            sender.LastException = ex;
            sender.LastQuery = SQL;

            Result = false;
         };

         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -   
   }
}
