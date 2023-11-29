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


            return ResolvesTo(Value, otherValue);

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

        public override bool Equals(object obj)
        {
            if (obj is not Operation)
            {
                return false;
            }
            Operation other = (Operation)obj;

            return (symbol.Equals(other.symbol)) && (operands.Equals(other.operands));
        }

        public override int GetHashCode()
        {
            int code = symbol.GetHashCode() * operands.GetHashCode();
            return code.GetHashCode();
        }
    }
    #endregion


}
