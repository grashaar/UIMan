namespace UnuGames
{
    public interface IProgressBar
    {
        float Value { get; set; }

        void UpdateValue(float value);
    }
}