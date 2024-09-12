using System.Text;
using System.Text.RegularExpressions;

namespace EzWordSearch.Domain.Boards
{
    public partial class Board
    {
        private readonly char[][] _board = null!;
        private readonly Dictionary<string, bool> _solutions = null!;
        private readonly BoardSettings _boardSettings = null!;
        public Board(BoardSettings boardSettings)
        {
            _boardSettings = boardSettings;
            _board = new char[boardSettings.Height][];
            for (int i = 0; i < _board.Length; i++)
            {
                _board[i] = new char[boardSettings.Width];
            }
            _solutions = new Dictionary<string, bool>(boardSettings.NumberOfWords);
        }
        internal Board() { }

        /// <summary>
        /// Tries to make the given move. If successful, marks the word as solved and further attempts to make a move for the 
        /// same word will fail.
        /// </summary>
        /// <param name="start">row,col coordinates of first letter of the word</param>
        /// <param name="end">row,col coordinates of last letter of the word</param>
        /// <returns>An enum representing the outcome</returns>
        /// <exception cref="Exception"></exception>
        public BoardMoveResult MakeMove(int[] start, int[] end)
        {
            if (GameOver())
            {
                return BoardMoveResult.Fail(BoardMoveResultType.GameOver);
            }
            if (!ValidateMove(start, end))
            {
                return BoardMoveResult.Fail(BoardMoveResultType.InvalidMove);
            }

            var direction = GetDirectionFromPairOfPoints(start[0], start[1], end[0], end[1]);
            int[] coord = [start[0], start[1]];
            StringBuilder sb = new();
            while (coord[0] != end[0] || coord[1] != end[1])
            {
                sb.Append(_board[coord[0]][coord[1]]);
                coord = NextPoint(coord[0], coord[1], direction);
            }
            sb.Append(_board[coord[0]][coord[1]]);
            string searchString = sb.ToString();
            var reversedSearchString = string.Join("", searchString.Reverse());
            bool reverse = false;
            if (!_solutions.TryGetValue(searchString, out var solution))
            {
                if (!_solutions.TryGetValue(reversedSearchString, out solution))
                {
                    return BoardMoveResult.Fail(BoardMoveResultType.NotASolution);
                }
                else
                {
                    reverse = true;
                }
            }
            if (solution)
            {
                return BoardMoveResult.Fail(BoardMoveResultType.AlreadySolved);
            }

            var result = reverse ? reversedSearchString : searchString;
            _solutions[result] = true;
            return BoardMoveResult.Ok(result);
        }
        public char[][] GetBoard()
        {
            char[][] result = new char[_board.Length][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new char[_board[0].Length];
            }
            for (int i = 0; i < result.Length; i++)
            {
                for (int j = 0; j < result[0].Length; j++)
                {
                    result[i][j] = _board[i][j];
                }
            }
            return result;
        }

