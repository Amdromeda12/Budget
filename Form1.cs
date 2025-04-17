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
        this.MouseDown += TopBar_MouseDown;
        this.MouseMove += mainForm_MouseMove;
        this.MouseUp += mainForm_MouseUp;

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
        Button clickedButton = sender as Button;
        bool isSpecial = clickedButton.Name.Contains("sB");

        if (!isSpecial) design.series.Enabled = true;
        if (isSpecial) design.series2.Enabled = true;

        string year = isSpecial ? currentYear2 : currentYear;
        Month response = DatabaseConnection.GetMonthData(year, clickedButton.Text);         //Här

        UpdateChart(response, isSpecial);
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

    private void UpdateChart(Month response, bool isSpecial)
    {
        var chart = isSpecial ? design.chart2 : design.chart1;

        chart.Series[0].Points[0].Label = $"{response.Name} House: {response.House}";
        chart.Series[0].Points[1].Label = $"{response.Name} Car: {response.Car}";

        foreach (var point in chart.Series[0].Points)
        {
            if (point.AxisLabel == "Category A")
            {
                point.SetValueY(response.House);       //Byter till exakt
            }
            if (point.AxisLabel == "Category B")
            {
                point.SetValueY(response.Car);
            }
        }
    }

    private void DropDownChanged(object sender, EventArgs e)
    {
        ComboBox cmb = (ComboBox)sender;
        bool dropdown2 = cmb.Name.Contains("dropdown2");

        int selectedIndex = cmb.SelectedIndex;
        ComboItem selectedValue = (ComboItem)cmb.SelectedValue;
        DatabaseConnection.DisplayMonths(selectedValue, dropdown2);

        if (dropdown2) { currentYear2 = selectedValue.Text; }
        else
        {
            currentYear = selectedValue.Text;
        }
        UpdateButtonColor(dropdown2);
    }

    private void UpdateButtonColor(bool dropdown2)          //TODO: jämnför olika årens totala kostnad och ändra färg utifrån det
    {
        var targetButtons = dropdown2 ? design.SmallButtons : design.BigButtons;
        var defaultColor = Color.White;
        var highlightColor = dropdown2 ? Color.Red : Color.Blue;
        var buttonPrefix = dropdown2 ? "sB" : "bB";

        var red = Color.Red;
        var blue = Color.Blue;



        foreach (var button in targetButtons)
        {
            button.BackColor = defaultColor;
            button.Enabled = false;
        }

        foreach (var month in DatabaseConnection.Months)
        {
            var targetName = buttonPrefix + month;
            var button = targetButtons.FirstOrDefault(b => b.Name == targetName);
            if (button != null)
            {
                button.Enabled = true;

                button.BackColor = highlightColor;
            }
            
            /* if (DatabaseConnection.Comparelist1[0].Car > DatabaseConnection.Comparelist2[0].Car)
            {
                button.BackColor = red;
            }
            else button.BackColor = blue; */
        }
    }

    //  Window Drag
    private bool dragging = false;
    private Point startPoint = new Point(0, 0);
    public void TopBar_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            dragging = true;
            startPoint = new Point(e.X, e.Y);
            this.Capture = true;
        }
    }
    public void mainForm_MouseUp(object sender, MouseEventArgs e)
    {
        dragging = false;
        this.Capture = false;
    }
    private void mainForm_MouseMove(object sender, MouseEventArgs e)
    {
        if (dragging)
        {
            Point p = PointToScreen(e.Location);
            Location = new Point(p.X - this.startPoint.X, p.Y - this.startPoint.Y);
        }
    }
    //

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
