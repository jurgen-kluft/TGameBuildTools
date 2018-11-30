namespace ContentVerification
{
	public class Skein160 : SkeinFish.Skein
	{
		public Skein160() : base(512, 160) { }
	}

	public class Skein256 : SkeinFish.Skein
	{
		public Skein256() : base(512, 256) { }
	}

	public class Skein512 : SkeinFish.Skein
	{
		public Skein512() : base(512, 512) { }
	}
}
