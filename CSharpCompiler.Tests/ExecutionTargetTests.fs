namespace CSharpCompiler.Tests

open NUnit.Framework
open System.Text
open System.IO
open CSharpCompiler
open CSharpCompiler.main
open ParseTests

module ExecutionTargetTests =
    [<SetUp>]
    let Setup () =
        ()

    [<Test>]
    let AdditiveExecution () =
        let expr = doParse("123 + 44555")
        let gen = new CodeGenerator(expr)
        gen.Generate()
        let _type = gen.Config.Type
        let func = _type.GetMethod("Function1")
        let result = func.Invoke(null, null)
        Assert.AreEqual(44678, result)
        Assert.Pass()

    [<Test>]
    let AdditiveExecution2 () =
        let expr = doParse("(123 + 44555) - 2")
        let gen = new CodeGenerator(expr)
        gen.Generate()
        let _type = gen.Config.Type
        let func = _type.GetMethod("Function1")
        let result = func.Invoke(null, null)
        Assert.AreEqual(44676, result)
        Assert.Pass()

    [<Test>]
    let AdditiveExecution3 () =
        let expr = doParse("(123 - 44555) - 2")
        let gen = new CodeGenerator(expr)
        gen.Generate()
        let _type = gen.Config.Type
        let func = _type.GetMethod("Function1")
        let result = func.Invoke(null, null)
        Assert.AreEqual(-44434, result)
        Assert.Pass()

    [<Test>]
    let AdditiveExecution4() =
        let expr = doParse("123 - 44555 - 2")
        let gen = new CodeGenerator(expr)
        gen.Generate()
        let _type = gen.Config.Type
        let func = _type.GetMethod("Function1")
        let result = func.Invoke(null, null)
        Assert.AreEqual(-44434, result)
        Assert.Pass()

    [<Test>]
    let MultiplicativeExecution1() =
        let expr = doParse("13 / 5 / 2")
        let gen = new CodeGenerator(expr)
        gen.Generate()
        let _type = gen.Config.Type
        let func = _type.GetMethod("Function1")
        let result = func.Invoke(null, null)
        Assert.AreEqual(1, result)
        Assert.Pass()

    [<Test>]
    let MultiplicativeExecution2() =
        let expr = doParse("13 / (5 / 2)")
        let gen = new CodeGenerator(expr)
        gen.Generate()
        let _type = gen.Config.Type
        let func = _type.GetMethod("Function1")
        let result = func.Invoke(null, null)
        Assert.AreEqual(6, result)
        Assert.Pass()

    [<Test>]
    let MultiplicativeExecution3() =
        let expr = doParse("(13 / 5) / 2")
        let gen = new CodeGenerator(expr)
        gen.Generate()
        let _type = gen.Config.Type
        let func = _type.GetMethod("Function1")
        let result = func.Invoke(null, null)
        Assert.AreEqual(1, result)
        Assert.Pass()
(*
int a = 13 / 5 / 2;
int b = 13 / (5 / 2);
Console.WriteLine($"a = {a}, b = {b}");  // output: a = 1, b = 6
*)