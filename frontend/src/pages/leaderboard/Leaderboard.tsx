import { Button, Table } from "@mantine/core"
import { useEffect, useState } from "react"
import { useNavigate } from "react-router-dom"
import { Player } from "../../types/player"
import { dateTimeFormat } from "../../util/dateTimeFormat"
import { useWordSearchFetch } from "../../util/useWordSearchFetch"

export const Leaderboard = () => {
    const wsf = useWordSearchFetch()
    const [ leaderboard, setLeaderboard ] = useState<Player[]>([])
    useEffect(() => {
        (async () => {
            const leaderboard = await wsf.get<Player[]>('/leaderboard')
            setLeaderboard(leaderboard ?? [])
        })()
    },[])
    const navigate = useNavigate()

    const calculateWinRatio = (player: Player): number => {
        return 100 * (player.wins) / (player.wins + player.losses + player.ties)
    }
    return (
    <div>
        { leaderboard.length == 0 && <h2>Retrieving leaderboard...</h2> }
        { leaderboard.length > 0 && 
        <div>
            <h2>Top 100 Players</h2>
            <div style={{maxHeight: '600px', overflowY: 'auto'}}>
            <Table>
                <Table.Thead>
                    <Table.Tr>
                        <Table.Th>Rank</Table.Th>
                        <Table.Th>Username</Table.Th>
                        <Table.Th>Wins</Table.Th>
                        <Table.Th>Losses</Table.Th>
                        <Table.Th>Ties</Table.Th>
                        <Table.Th>Win Ratio</Table.Th>
                        <Table.Th>Games Played</Table.Th>
                        <Table.Th>Last Match</Table.Th>
                    </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                    {leaderboard.map((val, idx) => <Table.Tr>
                        <Table.Td>{idx + 1}</Table.Td>
                        <Table.Td>{val.name}</Table.Td>
                        <Table.Td>{val.wins}</Table.Td>
                        <Table.Td>{val.losses}</Table.Td>
                        <Table.Td>{val.ties}</Table.Td>
                        <Table.Td>{calculateWinRatio(val).toFixed(1)}%</Table.Td>
                        <Table.Td>{val.wins + val.losses + val.ties}</Table.Td>
                        <Table.Td>{dateTimeFormat(val.lastMatch)}</Table.Td>
                    </Table.Tr>)}
                </Table.Tbody>
            </Table>
            </div>
            <Button variant="filled" color="#646f4b" onClick={() => navigate('../profile')}>Back</Button>
        </div>
        }
    </div>
    )
}