using System;
using System.Data.SqlClient;

string connectionString = "your-database-connection-string";
SqlInjectionPrevention prevention = new SqlInjectionPrevention(connectionString);
prevention.GetUserDetails("testuser");

// The SqlInjectionPrevention class is designed to prevent SQL injection attacks
// by using parameterized queries. This ensures that user input is treated as
// data and not executable code, which helps to protect the database from
// malicious SQL injection attacks.
public class SqlInjectionPrevention
{
    private string connectionString;

    public SqlInjectionPrevention(string connectionString)
    {
        this.connectionString = connectionString;
    }

    // This method retrieves user details from the database based on the provided
    // username. It uses a parameterized query to prevent SQL injection.
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
