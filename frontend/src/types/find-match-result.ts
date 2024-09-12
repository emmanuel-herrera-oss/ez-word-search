export enum FindMatchResultType {
    Success,
    PlayerAlreadyInAMatch
}
export interface FindMatchResult {
    type: FindMatchResultType
}