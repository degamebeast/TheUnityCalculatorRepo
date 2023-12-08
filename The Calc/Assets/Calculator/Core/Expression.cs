//Created by: Deontae Albertie

using System.Collections.Generic;

namespace delib.calculate
{

    public class Expression : List<Token>
    {

        //clean - whether or not the tokens should be cleaned up and compacted upon construction
        public Expression(bool clean = true) : base()
        {
            if (clean)
                CleanExpression();

            MarkParameters(this);
        }

        //clean - whether or not the tokens should be cleaned up and compacted upon construction
        public Expression(string initialExpr, bool clean = true) : base(Tokenize(initialExpr))
        {
            if (clean)
                CleanExpression();

            MarkParameters(this);
        }

        //clean - whether or not the tokens should be cleaned up and compacted upon construction
        public Expression(List<Token> initTokens, bool clean = true) : base(initTokens)
        {
            if (clean)
                CleanExpression();

            MarkParameters(this);
        }

        //calc - a calculator that all identifiers will be resolved to upon construction. This value IS NOT stored inside the expression afterwards
        //clean - whether or not the tokens should be cleaned up and compacted upon construction
        public Expression(string initialExpr, Calculator calc, bool clean = true) : base(Tokenize(initialExpr))
        {
            calc.ResolveIdentifiers(this, true);

            if (clean)
                CleanExpression();

            MarkParameters(this);
        }
        //calc - a calculator that all identifiers will be resolved to upon construction. This value IS NOT stored inside the expression afterwards
        //clean - whether or not the tokens should be cleaned up and compacted upon construction
        public Expression(List<Token> initTokens, Calculator calc, bool clean = true) : base(initTokens)
        {
            calc.ResolveIdentifiers(this, true);

            if (clean)
                CleanExpression();

            MarkParameters(this);
        }


        public Expression SubExpression(int index, int count)
        {
            return new Expression(GetRange(index, count));
        }


        //splits up the expression into smaller expressions base on the 'End_Statement' token as the delimiter
        public Expression[] GetStatements()
        {
            RemoveAllNulls();
            List<Expression> statements = new List<Expression>();

            int curStatementTracker = 0;

            for (int i = 0; i < Count; i++)
            {
                if (this[i].Type == TokenTypeValue.End_Statement)
                {
                    statements.Add(new Expression(GetRange(curStatementTracker, i - curStatementTracker)));
                    curStatementTracker = i + 1;
                }
            }
            if (curStatementTracker < Count)
            {
                statements.Add(new Expression(GetRange(curStatementTracker, Count - curStatementTracker)));
            }

            CleanExpression();

            return statements.ToArray();
        }





        //USE WITH CAUTION: Nulls are designed to allow expression check safety. ONLY REMOVE IF YOU KNOW YOU DONT NEED THEM
        public Expression RemoveAllNulls()
        {
            RemoveAll(token => token.Type == TokenTypeValue.Null);

            return this;
        }

        //USE WITH CAUTION: Nulls are designed to allow expression check safety. ONLY REMOVE IF YOU KNOW YOU DONT NEED THEM
        public Expression RemoveAllIgnores()
        {
            RemoveAll(token => token.Type == TokenTypeValue.Ignore);

            return this;
        }

        public Expression AddNullCaps()
        {
            Insert(0, Token.NullToken);
            Add(Token.NullToken);

            return this;
        }


        //removes excess nulls from the expression and then cleans up the tokens
        public void CleanExpression()
        {
            RemoveAllNulls();
            RemoveAllIgnores();
            AddNullCaps();

            CleanUpTokens(this);
        }




        //turns all non condensed number representions into constants, will also condense grouped tokens into there own expressions
        //For Example Token(-) next to Token(10) becomes Token(-10)
        //returns the resulting Expression object
        private Expression CleanUpTokens(Expression expr)
        {
            return CondenseSubtractionLines(CondenseDots(CondenseParenthesis(expr)));
        }

        //converts all tokens between parenthesis' into their own expressions
        public static Expression CondenseParenthesis(Expression expr)
        {
            for (int start = expr.Count - 1; start >= 0; start--)
            {
                if (expr[start].Type == TokenTypeValue.Open_Paren)
                {
                    for (int end = start; end < expr.Count; end++)
                    {
                        if (expr[end].Type == TokenTypeValue.Close_Paren)
                        {
                            Expression paren = new Expression(expr.GetRange(start + 1, end - 1 - start));
                            if (expr[start - 1].Type == TokenTypeValue.Function)
                                expr[start] = new Token(paren, TokenTypeValue.Parameters);
                            else
                                expr[start] = new Token(paren);

                            expr.RemoveRange(start + 1, end - start);
                            break;
                        }

                    }
                }
            }

            return expr;
        }

