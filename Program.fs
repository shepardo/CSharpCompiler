// Copyright 2018, Fernando Gonzalez Sanchez

namespace CSharpCompiler

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Text
open System.Threading
open System.Reflection
open System.Reflection.Emit


module main =

    type public Wrapper(value : bool) as this =
        [<DefaultValue>] val mutable private _value : bool
        do
            this._value <- value

        member public x.Value
            with get() : bool = x._value
            and set(value) = x._value <- value

    type public Scanner(sr : StreamReader) as this =
        let mutable _tokens : List<Token> = new List<Token>()
        let mutable _current : int = 0
        [<DefaultValue>]
        static val mutable private _tokenEof : Token
        
        static do 
            Scanner._tokenEof <- new Token(TokenClass.EOF, "")
        
        do
            this.Tokenize(sr)

        member private x.Tokenize(sr : StreamReader) : unit = 
            let eof = new Wrapper(false)
            let read() =
                if eof.Value then 
                    -1
                else
                    eof.Value <- sr.EndOfStream
                    if eof.Value then -1 else sr.Read()
            
            _tokens <- new List<Token>()
            _current <- 0
            let mutable c = read()
            let mutable prev_c = -1
            let mutable ch : char = (char)c
            
            while c <> -1 do
                ch <- (char)c
                match ch with
                | '+' -> _tokens.Add(new Token(TokenClass.PLUS, "+"))
                | '-' -> _tokens.Add(new Token(TokenClass.MINUS, "-"))
                | '(' -> _tokens.Add(new Token(TokenClass.LPARENT, "("))
                | ')' -> _tokens.Add(new Token(TokenClass.RPAREN, ")"))
                | _   -> 
                    if Char.IsDigit(ch) then
                        let sb = new StringBuilder("")

                        let _ch = ref ch
                        let _c = ref c
                        let rec while_isdigit() : unit =
                            if Char.IsDigit(!_ch) then
                                sb.Append(!_ch) |> ignore
                                _c := read()
                                if !_c <> -1 then
                                    _ch := (char)!_c
                                    while_isdigit()
                            ()

                        while_isdigit()
                        c <- !_c
                        ch <- !_ch
                        if not (Char.IsDigit(ch)) then
                            prev_c <- c
                        _tokens.Add(new Token(TokenClass.INT_NUMBER, sb.ToString()))
                    else
                        while not (eof.Value) && Char.IsWhiteSpace((char)c) do
                            c <- read()
                        if not eof.Value then                            
                            prev_c <- c

                if prev_c <> -1 then
                    c <- prev_c
                    prev_c <- -1
                else
                    c <- read()


        member public x.Next() : Token =
            if _current >= _tokens.Count then Scanner._tokenEof 
            else 
                _current <- _current + 1
                _tokens.[_current - 1]

        member public x.Next(expected : TokenClass) : Token =
            let mutable tok : Token = null
            if _current >= _tokens.Count then
                if expected = TokenClass.EOF then tok <- Scanner._tokenEof
                else raise (new Exception("No more tokens available."))
            else
                tok <- _tokens.[_current]
                _current <- _current + 1
                if tok.Class <> expected then raise (new Exception("No more tokens available"))
            tok
        
        member public x.Peek() : Token =
            if _current >= _tokens.Count then Scanner._tokenEof
            else _tokens.[_current]

    [<AllowNullLiteral>]
    type public NodeExpr() as this =
        [<DefaultValue>] val mutable private _left : NodeExpr
        [<DefaultValue>] val mutable private _right : NodeExpr
        [<DefaultValue>] val mutable private _data : Token

        do
            this._left <- null
            this._right <- null
            this._data <- null

        member public x.Left
            with get() : NodeExpr = x._left
            and set(value) = x._left <- value

        member public x.Right
            with get() : NodeExpr = x._right
            and set(value) = x._right <- value

        member public x.Data
            with get() : Token = x._data
            and set(value) = x._data <- value

    type public Parser(sc : Scanner) as this =
        [<DefaultValue>] val mutable private _sc : Scanner
        [<DefaultValue>] val mutable private _operators : Queue<Token>
        [<DefaultValue>] val mutable private _operands : Queue<Token>

        do
            this._sc <- sc
            this._operators <- new Queue<Token>()
            this._operands <- new Queue<Token>()
            ()

        (*
        expr 
	        :	(
		        ( expr PLUS term )
	        |	( expr MINUS term )
	        |	(term)
	        ) 
	        ;
        
        expr -> term rest
         rest -> 
            PLUS term rest
            MINUS term rest

         * *)
        member public x.expr() : NodeExpr =
            this._operators.Clear()
            this._operands.Clear()
            this.term()
            this.rest()
            this.BuildExprTree()

        member private x.term() : unit =
            let mutable tok = this._sc.Peek()
            if tok.Class <> TokenClass.INT_NUMBER then
                raise (new Exception("Not a valid term"))
            tok <- this._sc.Next()
            this._operands.Enqueue(tok)
  
        (*
        rest -> 
            PLUS term rest
            MINUS term rest 
        *)
        member private x.rest() : unit =
            let tok = this._sc.Peek()
            if tok.Class = TokenClass.PLUS || tok.Class = TokenClass.MINUS then
                this._operators.Enqueue(this._sc.Next())
                this.term()
                this.rest()

        member private x.BuildExprTree() : NodeExpr =
            if this._operators.Count <> 0 then
                let result : NodeExpr = new NodeExpr()
                result.Data <- this._operators.Dequeue()
                result.Left <- this.BuildExprTree()
                result.Right <- this.BuildExprTree()
                result
            else
                let t1 : Token = this._operands.Dequeue()
                let result : NodeExpr = new NodeExpr()
                result.Data <- t1
                result

    type public ObjectData() as this =
        [<DefaultValue>] val mutable private _assemblyName : AssemblyName
        [<DefaultValue>] val mutable private _assemblyBuilder : AssemblyBuilder
        [<DefaultValue>] val mutable private _typeBuilder : TypeBuilder
        [<DefaultValue>] val mutable private _ilGenerator : ILGenerator

        do
            this._assemblyName <- null
            this._assemblyBuilder <- null
            this._typeBuilder <- null
            this._ilGenerator <- null

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
            
    type public CodeGenerator(expr : NodeExpr) as this =
        [<DefaultValue>] val mutable _expr : NodeExpr
        [<DefaultValue>] val mutable _objectData : ObjectData
        [<DefaultValue>] val mutable _operators : Queue<OpCode>

        do
            this._expr <- expr
            this._objectData <- new ObjectData()
            this._operators <- new Queue<OpCode>()

        member public x.Generate() =
            this.GeneratePreamble()
            this.DoGenerate(this._expr)
            this.GenerateEpilog()

        member private x.GeneratePreamble() =
            let myAssemblyName = new AssemblyName()
            myAssemblyName.Name <- "SampleAssembly"

            let myAssembly = Thread.GetDomain().DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.RunAndSave)
            let myModule = myAssembly.DefineDynamicModule(myAssemblyName.Name + ".dll", true)
            let myTypeBuilder = myModule.DefineType("Example", TypeAttributes.Public)
            let myMethod = myTypeBuilder.DefineMethod("Function1", MethodAttributes.Public ||| MethodAttributes.Static, typeof<Object>, null)

            let myMethodIL = myMethod.GetILGenerator()
            this._objectData <- new ObjectData()
            this._objectData.AssemblyName <- myAssemblyName
            this._objectData.AssemblyBuilder <- myAssembly
            this._objectData.TypeBuilder <- myTypeBuilder
            this._objectData.ILGenerator <- myMethodIL

        member private x.GenerateEpilog() =
            let gen = this._objectData.ILGenerator
            gen.Emit(OpCodes.Box, typeof<int>)
            gen.Emit(OpCodes.Ret)
            this._objectData.TypeBuilder.CreateType() |> ignore
            this._objectData.AssemblyBuilder.Save(this._objectData.AssemblyName.Name + ".dll")

        member private x.DoGenerate(node : NodeExpr) =
            if node.Left <> null then this.DoGenerate(node.Left)
            if node.Right <> null then this.DoGenerate(node.Right)
            this.Visit(node)

        member private x.Visit(node : NodeExpr) =
            let gen = this._objectData.ILGenerator
            match node.Data.Class with
            | TokenClass.INT_NUMBER ->
                let value = Int32.Parse(node.Data.Text)
                gen.Emit(OpCodes.Ldc_I4, value)
            | TokenClass.MINUS -> gen.Emit(OpCodes.Sub)
            | TokenClass.PLUS -> gen.Emit(OpCodes.Add)
            | _ -> raise (new Exception("Unexpected node when visiting"))
            
    [<EntryPoint>]
    let main argv = 
        let buf = Encoding.UTF8.GetBytes("2 + 4")
        let ms = new MemoryStream(buf, 0, buf.Length)
        let sr = new StreamReader(ms)
        let scanner = new Scanner(sr)
        let parser = new Parser(scanner)
        let expr = parser.expr()
        let gen = new CodeGenerator(expr)
        gen.Generate()
        0 // return an integer exit code