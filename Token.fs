﻿namespace CSharpCompiler

open System.Diagnostics

//module Token =

    [<DebuggerDisplay("class = {_class}, text = {_text}")>]
    [<AllowNullLiteral>]
    type public Token(tokenClass : TokenClass, text : string) as this =
        [<DefaultValue>] val mutable private _class : TokenClass
        [<DefaultValue>] val mutable private _text : string
        do
            this._class <- tokenClass
            this._text <- text

        member public x.Class
            with get() : TokenClass = x._class
            and set(value) = x._class <- value

        member public x.Text
            with get() : string = x._text
            and set(value) = x._text <- value

        override x.Equals(b) =
            match b with
            | :? Token as t -> (tokenClass, text) = (t.Class, t.Text)
            | _ -> false

        override x.GetHashCode() =
            hash (tokenClass, text)