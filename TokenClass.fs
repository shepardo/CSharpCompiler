namespace CSharpCompiler

//module main =

// keywords to add:
// ref, out, bool, byte, char, decimal, double, float, int, long, object, sbyte, short, string, uint, ulong, ushort
// this, base, new, typeof, checked, unchecked
// 

    type TokenClass =
        INT_NUMBER = 1
        // operators
        | OP_PLUS = 2
        | OP_MINUS = 3
        | LPARENT = 4
        | RPARENT = 5
        | OP_TIMES_OR_INDIRECTION = 6
        | OP_DIVIDE = 7
        | OP_MODULUS = 8
        | OP_PLUS_PLUS = 9     // TODO
        | OP_MINUS_MINUS = 10  // TODO
        | OP_BANG = 111
        | OP_LEFT_SHIFT = 12
        | OP_RIGHT_SHIFT = 13
        | OP_RIGHT_SHIFT_UNSIGNED = 14
        | OP_LESS_THAN = 15
        | OP_LESS_THAN_EQUAL = 16
        | OP_GREATER_THAN = 17
        | OP_GREATER_THAN_EQUAL = 18
        //| IS = 19       // TODO
        //| AS = 20       // TODO
        | OP_EQUAL = 21
        | OP_NOT_EQUAL = 22
        | OP_BITWISE_AND_OR_ADDRESS_OF = 23
        | OP_BITWISE_XOR = 24
        | OP_BITWISE_OR = 25
        | OP_CONDITONAL_AND = 26
        | OP_CONDITIONAL_OR = 27
        | OP_CONDITONAL_TERNARY_OPEN = 28
        | OP_CONDITONAL_TERNARY_CLOSE = 29
        | OP_ASSIGNMENT = 30
        | OP_ASSIGNMENT_PLUS = 31
        | OP_ASSIGNMENT_MINUS = 32
        | OP_ASSIGNMENT_TIMES = 33
        | OP_ASSIGNMENT_DIVIDE = 34
        | OP_ASSIGNMENT_MODULUS = 35
        | OP_ASSIGNMENT_AND = 36
        | OP_ASSIGNMENT_OR = 37
        | OP_ASSIGNMENT_XOR = 38
        | OP_ASSIGNMENT_LEFT_SHIFT = 39
        | OP_ASSIGNMENT_RIGHT_SHIFT = 40
        | OP_ASSIGNMENT_RIGHT_SHIFT_UNSIGNED = 41
        | OP_BITWISE_NEGATE = 42

        | IDENTIFIER = 43
        | DOT = 44
        | LBRACE = 45
        | RBRACE = 46
        | LSQUARE_BRACKET = 47
        | RSQUARE_BRACKET = 48
        | COMMA = 49
        | OP_MEMBER_ARROW = 50
        | SEMICOLON = 51
        | FLOAT_NUMBER = 52
        | QUOTE = 53
        | DOUBLE_QUOTE = 54
        | AT = 55

        // TODO: Comments, string, Spaces

        // keywords
        | KEYWORD_ABSTRACT = 100
        | KEYWORD_AS = 101
        | KEYWORD_BASE = 102
        | KEYWORD_BOOL = 103
        | KEYWORD_BREAK = 104
        | KEYWORD_BYTE = 105
        | KEYWORD_CASE = 106
        | KEYWORD_CATCH = 107
        | KEYWORD_CHAR = 108
        | KEYWORD_CHECKED = 109
        | KEYWORD_CLASS = 110
        | KEYWORD_CONST = 111
        | KEYWORD_CONTINUE = 112
        | KEYWORD_DECIMAL = 113
        | KEYWORD_DEFAULT = 114
        | KEYWORD_DELEGATE = 115
        | KEYWORD_DO = 116
        | KEYWORD_DOUBLE = 117
        | KEYWORD_ELSE = 118
        | KEYWORD_ENUM = 119
        | KEYWORD_EVENT = 120
        | KEYWORD_EXPLICIT = 121
        | KEYWORD_EXTERN = 122
        | KEYWORD_FALSE = 123
        | KEYWORD_FINALLY = 124
        | KEYWORD_FIXED = 125
        | KEYWORD_FLOAT = 126
        | KEYWORD_FOR = 127
        | KEYWORD_FOREACH = 128
        | KEYWORD_GOTO = 129
        | KEYWORD_IF = 130
        | KEYWORD_IMPLICIT = 131
        | KEYWORD_IN = 132
        | KEYWORD_INT = 133
        | KEYWORD_INTERFACE = 134
        | KEYWORD_INTERNAL = 135
        | KEYWORD_IS = 136
        | KEYWORD_LOCK = 137
        | KEYWORD_LONG = 138
        | KEYWORD_NAMESPACE = 139
        | KEYWORD_NEW = 140
        | KEYWORD_NULL = 141
        | KEYWORD_OBJECT = 142
        | KEYWORD_OPERATOR = 143
        | KEYWORD_OUT = 144
        | KEYWORD_OVERRIDE = 145
        | KEYWORD_PARAMS = 146
        | KEYWORD_PRIVATE = 147
        | KEYWORD_PROTECTED = 148
        | KEYWORD_PUBLIC = 149
        | KEYWORD_READONLY = 150
        | KEYWORD_REF = 151
        | KEYWORD_RETURN = 152
        | KEYWORD_SBYTE = 153
        | KEYWORD_SEALED = 154
        | KEYWORD_SHORT = 155
        | KEYWORD_SIZEOF = 156
        | KEYWORD_STACKALLOC = 157
        | KEYWORD_STATIC = 158
        | KEYWORD_STRING = 159
        | KEYWORD_STRUCT = 160
        | KEYWORD_SWITCH = 161
        | KEYWORD_THIS = 162
        | KEYWORD_THROW = 163
        | KEYWORD_TRUE = 164
        | KEYWORD_TRY = 165
        | KEYWORD_TYPEOF = 166
        | KEYWORD_UINT = 167
        | KEYWORD_ULONG = 168
        | KEYWORD_UNCHECKED = 169
        | KEYWORD_UNSAFE = 170
        | KEYWORD_USHORT = 171
        | KEYWORD_USING = 172
        | KEYWORD_VIRTUAL = 173
        | KEYWORD_VOID = 174
        | KEYWORD_VOLATILE = 175
        | KEYWORD_WHILE = 176

        | EOF = 65535