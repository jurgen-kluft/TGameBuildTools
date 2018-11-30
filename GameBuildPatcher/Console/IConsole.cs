
namespace ColoredConsole
{
    using System;

    public interface IConsole
    {
        ConsoleColor ForegroundColor { get; set; }

        ConsoleColor BackgroundColor { get; set; }

        void Write(string text);

        void WriteLine();
    }
}
