namespace CSharpCompiler.Tests

open System
open NUnit.Framework
open System.Text
open System.IO
open System.Linq
open System.Reflection
open CSharpCompiler
open CSharpCompiler.main
open ParseTests
open Mono.Cecil
open Mono.Cecil.Cil
open CSharpCompiler.Utility

module CodeGenTests =
    [<SetUp>]
    let Setup () =
        ()

    let assertInstruction(inst : Instruction, offset : int32, opcode: OpCode, operand: Object) =
        Assert.AreEqual(offset, inst.Offset)
        Assert.AreEqual(opcode, inst.OpCode)
        if operand <> null && operand.GetType() = typeof<Mono.Cecil.TypeReference> then
            // For some reason the operand assert of two instances of Mono.Cecil.TypeReference fails
            // because the two instances have different MetadataToken, thus a special comparator is needed
            // (the Mono.Cecil.TypeReferenceEqualityComparer is internal, so, some reflection is needed to access it).
            let comparer = System.Reflection.Assembly.Load("Mono.Cecil").GetType("Mono.Cecil.TypeReferenceEqualityComparer")
            let ctor = comparer.GetConstructors(BindingFlags.Instance ||| BindingFlags.NonPublic ||| BindingFlags.Public).ToList()[0]
            let instance = ctor.Invoke([||])
            let types : Type[] = [|
                            typeof<Mono.Cecil.TypeReference>;
                            typeof<Mono.Cecil.TypeReference>
                        |]
            let result = Convert.ToBoolean(
                    comparer.GetMethod(
                        "Equals",
                        types
                    ).Invoke(
                        instance,
                        [|
                            inst.Operand;
                            operand
                        |]
                    )
            )
            Assert.IsTrue(
                result
            )
        else
            Assert.AreEqual(operand, inst.Operand)

    [<Test>]
    let SimpleCodeGen () =
        let expr = doParse("123 + 44555")
        let gen = new CodeGenerator(expr)
        gen.Generate()
        let assemDef = Mono.Cecil.AssemblyDefinition.ReadAssembly(System.Environment.CurrentDirectory + "\\" + gen.Config.AssemblyName.FullName + ".dll")
        let instructions = assemDef.MainModule.Types.ToList().Find(
            fun x -> x.FullName = "Example").Methods.ToList().Find(fun x -> x.Name = "Function1").Body.Instructions.ToList()
        assertInstruction(instructions[0], 0, OpCodes.Ldc_I4, 123)
        assertInstruction(instructions[1], 5, OpCodes.Ldc_I4, 44555)
        assertInstruction(instructions[2], 10, OpCodes.Add, null)
        assertInstruction(instructions[3], 11, OpCodes.Box, assemDef.MainModule.ImportReference(typeof<System.Int32>))
        assertInstruction(instructions[4], 16, OpCodes.Ret, null)
        Assert.Pass()



