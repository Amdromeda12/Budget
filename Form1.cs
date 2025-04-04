using System;
using System.Data.SQLite;
using System.Windows.Forms;
using Database.DatabaseConnection;
using System.Windows.Forms.DataVisualization.Charting;

namespace Budget;
public partial class Form1 : Form
{
    //      Gör en model för databasen sen hämta det.
    //      ButtonClick ska hämta Year + Knapp månad, Ta också bort paj ändring ifrån metoden
    //      Dropdownen ska hämta året.
    private Form1UI design;
    public Form1()
    {
        DatabaseConnection.InitializeDatabase();        //Ändra så den checkar bara 1 gång

        design = new Form1UI(this);

        design.topBar.MouseDown += TopBar_MouseDown;
        design.closeButton.Click += closeButtonPressed;
        design.minimizeButton.Click += minimizeButtonPressed;
        design.dropdown1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        design.dropdown2.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

        foreach (Button btn in design.BigButtons)
        {
            btn.Click += ButtonClick;
        }
        foreach (Button btn in design.SmallButtons)
        {
            btn.Click += ButtonClick;
        }

        DatabaseConnection.InsertMonth("Jan",1,30,30);
        DatabaseConnection.InsertMonth("Feb",2,30,30);
        DatabaseConnection.InsertMonth("Mars",3,30,30);


    }
    private Button? previousButton = null;

    private void ButtonClick(object sender, EventArgs e)
    {
        Button clickedButton = sender as Button;

        if (previousButton != null)
        {
            previousButton.FlatAppearance.BorderSize = 0;
        }
        clickedButton.FlatAppearance.BorderSize = 3; 
        clickedButton.FlatAppearance.BorderColor = Color.Yellow;

        if (clickedButton != null)
        {
            design.one.Text = clickedButton.Name;
            switch (clickedButton.Name)
            {
                case "bB0":
                    UpdatePieChart("Category A", +10);
                    break;
                case "bB1":
                    UpdatePieChart("Category B", +10); 
                    break;
                case "bB2":
                    UpdatePieChart("Category C", +10);
                    break;
                case "bB3":
                    UpdatePieChart("Category D", +10);
                    break;
                default:
                    break;
            }          //Här för pajen
        }
        previousButton = clickedButton;
    }
    private void UpdatePieChart(string categoryName, double changeValue)
    {
        foreach (var point in design.chart1.Series[0].Points)
        {
            if (point.AxisLabel == categoryName)
            {
                point.SetValueY(point.YValues[0] + changeValue);
            }
        }
        design.chart1.Invalidate();
    }

    public void TopBar_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
             MessageBox.Show("Test1"); //Ändra så man kan dra själva fönstret
        }
    }
    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        ComboBox cmb = (ComboBox)sender;

        int selectedIndex = cmb.SelectedIndex;
        ComboItem selectedValue = (ComboItem)cmb.SelectedValue;     //Funkar
        MessageBox.Show(selectedValue.Message);
    }
    
    public void closeButtonPressed(object sender, EventArgs e)
    {
        this.Close();
    }
    public void minimizeButtonPressed(object sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Minimized;
    }
}
