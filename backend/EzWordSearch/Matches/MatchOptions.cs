namespace EzWordSearch.Matches
{
    public class MatchOptions
    {
        public const string Name = "Match";
        public int TimeLimitSeconds { get; set; } = 300;
        public int SecondsToWaitBeforeMatchStart { get; set; } = 10;
        public int Width { get; set; } = 14;
        public int Height { get; set; } = 14;
        public int MinWordLength { get; set; } = 4;
        public int MaxWordLength { get; set; } = 8;
        public int NumberOfWords { get; set; } = 30;
    }
}
