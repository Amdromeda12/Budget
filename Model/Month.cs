public class Month
{
    public string? Name { get; set; }
    public int Car { get; set; }
    public int House { get; set; }
    public Month(string name, int car, int house)
    {
        Name = name;
        Car = car;
        House = house;
    }
}