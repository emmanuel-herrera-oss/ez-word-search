import { useContext } from "react"
import { NavigateFunction, useNavigate } from "react-router-dom"
import { DefaultPlayerProfile, PlayerProfileContext, PlayerProfileContextValue } from "../components/PlayerProfileContext"

// TODO: Base Path
export class WordSearchFetch {
    private loginUrl: string
    private navigate: NavigateFunction
    private playerProfileContext: PlayerProfileContextValue
    constructor(navigate: NavigateFunction, playerProfileContext: PlayerProfileContextValue) {
        this.navigate = navigate
        this.loginUrl = '/'
        this.playerProfileContext = playerProfileContext
    }
    async get<T>(url: string): Promise<T | undefined> {
        url = `${import.meta.env.VITE_API_URL}${url}`
        const response = await fetch(url, {
            method: 'GET',
            credentials: 'include'            
        })
        return await this.handleResponse<T>(response)
    }
    async post<T>(url: string, payload = {}): Promise<T | undefined> {
        url = `${import.meta.env.VITE_API_URL}${url}`
        const response = await fetch(url, {
            method: 'POST',
            credentials: 'include',
            body: JSON.stringify(payload),
            headers: {
                "Content-Type": 'application/json'
            }
        })
        return await this.handleResponse<T>(response)
    }
    private async handleResponse<T>(response: Response): Promise<T | undefined>{
        if(response.status == 401 || response.status == 403) {
            this.playerProfileContext.setPlayerProfile?.(DefaultPlayerProfile)
            this.navigate(this.loginUrl)
            throw new Error('Your session has expired') //Need this to stop whatever code came after fetch
        }
        else if(response.ok){
            if(response.status != 204) {
                const result = await response.json()
                return result as T
            }
        }
        else {
            throw new Error(`Got status ${response.status} from server.`)
        }
    }
}
export const useWordSearchFetch = () => {
    const playerProfileContext = useContext(PlayerProfileContext)
    const navigate = useNavigate()
    return new WordSearchFetch(navigate, playerProfileContext)
}