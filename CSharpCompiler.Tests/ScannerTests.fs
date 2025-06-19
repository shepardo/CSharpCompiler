namespace CSharpCompiler.Tests

open NUnit.Framework
open System.Collections.Generic
open System.IO
open System.Text
open CSharpCompiler
open CSharpCompiler.main

module ScannerTests =

    [<SetUp>]
    let Setup () =
        ()

    let doScan (expr : string) : List<Token> =
        let buf = Encoding.UTF8.GetBytes(expr)
        let ms = new MemoryStream(buf, 0, buf.Length)
        let sr = new StreamReader(ms)
        let scanner = new Scanner(sr)
        let tokens = new List<Token>()
        while scanner.Peek().Class <> TokenClass.EOF do
            tokens.Add(scanner.Next())
        tokens.Add(scanner.Next())
        tokens

    [<Test>]
    let AdditiveBinaryExpr () =
        let tokens = doScan("123 + 44555")
        Assert.AreEqual(TokenClass.INT_NUMBER, tokens[0].Class)
        Assert.AreEqual("123", tokens[0].Text)
        Assert.AreEqual(TokenClass.PLUS, tokens[1].Class)
        Assert.AreEqual(TokenClass.INT_NUMBER, tokens[2].Class)
        Assert.AreEqual("44555", tokens[2].Text)
        Assert.Pass()