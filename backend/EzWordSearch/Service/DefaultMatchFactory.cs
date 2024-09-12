using EzWordSearch.Domain.Boards;
using EzWordSearch.Domain.Matches;
using EzWordSearch.Matches;
using Microsoft.Extensions.Options;

namespace EzWordSearch.Service
{
    public class DefaultMatchFactory : IMatchFactory
    {
        private const string _dictionaryPath = ".\\words.tsv";
        private readonly List<string> _dictionary;
        private readonly IOptionsMonitor<MatchOptions> _options;
        private readonly MatchEventHandlers _eventHandlers;
        public DefaultMatchFactory(IOptionsMonitor<MatchOptions> options, MatchEventHandlers eventHandlers)
        {
            _dictionary = File.ReadAllLines(_dictionaryPath).ToList();
            _options = options;
            _eventHandlers = eventHandlers;
        }
        public Match CreateMatch(int numberOfPlayers)
        {
            var matchSettings = new MatchSettings
            {
                NumberOfPlayers = numberOfPlayers,
                TimeLimitSeconds = _options.CurrentValue.TimeLimitSeconds,
                SecondsToWaitBeforeMatchStart = _options.CurrentValue.SecondsToWaitBeforeMatchStart
            };
            var boardSettings = new BoardSettings
            {
                Dictionary = _dictionary.AsReadOnly(),
                Height = _options.CurrentValue.Height,
                Width = _options.CurrentValue.Width,
                MaxWordLength = _options.CurrentValue.MaxWordLength,
                MinWordLength = _options.CurrentValue.MinWordLength,
                NumberOfWords = _options.CurrentValue.NumberOfWords
            };
            var match = new Match(matchSettings, boardSettings);
            RegisterHandlers(match);
            return match;
        }
        private void RegisterHandlers(Match match)
        {
            match.PlayerJoinedEvent += _eventHandlers.PlayerJoined;
            match.PlayerLeftEvent += _eventHandlers.PlayerLeft;
            match.MoveMadeEvent += _eventHandlers.MoveMade;
            match.MatchEndedEvent += _eventHandlers.MatchEnded;
            match.WaitingForPlayersEvent += _eventHandlers.WaitingForPlayers;
            match.WaitingToStartEvent += _eventHandlers.WaitingToStart;
            match.MatchStartedEvent += _eventHandlers.MatchStarted;
            match.ErrorEvent += _eventHandlers.Error;
        }
    }
}
