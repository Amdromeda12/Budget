using System;
using System.Data.SQLite;
using Microsoft.VisualBasic;

namespace Database.DatabaseConnection;
public class DatabaseConnection
{
    public static List<string> Months { get; set; } = new List<string>();

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

    public static void DisplayMonths(ComboItem date)
    {
        string connectionString = "Data Source=Budget.db;Version=3;";
        //string monthsList = $"Months in {date.Text}:\n";                //TODO: VIKTIGT VIKTIGT DENNA FIXAR KNAPPAR BARA HITTA KNAPP+MÅNAD SOM "bBJan" OCH ÄNDRA BACKGRUND BORDE FUNKA!!!!!!


        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = $@"SELECT Name FROM Month WHERE year_Id = {date.Text};";

            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                command.Parameters.AddWithValue("@YearId", date);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    Months.Clear();
                    while (reader.Read())
                    {
                        //monthsList += reader["Name"].ToString() + "\n";
                        Months.Add(reader["Name"].ToString());
                    }
                }
            }

            // foreach (var mon in Months)
            // {
            //     MessageBox.Show(mon);
            // }
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