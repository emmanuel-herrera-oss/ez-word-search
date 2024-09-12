import { useContext, useState } from "react"
import "./board.css"
import { PlayerProfileContext } from "../PlayerProfileContext"

// playerIdx == 0 must always be the local player.
export interface BoardMove {
    playerId: string,
    start: number[],
    end: number[]
}
export interface BoardParameters {
    board: string[][],
    moves: BoardMove[],
    onPlayerMadeMove: (start: number[], end: number[]) => Promise<void>
}

export const Board = ({board, moves, onPlayerMadeMove }: BoardParameters) => {
    const playerProfileContext = useContext(PlayerProfileContext)
    const GRID_CELL_SIZE = 48
    const GRID_BORDER_SIZE = 1

    const [startCoords, setStartCoords] = useState<number[]>([])
    const [endCoords, setEndCoords] = useState<number[]>([])

    const getGridCoordinatesFromDocumentCoordinates = (clientY: number, clientX: number): number[] => {
        const rect = document.getElementById('ws-grid')?.getBoundingClientRect()
        const y = Math.floor((clientY - (rect?.top ?? 0)) / GRID_CELL_SIZE)
        const x = Math.floor((clientX - (rect?.left ?? 0)) / GRID_CELL_SIZE)
        return [y, x]
    }
    const handleMouseDown = (evt: React.MouseEvent<SVGElement>) => {
        setStartCoords(getGridCoordinatesFromDocumentCoordinates(evt.clientY, evt.clientX))
    }
    const handleMouseUp = async () => {
        if(startCoords.length == 2 && endCoords.length == 2) {
            if((startCoords[0] != endCoords[0]) || (startCoords[1] != endCoords[1])) {
                await onPlayerMadeMove(startCoords, endCoords)
            }
        }
        setStartCoords([])
        setEndCoords([])
    }
    const handleMouseMove = (evt: React.MouseEvent<SVGElement>) => {
        if(startCoords.length == 0) return
        setEndCoords(getGridCoordinatesFromDocumentCoordinates(evt.clientY, evt.clientX))
    }
    const handleMouseLeave = () => {
        setStartCoords([])
        setEndCoords([])
    }
    const generateGrid = (board: string[][], startCoords:number[], endCoords:number[]): JSX.Element[] => {
        const result: JSX.Element[] = []
        // Draw all the words that have already been played
        for(let i = 0;i < moves.length;i++) {
            result.push(<line 
                onPointerDown={handleMouseDown} 
                x1={moves[i].start[1] * GRID_CELL_SIZE + GRID_CELL_SIZE / 2}
                y1={moves[i].start[0] * GRID_CELL_SIZE + GRID_CELL_SIZE / 2}
                x2={moves[i].end[1] * GRID_CELL_SIZE + GRID_CELL_SIZE/2} 
                y2={moves[i].end[0] * GRID_CELL_SIZE + GRID_CELL_SIZE / 2} 
                style={{strokeWidth: GRID_CELL_SIZE / 2, }}
                className={`ws-board-move ${moves[i].playerId == playerProfileContext.playerProfile.playerId ? 'ws-board-move-me' : 'ws-board-move-opponent'}`}/>
            )
        }

        // Draw all of our cells & letters
        for(let row = 0;row < board.length;row++) {
            for(let col = 0;col < board[0].length;col++) {
                const line = <g className="ws-grid-cell" onPointerDown={handleMouseDown} onPointerUp={handleMouseUp} onPointerMove={handleMouseMove} >
                    <text 
                        x={col * GRID_CELL_SIZE + GRID_CELL_SIZE/(2 * GRID_BORDER_SIZE) + GRID_BORDER_SIZE} 
                        y={row * GRID_CELL_SIZE + GRID_CELL_SIZE/(2 * GRID_BORDER_SIZE) + GRID_BORDER_SIZE}
                        dominant-baseline="middle"
                        textAnchor="middle" 
                        fontWeight="bold"
                        fontFamily="Arial">
                            {board[row][col]}
                    </text>
                    <rect 
                        width={GRID_CELL_SIZE} 
                        height={GRID_CELL_SIZE} 
                        x={col * GRID_CELL_SIZE + GRID_BORDER_SIZE} 
                        y={row * GRID_CELL_SIZE + GRID_BORDER_SIZE} 
                        className={startCoords.length == 2 && endCoords.length == 2 ? 'ws-grid-rect-selecting':'ws-grid-rect'}
                    />
                </g>
                result.push(line)
            }
        }

        // Draw the current line that the player is dragging
        if(startCoords.length > 0 && endCoords.length > 0) {
            result.push(<line 
                onPointerUp={handleMouseUp} 
                onPointerMove={handleMouseMove}
                x1={startCoords[1] * GRID_CELL_SIZE + GRID_CELL_SIZE / 2}
                y1={startCoords[0] * GRID_CELL_SIZE + GRID_CELL_SIZE / 2}
                x2={endCoords[1] * GRID_CELL_SIZE + GRID_CELL_SIZE/2}
                y2={endCoords[0] * GRID_CELL_SIZE + GRID_CELL_SIZE / 2}
                className="ws-player-move"
                style={{strokeWidth: GRID_CELL_SIZE / 2}}/>)
        }
        return result
    }

    return (
    <div>    
            <svg 
                style={{touchAction: 'none'}}
                id="ws-grid" //id: Used to calculate size of svg after render. 
                height={board.length * GRID_CELL_SIZE + 2 * GRID_BORDER_SIZE}
                width={board[0].length * GRID_CELL_SIZE + 2 * GRID_BORDER_SIZE}
                xmlns="http://www.w3.org/2000/svg"
                onMouseLeave={handleMouseLeave}>
                    {generateGrid(board, startCoords, endCoords)}
            </svg> 
        
    </div>)
}

