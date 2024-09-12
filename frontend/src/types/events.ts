import { MatchPlayer } from "./match-player"

export enum WordSearchEventType {
    WaitingToStart,
    PlayedJoined,
    MatchStarted,
    MoveMade,
    MatchEnded,
    PlayerLeft,
    Error,
    MatchStartTimerExpired,
    MatchTimerExpired,
    WaitingForPlayers
}

export interface WordSearchEvent {
    type: WordSearchEventType
}

export interface MatchEndedEvent extends WordSearchEvent {
    winnerId: string,
    leaverId: string
    singlePlayer: boolean
}

export interface MatchStartedEvent extends WordSearchEvent {
    board: string[][],
    words: string[],
    timeLimitSeconds: number
}

export interface MoveMadeEvent extends WordSearchEvent {
    playerId: string,
    word: string,
    start: number[],
    end: number[]
}

export interface PlayerJoinedEvent extends WordSearchEvent {
    player: MatchPlayer
}

export interface PlayerLeftEvent extends WordSearchEvent {
    playerId: string
}

export interface WaitingToStartEvent extends WordSearchEvent {
    secondsToWait: number
}

export const MoveMadeEventDefault: MoveMadeEvent = {
    playerId: '',
    word: '',
    start: [],
    end: [],
    type: WordSearchEventType.MoveMade
}
