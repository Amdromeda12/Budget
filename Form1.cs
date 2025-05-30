using System;
using System.Data.SQLite;
using System.Windows.Forms;
using Database.DatabaseConnection;
using System.Windows.Forms.DataVisualization.Charting;

namespace Budget;

public partial class Form1 : Form
{
    //Tror flera database connections skulle kunna ändra / ta bort / sättas ihop
    //DeleteInDatabase: har ett "s" efter för gjorde fel hur stringet skickas runt
    //Edit knapp visible False har också ingen function
    //TODO: "Item" i CRUD skall ha ett månad och år fält, just nu har den bara 1
    //TODO: "Year" i CRUD skall ha både income och outcome
    //efter saker inlagda från CRUD view:n behöver starta om program

    private Form1UI design;  // Separera UI och logic
    private Button? previousButton = null;
    private Button? previousButton2 = null;
    private string currentYear; //Dropdown månads val
    private string currentYear2; //-||-
    private bool dragging = false; //Håller koll på musen
    private Point startPoint = new Point(0, 0); //-||-
    public Form1()
    {
        //Sätter igång med datasen 
        DatabaseConnection.InitializeDatabase();
        UpdateYearAndMonth();
        //

        design = new Form1UI(this);

        // Ger funktion till knappar ifrån design
        design.topBar.MouseDown += TopBarWindowDrag;
        design.closeButton.Click += closeButtonPressed;
        design.minimizeButton.Click += minimizeButtonPressed;
        design.flipper.Click += flipperPressed;
        design.add.Click += addButtonPressed;
        design.edit.Click += editButtonPressed;
        design.remove.Click += removeButtonPressed;
        design.dropdown1.SelectedIndexChanged += MonthDropDownSelect;
        design.dropdown2.SelectedIndexChanged += MonthDropDownSelect;
        design.CRUD.SelectedIndexChanged += CRUDChanged;
        this.MouseDown += TopBarWindowDrag;
        this.MouseMove += TopBarWindowDragMove;
        this.MouseUp += TopBarWindowDragFunction;
        design.CRUDView.CellClick += CRUDView_CellClick;

        foreach (Button btn in design.BigButtons)
        {
            btn.Click += MonthButton;
        }
        foreach (Button btn in design.SmallButtons)
        {
            btn.Click += MonthButton;
        }
        //

        design.populateDropDownWithYear();
    }
    public string SelectedType;
    public string CellId;
    private void CRUDView_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            DataGridViewRow selectedRow = design.CRUDView.Rows[e.RowIndex];
            CellId = selectedRow.Cells["Id"].Value?.ToString();
        }
    }
    private void removeButtonPressed(object? sender, EventArgs e)
    {
        DialogResult result = MessageBox.Show(
        "Are you sure you want to delete?",
        "Confirmation",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    );

        if (result == DialogResult.Yes)
        {
            DatabaseConnection.DeleteInDatabase(CellId, SelectedType);
            if (SelectedType == "Year") updateCRUD(1);
            if (SelectedType == "Month") updateCRUD(2);
            if (SelectedType == "Item") updateCRUD(3);
        }
    }
    private void editButtonPressed(object? sender, EventArgs e) //Inte använd
    {
        MessageBox.Show("edit");
    }
    private void addButtonPressed(object? sender, EventArgs e) //I CRUD Addbutton, Skapa en ny i dropdown, så om man har year så skapar man ett nytt år, om man väljer Month och så gör man en månad.
    {
        try
        {
            if (SelectedType == null) { return; }
            using (var modal = new AddModal(SelectedType))
            {
                var result = modal.ShowDialog(this);
                switch (SelectedType)
                {
                    case "Year":
                        if (result == DialogResult.OK)
                        {
                            int year = modal.Year;
                            DatabaseConnection.InsertYear(year);
                        }
                        updateCRUD(1);
                        break;
                    case "Month":
                        if (result == DialogResult.OK)
                        {
                            string month = modal.Month?.Substring(0, 3);
                            int year = modal.Year;
                            DatabaseConnection.InsertMonth(month, year);
                        }
                        updateCRUD(2);
                        break;
                    case "Item":
                        if (result == DialogResult.OK)
                        {
                            string name = modal.Name;
                            string type = modal.Type;
                            double amount = modal.Amount;
                            string description = modal.Description;
                            string month = modal.Month?.Substring(0, 3);
                            int year = modal.Year;

                            int targetmonthId = DatabaseConnection.GetMonthId(month, year.ToString());
                            DatabaseConnection.InsertItem(name, type, amount, description, month, targetmonthId);
                        }
                        updateCRUD(3);
                        break;
                }
            }
        }
        catch (Exception)
        {
            MessageBox.Show("Add Failed!");
        }
    }
    private void flipperPressed(object? sender, EventArgs e)
    {
        design.page2.Visible = !design.page2.Visible;
    }
    //<
    double firstIncome = 0;
    double secondIncome = 0;
    double firstExpense = 0;
    double secondExpense = 0;
    double firstTotal = 0;
    double secondTotal = 0;
    private void MonthButton(object sender, EventArgs e) //I första menyn, för att jämföra olika valda månader
    {
        Button clickedButton = sender as Button;
        bool isSpecial = clickedButton.Name.Contains("sB");
        string year = isSpecial ? currentYear2 : currentYear;

        Month response = DatabaseConnection.GetMonthData(year, clickedButton.Text);
        UpdatePieChartColor(response, isSpecial);

        if (isSpecial)
        {
            secondIncome = response.Income;
            secondExpense = response.Outcome;
            secondTotal = secondIncome - secondExpense;
        }
        else
        {
            firstIncome = response.Income;
            firstExpense = response.Outcome;
            firstTotal = firstIncome - firstExpense;
        }
        UpdateLabel(design.IncomeLabel, firstIncome, secondIncome);
        UpdateLabel(design.ExpenseLabel, firstExpense, secondExpense);
        UpdateLabel(design.SavingsLabel, firstTotal, secondTotal);
        //design.BalanceLabel.Text = response.Income.ToString();    //Används inte
        HandleButtonHighlight(clickedButton, isSpecial);
    }
    private string FormatDifference(double firstValue, double secondValue)
    {
        double diff = firstValue - secondValue;
        string suffix = diff < 0 ? " Less" : diff > 0 ? " More" : "";
        return diff.ToString("C") + suffix;
    }
    void UpdateLabel(Label label, double firstValue, double secondValue)
    {
        label.Text = FormatDifference(firstValue, secondValue);
    }
    // Allt detta hör ihop>
    private void HandleButtonHighlight(Button clickedButton, bool isSpecial) //Ökar bordersize på valda knappar för att se vilka som är valda, 1 uppe 1 nere
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
    private void UpdatePieChartColor(Month response, bool isSpecial) //Uppdaterar färgen på pajgrafen när man jämför måndader
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

                point.Label = $"Expense: {response.Outcome}";
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
    private void MonthDropDownSelect(object sender, EventArgs e) //Updaterar beonde på vad man väljer på första sidan
    {
        ComboBox cmb = (ComboBox)sender;
        bool dropdown2 = cmb.Name.Contains("dropdown2");

        int selectedIndex = cmb.SelectedIndex;
        ComboItem selectedValue = (ComboItem)cmb.SelectedValue;

        if (!dropdown2)
            currentYear = selectedValue.Text;
        else
            currentYear2 = selectedValue.Text;

        DatabaseConnection.GetMonthsFromYear(selectedValue, dropdown2);
        UpdateButtonColor(dropdown2);
    }
    private void CRUDChanged(object sender, EventArgs e)
    {
        ComboBox cmb = (ComboBox)sender;

        SelectedType = cmb.SelectedItem.ToString();

        switch (SelectedType)
        {
            case "Year":
                updateCRUD(1);
                break;
            case "Month":
                updateCRUD(2);
                break;
            case "Item":
                updateCRUD(3);
                break;
        }
    }
    private void updateCRUD(int target) //Hämtar relevant information av det man väljer i andra sidans dropdown
    {
        switch (target)
        {
            case 1:
                List<Year> yearData = DatabaseConnection.GetYearData();

                design.table2.Rows.Clear();
                design.table2.Columns.Clear();
                design.table2.Columns.Add("Id");
                design.table2.Columns.Add("Years");
                foreach (var year in yearData)
                {
                    design.table2.Rows.Add(year.Id, year.Year_Number);
                }
                design.CRUDView.Columns[0].FillWeight = 10;

                break;

            case 2:
                List<Month> monthData = DatabaseConnection.GetMonthData2();

                design.table2.Rows.Clear();
                design.table2.Columns.Clear();

                design.table2.Columns.Add("Id");
                design.table2.Columns.Add("Month");
                design.table2.Columns.Add("Year");
                design.table2.Columns.Add("Income");
                design.table2.Columns.Add("Outcome");

                foreach (var month in monthData)
                {
                    design.table2.Rows.Add(month.Id, month.Name, month.YearId, month.Income, month.Outcome);
                }
                design.CRUDView.Columns[0].FillWeight = 10;
                break;

            case 3:
                List<Item> itemData = DatabaseConnection.GetItemData();

                design.table2.Rows.Clear();
                design.table2.Columns.Clear();

                design.table2.Columns.Add("Id");
                design.table2.Columns.Add("Name");
                design.table2.Columns.Add("Type");
                design.table2.Columns.Add("Amount");
                design.table2.Columns.Add("Description");
                design.table2.Columns.Add("Month");

                foreach (var item in itemData)
                {
                    design.table2.Rows.Add(item.Id, item.Name, item.Type, item.Amount, item.Description, item.Month_Name);
                }
                design.CRUDView.Columns[0].FillWeight = 10;
                break;
        }
    }
    private void UpdateButtonColor(bool dropdown2) //Uppdaterar färgen på knappar utefter om de har mer eller mindre än den andra månaden vald
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
    public void TopBarWindowDrag(object sender, MouseEventArgs e) //För att göra det mörligt att dra fönstret i Custom topbaren
    {
        if (e.Button == MouseButtons.Left)
        {
            dragging = true;
            startPoint = new Point(e.X, e.Y);
            this.Capture = true;
        }
    }
    public void TopBarWindowDragFunction(object sender, MouseEventArgs e) //-||-
    {
        dragging = false;
        this.Capture = false;
    }
    private void TopBarWindowDragMove(object sender, MouseEventArgs e) //-||-
    {
        if (dragging)
        {
            Point p = PointToScreen(e.Location);
            Location = new Point(p.X - this.startPoint.X, p.Y - this.startPoint.Y);
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
    private static void UpdateYearAndMonth() //Auto skapa DB inlägg för nuvarande år och nuvarande månad + 1 in i databasen
    {
        int currentYearInt = DateTime.Now.Year;
        int currentMonthInt = DateTime.Now.Month + 1;

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

        for (int i = 0; i < currentMonthInt; i++)
        {
            string monthName = months[i];
            DatabaseConnection.InsertMonth(monthName, currentYearInt);

            int monthId = DatabaseConnection.GetMonthId(monthName, currentYear);
        }
    }
}
