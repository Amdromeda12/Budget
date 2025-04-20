public class Month
{
    public int Id {get;}
    public string Name { get; }
    public int YearId { get; }
    public double Income { get; }
    public double Outcome { get; }


    public Month(string name, int yearId, double income, double outcome)
    {
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
    public int Id;
    public string Year_Number;
}