namespace CSharpCompiler.Tests

open NUnit.Framework
open System.Text
open System.IO
open CSharpCompiler
open CSharpCompiler.main

module ParseTests =

    [<SetUp>]
    let Setup () =
        ()

    let doParse (expr : string) : BinaryNodeExpr =
        let buf = Encoding.UTF8.GetBytes(expr)
        let ms = new MemoryStream(buf, 0, buf.Length)
        let sr = new StreamReader(ms)
        let scanner = new Scanner(sr)
        let parser = new Parser(scanner)
        parser.parse()

    [<Test>]
    let AdditiveBinaryExpr () =
        let expr = doParse("123 + 44555")
        Assert.AreEqual(typeof<BinaryNodeExpr>, expr.GetType())
        Assert.AreEqual(TokenClass.OP_PLUS, expr.Data.Class)
        Assert.AreEqual(typeof<BinaryNodeExpr>, expr.Left.GetType())
        Assert.AreEqual(TokenClass.INT_NUMBER, expr.Left.Data.Class)
        Assert.AreEqual("123", expr.Left.Data.Text)
        Assert.AreEqual(typeof<BinaryNodeExpr>, expr.Right.GetType())
        Assert.AreEqual(TokenClass.INT_NUMBER, expr.Right.Data.Class)
        Assert.AreEqual("44555", expr.Right.Data.Text)
        Assert.Pass()


    [<Test>]
    let AdditiveBinaryExpr2 () =
        let expr = doParse("(123 + 44555) - 2")
        Assert.AreEqual(typeof<BinaryNodeExpr>, expr.GetType())
        Assert.AreEqual(TokenClass.OP_MINUS, expr.Data.Class)

        Assert.AreEqual(typeof<BinaryNodeExpr>, expr.Right.GetType())
        Assert.AreEqual(TokenClass.INT_NUMBER, expr.Right.Data.Class)
        Assert.AreEqual("2", expr.Right.Data.Text)

        Assert.AreEqual(typeof<BinaryNodeExpr>, expr.Left.GetType())
        Assert.AreEqual(TokenClass.OP_PLUS, expr.Left.Data.Class)
        Assert.AreEqual("123", expr.Left.Left.Data.Text)
        Assert.AreEqual("44555", expr.Left.Right.Data.Text)

        Assert.Pass()