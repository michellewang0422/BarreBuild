open Evaluator
open System
open Parser

[<EntryPoint>]
let main args =
    if Array.length args = 0 then
        printfn "Please provide a combination!"
        1
    else
    let text = args[0]
    //let file = args[0]
    //let text = IO.File.ReadAllText file
    match parse text with
    | Some ast -> 
        printfn "%A" (eval ast)
        0
    | None ->
        printfn "Invalid combination."
        printfn "Usage: dotnet run \"<move> <pos> <dir> <count>...\""
        printfn "Note: Moves that do not take a direction (plie, eleve, retire) should have 'na' for its <dir>."
        printfn "Certain moves cannot be done from second (tendu, degage, battement, retire)."
        1