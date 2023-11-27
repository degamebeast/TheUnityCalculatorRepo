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
        }

        //clean - whether or not the tokens should be cleaned up and compacted upon construction
        public Expression(string initialExpr, bool clean = true) : base(Tokenize(initialExpr))
        {
            if (clean)
                CleanExpression();
        }

        //clean - whether or not the tokens should be cleaned up and compacted upon construction
        public Expression(List<Token> initTokens, bool clean = true) : base(initTokens)
        {
            if (clean)
                CleanExpression();
        }

        //calc - a calculator that all identifiers will be resolved to upon construction. This value IS NOT stored inside the expression afterwards
        //clean - whether or not the tokens should be cleaned up and compacted upon construction
        public Expression(string initialExpr, Calculator calc, bool clean = true) : base(Tokenize(initialExpr))
        {
            calc.ResolveIdentifiers(this);

            if (clean)
                CleanExpression();
        }
        //calc - a calculator that all identifiers will be resolved to upon construction. This value IS NOT stored inside the expression afterwards
        //clean - whether or not the tokens should be cleaned up and compacted upon construction
        public Expression(List<Token> initTokens, Calculator calc, bool clean = true) : base(initTokens)
        {
            calc.ResolveIdentifiers(this);

            if (clean)
                CleanExpression();
        }


        public Expression SubExpression(int index, int count)
        {
            return new Expression(GetRange(index, count));
        }


        //splits up the expression into smaller expressions base on the 'End_Statement' token as the delimiter
        public Expression[] GetStatements()
        {
            RemoveNulls();
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
        public Expression RemoveNulls()
        {
            RemoveAll(token => token.Type == TokenTypeValue.Null);

            return this;
        }


        //removes excess nulls from the expression and then cleans up the tokens
        public void CleanExpression()
        {
            RemoveNulls();
            Insert(0, Token.NullToken);
            Add(Token.NullToken);

            CleanUpTokens(this);
        }




        //turns all non condensed number representions into constants, will also condense grouped tokens into there own expressions
        //For Example Token(-) next to Token(10) becomes Token(-10)
        //returns the resulting Expression object
        private Expression CleanUpTokens(Expression expr)
        {
            //converts all tokens between parenthesis' into their own expressions
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
                                expr[start] = new Token(paren, TokenTypeValue.Arguments);
                            else
                                expr[start] = new Token(paren);

                            expr.RemoveRange(start + 1, end - start);
                            break;
                        }

                    }
                }
            }

            //Converts all decimal point tokens into floating point numbers
            for (int index = expr.Count - 1; index >= 0; index--)
            {
                Token symbol = expr[index];
                if (symbol.Type != TokenTypeValue.Decimal_Point) continue;

                Token prev = expr[index - 1];
                Token next = expr[index + 1];
                if (prev.Type != TokenTypeValue.Constant)
                {
                    expr[index] = new Token(new Constant(float.Parse($"0.{next.Value}")));
                    expr.RemoveAt(index + 1);

                }
                else
                {
                    expr[index - 1] = new Token(new Constant(float.Parse($"{prev.Value}.{next.Value}")));
                    expr.RemoveRange(index, 2);
                }
            }
            //converts any 'subtraction' tokens that are representing negation into a negitive number
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

        //does not change/alter invoking object
        public Expression GetResolveArguments()
        {
            return ResolveArguments(this);
        }

        //returns a new Expression with the given arguments resolved down to there respective types
        //the passed in expr is not altered
        public static Expression ResolveArguments(Expression expr)
        {
            Expression copy = new Expression(expr);
            copy.RemoveNulls();
            List<Token> arguments = new List<Token>();

            int curStatementTracker = 0;

            for (int i = 0; i < copy.Count; i++)
            {
                if (copy[i].Type == TokenTypeValue.Seperator)
                {
                    arguments.Add(new Token(new Expression(copy.GetRange(curStatementTracker, i - curStatementTracker))));
                    curStatementTracker = i + 1;
                }
            }
            if (curStatementTracker < copy.Count)
            {
                arguments.Add(new Token(new Expression(copy.GetRange(curStatementTracker, copy.Count - curStatementTracker))));
            }

            return new Expression(arguments);
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

                if (Library.SymbolToTokenType.TryGetValue(character, out charTypeVal))
                {
                    tokens.Add(new Token(charTypeVal));
                    continue;
                }

                if (Library.Delimiters.Contains(character))
                    continue;

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

        public static bool Validate(string expr)
        {
            return new Expression(expr).Validate();
        }

        //returns true if this is a valid programmatical-mathematical expression (Within the context of this project). False otherwise
        public bool Validate()
        {
            if (Find(token => token.Type == TokenTypeValue.Invalid) != null) return false;

            bool valid = true;
            foreach (Expression expr in GetStatements())
            {
                if (!new EvaluationTree(expr).Validate())
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