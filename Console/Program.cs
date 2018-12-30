using System;

namespace Console
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                Log.Exception(eventArgs.Exception);
                System.Console.ReadKey();
            };
            
            new Controller();

            while (true)
            {
                Controller.Instance.OnFrame();
            }
        }
    }
}