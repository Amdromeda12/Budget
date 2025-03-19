namespace Budget;

public partial class Form1 : Form
{
    private Panel topBar;
    private Panel bottomMain;
    public Form1()
    {
        //Själva fönstret
        Text = "Budget";
        this.FormBorderStyle = FormBorderStyle.None;
        this.Size = new Size(1800, 1000);
        StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.DimGray;

        // Create Custom Top Bar
        topBar = new Panel();
        topBar.Size = new Size(this.Width, 40); // Set height
        topBar.Dock = DockStyle.Top;
        topBar.BackColor = Color.Purple; // Custom color
        topBar.MouseDown += TopBar_MouseDown;
        
        Button closeButton = new Button();
        closeButton.Text = "X";
        closeButton.ForeColor = Color.White;
        closeButton.BackColor = Color.Red;
        closeButton.FlatStyle = FlatStyle.Flat;
        closeButton.Size = new Size(40, 40);
        closeButton.Location = new Point(this.Width - 45, 0);
        closeButton.Click += (s, e) => this.Close(); // Close on click

        Button minimizeButton = new Button();
        minimizeButton.Text = "_";
        minimizeButton.ForeColor = Color.White;
        minimizeButton.BackColor = Color.Gray;
        minimizeButton.FlatStyle = FlatStyle.Flat;
        minimizeButton.Size = new Size(40, 40);
        minimizeButton.Location = new Point(this.Width - 90, 0);
        minimizeButton.Click += (s, e) => this.WindowState = FormWindowState.Minimized; // Minimize on click

        topBar.Controls.Add(closeButton);
        topBar.Controls.Add(minimizeButton);

        //

        // Main Content Panel (Right Side)
        Panel mainPanel = new Panel();
        mainPanel.Dock = DockStyle.Fill;
        mainPanel.Padding = new Padding(100,60,10,10);

        //SIDE BAR
        Panel sidebar = new Panel();
        sidebar.BackColor = Color.DarkGreen;
        sidebar.Dock = DockStyle.Left;
        sidebar.Width = 260;

        Panel contentWrapper = new Panel();
        contentWrapper.Dock = DockStyle.Fill;

        // TOP Panel (container for all panel pairs)
        
        FlowLayoutPanel container = new FlowLayoutPanel();
        container.FlowDirection = FlowDirection.LeftToRight;
        container.Height = 290;
        container.BackColor = Color.LightSteelBlue;
        container.Dock = DockStyle.Top;
        container.WrapContents = false;
        container.Padding = new Padding(10);

        string[] months = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", 
                    "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

        for (int i = 0; i < 12; i++)
        {
            // Parent panel to hold both big and small panels
            Panel parentPanel = new Panel();
            parentPanel.Width = 100;
            parentPanel.Height = 270;
            parentPanel.Margin = new Padding(9, 1, 9, 1);

            //Big button
            Button bigButton = new Button();
            bigButton.Name = "bB"+i;
            bigButton.BackColor = Color.DarkMagenta;
            bigButton.ForeColor = Color.Honeydew;
            bigButton.Dock = DockStyle.Top;
            bigButton.Height = 200;
            bigButton.MouseClick += ButtonClick;

            bigButton.FlatStyle = FlatStyle.Flat;
            bigButton.FlatAppearance.BorderSize = 0;

            bigButton.Text = months[i];
            bigButton.Font = new Font("Arial", 22, FontStyle.Bold);
            bigButton.TextAlign = ContentAlignment.TopCenter;

            //Small button
            Button smallButton = new Button();
            smallButton.Name = "sB"+i;
            if (i % 3 == 0)
            {
                smallButton.BackColor = Color.Red;
            }
            else
            {
                smallButton.BackColor = Color.SkyBlue;
            }
            smallButton.Dock = DockStyle.Bottom;
            smallButton.Height = 50;
            smallButton.MouseClick += ButtonClick;

            smallButton.FlatStyle = FlatStyle.Flat;
            smallButton.FlatAppearance.BorderSize = 0;
            smallButton.Text = "Small Button " + (i + 1);

            //Add to parent
            parentPanel.Controls.Add(bigButton);
            parentPanel.Controls.Add(smallButton);
            container.Controls.Add(parentPanel);
        }

        // BOTTOM Panel
        bottomMain = new Panel();
        bottomMain.BackColor = Color.LightSteelBlue;
        bottomMain.Dock = DockStyle.Bottom;
        bottomMain.Height = 580;

        // Add Top & Bottom Panels inside Main Content
        mainPanel.Controls.Add(bottomMain);
        mainPanel.Controls.Add(container);
        mainPanel.Controls.Add(contentWrapper);

        // Add Sidebar & Main Panel to Form
        this.Controls.Add(mainPanel);
        this.Controls.Add(sidebar);
        this.Controls.Add(topBar);
    }
    private void TopBar_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            MessageBox.Show("Test1");
        }
    }

    private Button previousButton = null;
    private void ButtonClick(object sender, MouseEventArgs e)
    {
        Button clickedButton = sender as Button;
        if (previousButton != null)
        {
            previousButton.FlatAppearance.BorderSize = 0;
        }

        clickedButton.FlatAppearance.BorderSize = 3; 
        clickedButton.FlatAppearance.BorderColor = Color.Yellow; // Highlight color

        previousButton = clickedButton;
    }
}
