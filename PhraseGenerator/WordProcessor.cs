using PhraseGenerator.Utils;
//using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        public class WordProcessor
        {
            private List<Word> words;

            public void LoadWordList(string wordBinaryFilename)
            {
                words = new List<Word>();
                byte[] wordBytes = FileHelper.FileToBytes(wordBinaryFilename);
                using (MemoryStream mem = new MemoryStream(wordBytes))
                {
                    using (BinaryReader br = new BinaryReader(mem))
                    {
                        br.ReadInt32();//extra int in binary file for total word frequency
                        byte[] bytes;
                        while ((bytes = br.ReadBytes(Word.byteSize)) != null && bytes.Length > 0)
                        {
                            Word w = Word.FromBytes(bytes);
                            words.Add(w);
                        }
                    }
                }
                WordDictionary.Instance.LoadWordsIntoDictionary(words);
            }
        }
    }
}