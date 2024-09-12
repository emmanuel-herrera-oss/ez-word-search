import { Alert, Button, LoadingOverlay, Stack } from "@mantine/core"
import { useContext, useEffect, useState } from "react"
import { useNavigate } from "react-router-dom"
import { DefaultPlayerProfile, PlayerProfile, PlayerProfileContext } from "../../components/PlayerProfileContext"
import { dateTimeFormat } from "../../util/dateTimeFormat"
import { useWordSearchFetch } from "../../util/useWordSearchFetch"
import { FindMatchResult, FindMatchResultType } from "../../types/find-match-result"

export const Profile = () => {
    const wsf = useWordSearchFetch()
    const [ loading, setLoading ] = useState(false)
    const navigate = useNavigate()
    const playerProfileContext = useContext(PlayerProfileContext)
    const [ error, setError ] = useState('')

    useEffect(() => {
        (async () => {
            const profile = await wsf.get<PlayerProfile>('/profile') ?? DefaultPlayerProfile
            playerProfileContext.setPlayerProfile?.(profile);
        })()
    }, [])
    useEffect(() => {
        setLoading(!playerProfileContext.playerProfile.playerId)
    }, [playerProfileContext.playerProfile])

    const findGame = async (singlePlayer: boolean) => {
        setLoading(true)
        const findMatchResult = await wsf.post<FindMatchResult>('/match', {singlePlayer})
        if(findMatchResult?.type === FindMatchResultType.Success) {
            setLoading(false)
            navigate('../match')
        }
        else {
            setLoading(false)
            setError('You\'re already in a match.')
        }
    }
    return(
    <div>
        <LoadingOverlay visible={loading} zIndex={1000} overlayProps={{ radius: "sm", blur: 2 }}/>
        <h3>Welcome <span style={{textDecoration: 'underline'}}>{playerProfileContext.playerProfile.name}</span></h3>
        <p>Wins: {playerProfileContext.playerProfile.wins}<br/>
        Losses: {playerProfileContext.playerProfile.losses}<br/>
        Ties: {playerProfileContext.playerProfile.ties}<br/>
        Last Game: {dateTimeFormat(playerProfileContext.playerProfile.lastMatch)}</p>
        <Stack gap="1rem">
            {error && <Alert variant="light" color="red" withCloseButton onClose={() => setError('')}>{error}</Alert>}
            <Button variant="filled" color="#646f4b" onClick={() => findGame(false)}>Find Match</Button>
            <Button variant="filled" color="#646f4b" onClick={() => findGame(true)}>Practice</Button>
            <Button variant="filled" color="#646f4b" onClick={() => navigate('../history')}>View Match History</Button>
            <Button variant="filled" color="#646f4b" onClick={() => navigate('../leaderboard')}>Leaderboard</Button>
        </Stack>
    </div>
    )
}