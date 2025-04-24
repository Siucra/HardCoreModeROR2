using System;

namespace MyModTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var mod = new ror2Hardcore();  // uses your original mod class
            mod.Awake();                   // runs your mod logic
            Console.WriteLine("Test complete.");
            Console.ReadLine();            // Keep console open
        }
    }
}