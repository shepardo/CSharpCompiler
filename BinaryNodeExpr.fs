
namespace CSharpCompiler

    [<AllowNullLiteral>]
    type public BinaryNodeExpr() as this =
        [<DefaultValue>] val mutable private _left : BinaryNodeExpr
        [<DefaultValue>] val mutable private _right : BinaryNodeExpr
        [<DefaultValue>] val mutable private _data : Token

        do
            this._left <- null
            this._right <- null
            this._data <- null

        member public x.Left
            with get() : BinaryNodeExpr = x._left
            and set(value) = x._left <- value

        member public x.Right
            with get() : BinaryNodeExpr = x._right
            and set(value) = x._right <- value

        member public x.Data
            with get() : Token = x._data
            and set(value) = x._data <- value
