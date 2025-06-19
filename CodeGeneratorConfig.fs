namespace CSharpCompiler

open System
open System.Reflection
open System.Reflection.Emit

    [<AllowNullLiteral>]
    type public CodeGeneratorConfig() as this =
        [<DefaultValue>] val mutable private _assemblyName : AssemblyName
        [<DefaultValue>] val mutable private _assemblyBuilder : AssemblyBuilder
        [<DefaultValue>] val mutable private _typeBuilder : TypeBuilder
        [<DefaultValue>] val mutable private _ilGenerator : ILGenerator
        [<DefaultValue>] val mutable private _type : Type

        do
            this._assemblyName <- null
            this._assemblyBuilder <- null
            this._typeBuilder <- null
            this._ilGenerator <- null
            this._type <- null

        member public x.AssemblyName
            with get() : AssemblyName = x._assemblyName
            and set(value) = x._assemblyName <- value

        member public x.AssemblyBuilder
            with get() : AssemblyBuilder = x._assemblyBuilder
            and set(value) = x._assemblyBuilder <- value

        member public x.TypeBuilder
            with get() : TypeBuilder = x._typeBuilder
            and set(value) = x._typeBuilder <- value

        member public x.ILGenerator
            with get() : ILGenerator = x._ilGenerator
            and set(value) = x._ilGenerator <- value

        member public x.Type
            with get() : Type = x._type
            and set(value) = x._type <- value


