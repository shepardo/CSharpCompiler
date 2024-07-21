namespace CSharpCompiler

module Utility =

    let (|Default|) defaultValue input =
        defaultArg input defaultValue

    type public Wrapper<'a>(value : 'a) as this =
        [<DefaultValue>] val mutable private _value : 'a
        do
            this._value <- value

        member public x.Value
            with get() : 'a = x._value
            and set(value) = x._value <- value