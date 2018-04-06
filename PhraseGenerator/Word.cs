using System.Collections.Generic;
using System.IO;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        public class Word
        {
            public const int maxWordLength = 24;
            public const int byteSize = (maxWordLength * sizeof(char)) + (sizeof(int) * 3);

            public string Value { get; private set; }
            public PartOfSpeech PoS { get; private set; }
            public float Frequency;
            public int Id;
            public Word(string word, PartOfSpeech pos, float frequency)
            {
                this.Value = word;
                this.PoS = pos;
                this.Frequency = frequency;
                this.Id = -1;
            }
            private Word() { }
            public override bool Equals(object obj)
            {
                if (this == obj) return true;
                if (obj == null || this.GetType() != obj.GetType()) return false;
                Word that = (Word)obj;
                if (this.Id != that.Id)
                {
                    return false;
                }
                return true;
            }
            public override int GetHashCode()
            {
                int result = 31 * Id;
                return result;
            }
            public override string ToString()
            {
                return "[" + Id + "] " + Value + " (" + PoS.Tag + ")";
            }
            public Word Copy()
            {
                Word copy = new Word(this.Value, this.PoS, this.Frequency);
                return copy;
            }
            public static byte[] ToBytes(Word w)
            {
                byte[] wordValue = stringToFixedLengthByteArray(w.Value, maxWordLength);
                byte[] bytes = new byte[byteSize];
                using (MemoryStream mem = new MemoryStream(bytes))
                {
                    using (BinaryWriter bw = new BinaryWriter(mem))
                    {
                        bw.Write(wordValue);
                        bw.Write(w.Frequency);
                        bw.Write(w.Id);
                        bw.Write(w.PoS.Id);
                    }
                }
                return bytes;
            }
            public static Word FromBytes(byte[] bytes)
            {
                byte[] wordValue;
                int frequency;
                int id;
                int posID;
                using (MemoryStream mem = new MemoryStream(bytes))
                {
                    using (BinaryReader br = new BinaryReader(mem))
                    {
                        wordValue = br.ReadBytes(maxWordLength * sizeof(char));
                        frequency = br.ReadInt32();
                        id = br.ReadInt32();
                        posID = br.ReadInt32();
                    }
                }
                string value = stringFromFixedLengthByteArray(wordValue);
                Word w = new Word() { Frequency = frequency, Id = id, Value = value, PoS = PartOfSpeech.FromId(posID) };
                return w;
            }
            public static byte[] stringToFixedLengthByteArray(string str, int length)
            {
                byte[] bytes = new byte[length * sizeof(char)];
                int len = str.Length > length ? length : str.Length;
                System.Text.Encoding.UTF8.GetBytes(str.Substring(0, len)).CopyTo(bytes, 0);
                return bytes;
            }
            public static string stringFromFixedLengthByteArray(byte[] bytes)
            {
                string str = System.Text.Encoding.UTF8.GetString(bytes).TrimEnd(new char[] { (char)((byte)0) });
                return str;
            }
        }
    }
}
