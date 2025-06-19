
namespace CSharpCompiler

open System
open System.Collections.Generic
open System.IO
open System.Text
open Utility

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
                | '*' -> _tokens.Add(new Token(TokenClass.TIMES, "*"))
                | '/' -> _tokens.Add(new Token(TokenClass.DIVIDE, "/"))
                | '%' -> _tokens.Add(new Token(TokenClass.MODULUS, "%"))
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
