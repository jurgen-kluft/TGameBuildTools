using System;
using System.Linq;
using System.Collections.Generic;

namespace ColoredConsole
{

    /// <summary>
    /// Convenience extension methods for re-coloring instances of <see cref="ColoredToken"/>.
    /// </summary>
    public static class ColoredTokenExtensions
    {
        public static ColoredToken[] Coalesce(this IEnumerable<ColoredToken> tokens, ConsoleColor color)
        {
            return tokens.Coalesce(color, null);
        }

        public static ColoredToken[] Coalesce(this IEnumerable<ColoredToken> tokens, ConsoleColor? color, ConsoleColor? backgroundColor)
        {
            return tokens == null ? null : tokens.Select(token => token.Coalesce(color, backgroundColor)).ToArray();
        }

        public static ColoredToken On(this ColoredToken token, ConsoleColor? backgroundColor)
        {
            return new ColoredToken(token.Text, token.Color, backgroundColor);
        }

        public static ColoredToken OnBlack(this ColoredToken token)
        {
            return token.On(ConsoleColor.Black);
        }

        public static ColoredToken OnBlue(this ColoredToken token)
        {
            return token.On(ConsoleColor.Blue);
        }

        public static ColoredToken OnCyan(this ColoredToken token)
        {
            return token.On(ConsoleColor.Cyan);
        }

        public static ColoredToken OnDarkBlue(this ColoredToken token)
        {
            return token.On(ConsoleColor.DarkBlue);
        }

        public static ColoredToken OnDarkCyan(this ColoredToken token)
        {
            return token.On(ConsoleColor.DarkCyan);
        }

        public static ColoredToken OnDarkGray(this ColoredToken token)
        {
            return token.On(ConsoleColor.DarkGray);
        }

        public static ColoredToken OnDarkGreen(this ColoredToken token)
        {
            return token.On(ConsoleColor.DarkGreen);
        }

        public static ColoredToken OnDarkMagenta(this ColoredToken token)
        {
            return token.On(ConsoleColor.DarkMagenta);
        }

        public static ColoredToken OnDarkRed(this ColoredToken token)
        {
            return token.On(ConsoleColor.DarkRed);
        }

        public static ColoredToken OnDarkYellow(this ColoredToken token)
        {
            return token.On(ConsoleColor.DarkYellow);
        }

        public static ColoredToken OnGray(this ColoredToken token)
        {
            return token.On(ConsoleColor.Gray);
        }

        public static ColoredToken OnGreen(this ColoredToken token)
        {
            return token.On(ConsoleColor.Green);
        }

        public static ColoredToken OnMagenta(this ColoredToken token)
        {
            return token.On(ConsoleColor.Magenta);
        }

        public static ColoredToken OnRed(this ColoredToken token)
        {
            return token.On(ConsoleColor.Red);
        }

        public static ColoredToken OnWhite(this ColoredToken token)
        {
            return token.On(ConsoleColor.White);
        }

        public static ColoredToken OnYellow(this ColoredToken token)
        {
            return token.On(ConsoleColor.Yellow);
        }

        public static ColoredToken Color(this ColoredToken token, ConsoleColor? color)
        {
            return new ColoredToken(token.Text, color, token.BackgroundColor);
        }

        public static ColoredToken Black(this ColoredToken token)
        {
            return token.Color(ConsoleColor.Black);
        }

        public static ColoredToken Blue(this ColoredToken token)
        {
            return token.Color(ConsoleColor.Blue);
        }

        public static ColoredToken Cyan(this ColoredToken token)
        {
            return token.Color(ConsoleColor.Cyan);
        }

        public static ColoredToken DarkBlue(this ColoredToken token)
        {
            return token.Color(ConsoleColor.DarkBlue);
        }

        public static ColoredToken DarkCyan(this ColoredToken token)
        {
            return token.Color(ConsoleColor.DarkCyan);
        }

        public static ColoredToken DarkGray(this ColoredToken token)
        {
            return token.Color(ConsoleColor.DarkGray);
        }

        public static ColoredToken DarkGreen(this ColoredToken token)
        {
            return token.Color(ConsoleColor.DarkGreen);
        }

        public static ColoredToken DarkMagenta(this ColoredToken token)
        {
            return token.Color(ConsoleColor.DarkMagenta);
        }

        public static ColoredToken DarkRed(this ColoredToken token)
        {
            return token.Color(ConsoleColor.DarkRed);
        }

        public static ColoredToken DarkYellow(this ColoredToken token)
        {
            return token.Color(ConsoleColor.DarkYellow);
        }

        public static ColoredToken Gray(this ColoredToken token)
        {
            return token.Color(ConsoleColor.Gray);
        }

        public static ColoredToken Green(this ColoredToken token)
        {
            return token.Color(ConsoleColor.Green);
        }

        public static ColoredToken Magenta(this ColoredToken token)
        {
            return token.Color(ConsoleColor.Magenta);
        }

        public static ColoredToken Red(this ColoredToken token)
        {
            return token.Color(ConsoleColor.Red);
        }

        public static ColoredToken White(this ColoredToken token)
        {
            return token.Color(ConsoleColor.White);
        }

        public static ColoredToken Yellow(this ColoredToken token)
        {
            return token.Color(ConsoleColor.Yellow);
        }
    }
}
