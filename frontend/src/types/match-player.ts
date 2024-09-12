import { Player } from "./player";

export enum MatchPlayerResult {
    WON,
    LOST,
    TIED
}

export interface MatchPlayer {
    player: Player,
    score: number,
    result: MatchPlayerResult,
    abandoned: boolean
    wordsPlayed: string[]
}