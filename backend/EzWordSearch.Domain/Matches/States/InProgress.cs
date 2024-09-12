using EzWordSearch.Domain.Matches.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.States
{
    internal class InProgress : IMatchState
    {
        private CancellationTokenSource _tokenSource = null!;
        public void Enter(Match match)
        {
            match.StartMatch(new MatchStartedEventArgs 
            {
                Board = match.Board.GetBoard(), // TODO: Standardize on properties instead of methods
                TimeLimitSeconds = match.Settings.TimeLimitSeconds,
                Words = match.Board.GetWords()
            });
            _tokenSource = new CancellationTokenSource();
            Task.Run(() => EnforceMatchTimer(match, _tokenSource.Token));
        }

        public void Exit(Match match)
        {
            _tokenSource.Cancel();
        }

        public IMatchState? HandleEvent(Match match, WordSearchEventArgs e)
        {
            if(e is PlayerLeftEventArgs)
            {
                // TODO: Less ?? throw new.. when casting
                match.RemovePlayer(e as PlayerLeftEventArgs ?? throw new Exception($"Could not cast to {nameof(PlayerLeftEventArgs)}"));
                return new Ended();
            }
            else if(e is MatchTimerExpiredEventArgs)
            {
                return new Ended();
            }
            else if(e is MoveMadeEventArgs)
            {
                match.MakeMove(e as MoveMadeEventArgs ?? throw new Exception($"Could not cast to {nameof(MoveMadeEventArgs)}"));
                return match.Board.GameOver() ? new Ended() : null;
            }
            else
            {
                return null;
            }
        }
        private async Task EnforceMatchTimer(Match match, CancellationToken cancellationToken)
        {
            await Task.Delay(match.Settings.TimeLimitSeconds * 1000, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            match.HandleEvent(new MatchTimerExpiredEventArgs());
        }

        public MatchStateType Type => MatchStateType.InProgress;
    }
}
