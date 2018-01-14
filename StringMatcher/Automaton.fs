﻿namespace StringMatcher

type Id = Id of string

type Letter = 
    | Letter of char
    | Range of char * char 
    | Any

type State = 
    | State of Id * Letter * State
    | Split of Id * State * State
    | Final

module Automaton =

    let getId = function
        | State (id, _, _) -> id
        | Split (id, _, _) -> id
        | Final            -> Id "Final"
    
    let rec private step letter state =
        match state, letter with
        | Split (_, left, right)           , l        -> step l left @ step l right
        | State (_, Any, next)             , _        -> [next]
        | State (_, Letter c', next)       , Letter c          
            when c' = c                               -> [next]
        | State (_, Range (min, max), next), Letter c 
            when min <= c && c <= max                 -> [next]
        | _                                           -> []

    let rec private expandEpsilons = function
        | Split (_, left, right) -> expandEpsilons left @ expandEpsilons right
        | state -> [state]
    
    let private consume currents letter =
       currents 
       |> List.collect (step letter) 
       |> List.collect expandEpsilons
       |> List.distinctBy getId

    let run start = 
        Seq.map Letter 
        >> Seq.fold consume (expandEpsilons start)
        >> List.contains Final
