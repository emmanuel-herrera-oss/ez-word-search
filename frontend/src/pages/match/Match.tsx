import { HttpTransportType, HubConnectionBuilder } from "@microsoft/signalr";
import { useContext, useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom"
import { PlayerProfileContext } from "../../components/PlayerProfileContext"
import { ScoreBoard } from "../../components/ScoreBoard/ScoreBoard";
import { Board, BoardMove } from "../../components/Board/Board";
import { WordList, WordListWord } from "../../components/WordList/WordList";
import { Button, Center, Group, Stack, Title } from "@mantine/core";

import { MatchPlayer } from "../../types/match-player"
import * as Events from "../../types/events"
import { MatchSnapshot, MatchStateType } from "../../types/match-snapshot";
import { timerFormat } from "../../util/timerFormat";
import { accurateTimer } from "../../util/accurateTimer";

export const Match = () => {
    const playerProfileContext = useContext(PlayerProfileContext)

    const [board, setBoard] = useState<string[][]>([])
    const [moves, setMoves] = useState<BoardMove[]>([])
    const [words, setWords] = useState<WordListWord[]>([])
    const [lastMove, setLastMove] = useState(Events.MoveMadeEventDefault)
    const [matchEnding, setMatchEnding] = useState<Events.MatchEndedEvent | null>(null)

    const [playersRaw, setPlayersRaw] = useState<MatchPlayer[]>([])
    const [playersOrdered, setPlayersOrdered] = useState<MatchPlayer[]>([])
    const [leaver, setLeaver] = useState<Events.PlayerLeftEvent | null>(null)

    
    const [gameState, setGameState] = useState(MatchStateType.WaitingForPlayers)

    const [startTimer, setStartTimer] = useState(0)
    const [timeRemaining, setTimeRemaining] = useState(0)
    const cancelStartTimerFn = useRef<(() => void)| null>(null)
    const cancelMatchTimerFn = useRef<(() => void) | null>(null)

    const navigate = useNavigate()

    const [error, setError] = useState('')

    const connection = useRef(new HubConnectionBuilder()
        .withUrl(import.meta.env.VITE_SIGNALR_URL, {
            skipNegotiation: true,
            transport: HttpTransportType.WebSockets
        })
        .build())

    const processServerEvents = (e: Events.WordSearchEvent) => {
        if (e.type == Events.WordSearchEventType.WaitingForPlayers) {
            setGameState(MatchStateType.WaitingForPlayers)
        }
        else if (e.type == Events.WordSearchEventType.PlayedJoined) {
            setPlayersRaw((previousValue) => [(e as Events.PlayerJoinedEvent).player, ...previousValue])
        }
        else if (e.type == Events.WordSearchEventType.WaitingToStart) {
            if(cancelStartTimerFn.current != null) {
                cancelStartTimerFn.current()
            }
            setStartTimer((e as Events.WaitingToStartEvent).secondsToWait)
            const cancelFn = accurateTimer(() => {
                setStartTimer((previousValue) => previousValue - 1)
            }, 1000)
            cancelStartTimerFn.current = cancelFn.cancel
            setGameState(MatchStateType.WaitingToStart)
        }
        else if (e.type == Events.WordSearchEventType.PlayerLeft) {
            const event = e as Events.PlayerLeftEvent
            setLeaver(event)
        }
        else if (e.type == Events.WordSearchEventType.MatchStarted) {
            const event = e as Events.MatchStartedEvent
            setBoard(event.board)
            setWords(event.words.map(i => ({ playerId: undefined, word: i })))
            if(cancelMatchTimerFn.current != null) {
                cancelMatchTimerFn.current()
            }
            setTimeRemaining(event.timeLimitSeconds)
            const cancelFn = accurateTimer(() => {
                setTimeRemaining((prevValue) => prevValue - 1)
            }, 1000);
            cancelMatchTimerFn.current = cancelFn.cancel
            setGameState(MatchStateType.InProgress)
        }
        else if (e.type == Events.WordSearchEventType.MoveMade) {
            setLastMove(e as Events.MoveMadeEvent)
        }

        else if (e.type == Events.WordSearchEventType.MatchEnded) {
            setMatchEnding(e as Events.MatchEndedEvent)
            setGameState(MatchStateType.Ended)
        }
        else if(e.type == Events.WordSearchEventType.Error) {
            connection.current.stop()
            setError('Either you\'re already in a match, or this match is no longer accepting players.')
        }
        else {
            console.log('---Unknown event---')
            console.log(e)
            console.log('-------------------')
        }
    }

    useEffect(() => {
        const conn = connection.current
        conn.on("ReceiveMessage", (message) => {
            processServerEvents(message)
        })
        conn.onclose(() => {
            setError('The server is down for maintenance, sorry!')
        })
        conn.start().then(() => connection.current.invoke("GetSnapshot")).then((snapshot: MatchSnapshot | null) => {
            if (snapshot == null || snapshot.state == MatchStateType.Ended) {
                navigate('../profile')
                return
            }
            setPlayersRaw(snapshot.players)
            if(snapshot.state == MatchStateType.WaitingToStart){
                setStartTimer(snapshot.startTimer)
                const cancelFn = accurateTimer(() => {
                    setStartTimer((previousValue) => previousValue - 1)
                }, 1000)
                cancelStartTimerFn.current = cancelFn.cancel
                setGameState(MatchStateType.WaitingToStart)
            }
        })
        return () => {
            conn.stop();
            if (cancelMatchTimerFn.current != null) {
                cancelMatchTimerFn.current()
            }
            if (cancelStartTimerFn.current != null) {
                cancelStartTimerFn.current()
            }
        }
    }, [])

    useEffect(() => {
        if(gameState < MatchStateType.InProgress){
            setPlayersRaw((previousValue) => previousValue.filter(p => p.player.playerId != leaver?.playerId))
        }
        else {
            const playerThatLeft = playersRaw.find(p => p.player.playerId == leaver?.playerId)
            if(playerThatLeft) {
                playerThatLeft.abandoned = true
                setPlayersRaw([...playersRaw])
            }
        }
    }, [leaver])
    useEffect(() => {
        const wordIndex = words.findIndex(i => i.word == lastMove.word)
        if (wordIndex < 0) return
        words[wordIndex].playerId = lastMove.playerId
        setWords([...words])

        moves.push({ playerId: lastMove.playerId, start: lastMove.start, end: lastMove.end })
        setMoves([...moves])

        const playerIdx = playersRaw.findIndex(i => i.player.playerId == lastMove.playerId)
        playersRaw[playerIdx].score++;
        setPlayersRaw([...playersRaw])

        if(lastMove.playerId == playerProfileContext.playerProfile.playerId) {
            const audio = new Audio('friendly-sound.mp3?url');
            audio.play();
        }
        else {
            const audio = new Audio('enemy-sound.mp3?url');
            audio.play();
        }
    }, [lastMove]
    )

    useEffect(() => {
        const sorted = playersRaw.sort(a => a.player.playerId == playerProfileContext.playerProfile.playerId ? -1 : 1)
        setPlayersOrdered(sorted ?? [])
    }, [playersRaw])

    const handleMoveAttempt = async (start: number[], end: number[]) => {
        connection.current.invoke('MakeMove', start, end)
    }

    const LeaveMatch = async () => {
        if (gameState == MatchStateType.InProgress) {
            await connection.current.invoke('LeaveMatch')
        }
        else if (gameState == MatchStateType.WaitingForPlayers || gameState == MatchStateType.WaitingToStart) {
            await connection.current.invoke('LeaveMatch')
            navigate('../profile')
        }
    }

    const EndGameReport = () => {
        if (matchEnding == null || matchEnding?.singlePlayer) {
            return <>
                <Title order={2}>Practice over!</Title>
                <Title order={3}>{`You got ${playersOrdered[0].score} out of ${words.length} words.`}</Title><br />
                <Button variant="filled" color="#646f4b" onClick={() => navigate('../profile')}>Return to Profile</Button>
            </>
        }
        if (!playersOrdered || playersOrdered.length == 0 || matchEnding?.leaverId == playerProfileContext.playerProfile.playerId) {
            return <>
                <Title order={2}>You abandoned the match!</Title>
                <Title order={3}>This will count as a loss.</Title><br />
                <Button variant="filled" color="#646f4b" onClick={() => navigate('../profile')}>Return to Profile</Button>
            </>
        }
        if(matchEnding?.leaverId != null && matchEnding?.leaverId != playerProfileContext.playerProfile.playerId) {
            return <>
            <Title order={2}>You won!</Title>
            <Title order={3}>Your opponent left the match early.</Title><br />
            <Button variant="filled" color="#646f4b" onClick={() => navigate('../profile')}>Return to Profile</Button>
        </>
        }
        
        const winnerId = matchEnding?.winnerId
        let outcomeString = ''
        if (typeof(winnerId) === 'undefined') {
            outcomeString = ''
        }
        else if (winnerId === null) {
            outcomeString = 'You tied!'
        }
        else if (winnerId == playerProfileContext.playerProfile.playerId) {
            outcomeString = 'You won!'
        }
        else {
            outcomeString = 'You lost...better luck next time!'
        }
        const scoreString = `The final score was ${playersOrdered[0].score} to ${playersOrdered[1].score}`

        return <>
            <Title order={2}>{outcomeString}</Title>
            <Title order={3}>{scoreString}</Title><br />
            <Button variant="filled" color="#646f4b" onClick={() => navigate('../profile')}>Return to Profile</Button>
        </>
    }
    if(error) {
        return <div>
            <>
                    <Title order={2}>Error</Title><br />
                    <Title order={3}>{error}</Title><br/>
                    <Center><Button variant="filled" color="#646f4b" onClick={() => navigate('../profile')}>Leave</Button></Center>
                </>
        </div>
    }
    return (
        <div>
            {
                gameState == MatchStateType.InProgress &&
                <Stack>
                    <Center>{timerFormat(timeRemaining)}</Center>
                    <Group gap={'1rem'} align="top">
                        <ScoreBoard players={playersOrdered} />
                        <Board board={board} moves={moves} onPlayerMadeMove={handleMoveAttempt}></Board>
                        <WordList wordList={words} />
                    </Group>
                    <Center>
                        <Button variant="filled" color="#646f4b" onClick={() => LeaveMatch()}>Leave</Button>
                    </Center>
                </Stack>
            }
            {
                gameState == MatchStateType.WaitingForPlayers &&
                <>
                    <Title order={2}>Waiting for players...</Title><br />
                    <Center>
                        <Button variant="filled" color="#646f4b" onClick={() => LeaveMatch()}>Leave</Button>
                    </Center>
                </>
            }
            {
                gameState == MatchStateType.WaitingToStart &&
                <>
                    <Title order={2}>Get Ready! Starting in {startTimer} seconds...</Title><br />
                    <Center><Button variant="filled" color="#646f4b" onClick={() => LeaveMatch()}>Leave</Button></Center>
                </>
            }
            {
                gameState == MatchStateType.Ended && <EndGameReport />
            }
        </div>
    )
}