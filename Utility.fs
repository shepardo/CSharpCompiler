namespace CSharpCompiler

open System
open System.Dynamic
open System.Reflection


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


    type ExposedObjectSimple(obj: obj) =
        inherit DynamicObject()

        let mutable m_object = obj

        override this.TryInvokeMember(binder: InvokeMemberBinder, args: obj[], result: byref<obj>) =
            // Find the called method using reflection
            let methodInfo = m_object.GetType().GetMethod(
                binder.Name,
                BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Instance)

            // Call the method
            result <- methodInfo.Invoke(m_object, args)
            true

