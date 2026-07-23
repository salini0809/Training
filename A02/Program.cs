namespace A02;

class Program
{
    static void Main(string[] args)
    {
      int n = new Random ().Next (1, 101);
      for(; ; ) {
         Console.WriteLine ("Enter the number you have gussed:");
         if(!int.TryParse(Console.ReadLine(),out int r)){
            Console.WriteLine ("Please enter a valid number");
            continue;
         }
         if (r == n) {
            Console.WriteLine ("You have guessed the number correctly!!");
            break;
         }
         else if (r < n) {
            Console.WriteLine ("your guess is too low");
         } else {
            Console.WriteLine ("your guess is too high");
         }
      }
    }
}