using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Database.DatabaseConnection;
using System.Data;

public class Form1UI
{
    public Chart chart1;
    public Series series { get; private set; }
    public Panel page2;
    public Chart chart2;
    public Series series2 { get; private set; }
    public DataGridView CRUDView { get; private set; }
    private Form mainForm;
    public Panel bottomMain { get; private set; }
    public Month currentMonth;
    public ComboBox dropdown1 { get; private set; }
    public ComboBox dropdown2 { get; private set; }
    public ComboBox CRUD { get; private set; }
    public Label one = new Label();
    public Label two = new Label();
    public Label three = new Label();
    public Panel topBar { get; private set; }
    public Button closeButton { get; private set; }
    public Button minimizeButton { get; private set; }
    public Button flipper { get; private set; }
    public Button add { get; private set; }
    public Button edit { get; private set; }
    public Button remove { get; private set; }
    public DataTable table2 { get; private set; }
    public static readonly List<string> monthOrder = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
    private FlowLayoutPanel container;
    public FlowLayoutPanel Container => container;
    public Button[] BigButtons { get; private set; }
    public Button[] SmallButtons { get; private set; }
    public Label IncomeLabel { get; private set; }
    public Label ExpenseLabel { get; private set; }
    public Label SavingsLabel { get; private set; }
    public Label BalanceLabel { get; private set; }

    public Form1UI(Form form)
    {
        mainForm = form;
        CustomUI();
    }

    public void CustomUI()
    {
        //Själva fönstret
        mainForm.Text = "Budget";
        mainForm.FormBorderStyle = FormBorderStyle.None;
        mainForm.Size = new Size(1800, 1000);
        mainForm.StartPosition = FormStartPosition.CenterScreen;
        mainForm.BackColor = Color.DimGray;

        //TOPBAR
        topBar = new Panel()
        {
            Size = new Size(mainForm.Width, 40),
            Dock = DockStyle.Top,
            BackColor = Color.Purple,
        };

        closeButton = new Button()
        {
            Text = "X",
            ForeColor = Color.White,
            BackColor = Color.Red,
            FlatStyle = FlatStyle.Flat,
            Size = new Size(40, 40),
            Location = new Point(mainForm.Width - 45, 0)
        };

        minimizeButton = new Button()
        {
            Text = "_",
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Size = new Size(40, 40),
            Location = new Point(mainForm.Width - 90, 0)
        };

        Label title = new Label()
        {
            Text = "Budget",
            Size = new Size(200, 38),
            Font = new Font("Arial", 24, FontStyle.Bold)
        };

        topBar.Controls.Add(closeButton);
        topBar.Controls.Add(minimizeButton);
        topBar.Controls.Add(title);
        //-------------------------------

        //------------------------------- Side Panel
        Panel sidebar = new Panel()
        {
            BackColor = Color.DarkGreen,
            Dock = DockStyle.Left,
            Width = 260
        };

        dropdown1 = new ComboBox()
        {
            Height = 50,
            Width = 100,
            DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "Text",
        };

        dropdown2 = new ComboBox()
        {
            Height = 50,
            Width = 100,
            Name = "dropdown2",
            DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "Text",
            Location = new Point(sidebar.Height - 0, 0)
        };

        flipper = new Button()
        {
            Name = "flipper",
            Width = 200,
            Height = 200,
            Text = "Switch View",
            BackColor = Color.Brown,
            Anchor = AnchorStyles.None
        };

        flipper.Location = new Point(
            (sidebar.Width - flipper.Width) / 2,
            (sidebar.Height - flipper.Height) / 2
        );
        //------------------------------- Main Panel
        Panel mainPanel = new Panel()
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(100, 60, 10, 10)
        };

        Panel contentWrapper = new Panel()
        {
            Dock = DockStyle.Fill
        };

        container = new FlowLayoutPanel()
        {
            FlowDirection = FlowDirection.LeftToRight,
            Height = 290,
            BackColor = Color.LightSteelBlue,
            Dock = DockStyle.Top,
            WrapContents = false,
            Padding = new Padding(10)
        };

