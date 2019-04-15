namespace Common
{
    public interface ISensorHW
    {
        string Id { get; set; }
        string Type { get; set; }
        float Value { get; set; }
    }
}
