namespace Common
{
    public interface ISensorHW
    {
        string Id { get; }
        string Type { get; }
        float Value { get; set; }
    }
}
