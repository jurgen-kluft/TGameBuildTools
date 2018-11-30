using System;

namespace ColoredConsole
{
    /// <summary>
    /// Convenience extension methods for coloring instances of <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        public static ColoredToken Color(this string text, ConsoleColor? color)
        {
            return new ColoredToken(text, color);
        }

        public static ColoredToken Black(this string text)
        {
            return text.Color(ConsoleColor.Black);
        }

        public static ColoredToken Blue(this string text)
        {
            return text.Color(ConsoleColor.Blue);
        }

        public static ColoredToken Cyan(this string text)
        {
            return text.Color(ConsoleColor.Cyan);
        }

        public static ColoredToken DarkBlue(this string text)
        {
            return text.Color(ConsoleColor.DarkBlue);
        }

        public static ColoredToken DarkCyan(this string text)
        {
            return text.Color(ConsoleColor.DarkCyan);
        }

        public static ColoredToken DarkGray(this string text)
        {
            return text.Color(ConsoleColor.DarkGray);
        }

        public static ColoredToken DarkGreen(this string text)
        {
            return text.Color(ConsoleColor.DarkGreen);
        }

        public static ColoredToken DarkMagenta(this string text)
        {
            return text.Color(ConsoleColor.DarkMagenta);
        }

        public static ColoredToken DarkRed(this string text)
        {
            return text.Color(ConsoleColor.DarkRed);
        }

        public static ColoredToken DarkYellow(this string text)
        {
            return text.Color(ConsoleColor.DarkYellow);
        }

        public static ColoredToken Gray(this string text)
        {
            return text.Color(ConsoleColor.Gray);
        }

        public static ColoredToken Green(this string text)
        {
            return text.Color(ConsoleColor.Green);
        }

        public static ColoredToken Magenta(this string text)
        {
            return text.Color(ConsoleColor.Magenta);
        }

        public static ColoredToken Red(this string text)
        {
            return text.Color(ConsoleColor.Red);
        }

        public static ColoredToken White(this string text)
        {
            return text.Color(ConsoleColor.White);
        }

        public static ColoredToken Yellow(this string text)
        {
            return text.Color(ConsoleColor.Yellow);
        }

        public static ColoredToken On(this string text, ConsoleColor? backgroundColor)
        {
            return new ColoredToken(text, null, backgroundColor);
        }

        public static ColoredToken OnBlack(this string text)
        {
            return text.On(ConsoleColor.Black);
        }

        public static ColoredToken OnBlue(this string text)
        {
            return text.On(ConsoleColor.Blue);
        }

        public static ColoredToken OnCyan(this string text)
        {
            return text.On(ConsoleColor.Cyan);
        }

        public static ColoredToken OnDarkBlue(this string text)
        {
            return text.On(ConsoleColor.DarkBlue);
        }

        public static ColoredToken OnDarkCyan(this string text)
        {
            return text.On(ConsoleColor.DarkCyan);
        }

        public static ColoredToken OnDarkGray(this string text)
        {
            return text.On(ConsoleColor.DarkGray);
        }

        public static ColoredToken OnDarkGreen(this string text)
        {
            return text.On(ConsoleColor.DarkGreen);
        }

        public static ColoredToken OnDarkMagenta(this string text)
        {
            return text.On(ConsoleColor.DarkMagenta);
        }

        public static ColoredToken OnDarkRed(this string text)
        {
            return text.On(ConsoleColor.DarkRed);
        }

        public static ColoredToken OnDarkYellow(this string text)
        {
            return text.On(ConsoleColor.DarkYellow);
        }

        public static ColoredToken OnGray(this string text)
        {
            return text.On(ConsoleColor.Gray);
        }

        public static ColoredToken OnGreen(this string text)
        {
            return text.On(ConsoleColor.Green);
        }

        public static ColoredToken OnMagenta(this string text)
        {
            return text.On(ConsoleColor.Magenta);
        }

        public static ColoredToken OnRed(this string text)
        {
            return text.On(ConsoleColor.Red);
        }

        public static ColoredToken OnWhite(this string text)
        {
            return text.On(ConsoleColor.White);
        }

        public static ColoredToken OnYellow(this string text)
        {
            return text.On(ConsoleColor.Yellow);
        }
    }
}
