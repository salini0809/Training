// ------------------------------------------------------------------------------------------------
// Training ~ A training program for new joinees at Metamation, Batch - July 2026.
// Copyright (c) TRUMPF Metamation India.
// ------------------------------------------------------------------------------------------------
// Tokens.cs
// Implements token classes for representing literals, variables, operators, functions, and errors
// ------------------------------------------------------------------------------------------------
#region class Token -------------------------------------------------------------------------------
/// <summary>Base class for all tokens produced by the tokenizer.</summary>
abstract class Token {
   #region Properties------------------------------------------------
   /// <summary>Returns the text representation of the token.</summary>
   public abstract string Text { get; }
   #endregion

   #region Methods---------------------------------------------------
   /// <summary>Returns the token's class name and text.</summary>
   public override string ToString ()=> $"{GetType().Name}.({Text})";
   #endregion
}
#endregion

#region class TNumber -----------------------------------------------------------------------------
/// <summary>Base class for tokens that represent numeric values or variables.</summary>
abstract class TNumber : Token {
   #region Properties------------------------------------------------
   /// <summary>Returns the numeric value represented by the token</summary>
   public abstract double Value { get; }
   #endregion
}
#endregion

#region class TLiteral ---------------------------------------------------------------------------
/// <summary>Represents a numeric literal entered directly in an expression.</summary>
class TLiteral : TNumber {
   #region Constructors----------------------------------------------
   /// <summary>Initializes a numeric literal with the given value.</summary>
   public TLiteral (double f) => mValue = f;
   #endregion

   #region Properties------------------------------------------------
   /// <summary>Returns the stored numeric value</summary>
   public override double Value => mValue;

   /// <summary>Returns the numeric value as a string</summary>
   public override string Text => mValue.ToString ();
   #endregion

   #region Private data----------------------------------------------
   readonly double mValue;     // Stores the numeric value of the literal.
   #endregion
}
#endregion

#region class TVariable ---------------------------------------------------------------------------
/// <summary>Represents a variable whose value is retrieved from the evaluator.</summary>
class TVariable : TNumber {
   #region Constructors----------------------------------------------
   /// <summary>Initializes a variable with its evaluator and name</summary>
   public TVariable (Evaluator eval, string name) => (Name, mEval) = (name, eval);
   #endregion

   #region Properties------------------------------------------------
   /// <summary>Returns the variable's name</summary>
   public string Name { get; private set; }

   /// <summary>Retrieves the variable's current value from the evaluator.</summary>
   public override double Value => mEval.GetVariable (Name);

   /// <summary>Returns the variable's name as text. </summary>
   public override string Text => Name;
   #endregion

   #region Private data----------------------------------------------
   readonly Evaluator mEval;    // Stores the evaluator used to retrieve a variable's current value.
   #endregion
}
#endregion

#region class TOperator ---------------------------------------------------------------------------
/// <summary>Base class for all operators (binary, unary and functions)</summary>
abstract class TOperator : Token {
   #region Constructors----------------------------------------------
   /// <summary>Initializes the base operator with its evaluator; accessible to derived classes.</summary>
   protected TOperator (Evaluator eval) => mEval = eval;
   #endregion

   #region Properties------------------------------------------------
   /// <summary>Returns the operator's basic precedence.</summary>
   public abstract int Priority { get; }

   /// <summary>Gets or sets the operator's precedence adjusted for bracket depth. </summary>
   public int FinalPriority { get; set; }

   /// <summary>Indicates whether the operator appears before its operand. </summary>
   public virtual bool IsPrefix => false;
   #endregion

   #region Private data----------------------------------------------
   readonly protected Evaluator mEval;
   #endregion
}
#endregion

#region class TOpArithmetic -----------------------------------------------------------------------
/// <summary>Represents binary arithmetic operators and assignment</summary>
class TOpArithmetic : TOperator {
   #region Constructors----------------------------------------------
   public TOpArithmetic (Evaluator eval, char ch) : base (eval) => Op = ch;
   #endregion

   #region Properties------------------------------------------------
   /// <summary>Returns the arithmetic operator symbol. </summary>
   public char Op { get; private set; }

   // <summary>Returns the precedence of the arithmetic operator. </summary>
   public override int Priority => sPriority[Op];

   /// <summary>Returns the operator symbol as text</summary>
   public override string Text => Op.ToString ();

   /// <summary>Indicates whether the operator is right-associative (^)</summary>
   public bool IsRightAssociative => Op == '^';
   #endregion

   #region Methods---------------------------------------------------
   /// <summary>Performs the specified binary arithmetic operation. </summary>
   public double Evaluate (double a, double b) => Op switch {
         '+' => a + b,
         '-' => a - b,
         '*' => a * b,
         '/' => b==0? throw new EvalException("Divide by zero"): a/b,
         '^' => Math.Pow (a, b),
         _ => throw new EvalException ($"Unknown operator: {Op}"),
    };
   #endregion

