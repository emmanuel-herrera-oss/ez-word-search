import { MatchPlayer } from "./match-player";

export interface Match {
    matchId: string,
    players: MatchPlayer[],
    duration: number,
    startTime: string
}