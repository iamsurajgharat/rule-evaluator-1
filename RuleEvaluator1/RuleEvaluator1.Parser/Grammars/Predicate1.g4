grammar Predicate1;
pexpr                   : '!' pexpr																# Pexpr_Negation
                        | expr POPERATORS expr													# Pexpr_Poperator
                        | pfunction																# Pexpr_Pfunction
                        | ID																	# Pexpr_Id
                        | BOOL																	# Pexpr_Bool
                        | '(' pexpr ')'															# Pexpr_Brackets
                        ;
expr                    : '-' expr																# Expr_UnaryOperation
                        | '!' pexpr																# Expr_NegationOperation
                        | expr OPERATOR expr													# Expr_BinaryOperation
                        | expr POPERATORS expr													# Expr_PBinaryOperation
                        | operand																# Expr_Value
                        | '(' expr ')'															# Expr_BracketExpression
                        | func																	# Expr_FunctionCall
                        | pfunction																# Expr_PFunctionCall
                        ;
operand                 : constant																# OperandConstant
                        | ID																	# Variable
                        ;
func                    : 'IF''(' pexpr ',' expr ',' expr ')'									# Func_If
                        | 'LOWER' '(' expr ')'													# Func_Lower
                        | 'UPPER' '(' expr ')'													# Func_Upper
                        | 'TRIM' '(' expr ')'													# Func_Trim
                        | 'ABS' '(' expr ')'													# Func_Abs
                        | 'CEILING' '(' expr ')'												# Func_Ceiling
                        | 'FLOOR' '(' expr ')'													# Func_Floor
                        | 'ROUND' '(' expr ')'													# Func_Round
                        | 'MAX' '(' expr (',' expr)+ ')'										# Func_Max
                        | 'MIN' '(' expr (',' expr)+ ')'										# Func_Min
                        | 'AVG' '(' expr (',' expr)+ ')'										# Func_Avg
                        | 'CASE' '(' expr (',' expr ',' expr)+ ',' expr ')'						# Func_Case
                        ;
OPERATOR                : '*'
                        | '/'
                        | '%'
                        | '+'
                        | '-'
                        ;
POPERATORS              : '<'|'>'|'<='|'>=' 
                        | '=='|'!='
                        | '&&'
                        | '||'
                        ;
pfunction               : 'BEGINS' '('expr ',' expr ')'											# Pfunc_Begins
                        | 'CONTAINS' '(' expr ',' expr ')'										# Pfunc_Contains
                        | 'ENDS' '(' expr ',' expr ')'											# Pfunc_Ends
                        | 'ISBLANK' '(' expr ')'												# Pfunc_Isblank
                        | 'ISNUMBER' '(' expr ')'												# Pfunc_Isnumber
                        ;
constant                : NUM																	# Number
                        | TEXT																	# Text
                        | BOOL																	# Bool
                        ;
NUM                     : [0-9]+(.[0-9]+)?
                        ;
TEXT                    : '"'[a-zA-Z0-9]*'"'
                        ;
BOOL                    : 'TRUE' | 'FALSE' | 'true' | 'false'
                        ;
ID                      : [a-zA-Z]+[a-zA-Z0-9]*
                        ;
WS                      : [ \t\r\n]+ -> skip
                        ;