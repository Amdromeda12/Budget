public class Month
{
    public string Id { get; }
    public string Name { get; }
    public int YearId { get; }
    public double Income { get; }
    public double Outcome { get; }


    public Month(string id, string name, int yearId, double income, double outcome)
    {
        Id = id;
        Name = name;
        YearId = yearId;
        Income = income;
        Outcome = outcome;
    }

    public override string ToString()
    {
        return $"Name: {Name}, Income: {Income}, Outcome: {Outcome}";
    }
}
public class Year
{
    public string Id { get; }
    public string Year_Number { get; }

    public Year(string id, string year_Number)
    {
        Id = id;
        Year_Number = year_Number;
    }
}