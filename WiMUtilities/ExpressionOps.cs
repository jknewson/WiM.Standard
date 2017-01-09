//------------------------------------------------------------------------------
//----- ExpressionOps -------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2014 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//             
// 
//   purpose:  parsing and calculating algorithm based on Shunting-yard's algorithm that parses mathematical 
//             expressions in infix notation.
//          
//discussion:  http://en.wikipedia.org/wiki/Shunting-yard_algorithm
//             numbers are added to outputQueue 
//             operations and functions are pushed onto the tokenStack 
//             function argument separator
//                  Until token is at the top of the stacks left parenthesis, pop operators off the stack into output que
//             Infix notation :         3 + 4 * AREA / ( 1 − 5 ) ^ 2 ^ 3
//             Reverse Polish Notation: 3 4 AREA * 1 5 − 2 3 ^ ^ / +
//
//             Queue - A FIFO (first in, first out) list where you push records on top and pop them off the bottom
//             Stack - A LIFO (last in, first out) list where you push/pop records on top of each other.
//
//             Regex: http://rick.measham.id.au/paste/explain.pl

#region "Comments"
//08.12.2014 jkn - Created
//10.15.2015 jkn - Added function support
#endregion

#region "Imports"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
#endregion

namespace WiM.Utilities
{
    public class ExpressionOps
    {
        #region "Fields"
	    
        #endregion
        #region "Properties"        
        public string InfixExpression { get; private set; }
        public String PostFixExpression { get; private set; }
        public Double Value { get; private set; }
        public Boolean IsValid { get; private set; }
        
        #endregion
        #region "Collections & Dictionaries"
        private IDictionary<OperationEnum, OperatorStruc> _operators;
        private Queue<String> outputQueue = new Queue<String>();
        private IDictionary<String, Double?> _variables;

        #endregion
        #region Constructors
        public ExpressionOps(string expr)
        {
            InfixExpression = expr;
            init();
            parseEquation();
            if (IsValid) evaluate();            
        }
        public ExpressionOps(String expr, Dictionary<String, Double?> variables)
        {
            InfixExpression = expr;
            init();

            _variables = variables;
            parseEquation();
            if (IsValid) evaluate();         
        }

        #endregion        
        #region "Methods"
        
        #endregion
        #region "Helper Methods"

