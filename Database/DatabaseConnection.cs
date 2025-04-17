using System;
using System.Data.SQLite;
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace Database.DatabaseConnection;
public class DatabaseConnection
{
    public static List<string> Months { get; set; } = new List<string>();
    public static List<Month> Comparelist1 { get; set; } = new List<Month>();           //Gör 2 listor som man kan sen jämnföra med varann för att få färger
    public static List<Month> Comparelist2 { get; set; } = new List<Month>();

    //Kolla genom dessa och gör dom bättre
    public static void InitializeDatabase() //Funkar
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
                    year_Id INTEGER,
                    Car INTEGER,
                    House INTEGER,
                    UNIQUE(Name, year_Id),
                    FOREIGN KEY (year_Id) REFERENCES Year(Id) ON DELETE CASCADE
                );";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public static void InsertMonth(string name, int year_Id, int car, int house)
    {
        string connectionString = "Data Source=budget.db;Version=3;";
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "INSERT INTO month (Name, year_Id, Car, House) VALUES (@name, @year_Id, @car, @house)";    //TODO: Fixa denna
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@year_Id", year_Id);
                cmd.Parameters.AddWithValue("@car", car);
                cmd.Parameters.AddWithValue("@house", house);
                cmd.ExecuteNonQuery();
            }
        }
    }
    public static void InsetYear(int Year_Number)
    {
        string connectionString = "Data Source=budget.db;Version=3;";
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "INSERT INTO Year (Year_Number) VALUES (@Year_Number)";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Year_Number", Year_Number);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void DisplayMonths(ComboItem date, bool dropdown2)    //denna
    {
        string connectionString = "Data Source=Budget.db;Version=3;";

        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT Name, Car, House FROM Month WHERE year_Id = @YearId;";

            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                command.Parameters.AddWithValue("@YearId", date.Text);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    var targetList = dropdown2 ? Comparelist2 : Comparelist1;
                    targetList.Clear();

                    while (reader.Read())
                    {
                        var month = new Month(
                            reader["Name"].ToString(),
                            Convert.ToInt32(reader["Car"]),
                            Convert.ToInt32(reader["House"])
                        );

                        if (!dropdown2)
                            Comparelist1.Add(month);

                        else
                            Comparelist2.Add(month);
                    }

                    if (!dropdown2)
                        Comparelist1 = Comparelist1
                        .OrderBy(m => Form1UI.monthOrder.IndexOf(m.Name))
                        .ToList();

                    else
                        Comparelist2 = Comparelist2
                        .OrderBy(m => Form1UI.monthOrder.IndexOf(m.Name))
                        .ToList();
                }
            }
        }
    }

    public static Month GetMonthData(string year, string month)
    {
        string connectionString = "Data Source=budget.db;Version=3;";
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT * FROM month WHERE year_Id = @name AND Name = @date";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", year);
                cmd.Parameters.AddWithValue("@date", month);

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
    public static List<string> GetYearData()
    {
        List<string> years = new List<string>();
        string connectionString = "Data Source=budget.db;Version=3;";

        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT DISTINCT Year_Number FROM year";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        years.Add(reader["Year_Number"].ToString());
                    }
                }
            }
        }
        return years;
    }
}