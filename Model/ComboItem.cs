public class ComboItem
{
    public int ID { get; set; }
    public string Text { get; set; }
    public string Message { get; set; }
    public override string ToString()
    {
        return Text;
    }
}