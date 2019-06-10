namespace Console
{
    public static class Program
    {
        public static void Main()
        {
            var controller = new Controller();
            
            while (controller.IsRunning)
            {
                controller.OnFrame();
            }
        }
    }
}