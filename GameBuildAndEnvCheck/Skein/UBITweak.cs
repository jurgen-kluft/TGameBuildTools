using System;

namespace SkeinFish
{
    public enum UbiType : ulong
    {
        Key = 0,
        Config = 4,
        Personalization = 8,
        PublicKey = 12,
        KeyIdentifier = 16,
        Nonce = 20,
        Message = 48,
        Out = 63
    }

    public class UbiTweak
    {
        private const ulong T1FlagFinal = unchecked((ulong)1 << 63);
        private const ulong T1FlagFirst = unchecked((ulong)1 << 62);

        public UbiTweak()
        {
            Tweak = new ulong[2];
        }

        /// <summary>
        /// Gets or sets the first block flag.
        /// </summary>
        public bool IsFirstBlock
        {
            get { return (Tweak[1] & T1FlagFirst) != 0; }
            set
            {
                long mask = value ? 1 : 0;
                Tweak[1] = (Tweak[1] & ~T1FlagFirst) | ((ulong)-mask & T1FlagFirst);
            }
        }

        /// <summary>
        /// Gets or sets the final block flag.
        /// </summary>
        public bool IsFinalBlock
        {
            get { return (Tweak[1] & T1FlagFinal) != 0; }
            set
            {
                long mask = value ? 1 : 0;
                Tweak[1] = (Tweak[1] & ~T1FlagFinal) | ((ulong)-mask & T1FlagFinal);
            }
        }

        /// <summary>
        /// Gets or sets the current tree level.
        /// </summary>
        public byte TreeLevel
        {
            get { return (byte) ((Tweak[1] >> 48) & 0x3f); }
            set
            {
                if (value > 63)
                    throw new Exception("Tree level must be between 0 and 63, inclusive.");

                Tweak[1] &= ~((ulong) 0x3f << 48);
                Tweak[1] |= (ulong) value << 48;
            }
        }

        /// <summary>
        /// Gets or sets the number of bits processed so far, inclusive.
        /// </summary>
        public ulong BitsProcessed
        {
            get { return Tweak[0]; }
            set { Tweak[0] = value; }
        }

        /// <summary>
        /// Gets or sets the current UBI block type.
        /// </summary>
        public UbiType BlockType
        {
            get { return (UbiType) (Tweak[1] >> 56); }
            set { Tweak[1] = (ulong)value << 56; }
        }

        /// <summary>
        /// Starts a new UBI block type by setting BitsProcessed to zero, setting the first flag, and setting the block type.
        /// </summary>
        /// <param name="type">The UBI block type of the new block.</param>
        public void StartNewBlockType(UbiType type)
        {
            BitsProcessed = 0;
            BlockType = type;
            IsFirstBlock = true;
        }

        /// <summary>
        /// The current Threefish tweak value.
        /// </summary>
        public ulong[] Tweak { get; private set; }
    }
}
