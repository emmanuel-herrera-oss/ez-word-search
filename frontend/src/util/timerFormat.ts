export const timerFormat = (seconds: number) => {
    return `${Math.floor(seconds / 60)}:${(seconds % 60).toFixed(0).padStart(2, '0')}`
}