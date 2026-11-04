// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Tokenizer.cs
// Parses the input expression and generates tokens for evaluation
// ------------------------------------------------------------------------------------------------
#region class Tokenizer----------------------------------------------------------------------------
/// <summary>Scans expression text and produces tokens for the evaluator</summary>
class Tokenizer {
   #region Constructors----------------------------------------------
   /// <summary>Initializes the tokenizer with the evaluator and expression text</summary>
   public Tokenizer (Evaluator eval, string text) => (mEval, mText) = (eval, text);
   #endregion

   #region Properties------------------------------------------------
   // Indicates whether the next value must be an operand, helping distinguish unary and binary +/-.
   bool IsOperandExpected => mPrev is null or TOperator or TPunctuation { Punct: '(' };
   #endregion

   #region Methods---------------------------------------------------
   /// <summary>Returns the next token from input expression</summary>
   public Token Next () {
      mPrev = Read ();
      return mPrev;
   }
   #endregion

   #region Implementation--------------------------------------------
   // Reads the next token from the current input position.
   Token Read () {
      while (mN < mText.Length) {
         char ch = mText[mN++];
         switch (ch) {
            case ' ' or '\t': continue;
            case '+' or '-':
               return IsOperandExpected ? new TOpUnary (mEval, ch) : new TOpArithmetic (mEval, ch);
            case '*' or '/' or '^' or '=': return new TOpArithmetic (mEval, ch);
            case '(' or ')': return new TPunctuation (ch);
            case (>= '0' and <= '9') or '.': return GetNumber ();
            case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z'): return GetIdentifier ();
            default: return new TError ($"Unknown symbol '{ch}'");
         }
      }
      return new TEnd ();
   }

   // Reads a name: either a known function or a variable
   Token GetIdentifier () {
      int start = mN - 1;
      while (mN < mText.Length && char.IsAsciiLetterOrDigit (mText[mN])) mN++;
      string sub = mText[start..mN];
      string lower = sub.ToLowerInvariant ();
      return TOpFunction.IsFunction (lower) ? new TOpFunction (mEval, lower)
                                            : new TVariable (mEval, sub);

   }

   // Reads a numeric literal such as 12, 3.5, or .5.
   Token GetNumber () {
      int start = mN - 1;
      while (mN < mText.Length && (char.IsAsciiDigit (mText[mN]) || mText[mN] == '.')) mN++;
      string sub = mText[start..mN];
      if (double.TryParse (sub, out double f)) return new TLiteral (f);
      return new TError ($"Invalid number: {sub}");
   }
   #endregion

   #region Private data----------------------------------------------
   readonly Evaluator mEval;  // The evaluator that owns this
   readonly string mText;     // The input text we're parsing through
   int mN;                    // Position within the text
   Token? mPrev;              // The token returned last (null at the very start)
   #endregion
}
#endregion