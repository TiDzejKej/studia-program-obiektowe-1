using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using System.Data;
using System.Windows;
using System.Reflection;
using System.Diagnostics;

public class DbConnection
{
    private MySqlConnection connection;
    private string host = "localhost";
    private string database = "my_friends";
    private string uid = "root";
    private string password = "password";

    private static DbConnection singleInstance;
    private static readonly object singleLock = new object();


    public static DbConnection Instance
    {
        get
        {
            lock (singleLock)
            {
                if (singleInstance == null)
                {
                    singleInstance = new DbConnection();
                }
                return singleInstance;
            }
        }
    }
    public DbConnection()
    {
        Setup();
    }

    private void Setup()
    {
        string connectionString = $"SERVER={host};DATABASE={database};UID={uid};PASSWORD={password};";
        connection = new MySqlConnection(connectionString);
    }

    public MySqlConnection OpenConnection()
    {
        try
        {
            connection.Open();
            return connection;
        }
        catch (MySqlException err)
        {
            MessageBox.Show(err.Message);
            throw err;
        }
    }

    public void CloseConnection()
    {
        try
        {
            connection.Close();
        }
        catch (MySqlException err)
        {
            MessageBox.Show(err.Message);
            throw err;
        }
    }

    public void ExecuteInsert(object obj, string tableName)
    {
        PropertyInfo[] properties = obj.GetType().GetProperties();
        List<string> columnNames = new List<string>();
        List<string> values = new List<string>();
        List <MySqlParameter> parameters = new List<MySqlParameter>();
        foreach (var property in properties)
        { 
            columnNames.Add(property.Name);
            values.Add($"@{property.Name}");
            parameters.Add(new MySqlParameter($"@{property.Name}", property.GetValue(obj) ?? DBNull.Value));
        }

        string query = $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", values)});";
  
        Instance.OpenConnection();
        using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                foreach (dynamic param in parameters)
                {
                    command.Parameters.Add(param);
                }

                command.ExecuteNonQuery();
            }
        
        Instance.CloseConnection();
    }

    public void ExecuteDelete(int id, string tableName)
    {
        string query = $"DELETE FROM {tableName} WHERE Id = @Id;";
        Instance.OpenConnection();

        using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }

        Instance.CloseConnection();
    }

    public List<T> ExecuteGetAll<T>(string tableName) where T : new()
    {
        Instance.OpenConnection();
        var items = new List<T>();
        var properties = typeof(T).GetProperties();

        string query = $"SELECT * FROM {tableName};";

       
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new T();
                        foreach (var property in properties)
                        {
                            var value = reader[property.Name];
                            if (value != DBNull.Value)
                            {
                                property.SetValue(item, value);
                            }
                        }
                        items.Add(item);
                    }
                }
            }
   

        Instance.CloseConnection();
        return items;
    }

}
