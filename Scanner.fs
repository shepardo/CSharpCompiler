
namespace CSharpCompiler

open System
open System.Collections.Generic
open System.IO
open System.Text
open Utility

    // TODO: add support for tracking location of errors
    type public Scanner(sr : StreamReader, errorManager: ErrorManager, filepath: string) as this =
        [<Literal>]
        let UNKNOWN_FILE_PATH: string = "<Unknown>"
        let mutable _tokens : List<Token> = new List<Token>()
        let mutable _current : int = 0
        let mutable _errorManager: ErrorManager = null
        [<DefaultValue>]
        val mutable _errorLocation: ErrorLocation
        [<DefaultValue>]
        static val mutable private _tokenEof : Token
        [<DefaultValue>]
        static val mutable private _keywords : Dictionary<string, Token>

        static do
            Scanner._tokenEof <- new Token(TokenClass.EOF, "")
            let keywords = [|
                ("abstract", TokenClass.KEYWORD_ABSTRACT); 
                ("as", TokenClass.KEYWORD_AS);
                ("base", TokenClass.KEYWORD_BASE);
                ("bool", TokenClass.KEYWORD_BOOL);
                ("break", TokenClass.KEYWORD_BREAK);
                ("byte", TokenClass.KEYWORD_BYTE);
                ("case", TokenClass.KEYWORD_CASE);
                ("catch", TokenClass.KEYWORD_CATCH);
                ("char", TokenClass.KEYWORD_CHAR);
                ("checked", TokenClass.KEYWORD_CHECKED);
                ("class", TokenClass.KEYWORD_CLASS);
                ("const", TokenClass.KEYWORD_CONST);
                ("continue", TokenClass.KEYWORD_CONTINUE);
                ("decimal", TokenClass.KEYWORD_DECIMAL);
                ("default", TokenClass.KEYWORD_DEFAULT);
                ("delegate", TokenClass.KEYWORD_DELEGATE);
                ("do", TokenClass.KEYWORD_DO);
                ("double", TokenClass.KEYWORD_DOUBLE);
                ("else", TokenClass.KEYWORD_ELSE);
                ("enum", TokenClass.KEYWORD_ENUM);
                ("event", TokenClass.KEYWORD_EVENT);
                ("explicit", TokenClass.KEYWORD_EXPLICIT);
                ("extern", TokenClass.KEYWORD_EXTERN);
                ("false", TokenClass.KEYWORD_FALSE);
                ("finally", TokenClass.KEYWORD_FINALLY);
                ("fixed", TokenClass.KEYWORD_FIXED);
                ("float", TokenClass.KEYWORD_FLOAT);
                ("for", TokenClass.KEYWORD_FOR);
                ("foreach", TokenClass.KEYWORD_FOREACH);
                ("goto", TokenClass.KEYWORD_GOTO);
                ("if", TokenClass.KEYWORD_IF);
                ("implicit", TokenClass.KEYWORD_IMPLICIT);
                ("in", TokenClass.KEYWORD_IN);
                ("int", TokenClass.KEYWORD_INT);
                ("interface", TokenClass.KEYWORD_INTERFACE);
                ("internal", TokenClass.KEYWORD_INTERNAL);
                ("is", TokenClass.KEYWORD_IS);
                ("lock", TokenClass.KEYWORD_LOCK);
                ("long", TokenClass.KEYWORD_LONG);
                ("namespace", TokenClass.KEYWORD_NAMESPACE);
                ("new", TokenClass.KEYWORD_NEW);
                ("null", TokenClass.KEYWORD_NULL);
                ("object", TokenClass.KEYWORD_OBJECT);
                ("operator", TokenClass.KEYWORD_OPERATOR);
                ("out", TokenClass.KEYWORD_OUT);
                ("override", TokenClass.KEYWORD_OVERRIDE);
                ("params", TokenClass.KEYWORD_PARAMS);
                ("private", TokenClass.KEYWORD_PRIVATE);
                ("protected", TokenClass.KEYWORD_PROTECTED);
                ("public", TokenClass.KEYWORD_PUBLIC);
                ("readonly", TokenClass.KEYWORD_READONLY);
                ("ref", TokenClass.KEYWORD_REF);
                ("return", TokenClass.KEYWORD_RETURN);
                ("sbyte", TokenClass.KEYWORD_SBYTE);
                ("sealed", TokenClass.KEYWORD_SEALED);
                ("short", TokenClass.KEYWORD_SHORT);
                ("sizeof", TokenClass.KEYWORD_SIZEOF);
                ("stackalloc", TokenClass.KEYWORD_STACKALLOC);
                ("static", TokenClass.KEYWORD_STATIC);
                ("string", TokenClass.KEYWORD_STRING);
                ("struct", TokenClass.KEYWORD_STRUCT);
                ("switch", TokenClass.KEYWORD_SWITCH);
                ("this", TokenClass.KEYWORD_THIS);
                ("throw", TokenClass.KEYWORD_THROW);
                ("true", TokenClass.KEYWORD_TRUE);
                ("try", TokenClass.KEYWORD_TRY);
                ("typeof", TokenClass.KEYWORD_TYPEOF);
                ("uint", TokenClass.KEYWORD_UINT);
                ("ulong", TokenClass.KEYWORD_ULONG);
                ("unchecked", TokenClass.KEYWORD_UNCHECKED);
                ("unsafe", TokenClass.KEYWORD_UNSAFE);
                ("ushort", TokenClass.KEYWORD_USHORT);
                ("using", TokenClass.KEYWORD_USING);
                ("virtual", TokenClass.KEYWORD_VIRTUAL);
                ("void", TokenClass.KEYWORD_VOID);
                ("volatile", TokenClass.KEYWORD_VOLATILE);
                ("while", TokenClass.KEYWORD_WHILE);
            |]
            Scanner._keywords <- new Dictionary<string, Token>()
            for keyword_data in keywords do
                let (keyword, token_class) = keyword_data
                Scanner._keywords.Add(fst keyword_data, new Token(snd keyword_data, fst keyword_data))
            ()

        do
            _errorManager <- errorManager
            if filepath <> null then
                this._errorLocation <- new ErrorLocation(1, 0, filepath)
            else
                this._errorLocation <- new ErrorLocation(1, 0, UNKNOWN_FILE_PATH)
            this.Tokenize(sr)
            ()
    
        new(sr : StreamReader) = Scanner(sr, Console.Out)
        new(sr : StreamReader, errorOutput: TextWriter) = Scanner(sr, new ErrorManager(errorOutput), null)

        member private x.AddErrorMessage(message: string): unit =
            let line = if this._errorLocation <> null then this._errorLocation.Line else 0
            let column = if this._errorLocation <> null then this._errorLocation.Column else 0
            let filepath = if this._errorLocation <> null then this._errorLocation.FilePath else UNKNOWN_FILE_PATH
            x.AddErrorMessage(message, line, column, filepath)

        member private x.AddErrorMessage(message: string, line: int32, column: int32, filePath: string): unit =
            ErrorManager.CreateErrorMessage(message, line, column, filePath) |> ignore
            ()

        member private x.Tokenize(sr : StreamReader) : unit =
            let eof = new Wrapper<bool>(false)
            let read() =
                if eof.Value then
                    -1
                else
                    eof.Value <- sr.EndOfStream
                    if eof.Value then -1
                    else 
                        this._errorLocation.IncColumn()
                        sr.Read()

            _tokens <- new List<Token>()
            _current <- 0
            let mutable c : int = read()
            let mutable prev_c : int = -1
            let mutable ch : char = (char)c

            let mutable sb: StringBuilder = null

            let _ch = ref ch
            let _c = ref c
            (*
            //_c := read()
            //_ch := (char)!_c
            *)

            // TODO: Check these recursions are actually tailcall'ed
            let rec while_is_digit() : unit =
                if Char.IsDigit(!_ch) then
                    sb.Append(!_ch) |> ignore
                    _c := read()
                    if !_c <> -1 then
                        _ch := (char)!_c
                        while_is_digit()
                ()

            // Floating point literal
            let read_exponent() : bool =
                if Char.ToLower(!_ch) <= 'e' then
                    false
                else
                    sb.Append(ch) |> ignore
                    c <- read()
                    ch <- (char)c
                    if ch = '+' || ch = '-' then
                        sb.Append(ch) |> ignore
                        c <- read()
                        ch <- (char)c
                        while_is_digit()
                        true
                    else if Char.IsDigit(ch) then
                        while_is_digit()
                        true
                    else
                        // Error condition, Enter placeholder token in the token stream
                        sb.Append('1') |> ignore
                        let mutable result = sb.ToString()
                        _tokens.Add(new Token(TokenClass.FLOAT_NUMBER, result))
                        x.AddErrorMessage(
                            String.Format(
                                "Error recognizing floating point number at literal {0}, expected (+,- or digit), found: {1}",
                                result, ch
                            )
                        )
                        false

            let read_floating_point_after_dot(): bool =
                (*
                real-literal:
                    decimal-digits . decimal-digits exponent-partopt real-type-suffixopt
                    . decimal-digits exponent-partopt real-type-suffixopt
                    decimal-digits exponent-part real-type-suffixopt
                    decimal-digits real-type-suffix
                exponent-part:
                    e signopt decimal-digits
                    E signopt decimal-digits
                sign: one of
                    + -
                real-type-suffix: one of
                    F f D d M m
                    *)

                if Char.IsDigit(!_ch) then
                    while_is_digit()
                if Char.ToLower(!_ch) = 'e' then
                    if read_exponent() then
                        _tokens.Add(new Token(TokenClass.FLOAT_NUMBER, sb.ToString()))
                        true
                    else
                        false
                else
                    // TODO: calculate right position in the file (line, column)
                    // Error, malformed floating point literal
                    let mutable result = sb.ToString()
                    _tokens.Add(new Token(TokenClass.FLOAT_NUMBER, result))
                    x.AddErrorMessage(
                        String.Format(
                            "Error recognizing floating point number at literal {0}, expected (digit), found: {1}",
                            result, ch
                        )
                    )
                    false

            // TODO: when gobling charactesr (like for char literal, must account for escape sequences & new line)
            while c <> -1 do
                ch <- (char)c
                match ch with
                | '+' ->
                    c <- read()
                    ch <- (char)c
                    if ch = '+' then
                        _tokens.Add(new Token(TokenClass.OP_PLUS_PLUS, "++"))
                    else if ch = '=' then
                        _tokens.Add(new Token(TokenClass.OP_ASSIGNMENT_PLUS, "+="))
                    else
                        _tokens.Add(new Token(TokenClass.OP_PLUS, "+"))
                        prev_c <- c
                | '-' ->
                    c <- read()
                    ch <- (char)c
                    if ch = '-' then
                        _tokens.Add(new Token(TokenClass.OP_MINUS_MINUS, "--"))
                    else if ch = '=' then
                        _tokens.Add(new Token(TokenClass.OP_ASSIGNMENT_MINUS, "-="))
                    else if ch = '>' then
                        _tokens.Add(new Token(TokenClass.OP_MEMBER_ARROW, "->"))
                    else
                        _tokens.Add(new Token(TokenClass.OP_MINUS, "-"))
                        prev_c <- c
                | '(' -> _tokens.Add(new Token(TokenClass.LPARENT, "("))
                | ')' -> _tokens.Add(new Token(TokenClass.RPARENT, ")"))
                | '[' -> _tokens.Add(new Token(TokenClass.LSQUARE_BRACKET, "["))
                | ']' -> _tokens.Add(new Token(TokenClass.RSQUARE_BRACKET, "]"))
                | '{' -> _tokens.Add(new Token(TokenClass.LBRACE, "{"))
                | '}' -> _tokens.Add(new Token(TokenClass.RBRACE, "}"))

                | '=' -> 
                    c <- read()
                    ch <- (char)c
                    if ch = '=' then
                        _tokens.Add(new Token(TokenClass.OP_EQUAL, "=="))
                    else
                        _tokens.Add(new Token(TokenClass.OP_ASSIGNMENT, "="))
                        prev_c <- c
                | '*' ->
                    c <- read()
                    ch <- (char)c
                    if ch = '=' then
                        _tokens.Add(new Token(TokenClass.OP_ASSIGNMENT_TIMES, "*="))
                    else
                        _tokens.Add(new Token(TokenClass.OP_TIMES_OR_INDIRECTION, "*"))
                        prev_c <- c
                | '/' -> 
                    c <- read()
                    ch <- (char)c
                    if ch = '=' then
                        _tokens.Add(new Token(TokenClass.OP_ASSIGNMENT_DIVIDE, "/="))
                    else
                        _tokens.Add(new Token(TokenClass.OP_DIVIDE, "/"))
                        prev_c <- c
                | '%' -> 
                    c <- read()
                    ch <- (char)c
                    if ch = '=' then
                        _tokens.Add(new Token(TokenClass.OP_ASSIGNMENT_MODULUS, "%="))
                    else
                        _tokens.Add(new Token(TokenClass.OP_MODULUS, "%"))
                        prev_c <- c
                | '!' -> 
                    c <- read()
                    ch <- (char)c
                    if ch = '=' then
                        _tokens.Add(new Token(TokenClass.OP_NOT_EQUAL, "!="))
                    else
                        _tokens.Add(new Token(TokenClass.OP_BANG, "!"))
                        prev_c <- c
                | '<' ->
                    c <- read()
                    ch <- (char)c
                    if ch = '=' then
                        _tokens.Add(new Token(TokenClass.OP_LESS_THAN_EQUAL, "<="))
                    else if ch = '<' then
                        c <- read()
                        ch <- (char)c
                        if ch = '=' then
                            _tokens.Add(new Token(TokenClass.OP_ASSIGNMENT_LEFT_SHIFT, "<<="))
                        else
                            _tokens.Add(new Token(TokenClass.OP_LEFT_SHIFT, "<<"))
                            prev_c <- c
                    else
                        _tokens.Add(new Token(TokenClass.OP_LESS_THAN, "<"))
                        prev_c <- c
                | '>' ->
                    c <- read()
                    ch <- (char)c
                    if ch = '=' then
                        _tokens.Add(new Token(TokenClass.OP_GREATER_THAN_EQUAL, ">="))
                    else if ch = '>' then
                        c <- read()
                        ch <- (char)c
                        if ch = '>' then
                            c <- read()
                            ch <- (char)c
                            if ch = '=' then
                                _tokens.Add(new Token(TokenClass.OP_ASSIGNMENT_RIGHT_SHIFT_UNSIGNED, ">>>="))
                            else
                                _tokens.Add(new Token(TokenClass.OP_RIGHT_SHIFT_UNSIGNED, ">>>"))
                                prev_c <- c
                        else
                            _tokens.Add(new Token(TokenClass.OP_RIGHT_SHIFT, ">>"))
                            prev_c <- c
                        
                    else
                        _tokens.Add(new Token(TokenClass.OP_GREATER_THAN, ">"))
                        prev_c <- c
                | '|' ->
                    c <- read()
                    ch <- (char)c
                    if ch = '|' then
                        _tokens.Add(new Token(TokenClass.OP_CONDITIONAL_OR, "||"))
                    else if ch = '=' then
                        _tokens.Add(new Token(TokenClass.OP_ASSIGNMENT_OR, "|="))
                    else
                        _tokens.Add(new Token(TokenClass.OP_BITWISE_OR, "|"))
                | '&' -> 
                    c <- read()
                    ch <- (char)c
                    if ch = '=' then
                        _tokens.Add(new Token(TokenClass.OP_ASSIGNMENT_AND, "&="))
                    else if ch = '&' then
                        _tokens.Add(new Token(TokenClass.OP_CONDITONAL_AND, "&&"))
                    else
                        _tokens.Add(new Token(TokenClass.OP_BITWISE_AND_OR_ADDRESS_OF, "&"))
                        prev_c <- c
                | '^' -> 
                    c <- read()
                    ch <- (char)c
                    if ch = '=' then
                        _tokens.Add(new Token(TokenClass.OP_ASSIGNMENT_XOR, "^="))
                    else
                        _tokens.Add(new Token(TokenClass.OP_BITWISE_XOR, "^"))
                        prev_c <- c
                | '~' -> _tokens.Add(new Token(TokenClass.OP_BITWISE_NEGATE, "~"))
                | '?' -> _tokens.Add(new Token(TokenClass.OP_CONDITONAL_TERNARY_OPEN, "?"))
                | ':' -> _tokens.Add(new Token(TokenClass.OP_CONDITONAL_TERNARY_CLOSE, ":"))
                | '.' -> 
                    c <- read()
                    ch <- (char)c
                    sb <- new StringBuilder("")
                    if Char.IsDigit(ch) then
                        // can also be a floating point literal
                        read_floating_point_after_dot() |> ignore
                    else
                        _tokens.Add(new Token(TokenClass.DOT, "."))
                        prev_c <- c
                | ',' -> _tokens.Add(new Token(TokenClass.COMMA, ","))
                | ';' -> _tokens.Add(new Token(TokenClass.SEMICOLON, ";"))
                | '\'' -> _tokens.Add(new Token(TokenClass.QUOTE, "'"))
                | '"' -> _tokens.Add(new Token(TokenClass.DOUBLE_QUOTE, "\""))
                | '@' -> _tokens.Add(new Token(TokenClass.AT, "@"))
                (*

        | IDENTIFIER = 43
        | FLOAT_NUMBER = 52
                *)

                | _   ->
                    let mutable is_hexadecimal = false            
                    _ch := (char)c
                    sb <- new StringBuilder("")

                    let is_hexdigit(ch) : bool =
                        let _ch = Char.ToLower(ch)
                        Char.IsDigit(_ch) || _ch = 'a' ||  _ch = 'b' || _ch = 'c' || _ch = 'd' || _ch = 'e' || _ch = 'f'

                    let rec while_is_hexdigit() : unit =
                        if is_hexdigit(!_ch) then
                            sb.Append(!_ch) |> ignore
                            _c := read()
                            if !_c <> -1 then
                                _ch := (char)!_c
                                while_is_hexdigit()
                        ()
                    
                    if !_ch = '0' then
                        c <- read()
                        //ch <- (char)c
                        _ch := (char)c
                        if Char.ToLower(!_ch) = 'x' then
                            is_hexadecimal <- true
                            c <- read()
                            //ch <- (char)c
                            _ch := (char)c
                            while_is_hexdigit()
                            // TODO: this is will be done at the parser level
                            // integer-type-suffix: one of
                            // U u L l UL Ul uL ul LU Lu lU lu

                            (*
                            let mutable result : int64 = 0
                            if not <| Int64.TryParse(sb.ToString(), Globalization.NumberStyles.HexNumber, null, ref result) then
                                // TODO: Error in conversion
                                ()
                            else
                            *)
                            // Since the correct parsing (Int64.TryParse, UInt64.TryParse, etc depends upon the suffix, 
                            // the parsing is deferred to the parsing phase).
                            let mutable result = sb.ToString()
                            _tokens.Add(new Token(TokenClass.INT_NUMBER, result))
                    if not is_hexadecimal then
                        if Char.IsDigit(!_ch) then
                            let mutable is_floating_point = false
                            while_is_digit()
                            c <- !_c
                            _ch := (char)c
                            if !_ch = '.' then
                                c <- read()
                                _ch := (char)c
                                read_floating_point_after_dot() |> ignore
                                ()
                            if not (Char.IsDigit(!_ch)) then
                                prev_c <- c
                            _tokens.Add(new Token(TokenClass.INT_NUMBER, sb.ToString()))

                        else if Char.IsLetter(!_ch) || !_ch = '_' || !_ch = '@' then
                            // TODO: identifier
                            // TODO: is UNICODE correctly handled?
                            // TODO: also check if identifier is keyword after converting it to lower case
                            ()
                        else
                            while not (eof.Value) && Char.IsWhiteSpace(!_ch) do
                                c <- read()
                                _ch := (char)c
                                // handle newlines by updating location
                                if !_ch = '\n' then
                                    this._errorLocation.IncLine()
                            if not eof.Value then
                                prev_c <- c
                    // TODO: Comments parsing

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

        member public x.Location() : int64 =
            // TODO: keep track of the new lines so column:row style location can be reported.
            sr.BaseStream.Position

        member public x.ErrorLocation : Option<ErrorLocation> =
            None
