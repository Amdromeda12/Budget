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
        clickedButton.FlatAppearance.BorderColor = Color.Black;
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

        if (!dropdown2)
            currentYear = selectedValue.Text;
        else
            currentYear2 = selectedValue.Text;

        DatabaseConnection.DisplayMonths(selectedValue, dropdown2);

        UpdateButtonColor(dropdown2);
    }

    
    private void UpdateButtonColor(bool dropdown2)
    {
        var firstMonth = dropdown2 ? DatabaseConnection.Comparelist2 : DatabaseConnection.Comparelist1;

        var buttonPrefix = dropdown2 ? "sB" : "bB";
        var buttonsize = dropdown2 ? design.SmallButtons : design.BigButtons;

        foreach (var button in buttonsize)
        {
            button.Enabled = false;
            button.BackColor = Color.White;
            button.Tag = null;
        }
        foreach (var month in firstMonth)
        {
            var targetName = buttonPrefix + month.Name;
            var button = buttonsize.FirstOrDefault(b => b.Name == targetName);

            button.Tag = month.Car;             //TODO: Ändra detta till "Total" och gör om i klassen för det

            button.Enabled = true;
        }
        for (int i = 0; i < design.BigButtons.Count(); i++)
        {
            object bigTagObj = design.BigButtons[i].Tag;
            object smallTagObj = design.SmallButtons[i].Tag;

            bool bigTagValid = int.TryParse(bigTagObj?.ToString(), out int bigTag);
            bool smallTagValid = int.TryParse(smallTagObj?.ToString(), out int smallTag);

            if (bigTagValid && smallTagValid)
            {
                if (bigTag > smallTag)
                {
                    design.BigButtons[i].BackColor = Color.Green;
                    design.SmallButtons[i].BackColor = Color.Red;
                }
                else if (bigTag < smallTag)
                {
                    design.BigButtons[i].BackColor = Color.Red;
                    design.SmallButtons[i].BackColor = Color.Green;
                }
                else
                {
                    design.BigButtons[i].BackColor = Color.MediumPurple;
                    design.SmallButtons[i].BackColor = Color.MediumPurple;
                }
            }
            else if (bigTagValid && !smallTagValid)
            {
                design.BigButtons[i].BackColor = Color.Blue;
                design.SmallButtons[i].BackColor = Color.Gray;
            }
            else if (!bigTagValid && smallTagValid)
            {
                design.BigButtons[i].BackColor = Color.Gray;
                design.SmallButtons[i].BackColor = Color.RosyBrown;
            }
            else
            {
                design.BigButtons[i].BackColor = Color.Gray;
                design.SmallButtons[i].BackColor = Color.Gray;
            }
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
        DatabaseConnection.InsertMonth("Jan", 2023, 2000, 3220);
        DatabaseConnection.InsertMonth("Feb", 2023, 2600, 2000);
        DatabaseConnection.InsertMonth("Mar", 2023, 2000, 3220);
        DatabaseConnection.InsertMonth("Apr", 2023, 34440, 1120);
        DatabaseConnection.InsertMonth("May", 2023, 865, 1120);
        DatabaseConnection.InsertMonth("Jun", 2023, 554, 1120);
        DatabaseConnection.InsertMonth("Jul", 2023, 2231, 1120);
        DatabaseConnection.InsertMonth("Aug", 2023, 1800, 2900);
        DatabaseConnection.InsertMonth("Sep", 2023, 2200, 3100);
        DatabaseConnection.InsertMonth("Oct", 2023, 2500, 3300);
        DatabaseConnection.InsertMonth("Nov", 2023, 25500, 3300);

        DatabaseConnection.InsertMonth("Jan", 2024, 2000, 3220);
        DatabaseConnection.InsertMonth("Feb", 2024, 2500, 3220);
        DatabaseConnection.InsertMonth("Mar", 2024, 100, 3220);
        DatabaseConnection.InsertMonth("Apr", 2024, 333, 1120);
        DatabaseConnection.InsertMonth("May", 2024, 324440, 1120);
        DatabaseConnection.InsertMonth("Jun", 2024, 21, 1120);
        DatabaseConnection.InsertMonth("Jul", 2024, 34440, 1120);
        DatabaseConnection.InsertMonth("Aug", 2024, 3345, 2900);
        DatabaseConnection.InsertMonth("Sep", 2024, 23200, 3100);
        DatabaseConnection.InsertMonth("Oct", 2024, 5, 3300);
        


        DatabaseConnection.InsetYear(2023);
        DatabaseConnection.InsetYear(2024);
    }
}