        BigButtons = new Button[12];
        SmallButtons = new Button[12];

        for (int i = 0; i < 12; i++)
        {
            Panel parentPanel = new Panel()
            {
                Width = 100,
                Height = 270,
                Margin = new Padding(9, 1, 9, 1)
            };

            Button bigButton = new Button()
            {
                Name = "bB" + monthOrder[i],
                BackColor = Color.Gray,
                Dock = DockStyle.Top,
                Height = 200,
                FlatStyle = FlatStyle.Flat,
                Text = monthOrder[i],
                Font = new Font("Arial", 22, FontStyle.Bold),
                TextAlign = ContentAlignment.TopCenter
            };
            bigButton.FlatAppearance.BorderSize = 0;
            BigButtons[i] = bigButton;

            Button smallButton = new Button()
            {
                Name = "sB" + monthOrder[i],
                BackColor = Color.Gray,
                Dock = DockStyle.Bottom,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                Text = monthOrder[i],
                Font = new Font("Arial", 22, FontStyle.Bold)
            };
            smallButton.FlatAppearance.BorderSize = 0;
            SmallButtons[i] = smallButton;

            //Add to parent
            parentPanel.Controls.Add(bigButton);
            parentPanel.Controls.Add(smallButton);
            container.Controls.Add(parentPanel);
        }

        // BOTTOM Panel
        bottomMain = new Panel()
        {
            BackColor = Color.LightSteelBlue,
            Dock = DockStyle.Bottom,
            Height = 580
        };

        //Paj 1 och 2
        chart1 = new Chart()
        {
            Dock = DockStyle.Left
        };

        var chartArea = new ChartArea();
        chart1.ChartAreas.Add(chartArea);

        series = new Series
        {
            ChartType = SeriesChartType.Pie,
            BorderWidth = 2,
            BorderColor = System.Drawing.Color.Black,
        };

        chart1.Titles.Clear();
        chart1.Titles.Add("Monthly Expenses Breakdown");
        chart1.Titles[0].Font = new Font("Arial", 14, FontStyle.Bold);
        chart1.Titles[0].ForeColor = Color.DarkBlue;
        chart1.Titles[0].Alignment = ContentAlignment.TopCenter;

        series.Points.AddXY("Category A", 0);
        series.Points.AddXY("Category B", 0);
        series.Points.AddXY("Category C", 0);        
        //

        chart1.Series.Add(series);
        bottomMain.Controls.Add(chart1);

        chart2 = new Chart()
        {
            Dock = DockStyle.Right
        };

        var chartArea2 = new ChartArea();
        chart2.ChartAreas.Add(chartArea2);

        series2 = new Series
        {
            ChartType = SeriesChartType.Pie,
            BorderWidth = 2,
            BorderColor = System.Drawing.Color.Black,
        };

        series2.Points.AddXY("Category A", 0);
        series2.Points.AddXY("Category B", 0);
        series2.Points.AddXY("Category C", 0);

        string[] titles = { "Income", "Expense", "Savings", "Balance" };
        Label[] labelRefs = new Label[4];

        var table3 = new TableLayoutPanel
        {
            RowCount = 4,
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            AutoSize = true,
        };

        table3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        for (int i = 0; i < 4; i++)
        {
            table3.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            var innerPanel = new TableLayoutPanel
            {
                RowCount = 2,
                ColumnCount = 1,
                Dock = DockStyle.Fill,
            };
            innerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            innerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            var titleLabel = new Label
            {
                Text = titles[i],
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
            };

            var valueLabel = new Label
            {
                Font = new Font("Segoe UI", 14),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
            };

            labelRefs[i] = valueLabel;

            innerPanel.Controls.Add(titleLabel, 0, 0);
            innerPanel.Controls.Add(valueLabel, 0, 1);

            table3.Controls.Add(innerPanel, 0, i);
        }
        IncomeLabel = labelRefs[0];
        ExpenseLabel = labelRefs[1];
        SavingsLabel = labelRefs[2];
        BalanceLabel = labelRefs[3];

