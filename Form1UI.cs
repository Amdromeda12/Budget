using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Database.DatabaseConnection;

public class Form1UI
{
    //  Fixa dropdown så det blir bättre
    //  
    //  
    public Chart chart1;
    public Series series { get; private set; }

    public Chart chart2;
    public Series series2 { get; private set; }

    private Form mainForm;
    public Panel bottomMain { get; private set; }
    public Month currentMonth;
    public ComboBox dropdown1 { get; private set; }
    public ComboBox dropdown2 { get; private set; }

    public Label one = new Label();
    public Label two = new Label();
    public Label three = new Label();

    public Panel topBar { get; private set; }
    public Button closeButton { get; private set; }
    public Button minimizeButton { get; private set; }

    public static readonly List<string> monthOrder = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
    private FlowLayoutPanel container;
    public FlowLayoutPanel Container => container; // Expose container to Form1
    public Button[] BigButtons { get; private set; } // Store references
    public Button[] SmallButtons { get; private set; }

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
            BackColor = Color.Purple,               //Ändra till bra färg
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
            DisplayMember = "Text",      //Här är viktigt
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
            //

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
            Enabled = false
        };

        series.Points.AddXY("Category A", 40);
        series.Points.AddXY("Category B", 30);
        //series.Points.AddXY("Category C", 30);    Ändra denna till en överskott or underskott
        //                                          Tex. Grön om det finns överskott eller röd om det är under

        chart1.Series.Add(series);
        bottomMain.Controls.Add(chart1);

        //2

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
            Enabled = false
        };

        series2.Points.AddXY("Category A", 40);
        series2.Points.AddXY("Category B", 30);
        //series.Points.AddXY("Category C", 30);    Ändra denna till en överskott or underskott
        //                                          Tex. Grön om det finns överskott eller röd om det är under

        chart2.Series.Add(series2);
        bottomMain.Controls.Add(chart2);

        //-------------------------------------------------------------------------


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

        mainPanel.Controls.Add(bottomMain);
        mainPanel.Controls.Add(container);
        mainPanel.Controls.Add(contentWrapper);

        mainForm.Controls.Add(mainPanel);
        mainForm.Controls.Add(sidebar);
        mainForm.Controls.Add(topBar);
    }

    public void populateYears()
    {
        List<string> result = DatabaseConnection.GetYearData();

        List<ComboItem> comboItems1 = new List<ComboItem>();

        for (int i = 0; i < result.Count; i++)
        {
            comboItems1.Add(new ComboItem { ID = i, Text = result[i] });
        }
        List<ComboItem> comboItems2 = comboItems1
       .Select(item => new ComboItem { ID = item.ID, Text = item.Text })
       .ToList();

        dropdown1.DataSource = comboItems1;
        dropdown2.DataSource = comboItems2;
    }
}