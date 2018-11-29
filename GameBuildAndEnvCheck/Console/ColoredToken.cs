using System;

namespace ColoredConsole
{

    public struct ColoredToken : IEquatable<ColoredToken>
    {
        private readonly string text;
        private readonly ConsoleColor? color;
        private readonly ConsoleColor? backgroundColor;

        public ColoredToken(string text)
            : this(text, null, null)
        {
        }

        public ColoredToken(string text, ConsoleColor? color)
            : this(text, color, null)
        {
        }

        public ColoredToken(string text, ConsoleColor? color, ConsoleColor? backgroundColor)
        {
            this.text = text;
            this.color = color;
            this.backgroundColor = backgroundColor;
        }

        public string Text
        {
            get { return this.text; }
        }

        public ConsoleColor? Color
        {
            get { return this.color; }
        }

        public ConsoleColor? BackgroundColor
        {
            get { return this.backgroundColor; }
        }

        public static implicit operator ColoredToken(string text)
        {
            return new ColoredToken(text);
        }

        public static bool operator ==(ColoredToken left, ColoredToken right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ColoredToken left, ColoredToken right)
        {
            return !left.Equals(right);
        }

        public ColoredToken Coalesce(ConsoleColor defaultColor)
        {
            return this.Coalesce(defaultColor, null);
        }

        public ColoredToken Coalesce(ConsoleColor? defaultColor, ConsoleColor? defaultBackgroundColor)
        {
            return new ColoredToken(this.text, this.color ?? defaultColor, this.backgroundColor ?? defaultBackgroundColor);
        }

        public override string ToString()
        {
            return this.text;
        }

        public override int GetHashCode()
        {
            return this.text == null ? 0 : this.text.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is ColoredToken && this.Equals((ColoredToken)obj);
        }

        public bool Equals(ColoredToken other)
        {
            return this.text == other.text && this.color == other.color && this.backgroundColor == other.backgroundColor;
        }
    }
}
