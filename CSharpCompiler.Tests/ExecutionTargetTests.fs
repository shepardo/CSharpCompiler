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
    let SimpleExecution () =
        let expr = doParse("123 + 44555")
        let gen = new CodeGenerator(expr)
        gen.Generate()
        let _type = gen.Config.Type
        let func = _type.GetMethod("Function1")
        let result = func.Invoke(null, null)
        Assert.AreEqual(44678, result)
        Assert.Pass()

    [<Test>]
    let SimpleExecution2 () =
        let expr = doParse("(123 + 44555) - 2")
        let gen = new CodeGenerator(expr)
        gen.Generate()
        let _type = gen.Config.Type
        let func = _type.GetMethod("Function1")
        let result = func.Invoke(null, null)
        Assert.AreEqual(44676, result)
        Assert.Pass()