        //Converts all decimal point tokens into floating point numbers and or Arguments
        public static Expression CondenseDots(Expression expr)
        {
            for (int index = expr.Count - 1; index >= 0; index--)
            {
                Token symbol = expr[index];
                if (symbol.Type != TokenTypeValue.Dot) continue;

                Token prev = expr[index - 1];
                Token next = expr[index + 1];

                if (next.Type == TokenTypeValue.Identifier)
                {
                    if (prev.Type == TokenTypeValue.Identifier)
                    {
                        expr[index - 1] = new Token($"{prev.ObjectName}.{next.ObjectName}");
                        expr.RemoveRange(index, 2);
                    }
                }
                else if (next.Type == TokenTypeValue.Integer)
                {
                    if (prev.Type == TokenTypeValue.Integer)
                    {
                        expr[index - 1] = new Token(new Constant(float.Parse($"{(Integer)prev.Value}.{(Integer)next.Value}")));
                        expr.RemoveRange(index, 2);
                    }
                    else if(!prev.Type.ResolvesTo(TokenTypeValue.Identifier))
                    {
                        expr[index] = new Token(new Constant(float.Parse($"0.{next.Value}")));
                        expr.RemoveAt(index + 1);
                    }
                }

            }

            return expr;
        }

        //converts any 'subtraction' tokens that are representing negation into a negitive number
        public static Expression CondenseSubtractionLines(Expression expr)
        {
            for (int index = expr.Count - 1; index >= 0; index--)
            {
                Token symbol = expr[index];
                if (symbol.Type != TokenTypeValue.Subtraction) continue;

                Token prev = expr[index - 1];
                if (Token.ResolvesTo(prev.Type, TokenTypeValue.Constant)) continue;


                expr[index] = new Token(new Constant(-1));
                expr.Insert(index + 1, new Token(TokenTypeValue.Multiplication));
            }

            return expr;
        }


        public static Expression MarkParameters(Expression expr)
        {
            for (int i = 1; i < expr.Count; i++)
            {
                if (expr[i - 1].Type == TokenTypeValue.Function)
                {
                    if (expr[i].Type == TokenTypeValue.Expression)
                    {
                        expr[i] = new Token(expr[i].Expression, TokenTypeValue.Parameters);
                    }
                }
            }


            return expr;
        }

        //does not change/alter invoking object
        public Expression GetResolveParameters()
        {
            return ResolveParameters(this);
        }



        //returns a new Expression with the given arguments resolved down to there respective types
        //the passed in expr is not altered
        public static Expression ResolveParameters(Expression expr)
        {
            Expression copy = new Expression(expr);
            copy.RemoveAllNulls();
            List<Token> parameters = new List<Token>();

            int curStatementTracker = 0;

            for (int i = 0; i < copy.Count; i++)
            {
                if (copy[i].Type == TokenTypeValue.Seperator)
                {
                    parameters.Add(new Token(new Expression(copy.GetRange(curStatementTracker, i - curStatementTracker))));
                    curStatementTracker = i + 1;
                }
            }
            if (curStatementTracker < copy.Count)
            {
                parameters.Add(new Token(new Expression(copy.GetRange(curStatementTracker, copy.Count - curStatementTracker))));
            }

            return new Expression(parameters);
        }

        //converts a string into a list of token's
        public static List<Token> Tokenize(string expr)
        {
            List<Token> tokens = new List<Token>();
            List<char> digitBuffer = new List<char>();
            List<char> letterBuffer = new List<char>();

            foreach (char character in expr)
            {
                if (char.IsDigit(character))
                {
                    if (letterBuffer.Count > 0)
                        letterBuffer.Add(character);
                    else
                        digitBuffer.Add(character);
                    continue;
                }

                if (digitBuffer.Count > 0)
                {
                    tokens.Add(new Token(int.Parse(new string(digitBuffer.ToArray()))));
                }

                digitBuffer.Clear();

                if (char.IsLetter(character) && digitBuffer.Count < 1)
                {
                    letterBuffer.Add(character);
                    continue;
                }

                if (letterBuffer.Count > 0)
                {
                    tokens.Add(new Token(new string(letterBuffer.ToArray())));
                }

                letterBuffer.Clear();




                TokenTypeValue charTypeVal;

                if (Library.SymbolTokenTypePairs.TryGetValue(character, out charTypeVal))
                {
                    tokens.Add(new Token(charTypeVal));
                    continue;
                }

                if (Library.Delimiters.Contains(character))
                {
                    tokens.Add(Token.IgnoreToken);
                    continue;
                }

                tokens.Add(Token.InvalidToken);
            }

            if (digitBuffer.Count > 0)
            {
                tokens.Add(new Token(int.Parse(new string(digitBuffer.ToArray()))));
            }

            if (letterBuffer.Count > 0)
            {
                tokens.Add(new Token(new string(letterBuffer.ToArray())));
            }


            return tokens;
        }

        #region validation
        public EvaluationTree GenerateEvaluationTree()
        {
            return new EvaluationTree(this);
        }

        public static bool Validate(string expr, params System.Type[] argTypes)
        {
            return new Expression(expr).Validate(null, argTypes);
        }

        public static bool Validate(string expr, Calculator calc, params System.Type[] argTypes)
        {
            return calc.ResolveIdentifiers(new Expression(expr)).Validate(calc, argTypes);
        }

        //returns true if this is a valid programmatical-mathematical expression (Within the context of this project). False otherwise
        public bool Validate(Calculator resCalc = null, params System.Type[] argTypes)
        {
            if (Find(token => token.Type == TokenTypeValue.Invalid) != null) return false;



            bool valid = true;
            foreach (Expression expr in GetStatements())
            {
                if (resCalc != null)
                {
                    resCalc.SetArguments(argTypes);
                    resCalc.ResolveIdentifiers(expr);
                }

                if (!new EvaluationTree(expr, argTypes).Validate())
                {
                    valid = false;
                    break;
                }
            }

            return valid;
        }
        #endregion
    }
}