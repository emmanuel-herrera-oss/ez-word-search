using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EzWordSearch.Domain.Matches.Events;
using EzWordSearch.Domain.Matches.States;
using EzWordSearch.Domain.Boards;
using EzWordSearch.Domain.Players;

namespace EzWordSearch.Domain.Matches
{
    public class Match
    {
        private readonly object _lock = new object();
        public Guid MatchId { get; set; } = Guid.NewGuid();
        internal Board Board { get; set; } = null!;
        internal MatchSettings Settings { get; private set; } = null!;
        public List<MatchPlayer> Players { get; private set; } = [];
        private IMatchState _state = new WaitingForPlayers();
        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public double Duration
        {
            get
            {
                return (StartTime != null && EndTime != null) ? (EndTime.Value - StartTime.Value).TotalSeconds : 0;
            }
        }

        internal Match() { }
        public Match(MatchSettings matchSettings, BoardSettings boardSettings)
        {
            Settings = matchSettings;
            Board = new Board(boardSettings);
            Task.Run(() => Board.Initialize());
        }

        public bool InProgress => _state is InProgress;
        public bool Ended => _state is Ended;

        public void HandleEvent(WordSearchEventArgs e)
        {
            lock (_lock)
            {
                var newState = _state.HandleEvent(this, e);
                if (newState != null)
                {
                    _state.Exit(this);
                    _state = newState;
                    _state.Enter(this);
                }
            }
        }
        public MatchSnapshot GetSnapshot()
        {
            double startTimer = 0;
            var state = _state as WaitingToStart;
            if (state != null)
            {
                startTimer = (state.StartAt - DateTime.UtcNow).TotalSeconds;
            }

            return new MatchSnapshot
            {
                Players = new List<MatchPlayer>(Players),
                State = _state?.Type ?? MatchStateType.Ended,
                StartTimer = Math.Ceiling(startTimer)
            };
        }
        internal bool AcceptingPlayers => Players.Count < Settings.NumberOfPlayers && _state is WaitingForPlayers;
        internal void MakeMove(MoveMadeEventArgs e)
        {
            var boardResult = Board.MakeMove(e.Start, e.End);
            if (boardResult.Word == null)
            {
                return;
            }
            var playerMakingMove = Players.First(i => i.Player.PlayerId == e.PlayerId);
            playerMakingMove.Score++;
            e.Word = boardResult.Word;
            MoveMadeEvent?.Invoke(this, e);
        }
        internal void EndMatch()
        {
            EndTime = DateTime.UtcNow;
            var winner = GetWinner();
            if (Settings.NumberOfPlayers > 1)
            {
                if (winner == null)
                {
                    foreach (var p in Players)
                    {
                        p.Player.Ties++;
                        p.Player.LastMatch = this.StartTime;
                        p.Result = MatchPlayerResultType.TIED;
                    }
                }
                else
                {
                    foreach (var loser in Players.Where(p => p.Player.PlayerId != winner.Player.PlayerId))
                    {
                        loser.Player.Losses++;
                        loser.Player.LastMatch = this.StartTime;
                        loser.Result = MatchPlayerResultType.LOST;
                    }
                    winner.Player.Wins++;
                    winner.Player.LastMatch = this.StartTime;
                    winner.Result = MatchPlayerResultType.WON;
                }
            }
            MatchEndedEvent?.Invoke(this, new MatchEndedEventArgs 
            { 
                WinnerId = winner?.Player.PlayerId,
                LeaverId = Players.FirstOrDefault(p => p.Abandoned)?.Player?.PlayerId,
                SinglePlayer = Settings.NumberOfPlayers == 1
            });
        }
        internal MatchPlayer? GetWinner()
        {
            if(Settings.NumberOfPlayers == 1)
            {
                return null;
            }
            var leaver = Players.FirstOrDefault(p => p.Abandoned);
            if(leaver != null)
            {
                return Players.First(p => p.Player.PlayerId != leaver.Player.PlayerId);
            }
            if (Players[0].Score > Players[1].Score)
            {
                return Players[0];
            }
            if (Players[1].Score > Players[0].Score)
            {
                return Players[1];
            }
            return null;
        }
        internal WordSearchErrorType? CanAddPlayer(Player newPlayer)
        {
            if (_state is not States.WaitingForPlayers)
            {
                return WordSearchErrorType.MatchIsNotAcceptingPlayers;
            }
            if (Players.Count >= Settings.NumberOfPlayers)
            {
                return WordSearchErrorType.MatchIsNotAcceptingPlayers;
            }
            if (Players.Any(p => p.Player.PlayerId == newPlayer.PlayerId))
            {
                return WordSearchErrorType.PlayerAlreadyInAMatch;
            }
            return null;
        }
        internal void AddPlayer(PlayerJoinedEventArgs e)
        {
            var addPlayerError = CanAddPlayer(e.Player.Player);
            if (addPlayerError != null)
            {
                HandleError((WordSearchErrorType) addPlayerError);
            }

            Players.Add(e.Player);
            PlayerJoinedEvent?.Invoke(this, e);
            return;
        }
        internal void StartMatch(MatchStartedEventArgs e)
        {
            StartTime = DateTime.UtcNow;
            MatchStartedEvent?.Invoke(this, e);
        }
        internal void RemovePlayer(PlayerLeftEventArgs e)
        {
            var playerToRemove = Players.FirstOrDefault(p => p.Player.PlayerId == e.PlayerId);
            if(playerToRemove != null)
            {
                if(_state is InProgress)
                {
                    if(Settings.NumberOfPlayers > 1)
                    {
                        playerToRemove.Abandoned = true;
                    }
                }
                else
                {
                    Players.Remove(playerToRemove);
                }
                PlayerLeftEvent?.Invoke(this, e);
            }
        }
        internal void WaitingToStart(WaitingToStartEventArgs e)
        {
            WaitingToStartEvent?.Invoke(this, e);
        }
        internal bool IsFull => Players.Count == Settings.NumberOfPlayers;
        
        internal void WaitingForPlayers(WaitingForPlayersEventArgs e)
        {
            WaitingForPlayersEvent?.Invoke(this, e);
        }
        internal void HandleError(WordSearchErrorType type)
        {
            var e = new WordSearchErrorEventArgs
            {
                ErrorType = type
            };
            ErrorEvent?.Invoke(this, e);
        }

        public event EventHandler<WordSearchErrorEventArgs>? ErrorEvent;
        public event EventHandler<PlayerJoinedEventArgs>? PlayerJoinedEvent;
        public event EventHandler<PlayerLeftEventArgs>? PlayerLeftEvent;
        public event EventHandler<WaitingToStartEventArgs>? WaitingToStartEvent;
        public event EventHandler<MatchStartedEventArgs>? MatchStartedEvent;
        public event EventHandler<MoveMadeEventArgs>? MoveMadeEvent;
        public event EventHandler<MatchEndedEventArgs>? MatchEndedEvent;
        public event EventHandler<WaitingForPlayersEventArgs>? WaitingForPlayersEvent;
    }
}
