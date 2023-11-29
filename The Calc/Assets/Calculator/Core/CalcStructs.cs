//Created by: Deontae Albertie

namespace delib.calculate
{


    #region structs

    //[System.Serializable]
    //stores the true value of a token and contains several useful methods for doing token comparisons
    public struct TokenType
    {
        public TokenTypeValue Value;


        public TokenType(TokenTypeValue val)
        {
            Value = val;
        }

        public static implicit operator TokenTypeValue(TokenType tt)
        {
            return tt.Value;
        }

        public override string ToString()
        {
            return $"{Value.ToString()}";
        }

        public bool ResolvesTo(TokenTypeValue compValue)
        {
            return ResolvesTo(compValue, Value);
        }

        //returns true if 'checkValue' can be resolved to 'comparerValue' based on the ResolvingTypes dictionary
        //returns false otherwise
        public static bool ResolvesTo(TokenTypeValue comparerValue, TokenTypeValue checkValue)
        {
            bool equal = false;

            if (Library.ResolvingTypes.ContainsKey(comparerValue))
            {
                equal = Library.ResolvingTypes[comparerValue].Contains(checkValue);
            }
            else
                equal = comparerValue == checkValue;


            return equal;
        }


        //same function as ResolvesTo excpept with the parameters order flipped around
        public override bool Equals(object obj)
        {
            TokenTypeValue otherValue;

            switch (obj)
            {
                case TokenType:
                    otherValue = ((TokenType)obj).Value;
                    break;
                case TokenTypeValue:
                    otherValue = (TokenTypeValue)obj;
                    break;
                default:
                    return false;
            }

            bool equals = ResolvesTo(Value, otherValue);

            return equals;

        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }

    //stores the left and right TokenType's for an operation
    public struct Operands
    {
        public TokenType left;
        public TokenType right;

        public Operands(TokenTypeValue l, TokenTypeValue r)
        {
            left = new TokenType(l);
            right = new TokenType(r);
        }

        public override string ToString()
        {
            return $"{left.ToString()}, {right.ToString()}";
        }

/*        public override bool Equals(object obj)
        {
            if (obj is not Operands)
            {
                return false;
            }
            Operands other = (Operands)obj;



            bool leftEquals = (left.Equals(other.left));
            bool rightEquals = (right.Equals(other.right));
            bool equals = leftEquals && rightEquals;

            return equals;
        }

        public override int GetHashCode()
        {
            int code = left.GetHashCode() * right.GetHashCode();
            return code.GetHashCode();
        }*/
    }

    //represents an operation that can be performed in a calculator
    //symbol is the operator
    public struct Operation
    {
        public TokenType symbol;
        public Operands operands;

        public Operation(TokenTypeValue tType, Operands ops)
        {
            symbol = new TokenType(tType);
            operands = ops;
        }

        public override string ToString()
        {
            return $"Operator: {symbol.ToString()}\t Operands: {operands.ToString()}";
        }

        public override bool Equals(object obj)
        {
            if (obj is not Operation)
            {
                return false;
            }
            Operation other = (Operation)obj;



            bool symbolEquals = (symbol.Equals(other.symbol));
            bool operandsEquals = (operands.Equals(other.operands));
            bool equals = symbolEquals && operandsEquals;

            return equals;
        }

        public override int GetHashCode()
        {
            int code = symbol.GetHashCode() * operands.GetHashCode();
            return code.GetHashCode();
        }
    }
    #endregion


}
