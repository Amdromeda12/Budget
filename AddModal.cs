using System;
using System.Drawing;
using System.Windows.Forms;

public class AddModal : Form
{
    private TextBox newItemName;
    private TextBox itemDescription;
    private ComboBox typeDropdown;
    private TextBox itemAmount;
    private ComboBox targetMonth;
    private TextBox targetYear;
    private Button okButton;

    public AddModal()
    {
        this.Text = "Add New Item";
        this.Size = new Size(350, 300);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        InitializeManualComponents();
    }

    private void InitializeManualComponents()
    {
        string[] months = {
    "January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
};

        int labelWidth = 100;
        int controlWidth = 200;
        int spacing = 10;
        int top = 20;
        int buttonWidth = 75;
        int buttonHeight = 25;

        newItemName = new TextBox()                         //NameBox
        {
            Size = new Size(controlWidth, 20),
            Location = new Point(labelWidth, top)
        };
        //--

        top += 30;
        typeDropdown = new ComboBox()                   //TypeDropdown
        {
            Size = new Size(controlWidth, 20),
            Location = new Point(labelWidth, top),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        typeDropdown.Items.AddRange(new string[] { "Income", "Expense" });
        //--

        top += 30;
        itemAmount = new TextBox()                      //AmountBox
        {
            Size = new Size(controlWidth, 20),
            Location = new Point(labelWidth, top)
        };
        //--

        top += 30;
        itemDescription = new TextBox()                         //Description
        {
            Size = new Size(controlWidth, 20),
            Location = new Point(labelWidth, top)
        };
        //--

        top += 30;
        targetMonth = new ComboBox()                     //MonthDropdown
        {
            Size = new Size(controlWidth, 20),
            Location = new Point(labelWidth, top),
            DropDownStyle = ComboBoxStyle.DropDownList
        }
            ;
        targetMonth.Items.AddRange(months);
        //--

        top += 30;
        targetYear = new TextBox()                      //YearBox
        {
            Size = new Size(controlWidth, 20),
            Location = new Point(labelWidth, top)
        };
        //--

        okButton = new Button()                         //OKbutton
        {
            Text = "OK",
            Size = new Size(buttonWidth, buttonHeight),
            Location = new Point((this.ClientSize.Width - buttonWidth) / 2, 220),
            Anchor = AnchorStyles.Bottom,
        };
        okButton.Click += okButton_Click;
        //--

        this.Controls.AddRange(new Control[] {                  //Labels / Names
            new Label() { Text = "Name:", Location = new Point(spacing, 20), AutoSize = true },
            newItemName,
            new Label() { Text = "Type:", Location = new Point(spacing, 50), AutoSize = true },
            typeDropdown,
            new Label() { Text = "Amount:", Location = new Point(spacing, 80), AutoSize = true },
            itemAmount,
            new Label() { Text = "Description:", Location = new Point(spacing, 110), AutoSize = true },
            itemDescription,
            new Label() { Text = "Month", Location = new Point(spacing, 140), AutoSize = true },
            targetMonth,
            new Label() { Text = "Year", Location = new Point(spacing, 170), AutoSize = true },
            targetYear,
            okButton
        });
        this.Controls.Add(okButton);
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    public string Name => newItemName.Text;
    public string Type => typeDropdown.SelectedItem?.ToString();
    public double Amount
    {
        get
        {
            if (double.TryParse(itemAmount.Text, out double value))
                return value;
            return 0.0;
        }
    }
    public string Description => itemDescription.Text;
    public string Month => targetMonth.SelectedItem?.ToString();
    public int Year
    {
        get
        {
            if (int.TryParse(targetYear.Text, out int value))
                return value;
            return 0;
        }
    }
}
