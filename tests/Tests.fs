namespace tests

open System.IO
open Parser
open Evaluator
open AST
open Combinator
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.TestMethodPassing () =
        Assert.IsTrue(true);

    [<TestMethod>]
    member this.ValidComboReturnsPrettyPrintString () =
        let input = "tendu first front 1"
        let expected = "tendu to the front in first, "
        let result = parse input
        match result with
        | Some res ->
            Assert.AreEqual(expected, (eval res))
        | None ->
            Assert.IsTrue false

    member this.ValidComboReturnsVideo () =
        let input = "tendu first front 1"
        let result = parse input
        match result with
        | Some res ->
            // need to clear video before eval
            eval res |> ignore
            let directoryPath = @"path\to\your\directory"
            let fileExists = Directory.EnumerateFiles("Combo.mp4") |> Seq.isEmpty |> not
            Assert.IsTrue(fileExists, "Expected evaluator to generate Combo.mp4 file")
        | None ->
            Assert.IsTrue false

    member this.ParsesValidStep () =
        let input = prepare "battement fifth front 2"
        let expected = Success (Battement, ("battement fifth front 2", 9, false))
        let result = step input
        Assert.AreEqual(expected, result)

    member this.ParsesValidMove () =
        let input = prepare "battement fifth front 2"
        let expected = Success ({ step = Battement; pos = Fifth; dir = Front }, ("battement fifth front 2", 21, false))
        let result = move input
        Assert.AreEqual(expected, result)