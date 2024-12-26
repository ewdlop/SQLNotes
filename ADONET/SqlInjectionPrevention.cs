using System;
using System.Data.SqlClient;

string connectionString = "your-database-connection-string";
SqlInjectionPrevention prevention = new SqlInjectionPrevention(connectionString);
prevention.GetUserDetails("testuser");


public class SqlInjectionPrevention
{
    private string connectionString;

    public SqlInjectionPrevention(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void GetUserDetails(string username)
    {
        string query = "SELECT * FROM Users WHERE Username = @Username";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            // Use parameterized query to prevent SQL injection
            command.Parameters.AddWithValue("@Username", username);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"Username: {reader["Username"]}, Email: {reader["Email"]}");
            }
        }
    }
}
