using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzWordSearch.Domain.Boards
{
    public class BoardMoveResult
    {
        public required BoardMoveResultType Status { get; set; }
        public string? Word { get; set; }
        public static BoardMoveResult Fail(BoardMoveResultType status)
        {
            if (status == BoardMoveResultType.Success)
            {
                throw new Exception($"Cannot return successful status when calling Fail.");
            }
            return new BoardMoveResult { Status = status };
        }
        public static BoardMoveResult Ok(string word)
        {
            return new BoardMoveResult
            {
                Status = BoardMoveResultType.Success,
                Word = word
            };
        }
    }
    public enum BoardMoveResultType
    {
        Success,
        GameOver,
        InvalidMove,
        AlreadySolved,
        NotASolution
    }
}