   #region Private data----------------------------------------------
   // Stores the operator's character-to-precedence mapping.
   static Dictionary<char, int> sPriority = new () {
      ['+'] = 1, ['-'] = 1, ['*'] = 2, ['/'] = 2, ['^'] = 5, ['='] = 0,
   };
   #endregion
}
#endregion

#region Class TOpUnary-----------------------------------------------
/// <summary>Represents unary plus and unary minus operators</summary>
class TOpUnary : TOperator {
   #region Constructors----------------------------------------------
   /// <summary> </summary>
   public TOpUnary (Evaluator eval, char op) : base (eval) => Op = op;
   #endregion

   #region Properties------------------------------------------------
   /// <summary>Returns the unary operator symbol.</summary>
   public char Op { get; }

   /// <summary>Returns the precedence of the unary operator. </summary>
   public override int Priority => 3;

   /// <summary>Indicates that the unary operator appears before its operand. </summary>
   public override bool IsPrefix => true;

   /// <summary>Returns the unary operator symbol as text</summary>
   public override string Text => Op.ToString ();
   #endregion

   #region Methods---------------------------------------------------
   /// <summary>Performs unary negation or unary identity</summary>
   public double Evaluate (double a) => Op switch {
      '-' => -a,
      '+' => a,
      _ => throw new EvalException ($"Unknown unary operator '{Op}'")
   };
   #endregion
}
#endregion

#region class TOpFunction -------------------------------------------------------------------------
/// <summary>A named math function such as sin, cos, sqrt (trig functions work in degrees)</summary>
class TOpFunction : TOperator {
   #region Constructors----------------------------------------------
   public TOpFunction (Evaluator eval, string name) : base (eval) => Func = name;
   #endregion

   #region Properties------------------------------------------------
   /// <summary>Returns the mathematical function's name. </summary>
   public string Func { get; private set; }

   /// <summary>Returns the function's precedence</summary>
   public override int Priority => 4;

   /// <summary>Indicates that the function appears before its argument. </summary>
   public override bool IsPrefix => true;

   /// <summary>Returns the function name as text</summary>
   public override string Text => Func;
   #endregion

   #region Methods---------------------------------------------------
   /// <summary>Evaluates the specified mathematical function. </summary>
   public double Evaluate (double f) => Func switch {
         "sin" => Math.Sin (D2R (f)),
         "cos" => Math.Cos (D2R (f)),
         "tan" => Math.Tan (D2R (f)),
         "sqrt" => Math.Sqrt (f),
         "log" => Math.Log (f),
         "exp" => Math.Exp (f),
         "asin" => R2D (Math.Asin (f)),
         "acos" => R2D (Math.Acos (f)),
         "atan" => R2D (Math.Atan (f)),
         _ => throw new EvalException ($"Unknown function: {Func}")
      };


   /// <summary>Checks whether the given name is a supported mathematical function.</summary>
   public static bool IsFunction (string name) => sNames.Contains (name);
   #endregion

   #region Implementation--------------------------------------------
   // Converts an angle from degrees to radians.
   double D2R (double f) => f * Math.PI / 180;

   // Converts an angle from radians to degrees.
   double R2D (double f) => f * 180 / Math.PI;
   #endregion

   #region Private data----------------------------------------------
   // Stores the names of all supported mathematical functions.
   static readonly HashSet<string> sNames = new () {
      "sin", "cos", "tan", "asin", "acos", "atan", "sqrt", "log", "exp"
     };
   #endregion
}
#endregion

#region class TPunctuation ------------------------------------------------------------------------
/// <summary>Represents opening and closing parentheses</summary>
class TPunctuation : Token {

   #region Constructors----------------------------------------------
   /// <summary> Initializes a punctuation token with the given character.</summary>
   public TPunctuation (char ch) => Punct = ch;
   #endregion

   #region Properties------------------------------------------------
   /// <summary>Returns the parenthesis character</summary>
   public char Punct { get; private set; }

   /// <summary>Returns the parenthesis as text</summary>
   public override string Text => Punct.ToString ();
   #endregion
}
#endregion

#region class TEnd---------------------------------------------------------------------------------
/// <summary>Represents the end of the input expression.</summary>
class TEnd : Token {
   #region Properties------------------------------------------------
   /// </summary>Returns "end" to represent the end of input</summary
   public override string Text=> "end";
   #endregion
}
#endregion

#region class TError ------------------------------------------------------------------------------
/// <summary>Represents an invalid token detected by the tokenizer.</summary>
class TError : Token {

   #region Constructors----------------------------------------------
   /// <summary> Initializes an error token with an error message</summary>
   public TError (string message) => Message = message;
   #endregion

   #region Properties------------------------------------------------
   /// <summary>Returns the error message</summary>
   public string Message { get; private set; }

   /// <summary>Returns the error message as the token's text</summary>
   public override string Text=>Message;
   #endregion
}
#endregion