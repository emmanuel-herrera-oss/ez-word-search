using EzWordSearch.Domain.Matches.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches.States
{
    internal class WaitingToStart : IMatchState
    {
        private CancellationTokenSource _tokenSource = null!;
        internal DateTime StartAt { get; private set; }
        public void Enter(Match match)
        {
            match.WaitingToStart(new WaitingToStartEventArgs { SecondsToWait = match.Settings.SecondsToWaitBeforeMatchStart });
            _tokenSource = new CancellationTokenSource();
            Task.Run(() => StartMatch(match, _tokenSource.Token));
        }

        public void Exit(Match match)
        {
            _tokenSource.Cancel();
        }

        public IMatchState? HandleEvent(Match match, WordSearchEventArgs e)
        {
            if(e is PlayerLeftEventArgs)
            {
                var playerLeftEventArgs = e as PlayerLeftEventArgs ?? throw new Exception($"Could not cast to {nameof(PlayerLeftEventArgs)}");
                match.RemovePlayer(playerLeftEventArgs);
                return new WaitingForPlayers();
            }
            else if(e is MatchStartTimerExpiredEventArgs)
            {
                return new InProgress();
            }
            else
            {
                return null;
            }
        }

        private async Task StartMatch(Match match, CancellationToken cancellationToken)
        {
            StartAt = DateTime.UtcNow.AddSeconds(match.Settings.SecondsToWaitBeforeMatchStart);
            await Task.Delay(match.Settings.SecondsToWaitBeforeMatchStart * 1000, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            match.HandleEvent(new MatchStartTimerExpiredEventArgs());
        }
        public MatchStateType Type => MatchStateType.WaitingToStart;
    }
}
