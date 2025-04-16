using System;
using System.Data.SQLite;
using System.Windows.Forms;
using Database.DatabaseConnection;
using System.Windows.Forms.DataVisualization.Charting;

namespace Budget;
public partial class Form1 : Form
{
    //      Dropdownen ska hämta året.
    //      Checkar vilket datum det är och skapar / lägger in det i en ny månad
    private Form1UI design;
    public Form1()
    {
        DatabaseConnection.InitializeDatabase();

        design = new Form1UI(this);

        design.topBar.MouseDown += TopBar_MouseDown;
        design.closeButton.Click += closeButtonPressed;
        design.minimizeButton.Click += minimizeButtonPressed;
        design.dropdown1.SelectedIndexChanged += DropDownChanged;
        design.dropdown2.SelectedIndexChanged += DropDownChanged;

        foreach (Button btn in design.BigButtons)
        {
            btn.Click += MonthButton;
        }
        foreach (Button btn in design.SmallButtons)
        {
            btn.Click += MonthButton;
        }

        design.populateYears();

        try { TestCodeMonth(); }
        catch (Exception) { return; }
    }

    private Button? previousButton = null;
    private Button? previousButton2 = null;
    private string currentYear;
    private string currentYear2;

    private void MonthButton(object sender, EventArgs e)
    {
        //En "DeSelect" if sats om den redan är "Selected"
        //En still PieChart om det är 2 "Selected"
        Button clickedButton = sender as Button;
        bool isSpecial = clickedButton.Name.Contains("sB");

        design.series.Enabled = true;

        string year = isSpecial ? currentYear2 : currentYear;
        Month response = DatabaseConnection.GetMonthData(year, clickedButton.Text);

        UpdateChart(response);
        HandleButtonHighlight(clickedButton, isSpecial);
    }

    private void HandleButtonHighlight(Button clickedButton, bool isSpecial)
    {
        ref Button previous = ref isSpecial ? ref previousButton2 : ref previousButton;

        if (previous != null)
        {
            previous.FlatAppearance.BorderSize = 0;
        }

        clickedButton.FlatAppearance.BorderSize = 5;
        clickedButton.FlatAppearance.BorderColor = isSpecial ? Color.Black : Color.Yellow;
        previous = clickedButton;
    }

    private void UpdateChart(Month response)
    {
        //Går inte när disabled
        design.chart1.Series[0].Points[0].Label = $"{response.Name} House: {response.House}";
        design.chart1.Series[0].Points[1].Label = $"{response.Name} Car: {response.Car}";

        UpdatePieChart("Category A", response.House);
        UpdatePieChart("Category B", response.Car);
    }
    //Tänka på denna "Under"
    private void UpdatePieChart(string categoryName, double changeValue)
    {
        foreach (var point in design.chart1.Series[0].Points)
        {
            if (point.AxisLabel == categoryName)
            {
                point.SetValueY(changeValue);       //Byter till exakt
            }
        }
        design.chart1.Invalidate();
    }

    //Tänka på denna "Under"
    private void DropDownChanged(object sender, EventArgs e)
    {
        ComboBox cmb = (ComboBox)sender;
        bool isSpecial = cmb.Name.Contains("dropdown2");

        int selectedIndex = cmb.SelectedIndex;
        ComboItem selectedValue = (ComboItem)cmb.SelectedValue;     //Funkar
        DatabaseConnection.DisplayMonths(selectedValue);

        //Någonstans här, Läs in vilka månader som har något i sig.
        //Ändra färg på alla knappar på ett bra sätt

        if (isSpecial) { currentYear2 = selectedValue.Text; }
        else
        {
            currentYear = selectedValue.Text;   //Rätt funtion
        }
    }

    public void TopBar_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            MessageBox.Show("Test1"); //Ändra så man kan dra själva fönstret
        }
    }

    public void closeButtonPressed(object sender, EventArgs e)
    {
        this.Close();
    }

    public void minimizeButtonPressed(object sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Minimized;
    }

    private static void TestCodeMonth()
    {
        DatabaseConnection.InsertMonth("Jan", 2024, 2000, 3220);
        DatabaseConnection.InsertMonth("Jan", 2025, 2000, 3220);
        DatabaseConnection.InsertMonth("Jan", 2026, 2000, 3220);
        DatabaseConnection.InsertMonth("Feb", 2024, 34440, 1120);
        DatabaseConnection.InsertMonth("Feb", 2025, 34440, 1120);
        DatabaseConnection.InsertMonth("Feb", 2026, 34440, 1120);
        DatabaseConnection.InsertMonth("Feb", 2025, 34440, 1120);
        DatabaseConnection.InsertMonth("Mar", 2025, 1800, 2900);
        DatabaseConnection.InsertMonth("Apr", 2025, 2200, 3100);
        DatabaseConnection.InsertMonth("May", 2025, 2500, 3300);

        DatabaseConnection.InsetYear(2023);
        DatabaseConnection.InsetYear(2024);
        DatabaseConnection.InsetYear(2025);
        DatabaseConnection.InsetYear(2026);
    }
}
