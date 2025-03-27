using System;
using System.Data.SQLite;
using System.Windows.Forms;
using Database.DatabaseConnection;

namespace Budget;
public partial class Form1 : Form
{
    private Form1UI design;
    public Form1()
    {
        DatabaseConnection.InitializeDatabase();

        design = new Form1UI(this);

        design.topBar.MouseDown += TopBar_MouseDown;
        design.closeButton.Click += closeButtonPressed;
        design.minimizeButton.Click += minimizeButtonPressed;

        foreach (Button btn in design.BigButtons)
        {
            btn.Click += ButtonClick;
        }
        foreach (Button btn in design.SmallButtons)
        {
            btn.Click += ButtonClick;
        }
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
            MessageBox.Show($"You clicked: {clickedButton.Name}");
        }
        previousButton = clickedButton;
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
}
