import { MatchPlayer } from "./match-player";

export enum MatchStateType {
    WaitingForPlayers,
    WaitingToStart,
    InProgress,
    Ended
}

export interface MatchSnapshot {
    players: MatchPlayer[],
    state: MatchStateType,
    startTimer: number
}

