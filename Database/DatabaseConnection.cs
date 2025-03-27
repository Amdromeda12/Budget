using System;
using System.Data.SQLite;

namespace Database.DatabaseConnection;
internal class DatabaseConnection
{

/*
    CREATE TABLE IF NOT EXISTS Year (
        Id INTEGER PRIMARY KEY,
        Year_Number INTEGER UNIQUE  -- Ensures we don't have duplicate years
    );

    CREATE TABLE IF NOT EXISTS Month (
        Id INTEGER PRIMARY KEY,                     1
        Name TEXT,                                  Jan
        Year_Id INTEGER,  -- Links month to a specific year         1991
        Car INTEGER,                        5
        House INTEGER,                      5
        UNIQUE(Name, Year_Id),  -- Ensures each month is unique within a year
        FOREIGN KEY (Year_Id) REFERENCES Year(Id) ON DELETE CASCADE
    );
*/



    //                                          KOLLA GENOM DETTA OCH GÖR OM DET, DETTA ÄR TEST KOD
    public static void InitializeDatabase()
    {
        string connectionString = "Data Source=Budget.db;Version=3;";
    
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "CREATE TABLE IF NOT EXISTS Month (Id INTEGER PRIMARY KEY, Name TEXT, Car INTEGER, House INTEGER)";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                command.ExecuteNonQuery();
            }
            Console.WriteLine("Database and table created successfully!");
        }
    }
    public static void InsertMonth(string name, int car, int house)
    {
        string connectionString = "Data Source=budget.db;Version=3;";
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "INSERT INTO month (Name, Car, House) VALUES (@name, @car, @house)";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@car", car);
                cmd.Parameters.AddWithValue("@house", house);
                cmd.ExecuteNonQuery();
            }
        }
        Console.WriteLine($"Inserted: {name}, Car: {car}, House: {house}");
    }

    public static void DisplayMonths()
    {
        string connectionString = "Data Source=budget.db;Version=3;";
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT * FROM month";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine("\nDatabase Records:");
                while (reader.Read())
                {
                    Console.WriteLine($"Name: {reader["Name"]}, Car: {reader["Car"]}, House: {reader["House"]}");
                }
            }
        }

    }

    public static Month GetMonthData(string monthName)
    {
        string connectionString = "Data Source=budget.db;Version=3;";
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT * FROM month WHERE Name = @name";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", monthName);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Month(
                            reader["Name"].ToString(),
                            Convert.ToInt32(reader["Car"]),
                            Convert.ToInt32(reader["House"])
                        );
                    }
                }
            }
        }
        return null; // No data found
    }
    //
}