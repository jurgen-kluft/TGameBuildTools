namespace ColoredConsole
{
    using System;

    public class SystemConsole : IConsole
    {
        public ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set { Console.ForegroundColor = value; }
        }

        public ConsoleColor BackgroundColor
        {
            get { return Console.BackgroundColor; }
            set { Console.BackgroundColor = value; }
        }

        public void Write(string text)
        {
            Console.Write(text);
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }
    }
}
