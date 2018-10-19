namespace Console
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var controller = new Controller();

            while (true)
            {
                Controller.ConsoleManager.Update();
                Interface.CallHook("OnFrame");
            }
        }
    }
}