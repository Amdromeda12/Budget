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
            CREATE TABLE IF NOT EXISTS Years (
                Id INTEGER PRIMARY KEY,
                Year_Number INTEGER UNIQUE
            );

            CREATE TABLE IF NOT EXISTS Months (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Year_Id INTEGER NOT NULL,
                Income REAL DEFAULT 0,
                Outcome REAL DEFAULT 0,
                UNIQUE(Name, Year_Id),
                FOREIGN KEY (Year_Id) REFERENCES Years(Id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS Items (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Description TEXT,
                Amount REAL NOT NULL,
                Type TEXT CHECK(Type IN ('Income', 'Expense')),
                Month_Id INTEGER NOT NULL,
                FOREIGN KEY (Month_Id) REFERENCES Months(Id) ON DELETE CASCADE
            );

            CREATE TRIGGER IF NOT EXISTS trg_insert_item
            AFTER INSERT ON Items
            BEGIN
                UPDATE Months
                SET 
                    Income = Income + CASE WHEN NEW.Type = 'Income' THEN NEW.Amount ELSE 0 END,
                    Outcome = Outcome + CASE WHEN NEW.Type = 'Expense' THEN NEW.Amount ELSE 0 END
                WHERE Id = NEW.Month_Id;
            END;

            CREATE TRIGGER IF NOT EXISTS trg_update_item
            AFTER UPDATE ON Items
            BEGIN
                UPDATE Months
                SET 
                    Income = Income - CASE WHEN OLD.Type = 'Income' THEN OLD.Amount ELSE 0 END,
                    Outcome = Outcome - CASE WHEN OLD.Type = 'Expense' THEN OLD.Amount ELSE 0 END
                WHERE Id = OLD.Month_Id;

                UPDATE Months
                SET 
                    Income = Income + CASE WHEN NEW.Type = 'Income' THEN NEW.Amount ELSE 0 END,
                    Outcome = Outcome + CASE WHEN NEW.Type = 'Expense' THEN NEW.Amount ELSE 0 END
                WHERE Id = NEW.Month_Id;
            END;

            CREATE TRIGGER IF NOT EXISTS trg_delete_item
            AFTER DELETE ON Items
            BEGIN
                UPDATE Months
                SET 
                    Income = Income - CASE WHEN OLD.Type = 'Income' THEN OLD.Amount ELSE 0 END,
                    Outcome = Outcome - CASE WHEN OLD.Type = 'Expense' THEN OLD.Amount ELSE 0 END
                WHERE Id = OLD.Month_Id;
            END;";

            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public static bool InsertMonth(string name, int year_Id)
    {
        string connectionString = "Data Source=budget.db;Version=3;";
        try
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Months (Name, Year_Id) VALUES (@name, @year_Id)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@year_Id", year_Id);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static bool InsertYear(int yearNumber)
    {
        string connectionString = "Data Source=budget.db;Version=3;";
        try
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Years (Year_Number) VALUES (@Year_Number)";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Year_Number", yearNumber);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }


    public static void DisplayMonths(ComboItem date, bool dropdown2)
    {
        string connectionString = "Data Source=Budget.db;Version=3;";

        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT Name, Income, Outcome FROM Months WHERE Year_Id = @YearId;";

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
                            Convert.ToInt32(date.Text), // this is the yearId
                            Convert.ToDouble(reader["Income"]),
                            Convert.ToDouble(reader["Outcome"])
                        );
                        targetList.Add(month);
                    }

                    var ordered = targetList
                        .OrderBy(m => Form1UI.monthOrder.IndexOf(m.Name))
                        .ToList();

                    if (dropdown2)
                        Comparelist2 = ordered;
                    else
                        Comparelist1 = ordered;
                }
            }
        }
    }
    public static Month GetMonthData(string yearId, string monthName)
    {
        string connectionString = "Data Source=budget.db;Version=3;";
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT * FROM Months WHERE Year_Id = @yearId AND Name = @monthName";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@yearId", yearId);
                cmd.Parameters.AddWithValue("@monthName", monthName);

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Month(
                            reader["Name"].ToString(),
                            Convert.ToInt32(reader["Year_Id"]),
                            Convert.ToDouble(reader["Income"]),
                            Convert.ToDouble(reader["Outcome"])
                        );
                    }
                }
            }
        }
        return null;
    }
    public static List<string> GetYearData()
    {
        List<string> years = new List<string>();
        string connectionString = "Data Source=budget.db;Version=3;";

        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT DISTINCT Year_Number FROM Years";

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
    public static int GetMonthId(string monthName, string yearNumber)
    {
        string connectionString = "Data Source=budget.db;Version=3;";
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();

            string getYearSql = "SELECT Year_Number FROM Years WHERE Year_Number = @year";
            using (SQLiteCommand yearCmd = new SQLiteCommand(getYearSql, conn))
            {
                yearCmd.Parameters.AddWithValue("@year", yearNumber);
                var yearId = Convert.ToInt32(yearCmd.ExecuteScalar());

                // Then get the Month_Id
                string getMonthSql = "SELECT Id FROM Months WHERE Name = @month AND Year_Id = @yearId";
                using (SQLiteCommand monthCmd = new SQLiteCommand(getMonthSql, conn))
                {
                    monthCmd.Parameters.AddWithValue("@month", monthName);
                    monthCmd.Parameters.AddWithValue("@yearId", yearId);

                    var result = monthCmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }
    }
    public static void InsertItem(string name, string description, double amount, string type, int monthId)
    {
        string connectionString = "Data Source=budget.db;Version=3;";
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string sql = "INSERT INTO Items (Name, Description, Amount, Type, Month_Id) VALUES (@name, @desc, @amt, @type, @monthId)";
            using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@desc", description);
                cmd.Parameters.AddWithValue("@amt", amount);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@monthId", monthId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}