        //table3.GetControlFromPosition(0, 2).Visible = false;
        table3.GetControlFromPosition(0, 3).Visible = false;

        bottomMain.Controls.Add(table3);
        chart2.Series.Add(series2);
        bottomMain.Controls.Add(chart2);

        //-------------------------------------------------------------------------
        //Page2

        page2 = new Panel()
        {
            Name = "page2",
            Height = 500,
            Width = 500,
            BackColor = ColorTranslator.FromHtml("#5a4d4d"),
            Dock = DockStyle.Fill,
            Visible = false
        };

        add = new Button { Name = "add", Height = 80, Width = 250, Text = "ADD", BackColor = Color.Gray };
        edit = new Button { Name = "edit", Height = 80, Width = 250, Text = "EDIT", BackColor = Color.Gray, Visible = false };
        remove = new Button { Name = "remove", Height = 80, Width = 250, Text = "REMOVE", BackColor = Color.Gray };
        add.Font = new Font(add.Font.FontFamily, 30);
        edit.Font = new Font(edit.Font.FontFamily, 30);
        remove.Font = new Font(remove.Font.FontFamily, 30);

        int totalWidth = add.Width * 3 + 20 * 2;

        int startX = (page2.Width - totalWidth) / 2 + 300;
        int centerY = (page2.Height - add.Height) / 3;

        add.Location = new Point(startX, centerY);
        edit.Location = new Point(startX + add.Width + 250, centerY);
        remove.Location = new Point(startX + (add.Width + 250) * 2, centerY);

        page2.Controls.Add(add);
        page2.Controls.Add(edit);
        page2.Controls.Add(remove);

        CRUD = new ComboBox
        {
            Name = "CRUD",
            Width = 250,
            Height = 40,
            Font = new Font("Arial", 16),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        CRUD.Items.AddRange(new string[] { "Year", "Month", "Item" });
        int spacing = 20;
        int dropdownY = centerY - CRUD.Height - spacing - 80;
        CRUD.Location = new Point(edit.Location.X, dropdownY);
        page2.Controls.Add(CRUD);

        CRUDView = new DataGridView
        {
            Name = "dataGrid",
            Dock = DockStyle.Bottom,
            Height = 700,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false
        };

        page2.Controls.Add(CRUDView);

        table2 = new DataTable();
        CRUDView.DataSource = table2;

        page2.Controls.Add(CRUDView);

        //


        TableLayoutPanel table = new TableLayoutPanel()
        {
            ColumnCount = 3,
            RowCount = 1,
            Dock = DockStyle.Fill,
        };
        table.Controls.Add(one, 0, 0);
        table.Controls.Add(two, 1, 0);
        table.Controls.Add(three, 2, 0);
        bottomMain.Controls.Add(table);
        sidebar.Controls.Add(dropdown1);
        sidebar.Controls.Add(dropdown2);
        sidebar.Controls.Add(flipper);

        mainPanel.Controls.Add(bottomMain);
        mainPanel.Controls.Add(container);
        mainPanel.Controls.Add(contentWrapper);

        mainForm.Controls.Add(page2);
        mainForm.Controls.Add(mainPanel);
        mainForm.Controls.Add(sidebar);
        mainForm.Controls.Add(topBar);
    }
    public void populateDropDownWithYear()
    {
        List<Year> result = DatabaseConnection.GetYearData();

        List<ComboItem> comboItems1 = new List<ComboItem>();

        for (int i = 0; i < result.Count; i++)
        {
            comboItems1.Add(new ComboItem { ID = i, Text = result[i].Year_Number });
        }
        List<ComboItem> comboItems2 = comboItems1
       .Select(item => new ComboItem { ID = item.ID, Text = item.Text })
       .ToList();

        dropdown1.DataSource = comboItems1;
        dropdown2.DataSource = comboItems2;
    }
}