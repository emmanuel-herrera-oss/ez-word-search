using EzWordSearch.Domain.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Matches
{
    public class MatchPlayer
    {
        // TODO: Can we get rid of extra PlayerId? Already have it in MatchPlayer.Player.PlayerId. Only here for EF.
        public Guid MatchPlayerId { get; set; }
        public Guid MatchId { get; set; }
        //public Guid PlayerId { get; set; }
        public required Player Player { get; set; }
        public int Score { get; set; }
        public bool Abandoned { get; set; }
        public MatchPlayerResultType Result { get; set; }
        private MatchPlayer() { }
        public static MatchPlayer Create(Match match, Player player)
        {
            return new MatchPlayer
            {
                Player = player,
                MatchId = match.MatchId,
                //PlayerId = player.PlayerId
            };
        }
    }
    public enum MatchPlayerResultType
    {
        WON,
        LOST,
        TIED
    }
}
