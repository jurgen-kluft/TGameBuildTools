using System.Text;

namespace Octodiff.Core
{
    class BinaryFormat
    {
        public static readonly byte[] SignatureHeader = Encoding.ASCII.GetBytes("RSYNCSIG");
        public static readonly byte[] DeltaHeader = Encoding.ASCII.GetBytes("RSYNC");
        public static readonly byte[] EndOfMetadata = Encoding.ASCII.GetBytes(">>>");
        public const byte CopyCommand = 0x60;
        public const byte DataCommand = 0x80;

        public const byte Version = 0x01;
    }
}