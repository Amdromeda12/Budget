using System;
using System.Windows.Forms;
public class Form1UI
{
    private Form mainForm;
    private Panel bottomMain;
    public Month currentMonth;

    public Label one = new Label();
    public Label two = new Label();
    public Label three = new Label();

    public Panel topBar { get; private set; }
    public Button closeButton { get; private set; }
    public Button minimizeButton { get; private set; }

    private string[] months = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
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
        topBar = new Panel(){
            Size = new Size(mainForm.Width, 40),
            Dock = DockStyle.Top,
            BackColor = Color.Purple,               //Ändra till bra färg
        };
        
        closeButton = new Button(){
            Text = "X",
            ForeColor = Color.White,
            BackColor =  Color.Red,
            FlatStyle = FlatStyle.Flat,
            Size = new Size(40, 40),
            Location = new Point(mainForm.Width - 45, 0)
        };

        minimizeButton = new Button(){
            Text = "_",
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Size = new Size(40, 40),
            Location = new Point(mainForm.Width - 90, 0)
        };

        Label title = new Label(){
            Text = "Budget",
            Size = new Size(200, 38),
            Font = new Font("Arial", 24, FontStyle.Bold)
        };

        topBar.Controls.Add(closeButton);
        topBar.Controls.Add(minimizeButton);
        topBar.Controls.Add(title);
        //-------------------------------

        Panel mainPanel = new Panel(){
            Dock = DockStyle.Fill,
            Padding = new Padding(100,60,10,10)
        };

        Panel sidebar = new Panel(){
            BackColor = Color.DarkGreen,
            Dock = DockStyle.Left,
            Width = 260
        };

        Panel contentWrapper = new Panel(){
            Dock = DockStyle.Fill
        };

        container = new FlowLayoutPanel(){
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
            Panel parentPanel = new Panel(){
                Width = 100,
                Height = 270,
                Margin = new Padding(9, 1, 9, 1)
            };
            
            Button bigButton = new Button(){
                Name = "bB"+i,
                BackColor = Color.DarkMagenta,
                ForeColor = Color.Honeydew,
                Dock = DockStyle.Top,
                Height = 200,
                FlatStyle = FlatStyle.Flat,
                Text = months[i],
                Font = new Font("Arial", 22, FontStyle.Bold),
                TextAlign = ContentAlignment.TopCenter

            };
            bigButton.FlatAppearance.BorderSize = 0;
            BigButtons[i] = bigButton;

            Button smallButton = new Button(){
                Name = "sB"+i,
                Dock = DockStyle.Bottom,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                Text = months[i],
                Font = new Font("Arial", 22, FontStyle.Bold)
            };
            smallButton.FlatAppearance.BorderSize = 0;
            SmallButtons[i] = smallButton;

            //Fixa Dessa, Ska vara I koden istället för utseende
            if (i % 3 == 0)
            {
                smallButton.BackColor = Color.Red;
            }
            else
            {
                smallButton.BackColor = Color.SkyBlue;
            }
            //
            
            //Add to parent
            parentPanel.Controls.Add(bigButton);
            parentPanel.Controls.Add(smallButton);
            container.Controls.Add(parentPanel);
        }

        // BOTTOM Panel
        bottomMain = new Panel(){
            BackColor = Color.LightSteelBlue,
            Dock = DockStyle.Bottom,
            Height = 580
        };

        TableLayoutPanel table = new TableLayoutPanel(){
            ColumnCount = 3,
            RowCount = 1,
            Dock = DockStyle.Fill,
        };
        table.Controls.Add(one, 0, 0);
        table.Controls.Add(two, 1, 0);
        table.Controls.Add(three, 2, 0);
        bottomMain.Controls.Add(table);

        mainPanel.Controls.Add(bottomMain);
        mainPanel.Controls.Add(container);
        mainPanel.Controls.Add(contentWrapper);

        mainForm.Controls.Add(mainPanel);
        mainForm.Controls.Add(sidebar);
        mainForm.Controls.Add(topBar);
    }
}