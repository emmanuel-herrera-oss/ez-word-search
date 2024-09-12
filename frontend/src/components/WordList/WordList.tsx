import { useContext } from "react"
import { PlayerProfileContext } from "../PlayerProfileContext"
import "./word-list.css"

export interface WordListWord {
    playerId: string | undefined,
    word: string
}
export interface WordListParameters {
    wordList: WordListWord[]
}

const WordComponent = ({wordListWord}: {wordListWord: WordListWord}) => {
    const playerProfileContext = useContext(PlayerProfileContext)
    if(typeof(wordListWord.playerId) === 'undefined') {
        return <span className="ws-wl-word">{wordListWord.word}</span>
    }
    if(wordListWord.playerId === playerProfileContext.playerProfile.playerId) {
        return (
            <span className="ws-wl-strikethrough ws-wl-me">
                <span className="ws-wl-word ws-wl-me">{wordListWord.word}</span>
            </span>
        )
    }
    return (
        <span className="ws-wl-strikethrough ws-wl-opponent">
            <span className="ws-wl-word ws-wl-opponent">{wordListWord.word}</span>
        </span>
    )
}

export const WordList = ({wordList}: WordListParameters) => {
    return <div>
        <span className="wl-title">Word List</span>
        <hr></hr>
        <div className="ws-wl-container">
            {wordList.map(w => <WordComponent wordListWord={w}/>)}
        </div>
    </div>
}