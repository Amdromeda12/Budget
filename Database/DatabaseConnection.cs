using System;
using System.Data.SQLite;

namespace Database.DatabaseConnection;
internal class DatabaseConnection
{
    //KOLLA GENOM DETTA OCH GÖR OM DET, DETTA ÄR TEST KOD
    public static void InitializeDatabase()
    {
        string connectionString = "Data Source=Budget.db;Version=3;";
    
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = @"
                CREATE TABLE IF NOT EXISTS Year (
                    Id INTEGER PRIMARY KEY,
                    Year_Number INTEGER UNIQUE
                );

                CREATE TABLE IF NOT EXISTS Month (
                    Id INTEGER PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Year_Id INTEGER,
                    Car INTEGER,
                    House INTEGER,
                    UNIQUE(Name, Year_Id),
                    FOREIGN KEY (Year_Id) REFERENCES Year(Id) ON DELETE CASCADE
                );";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public static void InsertMonth(string name, int Year_Id, int car, int house)
    {
        string connectionString = "Data Source=budget.db;Version=3;";
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "INSERT INTO month (Name, Year_Id, Car, House) VALUES (@name, @Year_Id, @car, @house)";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@Year_Id", Year_Id);
                cmd.Parameters.AddWithValue("@car", car);
                cmd.Parameters.AddWithValue("@house", house);
                cmd.ExecuteNonQuery();
            }
        }
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