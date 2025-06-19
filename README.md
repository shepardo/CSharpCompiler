




*** Current Objectives


12:07

- Aim for an engine of expression, for all C# 1.0 operators.
* Add support for Parentesis in expressions.
- Add unit tests for edge cases, including multiple operands.
- add support for tracking location of errors
- 



----------------------------------------------


https://www.mono-project.com/docs/about-mono/languages/csharp/


```


- Add support for more operators (*,/,%,&,^,|,&&,||,==,!=, =, +=, etc, -, ++,--,new,sizeof), 
https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/#operator-precedence



*** References


https://www.ecma-international.org/wp-content/uploads/ECMA-334_2nd_edition_december_2002.pdf

https://github.com/antlr/grammars-v4/blob/master/csharp/CSharpParser.g4
Has features of C# 8.0 like null coalesce operator ?? but doesnt seem to support pattern matching...

https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-version-history

https://dotnetcrunch.in/csharp-version-history-features/


*** C# Version History

We have mentioned the features of the C# language and demonstrated the evolution of C# from version C#1.0 to C#10 (the current version at the time of writing this post).
C# 1.0

Microsoft released the first version of C# with Visual Studio 2002. The use of Managed Code was introduced with this version. C# 1.0 was the first language that the developer adopted to build .NET applications.

The major features of C# 1.0 include:

    Classes
    Structs
    Interfaces
    Events
    Properties
    Delegates

C# 2.0

Microsoft released the second version of C# language with Visual Studio 2005. C# 2.0 has introduced a few new features in this edition which helped the developers to code their applications in a more generic way.

Here are the new features that were introduced with C# 2.0:

    Partial Class
    Generics
    Static Classes
    Nullable Types
    Co-variance and Contravariance

C# 3.0

Visual Studio 2008 came with C# version 3.0 and it has a bunch of new features. It was the life-changing language for Microsoft platform developers to build their applications. Till now, many developers are still using this version to build their apps. The new features that came with C# 3.0 were:

    Lambda Expression
    Extension Methods
    Expression Trees
    Anonymous Types
    LINQ
    Implicit Type (var)

C# 4.0

Though C# 4.0 was released with Visual Studio 2010 with .NET Framework 4, very few developers use its new features. Here is a list of new features of C# that came with this version:

    Late Binding
    Named Arguments
    Optional Parameters
    More COM Support
    Dynamic Keyword

C# 5.0

Visual Studio 2012 came up with C# 5.0 and it was made available to the audience in the year 2012. In C# version 5.0, there are two key features:

    Async Programming
    Caller Information

C# 6.0

The C# 6.0 release contained many features that improve productivity for developers. Some of the features in this release were:

    Read-only Auto-properties
    String Interpolation
    await in catch and finally blocks
    index initializers
    Null- conditional operators

C# 7.0

With Visual Studio 2017 (March 7 – 2017) we got a new version of C# – C# 7.0. There is a lot of new exciting features that nicely build on top of existing ones.

    Out variables
    Pattern matching
    Tuples
    Deconstruction
    Discards
    Local Functions
    Binary Literals
    Digit Separators
    Ref returns and locals
    Generalized async return types
    More expression-bodied members
    Throw expressions

C# 8.0

C# 8.0 adds the following features and enhancements to the C# language:

    Readonly members
    Default interface methods
    Pattern matching enhancements
    Using declarations
    Nullable reference types
    Asynchronous stream

C# 9.0

C#9 is the new version getting ready to be made available. Mads Torgersen, C# Lead Designer from .NET Team has shared some of the major features being added to C#9:

    Init-only properties
    Init accessors & read-only fields
    Records With-expressions
    Value-based equality
    Data members
    Positional records
    Records and mutation
    Top-level programs
    Improved pattern matching

C# 10

C# 10.0 is supported on .NET 6. In C#10, the following features & enhancements are being added to the C# language:

    Global usings
    Null parameter checking
    File-scoped namespaces
    Record structs
    Extended property patterns

At the time of updating this blog post, C#10 is the latest available version, you can refer to Language Feature Status on the Roslyn (C#/VB Compiler) GitHub repo.

To know these features in detail follow our post on C# 10 features.

-------------------

*** F# Notes


https://fsharp.org/
https://fsharp.org/specs/language-spec/4.1/FSharpSpec-4.1-latest.pdf


https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/generics/

Reference cells
https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/reference-cells
https://stackoverflow.com/questions/3221200/f-let-mutable-vs-ref
https://lorgonblog.wordpress.com/2008/11/09/the-f-ref-type/

https://stackoverflow.com/questions/1797241/while-or-tail-recursion-in-f-what-to-use-when
https://stackoverflow.com/questions/3248091/f-tail-recursive-function-example
https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/functions/recursive-functions-the-rec-keyword


Optional Parameters
https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/parameters-and-arguments
https://stackoverflow.com/questions/30255271/how-to-set-default-argument-value-in-f
https://www.markhneedham.com/blog/2010/04/12/f-the-defaultarg-function/
https://github.com/MicrosoftDocs/visualfsharpdocs/blob/main/docs/conceptual/operators.defaultarg%5B't%5D-function-%5Bfsharp%5D.md
https://www.fssnip.net/5z/title/Active-pattern-to-define-default-values
https://fsharp.github.io/fsharp-core-docs/reference/fsharp-core-optionmodule.html

https://stackoverflow.com/questions/30868572/is-there-default-parameter-in-f
https://musingstudio.com/2015/03/18/how-to-write-optional-parameters-in-f-with-default-values/
https://github.com/fsharp/fslang-suggestions/issues/1167
https://github.com/dotnet/fsharp/issues/5701
https://github.com/fsharp/fslang-design/blob/main/FSharp-4.1/FS-1027-complete-optional-defaultparametervalue.md


Unit testing
https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-fsharp-with-nunit


Cast operators

5 :> obj                 // upcast int to obj
(upcast 5 : obj)         // same
(box 5) :?> int          // downcast an obj to int (successfully)
(downcast (box 5) : int) // same
(box "5") :?> int        // downcast an obj to int (unsuccessfully)

https://stackoverflow.com/questions/31616761/f-casting-operators
https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/casting-and-conversions
https://stackoverflow.com/questions/2480250/f-equivalent-of-the-c-sharp-typeofienumerable

https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/modules
https://www.luisquintanilla.me/posts/fsharpmodulesample
https://www.reddit.com/r/fsharp/comments/u287kc/is_it_possible_to_divide_a_module_up_between_files/



Empty types
https://stackoverflow.com/questions/6668653/how-to-create-empty-class-in-f
https://theburningmonk.com/2011/12/f-define-empty-class-struct-or-interface-types/

Classes
https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/classes
https://stackoverflow.com/questions/38290418/f-difference-between-type-defined-with-and-without-parenthesis
https://dev.to/shimmer/f-tip-1-don-t-use-classes-4c1j
https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/interfaces
https://fsharpforfunandprofit.com/posts/classes/


DefaultValue
https://fsharp.github.io/fsharp-core-docs/reference/fsharp-core-defaultvalueattribute.html
https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/members/explicit-fields-the-val-keyword


Abstract classes
https://stackoverflow.com/questions/37632458/interfaces-and-abstract-classes-in-f
https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/abstract-classes

Interfaces
https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/interfaces
https://stackoverflow.com/questions/34719086/how-do-you-implement-an-interface-in-f
https://fsharpforfunandprofit.com/posts/interfaces/
https://www.tutorialspoint.com/fsharp/fsharp_interfaces.htm


https://fsharpforfunandprofit.com/posts/classes/
https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/interfaces
https://dev.to/shimmer/f-tip-1-don-t-use-classes-4c1j
https://fsharpforfunandprofit.com/posts/interfaces/
https://fsharpforfunandprofit.com/posts/classes/

https://stackoverflow.com/questions/33014900/classes-without-constructor-in-f

https://www.tutorialspoint.com/fsharp/fsharp_if_elif_else_statement.htm
https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/conditional-expressions-if-then-else


Linq
https://dotnetcodr.com/2017/06/10/linq-statements-in-f/
https://www.c-sharpcorner.com/UploadFile/mgold/writing-equivalent-linq-expressions-in-fsharp/
https://tomasp.net/blog/2015/query-translation/
https://markheath.net/post/linq-challenge-2-answers-fsharp
https://tomasp.net/blog/dynamic-flinq.aspx/



***

https://www.codeconvert.ai/csharp-to-fsharp-converter
https://github.com/willsam100/FShaper

-----------

*** Really good articles

https://softwareparticles.com/how-to-dynamically-execute-code-in-net/

https://www.codeproject.com/Articles/121568/Dynamic-Type-Using-Reflection-Emit
https://www.codeproject.com/Articles/13337/Introduction-to-Creating-Dynamic-Types-with-Reflec


