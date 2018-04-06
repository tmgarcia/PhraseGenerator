using System.Collections.Generic;
using System.IO;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        public class BigramEntry
        {
            public const int byteSize = 3 * sizeof(int);//2 word ids, frequency
            private int _firstWordId;
            private int _secondWordId;
            private Word _first;
            public Word First
            {
                get
                {
                    if (_first == null)
                    {
                        _first = WordDictionary.Instance.GetWordById(_firstWordId);
                    }
                    return _first;
                }
            }
            private Word _second;
            public Word Second
            {
                get
                {
                    if (_second == null)
                    {
                        _second = WordDictionary.Instance.GetWordById(_secondWordId);
                    }
                    return _second;
                }
            }
            public int Frequency
            {
                private set;
                get;
            }
            public BigramEntry(Word firstWord, Word secondWord, int frequency)
            {
                _first = firstWord;
                _second = secondWord;
                this.Frequency = frequency;
            }
            private BigramEntry(int firstWordId, int secondWordId, int frequency)
            {
                this._firstWordId = firstWordId;
                this._secondWordId = secondWordId;
                this.Frequency = frequency;
            }
            public static byte[] ToBytes(BigramEntry entry)
            {
                byte[] bytes = new byte[byteSize];
                using (MemoryStream mem = new MemoryStream(bytes))
                {
                    using (BinaryWriter bw = new BinaryWriter(mem))
                    {
                        bw.Write(entry.First.Id);
                        bw.Write(entry.Second.Id);
                        bw.Write(entry.Frequency);
                    }
                }
                return bytes;
            }
            public static BigramEntry FromBytes(byte[] bytes)
            {
                int firstId;
                int secondId;
                int frequency;
                using (MemoryStream mem = new MemoryStream(bytes))
                {
                    using (BinaryReader br = new BinaryReader(mem))
                    {
                        firstId = br.ReadInt32();
                        secondId = br.ReadInt32();
                        frequency = br.ReadInt32();
                    }
                }
                //By storing the word ids and delaying the grabbing of the actual words until they're needed,
                //loading in the Bigram Dictionary binary file is greatly sped up.
                BigramEntry entry = new BigramEntry(firstId, secondId, frequency);
                return entry;
            }
            public override string ToString()
            {
                return "" + Frequency + " - " + First.ToString() + ", " + Second.ToString();
            }
        }
    }
}
