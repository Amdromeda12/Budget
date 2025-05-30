public class Item
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public double Amount { get; set; }
    public string? Description { get; set; }
    public string Month_Name { get; set; }
    public int Month_Id { get; set; }

    public Item(string id,string name, string type, double amount, string description,string month_Name, int monthId)
    {
        Id = id;
        Name = name;
        Type = type;
        Amount = amount;
        Description = description;
        Month_Name = month_Name;
        Month_Id = monthId;
    }
}