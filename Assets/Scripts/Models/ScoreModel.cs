public class ScoreModel : IData
{
    public int Id { get; set; }
    public ScoreType Type { get; set; }
    public int ScoreValue { get; set; }
    public float Accelerate { get; set; }
    public float Recovery { get; set; }

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Type.ToString();
    }
}
