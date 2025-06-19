// Copyright 2018, Fernando Gonzalez Sanchez

namespace CSharpCompiler

open System.IO
open System.Text

module main =
            
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