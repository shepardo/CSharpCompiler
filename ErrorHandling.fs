namespace CSharpCompiler

open System.Collections.Generic
open System.IO

module ErrorHandling =
    type public ErrorLocation(line: int32, column: int32, filePath: string) as this =
        class
            [<DefaultValue>] val mutable private _line: int32
            [<DefaultValue>] val mutable private _column: int32
            [<DefaultValue>] val mutable private _filePath: string

            do
                this._line <- line
                this._column <- column
                this._filePath <- filePath

            member public x.Line
                with get(): int32 = x._line

            member public x.Column
                with get(): int32 = x._column

            member public x.FilePath
                with get(): string = x._filePath
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

    type public ErrorManager(textWriter: TextWriter) as this =
        [<DefaultValue>] val mutable private _errorCollection: List<ErrorMessage>
        [<DefaultValue>] val mutable private _textWriter : TextWriter
        do
            this._errorCollection <- new List<ErrorMessage>()
            this._textWriter <- textWriter

        member public x.AddMessage(errorMessage: ErrorMessage): Unit =
            this._errorCollection.Add(errorMessage)

        member private x.PrintMessage(errorMessage: ErrorMessage): Unit =
            this._textWriter.Write(
                "{0} at file {1} [{2}:{3}]",
                errorMessage.Message,
                errorMessage.Location.FilePath,
                errorMessage.Location.Line,
                errorMessage.Location.Column
            )

        member public x.PrintMessages(): int32 =
            for errorMessage in this._errorCollection do
                this.PrintMessage(errorMessage)
                this._textWriter.WriteLine()
            this._errorCollection.Count
