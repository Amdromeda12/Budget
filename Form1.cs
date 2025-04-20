using System;
using System.Data.SQLite;
using System.Windows.Forms;
using Database.DatabaseConnection;
using System.Windows.Forms.DataVisualization.Charting;

namespace Budget;
public partial class Form1 : Form
{
    private Form1UI design;
    public Form1()
    {
        DatabaseConnection.InitializeDatabase();
        UpdateYearAndMonth();

        design = new Form1UI(this);
        design.topBar.MouseDown += TopBar_MouseDown;
        design.closeButton.Click += closeButtonPressed;
        design.minimizeButton.Click += minimizeButtonPressed;
        design.flipper.Click += flipperPressed;
        design.add.Click += addButtonPressed;
        design.edit.Click += editButtonPressed;
        design.remove.Click += removeButtonPressed;
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
    }

    private void removeButtonPressed(object? sender, EventArgs e)
    {
        MessageBox.Show("remove");
    }

    private void editButtonPressed(object? sender, EventArgs e)
    {
        MessageBox.Show("edit");
    }

    private void addButtonPressed(object? sender, EventArgs e)
    {
        MessageBox.Show("add");
    }

    private void flipperPressed(object? sender, EventArgs e)
    {
        design.page2.Visible = !design.page2.Visible;
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

        string year = isSpecial ? currentYear2 : currentYear;
        Month response = DatabaseConnection.GetMonthData(year, clickedButton.Text);
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

        double income = response.Income == 0 ? 0.01 : response.Income;
        double outcome = response.Outcome == 0 ? 0.01 : response.Outcome;
        double difference = response.Income - response.Outcome;
        chart.Titles.Clear();

        chart.Titles.Add(response.Name + " " + response.YearId);
        chart.Titles[0].Font = new Font("Arial", 14, FontStyle.Bold);
        chart.Titles[0].ForeColor = Color.DarkBlue;
        chart.Titles[0].Alignment = ContentAlignment.TopCenter;

        foreach (var point in chart.Series[0].Points)
        {
            if (point.AxisLabel == "Category A")
            {
                point.SetValueY(income);
                point.Color = Color.AliceBlue;

                point.Label = $"Income: {response.Income}";
            }
            else if (point.AxisLabel == "Category B")
            {
                point.SetValueY(outcome);
                point.Color = Color.Yellow;

                point.Label = $"Outcome: {response.Outcome}";
            }
            else if (point.AxisLabel == "Category C")
            {
                if (difference > 0)
                {
                    point.Color = Color.Green;
                    point.Label = $"Gained: {difference}";
                }
                else
                {
                    point.Color = Color.Red;
                    point.Label = $"Lost: {difference}";
                }
                point.SetValueY(difference);
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

            button.Tag = month.Income;
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

    private static void UpdateYearAndMonth()
    {
        int currentYearInt = DateTime.Now.Year;
        int currentMonthInt = DateTime.Now.Month+1;

        string currentYear = currentYearInt.ToString();
        DatabaseConnection.InsertYear(currentYearInt);
        string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        for (int i = 0; i < currentMonthInt; i++)
        {
            DatabaseConnection.InsertMonth(months[i], currentYearInt);
        }
        var monthIdsCurrentYear = months
            .Take(currentMonthInt)
            .Select(month => DatabaseConnection.GetMonthId(month, currentYear))
            .ToList();

        /* foreach (var monthId in monthIdsCurrentYear)
        {
            DatabaseConnection.InsertItem("Rent", "Apartment rent", 1000, "Expense", monthId);
            DatabaseConnection.InsertItem("Groceries", "Weekly groceries", 200, "Expense", monthId);
            DatabaseConnection.InsertItem("Salary", "Monthly salary", 3000, "Income", monthId);
        } */
    }
}
