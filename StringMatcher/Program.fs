﻿open System
open System.Diagnostics
open StringMatcher
open Result

let printGravizoLink automaton =
    let dotscript = AutomatonPrinter.toDot automaton
    printfn "%s%s" "https://g.gravizo.com/svg?" (Uri.EscapeDataString dotscript)

let gatherUserInputs () =
    printf "Glob pattern: "
    let pattern = Console.ReadLine()
    printf "Text: "
    let text = Console.ReadLine()
    printf "Print graph of automaton? (y/n) "
    let print = Console.ReadKey(true).KeyChar = 'y'
    printfn ""
    (pattern, text, print)

let exit code =
    if Debugger.IsAttached then Console.ReadKey(true) |> ignore
    code

let getInputs (args:string[]) =
    match args.Length with
    | 1 when args.[0] = "--interactive" || args.[0] = "-i" -> 
        gatherUserInputs ()
    | 2 -> (args.[0], args.[1], false)
    | 3 when args.[2] = "--printGraph" || args.[2] = "-p" -> 
        (args.[0], args.[1], true)
    | _ -> 
        printfn "Usage: StringMatcher.exe <pattern string> <test string> [--printGraph or -p]"
        printfn "       StringMatcher.exe [--interactive or -i]"
        gatherUserInputs ()

[<EntryPoint>]
let main argv = 
    let (pattern, text, printGraph) = getInputs argv
    match GlobParser.toAutomaton pattern with
    | Failure msg -> 
        printfn "Incorrect pattern:\n%s" msg
        exit 2
    | Success a ->
        if printGraph then printGravizoLink a
        let result = Automaton.run a text
        printfn "Match: %A" result
        exit (if result then 0 else 1)