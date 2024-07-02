module Parser

open AST
open Combinator
open System

let combo, comboImpl = recparser()

(* parses a string and returns a Step *)
let step =
    (pstr "plie" |>> (fun _ -> Plie)) <|>
    (pstr "tendu" |>> (fun _ -> Tendu)) <|>
    (pstr "degage" |>> (fun _ -> Degage)) <|>
    (pstr "battement" |>> (fun _ -> Battement)) <|>
    (pstr "eleve" |>> (fun _ -> Eleve)) <|>
    (pstr "retire" |>> (fun _ -> Retire))

(* parses a string and returns a Position *)
let pos =
    (pstr "first" |>> (fun _ -> First)) <|>
    (pstr "second" |>> (fun _ -> Second)) <|>
    (pstr "fifth" |>> (fun _ -> Fifth))

(* parses a string and returns a Direction *)
let dir =
    (pstr "front" |>> (fun _ -> Front)) <|>
    (pstr "side" |>> (fun _ -> Side)) <|>
    (pstr "back" |>> (fun _ -> Back)) <|>
    (pstr "na" |>> (fun _ -> NA))

let pad p = pbetween pws0 p pws0

(* parses a move expression and returns a Move *)
let move =
    pseq  
        (pseq (pad step) (pad pos) (fun (s, p) -> (s, p))) 
        dir
        (fun ((s, p), d) -> { step = s; pos = p; dir = d })

(* parses a numeric char and returns an int representing the  number of repetitions for each move *)
let reps = pdigit |>> (fun c -> int(string c))

let seq = pseq move (pad reps) Sequence

(* parses for one or more Sequence *)
comboImpl := pmany0 seq |>> Series

let grammar = pleft combo peof

let parse input =
    let i = prepare input
    match grammar i with
    | Success(ast, _) -> Some ast
    | Failure(_,_) -> None