using System;
using System.Data;
using System.Data.SqlClient;

namespace AIOSICT011.DataAccess
{
    // Data access tier helper: one place for the connection string and the common ADO.NET calls (Lesson 9).
    public static class Db
    {
        public const string DatabaseName = "AIOSICT011DB2";

        // Change both connection strings if you use SQL Express instead of LocalDB.
        public static string ConnectionString =
            @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=AIOSICT011DB2;Integrated Security=True";

        private static string MasterConnectionString =
            @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";

        // Returns a new SQL connection
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        // SELECT query: returns the rows in a DataTable (Lesson 11)
        public static DataTable GetData(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // Query that returns one value (COUNT, SUM, SCOPE_IDENTITY ...)
        public static object GetValue(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    con.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        // INSERT, UPDATE, DELETE: returns how many rows were changed
        public static int Execute(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // Creates the database and tables the first time the program runs.
        public static void EnsureDatabaseCreated()
        {
            using (SqlConnection master = new SqlConnection(MasterConnectionString))
            {
                master.Open();

                SqlCommand check = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name = @n", master);
                check.Parameters.AddWithValue("@n", DatabaseName);
                int count = Convert.ToInt32(check.ExecuteScalar());
                if (count > 0) return;   // already created

                SqlCommand create = new SqlCommand("CREATE DATABASE " + DatabaseName, master);
                create.ExecuteNonQuery();

                try
                {
                    using (SqlConnection con = GetConnection())
                    {
                        con.Open();
                        SqlCommand build = new SqlCommand(DbSchema.Script, con);
                        build.ExecuteNonQuery();
                    }
                }
                catch
                {
                    // If building the tables failed, remove the empty database so the next run can try again.
                    SqlConnection.ClearAllPools();
                    SqlCommand drop = new SqlCommand("DROP DATABASE " + DatabaseName, master);
                    drop.ExecuteNonQuery();
                    throw;
                }
            }
        }
    }
}
