import { useEffect } from "react";
import { Button } from "@mantine/core"
import { useNavigate, useSearchParams } from "react-router-dom";
import { useWordSearchFetch } from "../../util/useWordSearchFetch";
import { LoginResult } from "../../types/login-result";
import "./home.css"

const refreshAfterKey = 'WS_REFRESH_AFTER'

const performLogin = () => {
    const url = import.meta.env.VITE_SSO_URL
    const x = window.innerWidth / 2 - 200
    const y = window.innerHeight / 2 - 300
    window.localStorage.setItem(refreshAfterKey, '')
    window.open(url, '__blank', `height=600,width=400,left=${x},top=${y}`)
}
export const Home = () => {
    const [ searchParams ]= useSearchParams()
    const navigate = useNavigate()
    const wsf = useWordSearchFetch()


    const storageEventListener = (evt: StorageEvent) => {
        if(evt.key == refreshAfterKey && evt.newValue) {
            navigate('/play/profile')
        }
    }
    useEffect(() => {
        window.addEventListener('storage', storageEventListener)
        return () => {
            window.removeEventListener('storage', storageEventListener)
        }
    }, [])
    
    useEffect(() => {
        if(searchParams.get('code')) {
            (async() => {
                const loginResult = await wsf.post<LoginResult>('/auth/login', {code: searchParams.get('code')})
                if(!loginResult) {
                    throw new Error('Error logging in.')
                }
                window.localStorage.setItem(refreshAfterKey, loginResult.refreshAfter.toString())
                window.close()
            })()
        }
    }, [searchParams])
    if(searchParams.get('code')) {
        return <p>Logging you in...</p>
    }
    else {
        return (
        <div className="ws-home-container">
            <img src="/demo.png?url" className="ws-home-demo-image"/>
            <div style={{flexGrow: '1'}}>
                <h2>Multiplayer Word Search</h2>
                <h3>See how you stack up against other word search players from around the world!</h3>
                <p>
                    Click the button below to get started.
                </p>
                <Button onClick={performLogin}>Play Now</Button>
            </div>
        </div>
        )
    }
}