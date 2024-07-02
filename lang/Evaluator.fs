module Evaluator

open AST
open SixLabors.ImageSharp
open System.IO
open System
open System.Diagnostics

(* Prints out barre combos all nice looking. *)

(* Converts step, position, direction objects to String for printing *)
let stepprint step =
    match step with
    | Plie -> "plie"
    | Tendu -> "tendu"
    | Degage -> "degage"
    | Battement -> "battement"
    | Eleve -> "eleve"
    | Retire -> "retire"

let posprint pos =
    match pos with
    | First -> "first"
    | Second -> "second"
    | Fifth -> "fifth"

let dirprint dir =
    match dir with
    | Front -> "front"
    | Side -> "side"
    | Back -> "back"
    | NA -> "na"

let rec prettyprint c =
    match c with
    | Sequence(move, count) when count > 1 ->
        (stepprint move.step) + " to the " + (dirprint move.dir) +  " in " + (posprint move.pos) + ", " + prettyprint (Sequence(move, count-1))
    | Sequence(move, count) when count <= 1 ->
        (stepprint move.step) + " to the " + (dirprint move.dir) +  " in " + (posprint move.pos) + ", "
    | Series(seq::seqs) ->
        (prettyprint seq) + (prettyprint (Series(seqs)))
    | _ -> ""


(* helper function for getframes to move the two frames for each move into a new directory*)
let getmove (frame1: string) (frame2: string) (source: string) (target: string) (count: int) : unit =
    use image1 = Image.Load (source + frame1)
    let imagePath1 = target + (count.ToString("D3")) + "a.jpg"
    image1.Save (imagePath1)

    use image2 = Image.Load (source + frame2)
    //let imagePath2 = target + string(count) + "b.jpg" 
    let imagePath2 = target + (count.ToString("D3")) + "b.jpg"
    image2.Save (imagePath2)

(* recursively gets the correct frames *)
let rec getframes (c: Combo) (source: string) (target: string) (frameCount: int) : int =
    match c with
    | Sequence(move, count) when count <= 1 ->
        let frame1 = (stepprint move.step) + "_" + (dirprint move.dir) +  "_" + (posprint move.pos) + "1.JPG"
        let frame2 = (stepprint move.step) + "_" + (dirprint move.dir) +  "_" + (posprint move.pos) + "2.JPG"
        getmove frame1 frame2 source target frameCount
        frameCount + 1
    | Sequence(move, count) when count > 1 ->
        let frame1 = (stepprint move.step) + "_" + (dirprint move.dir) +  "_" + (posprint move.pos) + "1.JPG"
        let frame2 = (stepprint move.step) + "_" + (dirprint move.dir) +  "_" + (posprint move.pos) + "2.JPG"
        getmove frame1 frame2 source target frameCount
        getframes (Sequence(move, count-1)) source target (frameCount + 1)
    | Series(seq::seqs) ->
        let newFrameCount = getframes seq source target frameCount
        getframes (Series(seqs)) source target newFrameCount
    | Series([]) -> 0
    | _ -> -1

let genvideo (path: string) (target: string) (output: string) (fps: int) : unit =
    // Set the path to the FFmpeg executable
    let ffmpegPath = path + "/ffmpeg"  // Assumes 'ffmpeg' is in the system PATH

    // Specify the input pattern for image files (e.g., '*.jpg')
    let inputPattern = Path.Combine(target, "*.jpg")

    // Construct the FFmpeg command to create a video from images
    let arguments = sprintf "-framerate %d -pattern_type glob -i \"%s/*.jpg\" -c:v libx264 -pix_fmt yuv420p %s" fps target output

    // Start a new process to execute the FFmpeg command
    let processStartInfo = new ProcessStartInfo(FileName = ffmpegPath, Arguments = arguments, CreateNoWindow = true)
    let ffmpegprocess = new Process()
    ffmpegprocess.StartInfo <- processStartInfo
    ffmpegprocess.Start() |> ignore
    ffmpegprocess.WaitForExit()

    printfn "Video created successfully: %s" output

(* clear the directory for a new combo *)
let clearDirectory target : unit =
    let files = Directory.GetFiles(target)
    for file in files do
        File.Delete(file)

let path = __SOURCE_DIRECTORY__
let source = path + "/MoveLibrary/"
let target = path + "/Combo/"

let eval c =
    clearDirectory target
    getframes c source target 0 |> ignore
    genvideo path target "Combo.mp4" 2 |> ignore
    prettyprint c