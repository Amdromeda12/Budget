public class Month
{
    public string? Name { get; set; }
    public int Year_Id { get; set; }
    public int Car { get; set; }
    public int House { get; set; }


    public Month(string name, int car, int house)
    {
        Name = name;
        Car = car;
        House = house;
    }
    public override string ToString()
    {
        return $"Name: {Name}, House: {House}, Car: {Car}";
    }
}
public class Year
{
    public string Id;
    public int Year_Number;
}