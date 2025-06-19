
namespace CSharpCompiler

open System
open Utility

    type public Parser(sc : Scanner) as this =
        [<DefaultValue>] val mutable private _sc : Scanner

        do
            this._sc <- sc
            ()

        member public x.parse() : BinaryNodeExpr =
            this.additive_expr()

        (*
        additive_expr
	        :	(
		        ( additive_expr PLUS multiplicative_expr )
	        |	( additive_expr MINUS multiplicative_expr )
	        |	(multiplicative_expr)
	        )
	        ;
         * *)
        member public x.additive_expr() : BinaryNodeExpr =
            let mutable left_term = this.multiplicative_expr()
            let mutable tok = this._sc.Peek()

            while   tok.Class = TokenClass.PLUS ||
                    tok.Class = TokenClass.MINUS do

                let operator = this._sc.Next()
                let right_term = this.multiplicative_expr()
                left_term <- x.CreateBinaryExpr(operator, left_term, right_term)
                tok <- this._sc.Peek()
            left_term

        (*
        multiplicative_expr ->
            multiplicative_expr TIMES unary_expr
            multiplicative_expr DIVSION unary_expr
            multiplicative_expr MODULUS unary_expr
        |   '(' additive_expr ')'
        unary_expr ->
            INT_NUMBER
        |   '(' additive_expr ')'
        *)
        member private x.multiplicative_expr() : BinaryNodeExpr =
            let mutable left_term = this.unary_expr()
            let mutable tok = this._sc.Peek()

            while  tok.Class = TokenClass.TIMES ||
                tok.Class = TokenClass.DIVIDE ||
                tok.Class = TokenClass.MODULUS do

                let operator = this._sc.Next()
                let right_term = this.unary_expr()
                left_term <- x.CreateBinaryExpr(operator, left_term, right_term)
                tok <- this._sc.Peek()
            left_term

        (*
        unary_expr ->
            simple_unary_expr
        |   '(' additive_expr ')'
        *)
        member private x.unary_expr() : BinaryNodeExpr =
            let mutable tok = this._sc.Peek()

            if tok.Class = TokenClass.LPARENT then
                tok <- this._sc.Next()
                let result = this.additive_expr()
                tok <- this._sc.Peek()
                if tok.Class = TokenClass.RPARENT then
                    tok <- this._sc.Next()
                else
                    // TODO: insert ')' and continue parsing
                    raise (new Exception("Expected ')'"))
                result
            else
                this.simple_unary_expr()

        (*
        simple_unary_expr ->
            INT_NUMBER
        *)
        member private x.simple_unary_expr(): BinaryNodeExpr =
            let mutable tok = this._sc.Peek()
            // TODO: support other data types
            if tok.Class <> TokenClass.INT_NUMBER then
                // TODO: insert a dummy token to continue parsing
                raise (new Exception("Not a valid term"))
            tok <- this._sc.Next()
            x.CreateBinaryExprFromOperand(tok)

        member private x.CreateBinaryExprFromOperand(operand: Token) : BinaryNodeExpr =
            let result : BinaryNodeExpr = new BinaryNodeExpr()
            result.Data <- operand
            result

        member private x.CreateBinaryExpr(operator: Token, left, right: Token) : BinaryNodeExpr =
            x.CreateBinaryExpr(operator, x.CreateBinaryExprFromOperand(left), x.CreateBinaryExprFromOperand(right))

        member private x.CreateBinaryExpr(operator: Token, left, right: BinaryNodeExpr) : BinaryNodeExpr =
            let result : BinaryNodeExpr = new BinaryNodeExpr()
            result.Data <- operator
            result.Left <- left
            result.Right <- right
            result
