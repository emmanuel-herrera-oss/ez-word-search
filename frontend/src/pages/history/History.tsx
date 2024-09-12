import { Button, Table, Title } from "@mantine/core"
import { useContext, useEffect, useState } from "react"
import { PlayerProfileContext } from "../../components/PlayerProfileContext"
import { useNavigate } from "react-router-dom"
import { dateTimeFormat } from "../../util/dateTimeFormat"
import { Match } from "../../types/match"
import { MatchPlayer, MatchPlayerResult } from "../../types/match-player"
import { useWordSearchFetch } from "../../util/useWordSearchFetch"
import { timerFormat } from "../../util/timerFormat"

export const History = () => {
    const playerProfileContext = useContext(PlayerProfileContext)
    const [ history, setHistory ] = useState<Match[]>([])
    const [ loading, setLoading]  = useState(false)
    const [ loadingProfile, setLoadingProfile ] = useState(false)
    const navigate = useNavigate()
    const wsf = useWordSearchFetch()

    useEffect(() => {
        (async () => {
            setLoading(true)
            const history = await wsf.get<Match[]>('/match/history') ?? []
            setHistory(history)
            setLoading(false)
        })()
    }, [])
    const getOpponent = (match: Match) => {
        return match.players.find((i: MatchPlayer) => i.player.playerId != playerProfileContext.playerProfile.playerId)
    }
    const getMe = (match: Match) => {
        const me = match.players.find((i: MatchPlayer) => i.player.playerId == playerProfileContext.playerProfile.playerId)
        return me
    }
    useEffect(() => {
        setLoadingProfile(!playerProfileContext.playerProfile.name)
    }, [playerProfileContext.playerProfile])
    const getMyResult = (match: Match) => {
        if(getMe(match)?.abandoned) {
            return 'Loss (abandoned)'
        }
        else if(getOpponent(match)?.abandoned) {
            return 'Win (opponent abandoned)'
        }
        switch(getMe(match)?.result){
            case MatchPlayerResult.WON:
                return 'Win';
            case MatchPlayerResult.LOST:
                return 'Lost';
            case MatchPlayerResult.TIED:
                return 'Tied'
            default:
                return 'Other'
        }
    }

    const Summary = () => {
        const wins = history.reduce((p, m) => p + (getMe(m)?.result == MatchPlayerResult.WON ? 1 : 0), 0)
        const losses = history.reduce((p, m) => p + (getMe(m)?.result == MatchPlayerResult.LOST ? 1 : 0), 0)
        const ties = history.reduce((p, m) => p + (getMe(m)?.result == MatchPlayerResult.TIED ? 1 : 0), 0)
        return <Table.Tr>
            <Table.Td>{wins}</Table.Td>
            <Table.Td>{losses}</Table.Td>
            <Table.Td>{ties}</Table.Td>
            <Table.Td>{wins + losses + ties}</Table.Td>
            <Table.Td>{(100 * (wins) / (wins + losses + ties)).toFixed(1) + '%'}</Table.Td>
        </Table.Tr>
    }
    return(
    <>
        { (loading || loadingProfile) && <h2>Retrieving your history...</h2>}
        { !loading && 
        <div>
                <Title order={2}>Summary</Title>
                <Table>
                    <Table.Thead>
                        <Table.Tr>
                            <Table.Th>Wins</Table.Th>
                            <Table.Th>Losses</Table.Th>
                            <Table.Th>Ties</Table.Th>
                            <Table.Th>Total Games</Table.Th>
                            <Table.Th>Win Ratio</Table.Th>
                        </Table.Tr>
                    </Table.Thead>
                    <Table.Tbody>
                        <Summary/>
                    </Table.Tbody>
                </Table>
                    <Title order={2}>Your Matches</Title>
                    <div style={{maxHeight: '500px', overflowY: 'auto'}}>
                    <Table>
                        <Table.Thead>
                            <Table.Tr>
                                <Table.Th>Date</Table.Th>
                                <Table.Th>Opponent</Table.Th>
                                <Table.Th>Your Score</Table.Th>
                                <Table.Th>Opponent Score</Table.Th>
                                <Table.Th>Result</Table.Th>
                                <Table.Th>Duration (mm:ss)</Table.Th>
                            </Table.Tr>
                        </Table.Thead>
                        <Table.Tbody>
                            {history.map(val => <Table.Tr>
                                <Table.Td>{dateTimeFormat(val.startTime)}</Table.Td>
                                <Table.Td>{getOpponent(val)?.player.name}</Table.Td>
                                <Table.Td>{getMe(val)?.score}</Table.Td>
                                <Table.Td>{getOpponent(val)?.score}</Table.Td>
                                <Table.Td>{getMyResult(val)}</Table.Td>
                                <Table.Td>{timerFormat(Math.floor(val.duration))}</Table.Td>
                            </Table.Tr>)}
                        </Table.Tbody>
                    </Table>
                </div>
                <br/>
            <Button variant="filled" color="#646f4b" onClick={() => navigate('../profile')}>Back</Button>
        </div>
        }
    </>
    )
}