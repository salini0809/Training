// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Evaluator.cs
// Evaluates expressions with operator precedence, parentheses, functions, and variable
// assignments while handling errors
// ------------------------------------------------------------------------------------------------
#region class EvalException -----------------------------------------------------------------------
/// <summary>Raised for any problem in the expression (syntax or math)</summary>
class EvalException : Exception {
   #region Constructors----------------------------------------------
   /// <summary>Creates an evaluation exception with the specified message.</summary>
   public EvalException (string message) : base (message) { }
   #endregion
}
#endregion

#region class Evaluator ---------------------------------------------------------------------------
/// <summary>Evaluates mathematical expressions using operator precedence, stacks, functions, and variables.</summary>
class Evaluator {
   #region Properties ----------------------------------------------
   /// <summary>Gets the current bracket-depth priority bonus; only the class can set it.</summary>
   public int BasePriority { get; private set; }
   #endregion

   #region Methods -------------------------------------------------
   /// <summary>Evaluates an expression, updates assigned variables, and returns the result.</summary>
   public double Evaluate (string text) {
      Reset ();
      var tokens = Tokenize (text);
      string? target = null;
      if (tokens.Count > 1 && tokens[0] is TVariable tv && tokens[1] is TOpArithmetic { Op: '=' }) {
         if (tokens.Count == 2) throw new EvalException ("Missing expression after '='");
         target = tv.Name;
         tokens.RemoveRange (0, 2);
      }
      if (tokens.Count == 0) throw new EvalException ("Empty expression");
      for (int i = 0; i + 1 < tokens.Count; i++)
         if (tokens[i] is TVariable v && tokens[i + 1] is TPunctuation { Punct: '(' })
            throw new EvalException ($"Unknown function '{v.Name}'");
      foreach (var token in tokens) Process (token);
      if (mExpectOperand) throw new EvalException ("Missing operand at the end of the expression");
      if (BasePriority != 0) throw new EvalException ("Missing ')'");
      while (mOperators.Count > 0) ApplyOperator ();
      double result = PopOperand () + 0.0;
      if (mOperands.Count > 0) throw new EvalException ("Too many operands");
      if (target != null) mVariables[target] = result;
      return result;
   }

   /// <summary>Returns a variable's stored value or throws an error if it is undefined.</summary>
   public double GetVariable (string name) => mVariables.TryGetValue (name, out double f) ? f
         : throw new EvalException ($"Unknown variable '{name}'");
   #endregion

   #region Implementation -------------------------------------------
   // Converts expression text into tokens and throws an error if tokenization fails.
   List<Token> Tokenize (string text) {
      var tokens = new List<Token> ();
      var tokenizer = new Tokenizer (this, text);
      for (; ; ) {
         var token = tokenizer.Next ();
         if (token is TEnd) break;
         if (token is TError err) throw new EvalException (err.Message);
         tokens.Add (token);
      }
      return tokens;
   }

   // Processes a token and updates the operand stack, operator stack, and evaluation state
   void Process (Token token) {
      switch (token) {
         case TNumber num:
            if (!mExpectOperand) throw new EvalException ($"Missing operator before '{num.Text}'");
            mOperands.Push (num.Value);
            mExpectOperand = false;
            break;
         case TOperator op:
            if (op is TOpArithmetic { Op: '=' })
               throw new EvalException ("'=' can only be used for assignment, like: x = 5");
            if (op.IsPrefix) {
               if (!mExpectOperand) throw new EvalException ($"Missing operator before '{op.Text}'");
            } else {
               if (mExpectOperand) throw new EvalException ($"Missing operand before '{op.Text}'");
            }
            op.FinalPriority = op.Priority + BasePriority;
            if (!op.IsPrefix) while (mOperators.Count > 0 && !OkToPush (op)) ApplyOperator ();
            mOperators.Push (op);
            mExpectOperand = true;
            break;
         case TPunctuation { Punct: '(' }:
            if (!mExpectOperand) throw new EvalException ("Missing operator before '('");
            BasePriority += 10;
            break;
         case TPunctuation { Punct: ')' }:
            if (BasePriority == 0) throw new EvalException ("Unmatched ')'");
            if (mExpectOperand) throw new EvalException ("Missing operand before ')'");
            BasePriority -= 10;
            break;
         default: throw new EvalException ($"Unexpected token '{token.Text}'");
      }
   }

   // Determines whether an incoming operator can be pushed without applying the current top operator.
   bool OkToPush (TOperator op) {
      var top = mOperators.Peek ();
      if (top.FinalPriority != op.FinalPriority) return top.FinalPriority < op.FinalPriority;
      return op is TOpArithmetic { IsRightAssociative: true };
   }

   // Pops the top operator, applies it to its operand(s) and pushes the result
   void ApplyOperator () {
      var op = mOperators.Pop ();
      double result;
      switch (op) {
         case TOpUnary u: result = u.Evaluate (PopOperand ()); break;
         case TOpFunction f: result = f.Evaluate (PopOperand ()); break;
         case TOpArithmetic a:
            double right = PopOperand (), left = PopOperand ();
            result = a.Evaluate (left, right);
            break;
         default: throw new EvalException ($"Unexpected operator '{op.Text}'");
      }
      if (!double.IsFinite (result))
         throw new EvalException ($"Math error: '{op.Text}' gives an undefined or infinite result");
      mOperands.Push (result);
   }

   // Removes and returns the top operand, or throws an error if none is available.
   double PopOperand ()
      => mOperands.Count > 0 ? mOperands.Pop () : throw new EvalException ("Too few operands");

   // Clears the working stacks and resets expression state while preserving variables.
   void Reset () {
      mOperands.Clear ();
      mOperators.Clear ();
      BasePriority = 0;
      mExpectOperand = true;
   }
   #endregion

   #region Private data ---------------------------------------------------
   readonly Stack<double> mOperands = new ();                // Stores numeric operands waiting to be processed.
   readonly Stack<TOperator> mOperators = new ();            // Stores operators waiting to be applied according to precedence
   readonly Dictionary<string, double> mVariables = new ();  // Stores variable names and their assigned values
   bool mExpectOperand = true;                               // Indicates whether the next token must provide an operand.
   #endregion
}
#endregion
