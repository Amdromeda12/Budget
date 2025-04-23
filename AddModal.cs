using System;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

public class AddModal : Form
{
    private TextBox newName;
    private TextBox itemDescription;
    private ComboBox typeDropdown;
    private TextBox itemAmount;
    private ComboBox targetMonth;
    private TextBox targetYear;
    private Button okButton;
    private string _type;
    public AddModal(string type)
    {
        this.Text = $"Add New {type}";
        this.Size = new Size(350, 300);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        switch (type)
        {
            case "Year":
                {
                    YearAdd();
                }
                break;
            case "Month":
                {
                    MonthAdd(); ;
                }
                break;
            case "Item":
                {
                    ItemAdd(); ;
                }
                break;
        }
    }
    public string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    private void YearAdd()
    {
        int labelWidth = 100;
        int controlWidth = 200;
        int spacing = 10;
        int top = 20;
        int buttonWidth = 75;
        int buttonHeight = 25;

        targetYear = new TextBox()                      //YearBox
        {
            Size = new Size(70, 20),
            Location = new Point(labelWidth, top)
        };

        okButton = new Button()                         //OKbutton
        {
            Text = "OK",
            Size = new Size(buttonWidth, buttonHeight),
            Location = new Point((this.ClientSize.Width - buttonWidth) / 2, 220),
            Anchor = AnchorStyles.Bottom,
        };
        okButton.Click += okButton_Click;

        Label customTextLabel = new Label()
        {
            Text = "INFO:\nShould get a new year each december? I hope, I think",
            Location = new Point(spacing, 110),
            AutoSize = true
        };
        this.Controls.Add(customTextLabel);

        this.Controls.AddRange(new Control[] {                  //Labels / Names
            new Label() { Text = "Year", Location = new Point(spacing, 50), AutoSize = true },
            targetYear,
            okButton
        });
        this.Controls.Add(okButton);
    }

    private void MonthAdd()
    {
        int labelWidth = 100;
        int controlWidth = 200;
        int spacing = 10;
        int top = 20;
        int buttonWidth = 75;
        int buttonHeight = 25;

        targetMonth = new ComboBox()                     //MonthDropdown
        {
            Size = new Size(controlWidth, 20),
            Location = new Point(labelWidth, top),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        targetMonth.Items.AddRange(months);

        top += 30;
        targetYear = new TextBox()                      //YearBox
        {
            Size = new Size(70, 20),
            Location = new Point(labelWidth, top)
        };

        okButton = new Button()                         //OKbutton
        {
            Text = "OK",
            Size = new Size(buttonWidth, buttonHeight),
            Location = new Point((this.ClientSize.Width - buttonWidth) / 2, 220),
            Anchor = AnchorStyles.Bottom,
        };
        okButton.Click += okButton_Click;

        Label customTextLabel = new Label()
        {
            Text = "INFO:\nThere will always be up to current month +1 \n from start of year",
            Location = new Point(spacing, 110),
            AutoSize = true
        };
        this.Controls.Add(customTextLabel);

        this.Controls.AddRange(new Control[] {                  //Labels / Names
            new Label() { Text = "Month:", Location = new Point(spacing, 20), AutoSize = true },
            targetMonth,
            new Label() { Text = "Year", Location = new Point(spacing, 50), AutoSize = true },
            targetYear,
            okButton
        });
        this.Controls.Add(okButton);
    }

    private void ItemAdd()
    {
        int labelWidth = 100;
        int controlWidth = 200;
        int spacing = 10;
        int top = 20;
        int buttonWidth = 75;
        int buttonHeight = 25;

        newName = new TextBox()                         //NameBox
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
        };
        targetMonth.Items.AddRange(months);
        //--

        top += 30;
        targetYear = new TextBox()                      //YearBox
        {
            Size = new Size(70, 20),
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
            newName,
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

    public string Name => newName.Text;
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