        public List<string> GetWords()
        {
            return new List<string>(_solutions.Keys);
        }
        private bool ValidateMove(int[] start, int[] end)
        {
            if (start == null || start.Length != 2 || end == null || end.Length != 2)
            {
                return false;
            }
            if (start[0] < 0 || start[1] < 0 || end[0] < 0 || end[1] < 0)
            {
                return false;
            }
            if (start[0] > _board.Length || start[1] > _board[0].Length || end[0] > _board.Length || end[1] > _board[0].Length)
            {
                return false;
            }
            var rise = end[0] - start[0];
            var run = end[1] - start[1];
            if(run != 0)
            {
                var slope = 1.0 * rise / run;
                if(Math.Abs(slope) != 1 && Math.Abs(slope) != 0)
                {
                    return false;
                }
            }
            return true;
        }
        public bool GameOver()
        {
            return !_solutions.Values.Any(i => i == false);
        }
        [GeneratedRegex("^[A-Z]+$")]
        private static partial Regex OnlyLettersRegex();
        private bool IsSuitableWord(string word)
        {
            if (word.Length > _boardSettings.MaxWordLength) return false;
            if (word.Length < _boardSettings.MinWordLength) return false;
            if (_solutions.ContainsKey(word)) return false;
            if (!OnlyLettersRegex().IsMatch(word)) return false;
            return true;
        }
        public void Initialize()
        {
            while(_solutions.Count < _boardSettings.NumberOfWords)
            {
                var placements = GetPlacements();
                string wordToPlace = null!;
                do
                {
                    int randomWordIndex = Random.Shared.Next(0, _boardSettings.Dictionary.Count);
                    wordToPlace = _boardSettings.Dictionary[randomWordIndex].ToUpper();
                }
                while (!IsSuitableWord(wordToPlace));
                while (placements.Count > 0)
                {
                    var randomPlacementIndex = Random.Shared.Next(0, placements.Count);
                    var placement = placements[randomPlacementIndex];
                    placements.RemoveAt(randomPlacementIndex);
                    // TODO: Can we replace this with an interative solution?
                    var placementResult = Place(wordToPlace.ToCharArray(), placement.Item1, placement.Item2, 0, placement.Item3, [[-1, -1], [0, 0]]);
                    if(placementResult)
                    {
                        _solutions.Add(wordToPlace, false);
                        break;
                    }
                }
            }
            // Fill unused spaces with random letters
            for (int i = 0; i < _board.Length; i++)
            {
                for (int j = 0; j < _board[0].Length; j++)
                {
                    if (_board[i][j] == '\0')
                    {
                        _board[i][j] = (char)(Random.Shared.Next(0, 26) + 65);
                    }
                }
            }
        }
        private List<(int, int, Direction)> GetPlacements()
        {
            List<(int, int, Direction)> result = new List<(int, int, Direction)>();
            for (int i = 0; i < _board.Length;i++)
            {
                for(int j = 0;j < _board[0].Length;j++)
                {
                    result.Add((i, j, Direction.LEFT));
                    result.Add((i, j, Direction.UP_LEFT));
                    result.Add((i, j, Direction.UP));
                    result.Add((i, j, Direction.UP_RIGHT));
                    result.Add((i, j, Direction.RIGHT));
                    result.Add((i, j, Direction.RIGHT_DOWN));
                    result.Add((i, j, Direction.DOWN));
                    result.Add((i, j, Direction.DOWN_LEFT));
                }
            }
            return result;
        }
        // Try to place the next letter in the current word in the give direction. Return false if conflict, true otherwise.
        private bool Place(char[] word, int i, int j, int n, Direction direction, int[][] startEnd)
        {
            if (n >= word.Length)
            {
                var previous = PreviousPoint(i, j, direction);
                startEnd[1][0] = previous[0];
                startEnd[1][1] = previous[1];
                return true;
            }
            if (i < 0 || j < 0 || i >= _board.Length || j >= _board[0].Length)
            {
                return false;
            }
            if (_board[i][j] != '\0' && _board[i][j] != word[n])
            {
                return false;
            }
            var tmp = _board[i][j];
            _board[i][j] = word[n];
            var next = NextPoint(i, j, direction);
            if (startEnd[0][0] == -1)
            {
                startEnd[0][0] = i;
                startEnd[0][1] = j;
            }
            var result = Place(word, next[0], next[1], n + 1, direction, startEnd);
            if (!result)
            {
                _board[i][j] = tmp;
                return false;
            }
            return true;
        }
        private static Direction GetDirectionFromPairOfPoints(int startY, int startX, int endY, int endX)
        {
            int yc = endY - startY;
            int xc = endX - startX;

            if (yc < 0 && xc == 0) return Direction.UP;
            if (yc < 0 && xc > 0) return Direction.UP_RIGHT;
            if (yc == 0 && xc > 0) return Direction.RIGHT;
            if (yc > 0 && xc > 0) return Direction.RIGHT_DOWN;
            if (yc > 0 && xc == 0) return Direction.DOWN;
            if (yc > 0 && xc < 0) return Direction.DOWN_LEFT;
            if (yc == 0 && xc < 0) return Direction.LEFT;
            if (yc < 0 && xc < 0) return Direction.UP_LEFT;
            throw new Exception($"Could not determine direction from the given coordinates.");
        }
        private static int[] NextPoint(int i, int j, Direction direction)
        {
            if (direction == Direction.LEFT) return [i, j - 1];
            if (direction == Direction.UP_LEFT) return [i - 1, j - 1];
            if (direction == Direction.UP) return [i - 1, j];
            if (direction == Direction.UP_RIGHT) return [i - 1, j + 1];
            if (direction == Direction.RIGHT) return [i, j + 1];
            if (direction == Direction.RIGHT_DOWN) return [i + 1, j + 1];
            if (direction == Direction.DOWN) return [i + 1, j];
            if (direction == Direction.DOWN_LEFT) return [i + 1, j - 1];
            throw new ArgumentException($"Unexpected direction: {direction}.");
        }
        private static int[] PreviousPoint(int i, int j, Direction direction)
        {
            if (direction == Direction.LEFT) return [i, j + 1];
            if (direction == Direction.UP_LEFT) return [i + 1, j + 1];
            if (direction == Direction.UP) return [i + 1, j];
            if (direction == Direction.UP_RIGHT) return [i + 1, j - 1];
            if (direction == Direction.RIGHT) return [i, j - 1];
            if (direction == Direction.RIGHT_DOWN) return [i - 1, j - 1];
            if (direction == Direction.DOWN) return [i - 1, j];
            if (direction == Direction.DOWN_LEFT) return [i - 1, j + 1];
            throw new ArgumentException($"Unexpected direction: {direction}.");
        }
        private enum Direction
        {
            LEFT,
            UP_LEFT,
            UP,
            UP_RIGHT,
            RIGHT,
            RIGHT_DOWN,
            DOWN,
            DOWN_LEFT
        };
    }
}
