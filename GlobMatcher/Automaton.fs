﻿namespace GlobMatcher

type UniqueId = UniqueId of string
type Accept = Accept | Continue
type State = State of UniqueId * Accept
type Word = Word of char | Epsilon
type Transition = {Start: State; End: State; Accepts: Word}
type Automaton = Automaton of State * Transition list

module Automaton =
    open Util

    let private accepts word {Accepts = word'} = word' = word

    let private isOutgoingFrom state {Start = start} = state = start

    let private expandStates selector = List.map selector >> List.concat >> removeDuplicates

    let private getReachable word transitions state =
        transitions
        |> List.filter (isOutgoingFrom state) 
        |> List.filter (accepts word) 
        |> List.map (fun {End = nextState} -> nextState)

    let private consume word transitions =
        expandStates (getReachable word transitions)

    let private hasFinal states = 
        let isFinal = function
            | State (_, Accept) -> true
            | _ -> false
        states |> List.exists isFinal

    let rec private addEpsilonReachable transitions added =
        let addEpsilonReachable' state =
            let added' = getReachable Epsilon transitions state |> List.except (state::added)
            match added' with 
            | [] -> [state]
            | _ -> state::addEpsilonReachable transitions (added@added') added'
        expandStates addEpsilonReachable'

    let run (Automaton (initial, transitions)) text =
        let rec run' (text:string) current =
            let current' = addEpsilonReachable transitions [] current
            if text.Length = 0 then
                hasFinal current'
            else
                let next = consume (Word text.[0]) transitions current'
                run' text.[1..] next
        run' text [initial]
