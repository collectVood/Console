namespace Console
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var controller = new Controller();

            while (true)
            {
                controller.OnFrame();
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}