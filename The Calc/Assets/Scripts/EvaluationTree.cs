using System.Collections.Generic;

namespace delib.calculate
{
    public class EvaluationTree
    {
        //represents an indivdual node within a tree
        protected class TreeNode
        {
            public Token token;
            public EvaluationTree subTree;
            public TreeNode parent;
            public TreeNode left;
            public TreeNode right;


            //returns true if this node is the root node of a tree
            public bool IsRoot
            {
                get
                {
                    return parent == null;
                }
            }

            //returns true if this node has no children
            public bool IsLeaf
            {
                get
                {
                    return (left == null || left.token.Type == TokenTypeValue.Null) && (right == null || right.token.Type == TokenTypeValue.Null);
                }
            }

            //returns a TreeNode representing a null token value
            private TreeNode NullNode
            {
                get
                {
                    return new TreeNode(new Token(TokenTypeValue.Null));
                }
            }

            public TreeNode(EvaluationTree tree)
            {
                if (tree == null)
                    return;
                subTree = tree;


                if (tree.root == null)
                    return;
                token = tree.root.token;
                parent = tree.root.parent;
                left = tree.root.left;
                right = tree.root.right;
            }
            public TreeNode(Token t, TreeNode p = null, TreeNode l = null, TreeNode r = null)
            {
                token = t;
                parent = p;
                left = l;
                right = r;

                //if the node itself is not null then it's children should be set to null if they are unitialized
                if (token.Type != TokenTypeValue.Null)
                {
                    if (left == null)
                        left = NullNode;
                    if (right == null)
                        right = NullNode;
                }

                //parents child nodes
                if (left != null)
                    left.parent = this;
                if (right != null)
                    right.parent = this;
            }

            //changes the children of this node to be the passed in nodes
            public void SetChildren(TreeNode leftNode, TreeNode rightNode)
            {
                left = leftNode;
                right = rightNode;
                //if the node itself is not null then it's children should be set to null if they are unitialized
                if (token.Type != TokenTypeValue.Null)
                {
                    if (left == null)
                        left = NullNode;
                    if (right == null)
                        right = NullNode;
                }
                left.parent = this;
                right.parent = this;

            }

            //changes the children of this node to be new nodes based on the passed in tokens
            public void SetChildren(Token leftData, Token rightdata)
            {
                if (leftData != null)
                    left = new TreeNode(leftData, this);
                else
                    left = null;
                if (rightdata != null)
                    right = new TreeNode(rightdata, this);
                else
                    right = null;
                //if the node itself is not null then it's children should be set to null if they are unitialized
                if (token.Type != TokenTypeValue.Null)
                {
                    if (left == null)
                        left = NullNode;
                    if (right == null)
                        right = NullNode;
                }
                left.parent = this;
                right.parent = this;
            }
        }

        private TreeNode root;

        public EvaluationTree()
        {
            root = null;
        }

        public EvaluationTree(Token token) : this(new TreeNode(token))
        {

        }
        protected EvaluationTree(TreeNode node)
        {
            root = node;
            if (root != null)
            {
                root.subTree = this;
            }
        }

        public EvaluationTree(Expression expr)
        {
            GenerateTree(expr, this);
            if (root != null)
            {
                root.subTree = this;
            }
        }

        //generates a tree based on the passed in expression
        //if 'storeTree' is given a value then the generated tree will be stored into it
        public static EvaluationTree GenerateTree(Expression expr, EvaluationTree storeTree = null)
        {
            if (expr == null)
                return null;
            if (storeTree == null)
                storeTree = new EvaluationTree();


            for (int priority = 1; priority <= Library.ConstantPriority; priority++)
            {
                for (int index = 0; index < expr.Count; index++)
                {
                    if (expr[index].Priority == priority)
                    {
                        storeTree.root = new TreeNode(expr[index], null, GenerateTree(expr.SubExpression(0, index)).root, GenerateTree(expr.SubExpression(index + 1, (expr.Count - 1) - index)).root);
                        return storeTree;
                    }
                }
            }



            return storeTree;
        }


        #region Validation
        //returns true if this tree represents a valid expression
        public bool Validate()
        {
            return ValidateTree(this);
        }


        //returns true if the given tree represents a valid expression
        protected static bool ValidateTree(EvaluationTree tree)
        {
            if (tree.root == null)
                return false;

            return ValidateBranch(tree.root);
        }

        //returns true if the given branch of a tree represents a valid expression
        protected static bool ValidateBranch(TreeNode node)
        {
            if (node == null)
                return false;

            if (node.IsLeaf)
                return ValidateNode(node);

            return ValidateNode(node) && ValidateBranch(node.left) && ValidateBranch(node.right);
        }


        //returns true if the given node represents a valid operation
        protected static bool ValidateNode(TreeNode node)
        {
            if (node == null)
                return false;

            if (node.token.Type == TokenTypeValue.Invalid || node.token.Type == TokenTypeValue.Identifier)//if an expression has unresolved identfiers then a result cannot be generated and thus it is an invalid expression
                return false;

            if (node.IsLeaf)
                return node.token.Type.ResolvesTo(TokenTypeValue.Constant) || node.token.Type.ResolvesTo(TokenTypeValue.Null);

            TokenTypeValue operation = node.token.Type.Value;
            Operands operands = new Operands(node.left.token.Type.Value, node.right.token.Type.Value);

            List<Operands> ops = null;
            if (Library.ValidOperands.TryGetValue(operation, out ops))
            {
                return ops.Contains(operands);
            }


            return false;
        }

        #endregion
    }
}
