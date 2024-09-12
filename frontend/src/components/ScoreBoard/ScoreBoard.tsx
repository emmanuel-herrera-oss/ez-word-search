import { MatchPlayer } from "../../types/match-player"
import "./score-board.css"

export const ScoreBoard = ({players}: {players: MatchPlayer[]}) => {
    return <div>
        <span className="ws-sb-title">Scores</span>
        <hr></hr>
        <div className="ws-sb-score-container">
            {players.map(p => <div>{p.player.name}: {p.score}</div>)}
        </div>
    </div>
}