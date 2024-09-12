import { Outlet, useLocation, useNavigate } from "react-router-dom"
import './root.css'
import { DefaultPlayerProfile, PlayerProfile, PlayerProfileContext } from "../../components/PlayerProfileContext"
import { useContext, useEffect, useState } from "react"
import { Stack } from "@mantine/core"
import { useWordSearchFetch } from "../../util/useWordSearchFetch"
export const Root = () => {
    const playerProfileContext = useContext(PlayerProfileContext)
    const wsf = useWordSearchFetch()
    const location = useLocation()
    useEffect(() => {
        (async () => {
            if(location.pathname != '/' && location.pathname != '/play/home' && location.pathname != '/play/profile') {
                const profile = await wsf.get<PlayerProfile>('/profile') ?? DefaultPlayerProfile
                playerProfileContext.setPlayerProfile?.(profile)
            }
        })()
    }, [])
    return (
    <section className="ws-root-layout">
        <div className="ws-root-header-container"><div><Header/></div></div>
        <div className="ws-root-content-container"><Outlet/></div>
        <div><Footer/></div>
    </section>
    )
}

const Header = () => {
    const playerCountRefreshInterval = 30000
    const playerProfileContext = useContext(PlayerProfileContext)
    const wsf = useWordSearchFetch()
    const [ playerCount, setPlayerCount ] = useState(0)
    const navigate = useNavigate()
    const updatePlayerCount = async () => {
        const count = await wsf.get<number>('/match/count') ?? 0
        setPlayerCount(count)
    }
    useEffect(() => {
        updatePlayerCount();
        const intervalId = setInterval(() => {
            updatePlayerCount()
        }, playerCountRefreshInterval)
        return () => {
            clearInterval(intervalId)
        }
    }, [])
    const logout = async () => {
        await wsf.post('/auth/logout')
        playerProfileContext.setPlayerProfile?.(DefaultPlayerProfile)
        navigate('/')
    }
    return (
    <header className="ws-root-header">
        <img src="logo.png?url" style={{cursor: 'pointer'}} onClick={() => navigate('/play/profile')}/>
        <Stack gap="0.5rem">
            {Boolean(playerProfileContext?.playerProfile.name) && <div>Welcome, <span style={{textDecoration: 'underline'}}>{playerProfileContext?.playerProfile.name}</span></div>}
            {Boolean(playerProfileContext?.playerProfile.name) && <span style={{textDecoration: 'underline', cursor: 'pointer'}} onClick={logout}>Log Out</span>}
            <div>There are currently {playerCount} players online</div>
        </Stack>
    </header>
    )
}

const Footer = () => {
    return (
    <footer className="ws-root-footer">
        <span>&copy; EzWordSearch.com</span>
    </footer>
    )
}