namespace CSharpCompiler

open System
open System.Collections.Generic
open System.Reflection
open System.Reflection.Emit
open System.Threading

    type public CodeGenerator(expr : BinaryNodeExpr) as this =
        [<DefaultValue>] val mutable _expr : BinaryNodeExpr
        [<DefaultValue>] val mutable _config: CodeGeneratorConfig
        [<DefaultValue>] val mutable _operators : Queue<OpCode>

        do
            this._expr <- expr
            this._config <- null
            this._operators <- new Queue<OpCode>()

        member public x.Config
            with get() : CodeGeneratorConfig = x._config

        member public x.Generate() =
            this.GeneratePreamble()
            this.DoGenerate(this._expr)
            this.GenerateEpilog()

        member private x.GeneratePreamble() =
            // TODO: For now hardcoded
            let myAssemblyName = new AssemblyName()
            myAssemblyName.Name <- "SampleAssembly"

            let myAssembly = Thread.GetDomain().DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.RunAndSave)
            let myModule = myAssembly.DefineDynamicModule(myAssemblyName.Name + ".dll", true)
            let myTypeBuilder = myModule.DefineType("Example", TypeAttributes.Public)
            let myMethod = myTypeBuilder.DefineMethod("Function1", MethodAttributes.Public ||| MethodAttributes.Static, typeof<Object>, null)

            let myMethodIL = myMethod.GetILGenerator()
            this._config <- new CodeGeneratorConfig()
            this._config.AssemblyName <- myAssemblyName
            this._config.AssemblyBuilder <- myAssembly
            this._config.TypeBuilder <- myTypeBuilder
            this._config.ILGenerator <- myMethodIL

        member private x.GenerateEpilog() =
            let gen = this._config.ILGenerator
            gen.Emit(OpCodes.Box, typeof<int>)
            gen.Emit(OpCodes.Ret)
            this._config.Type <- this._config.TypeBuilder.CreateType() // |> ignore
            this._config.AssemblyBuilder.Save(this._config.AssemblyName.Name + ".dll")

        member private x.DoGenerate(node : BinaryNodeExpr) =
            if node.Left <> null then this.DoGenerate(node.Left)
            if node.Right <> null then this.DoGenerate(node.Right)
            this.Visit(node)

        member private x.DoGenerate2(root : BinaryNodeExpr) =
            let stack = new Stack<BinaryNodeExpr>()
            let mutable node = root
            stack.Push(node)
            while stack.Count <> 0 do
                node <- stack.Pop()
                if node.Left <> null then stack.Push(node.Left)
                if node.Right <> null then stack.Push(node.Right)
                node <- stack.Pop()
                this.Visit(node)

        member private x.DoGenerateRecursive(node : BinaryNodeExpr) =
            if node.Left <> null then this.DoGenerate(node.Left)
            if node.Right <> null then this.DoGenerate(node.Right)
            this.Visit(node)

        member private x.Visit(node : BinaryNodeExpr) =
            let gen = this._config.ILGenerator
            match node.Data.Class with
            | TokenClass.INT_NUMBER ->
                let value = Int32.Parse(node.Data.Text)
                gen.Emit(OpCodes.Ldc_I4, value)
            | TokenClass.MINUS -> gen.Emit(OpCodes.Sub)
            | TokenClass.PLUS -> gen.Emit(OpCodes.Add)
            | TokenClass.DIVIDE -> gen.Emit(OpCodes.Div)
            | TokenClass.TIMES -> gen.Emit(OpCodes.Mul)
            | TokenClass.MODULUS -> gen.Emit(OpCodes.Rem)
            | _ -> raise (new Exception(String.Format("Unexpected node '{0}' when visiting", node.Data.Class)))

