// Copyright 2018, Fernando Gonzalez Sanchez

namespace CSharpCompiler

open System
open System.Collections.Generic
open System.IO
open System.Text
open System.Threading
open System.Reflection
open System.Reflection.Emit
open Utility


module main =

    // TODO: add support for tracking location of errors
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
            let eof = new Wrapper<bool>(false)
            let read() =
                if eof.Value then 
                    -1
                else
                    eof.Value <- sr.EndOfStream
                    if eof.Value then -1 else sr.Read()
            
            _tokens <- new List<Token>()
            _current <- 0
            let mutable c : int = read()
            let mutable prev_c = -1
            let mutable ch : char = (char)c
            
            while c <> -1 do
                ch <- (char)c
                match ch with
                | '+' -> _tokens.Add(new Token(TokenClass.PLUS, "+"))
                | '-' -> _tokens.Add(new Token(TokenClass.MINUS, "-"))
                | '(' -> _tokens.Add(new Token(TokenClass.LPARENT, "("))
                | ')' -> _tokens.Add(new Token(TokenClass.RPARENT, ")"))
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
                else raise (new Exception(String.Format("No more tokens available, expected {0}.", expected)))
            else
                tok <- _tokens.[_current]
                _current <- _current + 1
                if tok.Class <> expected then raise (new Exception(String.Format("Token mismatch, expected {0}, found {1}", expected, tok.Class)))
            tok
        
        member public x.Peek() : Token =
            if _current >= _tokens.Count then Scanner._tokenEof
            else _tokens.[_current]

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

    type public Parser(sc : Scanner) as this =
        [<DefaultValue>] val mutable private _sc : Scanner
        // TODO: is it OK to use a stack or a queue is preferred (ie for right / left associativity operators)?
        [<DefaultValue>] val mutable private _operators : Stack<Token>
        [<DefaultValue>] val mutable private _operands : Queue<Token>

        do
            this._sc <- sc
            this._operators <- new Stack<Token>()
            this._operands <- new Queue<Token>()
            ()


        member public x.parse() : BinaryNodeExpr =
            this._operators.Clear()
            this._operands.Clear()
            this.expr()
            this.BuildExprTree()
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
         |  MINUS term rest
         |  <nothing>
         * *)
        member public x.expr() : unit =
            this.term()
            this.rest()

        (*
        term ->
            simple_term
        |   '(' expr ')'
        *)
        member private x.term() : unit =
            let mutable tok = this._sc.Peek()
            
            if tok.Class = TokenClass.LPARENT then
                tok <- this._sc.Next()
                this.expr()
                tok <- this._sc.Peek()
                if tok.Class = TokenClass.RPARENT then
                    tok <- this._sc.Next()
                else
                    // TODO: insert ')' and continue parsing
                    raise (new Exception("Expected ')'"))
            else
                this.simple_term()
            
            
        (*
        term ->
            INT_NUMBER
        *)
        member private x.simple_term(): unit =
            let mutable tok = this._sc.Peek()
            // TODO: support other data types
            if tok.Class <> TokenClass.INT_NUMBER then
                // TODO: insert a dummy token to continue parsing
                raise (new Exception("Not a valid term"))
            tok <- this._sc.Next()
            this._operands.Enqueue(tok)
  
        (*
        rest -> 
            PLUS term rest
            MINUS term rest 
        *)
        member private x.rest((*Default false expects_rparen*) ?expects_rparen: bool) : unit =
            let expects_parent = defaultArg expects_rparen false
            let tok = this._sc.Peek()
            if tok.Class = TokenClass.PLUS || tok.Class = TokenClass.MINUS then
                this._operators.Push(this._sc.Next())
                this.term()
                this.rest((*None*))

        member private x.BuildExprTree() : BinaryNodeExpr =
            if this._operators.Count <> 0 then
                let result : BinaryNodeExpr = new BinaryNodeExpr()
                result.Data <- this._operators.Pop()
                result.Left <- this.BuildExprTree()
                result.Right <- this.BuildExprTree()
                result
            else
                let t1 : Token = this._operands.Dequeue()
                let result : BinaryNodeExpr = new BinaryNodeExpr()
                result.Data <- t1
                result

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

        member private x.Visit(node : BinaryNodeExpr) =
            let gen = this._config.ILGenerator
            match node.Data.Class with
            | TokenClass.INT_NUMBER ->
                let value = Int32.Parse(node.Data.Text)
                gen.Emit(OpCodes.Ldc_I4, value)
            | TokenClass.MINUS -> gen.Emit(OpCodes.Sub)
            | TokenClass.PLUS -> gen.Emit(OpCodes.Add)
            | _ -> raise (new Exception(String.Format("Unexpected node '{0}' when visiting", node.Data.Class)))
            
    [<EntryPoint>]
    let main argv = 
        let buf = Encoding.UTF8.GetBytes("(123 + 44555) - 2")
        let ms = new MemoryStream(buf, 0, buf.Length)
        let sr = new StreamReader(ms)
        let scanner = new Scanner(sr)
        let parser = new Parser(scanner)
        let expr = parser.parse()
        let gen = new CodeGenerator(expr)
        gen.Generate()
        0 // return an integer exit code