namespace CSharpCompiler


module ErrorHandling =
    type public ErrorLocation(lineNumber: int32, columnNumber: int32, filePath: string) as this =
        class
            [<DefaultValue>] val mutable private _lineNumber: int32
            [<DefaultValue>] val mutable private _columnNumber: int32
            [<DefaultValue>] val mutable private _filePath: string

            do
                this._lineNumber <- lineNumber
        end

    type public ErrorMessage(message : string, location: ErrorLocation) as this =
        [<DefaultValue>] val mutable private _message: string
        [<DefaultValue>] val mutable private _location: ErrorLocation
        do
            this._message<- message
            this._location  <- location

        member public x.Message
            with get() : string = x._message
            and set(value) = x._message <- value
        member public x.Location
            with get() : ErrorLocation = x._location
            and set(value) = x._location <- value

    type public ErrorManager() as this =
        do()