        private void init()
        {
            this.IsValid = false;
            initOperators();
        }
        private void parseEquation()
        {
            Stack<String> tokenStack = new Stack<String>();
            List<String> tokenList;
            try
            {
                //Dijkstra's Shunting Yard Algorithm
                Regex re = new Regex(@"((?<!\d[eE])[+-]|(?:>=|<=|=|<|>)|[,\*\(\)\^\/\ ])");
                
                tokenList = re.Split(InfixExpression).Select(t => t.Trim()).Where(t => t != "").ToList();

                adjustForUnary(ref tokenList);

                for (int tokenNumber = 0; tokenNumber < tokenList.Count(); ++tokenNumber)
                {
                    String token = tokenList[tokenNumber];
                    TokenClassEnum tokenClass = getTokenClass(token);

                    switch (tokenClass)
                    {
                        case TokenClassEnum.e_variable:
                        case TokenClassEnum.e_constant:
                        case TokenClassEnum.e_value:
                            outputQueue.Enqueue(token);
                            break;

                        case TokenClassEnum.e_function:
                            tokenStack.Push(token);
                            break;
                        case TokenClassEnum.e_relational:
                            tokenStack.Push(token);
                            break;
                        case TokenClassEnum.e_operator:
                            if (token == "-" && (tokenStack.Count == 0 || tokenList[tokenNumber - 1] == "("))
                            {
                                tokenStack.Push(token);
                                break;
                            }//end if


                            //remove any operators already on the tokenstack that have higher or equal precedence and append them to the output list.
                            var stackHigherOrEquivalentPrecedence = false;
                            do
                            {
                                stackHigherOrEquivalentPrecedence = false;
                                if (tokenStack.Count > 0)
                                {                                        
                                    String stackTopToken = tokenStack.Peek();
                                    if (getTokenClass(stackTopToken) == TokenClassEnum.e_operator)
                                    {
                                        AssociativityEnum tokenAssociativity = getOperatorAssociativity(token);
                                        int tokenPrecedence = getOperatorPrecedence(token);
                                        int stackTopPrecedence = getOperatorPrecedence(stackTopToken);

                                        if (tokenAssociativity == AssociativityEnum.e_left && tokenPrecedence <= stackTopPrecedence ||
                                                tokenAssociativity == AssociativityEnum.e_right && tokenPrecedence < stackTopPrecedence)
                                        {
                                            stackHigherOrEquivalentPrecedence = true;
                                            outputQueue.Enqueue(tokenStack.Pop());                                            
                                        }//end if
                                    }//end if
                                }//end if
                            } while (stackHigherOrEquivalentPrecedence);
                            
                            tokenStack.Push(token);
                            break;
                        case TokenClassEnum.e_leftparenthesis:
                            tokenStack.Push(token);
                            break;
                        case TokenClassEnum.e_functionArgSeparator:
                            while (!(tokenStack.Peek().Equals("(")))
                            {
                                outputQueue.Enqueue(tokenStack.Pop());
                            }//next

                            break;
                        case TokenClassEnum.e_rightparenthesis:
                            while (!(tokenStack.Peek().Equals("(")))
                            {
                                outputQueue.Enqueue(tokenStack.Pop());
                            }//next
                            tokenStack.Pop();

                            if (tokenStack.Count > 0 && getTokenClass(tokenStack.Peek()) == TokenClassEnum.e_function)
                                outputQueue.Enqueue(tokenStack.Pop());
                           
                            break;
                    }//end switch

                    if (tokenClass == TokenClassEnum.e_value || tokenClass == TokenClassEnum.e_rightparenthesis ||
                        tokenClass == TokenClassEnum.e_variable || tokenClass == TokenClassEnum.e_functionArgSeparator)
                    {
                        if (tokenNumber < tokenList.Count() - 1)
                        {
                            String nextToken = tokenList[tokenNumber + 1];
                            TokenClassEnum nextTokenClass = getTokenClass(nextToken);
                        }//end if
                    }//end if

                }//next token

                queOperationStack(tokenStack);

                //---------------- set equation -----------------------
                PostFixExpression = String.Join(",", outputQueue.Select(t => t).ToArray());

                IsValid = true;
            }
            catch (Exception)
            {

                IsValid = false;
            }
        }
        private void evaluate()
        {
            Stack<String> expressionStack = new Stack<String>();
            try
            {
                while (outputQueue.Count > 0)
                {
                    String operand = outputQueue.Dequeue();

                    switch (this.getTokenClass(operand))
                    {
                        case TokenClassEnum.e_variable:
                            expressionStack.Push(Convert.ToString(_variables[operand].Value));
                            break;
                        case TokenClassEnum.e_constant:
                            expressionStack.Push(Convert.ToString(getConstantValue(operand)));
                            break;
                        case TokenClassEnum.e_value:
                            expressionStack.Push(operand);
                            break;
                        case TokenClassEnum.e_function:
                            List<double> funcArgs = new List<double>();
                            var argNumber = getFunctionArgNumber(getFunctionEnum(operand));
                            for (int argNum = 0; argNum < argNumber; ++argNum)
                                funcArgs.Add(Convert.ToDouble(expressionStack.Pop()));

                            var funcResult = doFunction(getFunctionEnum(operand), funcArgs);
                            expressionStack.Push(funcResult.ToString());
                            break;
                        case TokenClassEnum.e_operator:

                            String rightOperand = expressionStack.Pop();
                            String leftOperand = expressionStack.Pop();

                            var result = doOperation(Convert.ToDouble(leftOperand), Convert.ToDouble(rightOperand), getOperationEnum(operand));

                            expressionStack.Push(result.ToString());
                            break;
                        case TokenClassEnum.e_relational:

                            String rightRelation = expressionStack.Pop();
                            String leftRelation = expressionStack.Pop();

                            var isRelated = doRelation(Convert.ToDouble(leftRelation), Convert.ToDouble(rightRelation), getRelationalEnum(operand));
                            expressionStack.Push(Convert.ToInt32(isRelated).ToString());
                            break;
                        default:
                            throw new Exception(operand + " found in calculate");
                    }//end switch                     
                }//next

                if (expressionStack.Count != 1) new ArgumentException("Invalid formula");

                this.Value = Convert.ToDouble(expressionStack.Pop());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }       
        
        private void queOperationStack(Stack<string> stack)
        {

            while (stack.Count > 0)
            {
                String operand = stack.Pop();
                if (this.getTokenClass(operand) == TokenClassEnum.e_leftparenthesis || this.getTokenClass(operand) == TokenClassEnum.e_rightparenthesis)
                {
                    IsValid = false;
                    throw new ArgumentException("Mismatched parentheses");
                }

                outputQueue.Enqueue(operand);
            }
        }
        private void adjustForUnary(ref List<string> tokenList)
        {

            for (int tokenNumber = 0; tokenNumber < tokenList.Count(); ++tokenNumber)
            {
                String token = tokenList[tokenNumber];
                if ((getOperationEnum(token) == OperationEnum.e_minus || getOperationEnum(token) == OperationEnum.e_plus) &&
                                                                        tokenNumber > 1 && (getTokenClass(tokenList[tokenNumber - 1]) == TokenClassEnum.e_operator ||
                                                                                            //getTokenClass(tokenList[tokenNumber - 1]) == TokenClassEnum.e_rightparenthesis ||
                                                                                            getTokenClass(tokenList[tokenNumber - 1]) == TokenClassEnum.e_leftparenthesis ||
                                                                                            getTokenClass(tokenList[tokenNumber - 1]) == TokenClassEnum.e_function ||
                                                                                            getTokenClass(tokenList[tokenNumber - 1]) == TokenClassEnum.e_functionArgSeparator))                                                                                                                 
                {
                    //remove neg from the list, and add it to the begin of next var
                    tokenList[tokenNumber + 1] = tokenList[tokenNumber] + tokenList[tokenNumber + 1];
                    tokenList.RemoveAt(tokenNumber);
              
                }//end if

            }//next
        }   
        private TokenClassEnum getTokenClass(string token)
        {
            double tempValue;
            if (double.TryParse(token, out tempValue) ||
                token.Equals("R", StringComparison.CurrentCultureIgnoreCase) ||
                token.Equals("S", StringComparison.CurrentCultureIgnoreCase))
            {
                return TokenClassEnum.e_value;
            }
            else if (isFunction(token))
            {
                return TokenClassEnum.e_function;
            }
            else if (token == "(")
            {
                return TokenClassEnum.e_leftparenthesis;
            }
            else if (token == ")")
            {
                return TokenClassEnum.e_rightparenthesis;
            }
            else if (token == ",")
            {
                return TokenClassEnum.e_functionArgSeparator;
            }
            else if (isOperator(token))
            {
                return TokenClassEnum.e_operator;
            }
            else if (isRelational(token))
            {
                return TokenClassEnum.e_relational;
            }
            else if (isConstant(token))
            {
                return TokenClassEnum.e_constant;
            }
            else
            {
                //lookup variable name
                try
                {
                    if (_variables[token].HasValue)
                        tempValue = Convert.ToDouble(_variables[token].Value);

                    return TokenClassEnum.e_variable;
                }
                catch (Exception ex)
                {
                    IsValid = false;
                    throw new Exception("token class unidentified");
                }
            }
        }
        
        #region ConstantHelpers
        private bool isConstant(string token)
        {
            ConstantEnum oper = getConstantEnum(token);
            if (oper == ConstantEnum.e_undefined)
                return false;
            return true;
        }
        private double getConstantValue(String token)
        {
            switch (getConstantEnum(token))
            {
                case ConstantEnum.e_eulernumber:
                    return Math.E;
                case ConstantEnum.e_goldenratio:
                    return 1.61803398874989;
                case ConstantEnum.e_pi:
                    return Math.PI;

                default:
                    IsValid = false;
                    return 0;
            }//end switch
        }
        private ConstantEnum getConstantEnum(String a)
        {
            switch (a.ToLower())
            {
                case "e":
                case "e#":
                    return ConstantEnum.e_eulernumber;
                case "pi":
                    return ConstantEnum.e_pi;
                case "phi":
                    return ConstantEnum.e_goldenratio;

                default:
                    return ConstantEnum.e_undefined;
            }//end switch
        }
        #endregion
        #region OperatorHelpers
        private void initOperators() {
            _operators = new Dictionary<OperationEnum, OperatorStruc>();

            _operators.Add(OperationEnum.e_plus, new OperatorStruc(1, AssociativityEnum.e_left));
            _operators.Add(OperationEnum.e_minus, new OperatorStruc(1, AssociativityEnum.e_left));
            _operators.Add(OperationEnum.e_multiply, new OperatorStruc(2, AssociativityEnum.e_left));
            _operators.Add(OperationEnum.e_divide, new OperatorStruc(2, AssociativityEnum.e_left));
            _operators.Add(OperationEnum.e_percent, new OperatorStruc(2, AssociativityEnum.e_left));
            _operators.Add(OperationEnum.e_exponent, new OperatorStruc(3, AssociativityEnum.e_right));
        }
        private bool isOperator(string token)
        {
            OperationEnum oper = getOperationEnum(token);
            if (oper == OperationEnum.e_undefined)
                return false;
            return true;
        }
        private int getOperatorPrecedence(String token)
        {
            return _operators[getOperationEnum(token)].Precedence;
        }
        private AssociativityEnum getOperatorAssociativity(String token)
        {
            return _operators[getOperationEnum(token)].Associativity;
        }
        private Double doOperation(Double val1, Double val2, OperationEnum operation)
        {
            switch (operation)
            {
                case OperationEnum.e_multiply:
                    return val1 * val2;
                case OperationEnum.e_divide:
                    return val1 / val2;
                case OperationEnum.e_percent:
                    return (int)val1 % (int)val2;
                case OperationEnum.e_plus:
                    return val1 + val2;
                case OperationEnum.e_minus:
                    return val1 - val2;
                case OperationEnum.e_exponent:
                    return (float)System.Math.Pow(val1, val2);
                default:
                    IsValid = false;
                    return 0;
            }
        }
        private OperationEnum getOperationEnum(String a)
        {
            switch (a)
            {
                case "+":
                    return OperationEnum.e_plus;
                case "-":
                    return OperationEnum.e_minus;
                case "*":
                    return OperationEnum.e_multiply;
                case "/":
                    return OperationEnum.e_divide;
                case "^":
                    return OperationEnum.e_exponent;
                case "%":
                    return OperationEnum.e_percent;
                default:
                    return OperationEnum.e_undefined;
            }//end switch
        }
        #endregion
        #region RelationalHelpers
        private bool isRelational(string token)
        {
            RelationalEnum oper = getRelationalEnum(token);
            if (oper == RelationalEnum.e_undefined)
                return false;
            return true;
        }
        private Boolean doRelation(Double val1, Double val2, RelationalEnum operation)
        {
            switch (operation)
            {
                case RelationalEnum.e_equal:
                    return val1 == val2;
                case RelationalEnum.e_greaterthan:
                    return val1 > val2;
                case RelationalEnum.e_greaterthanorequal:
                    return val1 >= val2;
                case RelationalEnum.e_lessthanorequal:
                    return val1 <= val2;
                case RelationalEnum.e_lessthan:
                    return val1 < val2;
                case RelationalEnum.e_and:
                    return Convert.ToBoolean(val1) && Convert.ToBoolean(val2);
                case RelationalEnum.e_or:
                    return Convert.ToBoolean(val1) || Convert.ToBoolean(val2);

                default:
                    IsValid = false;
                    return false;
            }
        }
        private RelationalEnum getRelationalEnum(String a)
        {
            switch (a.ToUpper())
            {
                case "=":
                    return RelationalEnum.e_equal;
                case "<=":
                    return RelationalEnum.e_lessthanorequal;
                case ">=":
                    return RelationalEnum.e_greaterthanorequal;
                case "<":
                    return RelationalEnum.e_lessthan;
                case ">":
                    return RelationalEnum.e_greaterthan;
                case "AND":
                    return RelationalEnum.e_and;
                case "OR":
                    return RelationalEnum.e_or;
                default:
                    return RelationalEnum.e_undefined;
            }//end switch
        }
        #endregion
        #region FunctionHelpers
        private bool isFunction(string token)
        {
            FunctionEnum oper = getFunctionEnum(token);
            if (oper == FunctionEnum.e_undefined)
                return false;
            return true;
        }
        private Int32 getFunctionArgNumber(FunctionEnum operand)
        {
            switch (operand)
            {
                case FunctionEnum.e_sqrt:
                case FunctionEnum.e_exponential:
                case FunctionEnum.e_naturallog:
                    return 1;

                case FunctionEnum.e_logn:
                case FunctionEnum.e_max:
                case FunctionEnum.e_min:
                case FunctionEnum.e_round:
                    return 2;

                default:
                    IsValid = false;
                    return 0;
            }
        }
        private Double doFunction(FunctionEnum function, List<double> funcArgs)
        {
            if (funcArgs.Count() != getFunctionArgNumber(function)) function = FunctionEnum.e_undefined;
            switch (function)
            {
                case FunctionEnum.e_sqrt:
                    return System.Math.Sqrt(funcArgs[0]);
                case FunctionEnum.e_exponential:
                    return System.Math.Exp(funcArgs[0]);
                case FunctionEnum.e_naturallog:
                    return System.Math.Log(funcArgs[0]);
                case FunctionEnum.e_logn:
                    return System.Math.Log(funcArgs[1], funcArgs[0]);
                case FunctionEnum.e_max:
                    return System.Math.Max(funcArgs[1], funcArgs[0]);
                case FunctionEnum.e_min:
                    return System.Math.Min(funcArgs[1], funcArgs[0]);
                case FunctionEnum.e_round:
                    return System.Math.Round(funcArgs[1], Convert.ToInt32(funcArgs[0]));
                

                default:
                    IsValid = false;
                    return -9999;
            }
        }
        private FunctionEnum getFunctionEnum(String f)
        {
            switch (f.ToLower())
            {
                case "sqrt":
                    return FunctionEnum.e_sqrt;
                case "logn":
                    return FunctionEnum.e_logn;
                case "ln":
                    return FunctionEnum.e_naturallog;
                case "max":
                    return FunctionEnum.e_max;
                case "min":
                    return FunctionEnum.e_min;
                case "round":
                    return FunctionEnum.e_round;
                case "exp":
                    return FunctionEnum.e_exponential;

                default:
                    return FunctionEnum.e_undefined;
            }//end switch

        }        
        #endregion
        #endregion
        #region Structures
        private struct OperatorStruc
        {
            public Int32 Precedence { get; set; }
            public AssociativityEnum Associativity { get; set; }

            public OperatorStruc(Int32 presidence, AssociativityEnum associativity):this() 
            {
                Precedence = presidence;
                Associativity = associativity;
            }
        }
        #endregion
        #region "Enumerated Constants"
        public enum TokenClassEnum
        {
            e_value,
            e_function,
            e_rightparenthesis,
            e_leftparenthesis,
            e_operator,
            e_variable,
            e_functionArgSeparator,
            e_constant,
            e_relational
        }
        public enum OperationEnum 
        { 
            e_undefined =-1,
            e_plus = 1, 
            e_minus = 2, 
            e_multiply = 3, 
            e_divide = 4, 
            e_exponent = 5, 
            e_percent = 8
        };
        public enum RelationalEnum
        {
            e_undefined = -1,
            e_equal = 1,
            e_greaterthanorequal =2,
            e_lessthanorequal = 3,
            e_greaterthan = 4,
            e_lessthan = 5,
            e_and,
            e_or
        };
        public enum ConstantEnum
        {
            e_undefined = -1,
            e_eulernumber = 1,
            e_pi = 2,
            e_goldenratio =3
        };
        public enum AssociativityEnum
        {
            e_right,
            e_left
        }
        public enum FunctionEnum
        {
            e_undefined = -1,
            e_sqrt = 1,
            e_logn = 2,
            e_max =3,
            e_min =4,
            e_round =5,
            e_exponential=6,
            e_naturallog =7

        };
        #endregion
        
    }//end class
}//end namespace
