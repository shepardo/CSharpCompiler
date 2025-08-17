namespace CSharpCompiler.Tests

open NUnit.Framework
open System.Collections.Generic
open System.IO
open System.Text
open CSharpCompiler
open CSharpCompiler.main

module ScannerTest =

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
    let AdditiveBinaryExprIntegerTest () =
        let tokens = doScan("123 + 44555")
        let expected_tokens : Token list = [ 
            new Token(TokenClass.INT_NUMBER, "123"); 
            new Token(TokenClass.OP_PLUS, "+"); 
            new Token(TokenClass.INT_NUMBER, "44555");
            new Token(TokenClass.EOF, "")
        ]
        Assert.AreEqual(ResizeArray<Token> expected_tokens, tokens)
        Assert.Pass()

    [<Test>]    
    let AdditiveBinaryExprFloating () =
        let tokens = doScan("123.02 + 44555e+010")
        let expected_tokens : Token list = [ 
            new Token(TokenClass.FLOAT_NUMBER, "123.02"); 
            new Token(TokenClass.OP_PLUS, "+"); 
            new Token(TokenClass.FLOAT_NUMBER, "44555e+010");
            new Token(TokenClass.EOF, "")
        ]
        Assert.AreEqual(ResizeArray<Token> expected_tokens, tokens)
        Assert.Pass()