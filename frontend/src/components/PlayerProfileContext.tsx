import { createContext } from "react";

export interface PlayerProfile {
    playerId: string,
    name: string,
    wins: number,
    losses: number,
    ties: number,
    lastMatch: string
}

export interface PlayerProfileContextValue {
    playerProfile: PlayerProfile,
    setPlayerProfile: React.Dispatch<React.SetStateAction<PlayerProfile>> | null
}

export const PlayerProfileContext = createContext<PlayerProfileContextValue>({
    playerProfile: {
        playerId: '',
        name: '',
        wins: 0,
        losses: 0,
        ties: 0,
        lastMatch: ''
    },
    setPlayerProfile: null
})

export const DefaultPlayerProfile: PlayerProfile = {
    playerId: '',
    name: '',
    wins: 0,
    losses: 0,
    ties: 0,
    lastMatch: ''
}