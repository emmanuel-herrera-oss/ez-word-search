export const dateTimeFormat = (dateTime: string):string => {
    if(!dateTime) {
        return ''
    }
    const dateObj = Date.parse(`${dateTime}`)
    return new Intl.DateTimeFormat(undefined, {
        year: 'numeric',
        month: 'numeric',
        day: 'numeric',
        hour: 'numeric',
        minute: 'numeric'
    })
    .format(dateObj)
}