namespace ColoredConsole
{
    public static class ColorConsole
    {
        private static IConsole console = new SystemConsole();

        public static IConsole Console
        {
            get
            {
                return console;
            }

            set
            {
				if (value != null)
				{
					console = value;
				}
            }
        }

		public static void Write(params ColoredToken[] tokens)
		{
			if (tokens == null || tokens.Length == 0)
			{
				return;
			}

			foreach (var token in tokens)
			{
				if (token.Color.HasValue || token.BackgroundColor.HasValue)
				{
					var originalColor = console.ForegroundColor;
					var originalBackgroundColor = console.BackgroundColor;
					try
					{
						console.ForegroundColor = token.Color ?? originalColor;
						console.BackgroundColor = token.BackgroundColor ?? originalBackgroundColor;
						console.Write(token.Text);
					}
					finally
					{
						console.ForegroundColor = originalColor;
						console.BackgroundColor = originalBackgroundColor;
					}
				}
				else
				{
					console.Write(token.Text);
				}
			}

		}

        public static void WriteLine(params ColoredToken[] tokens)
        {
            Write(tokens);
            console.WriteLine();
        }
    }
}
