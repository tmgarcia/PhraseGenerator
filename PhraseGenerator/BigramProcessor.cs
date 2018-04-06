using PhraseGenerator.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        public class BigramProcessor
        {
            private WordProcessor wordProcessor;
            private List<BigramEntry> entries;

            //public TextAsset BigramsBinaryFile;
            //public TextAsset WordsBinaryFile;

            /// <summary>
            /// Loads in the bigram and word dictionary files from the binary text assets.
            /// </summary>
            public void LoadInFromBinary(string BigramsBinaryFilename, string WordsBinaryFilename)
            {
                if (BigramsBinaryFilename != null && WordsBinaryFilename != null)
                {
                    entries = new List<BigramEntry>();
                    wordProcessor = new WordProcessor();
                    wordProcessor.LoadWordList(WordsBinaryFilename);
                    byte[] bytesFromFile = FileHelper.FileToBytes(BigramsBinaryFilename);
                    CreateEntriesFromBinary(bytesFromFile);
                    BigramDictionary.Instance.LoadBigramsIntoDictionary(entries);
                }
                else
                {
                    //Debug.LogError("Missing bigram and/or word binary file");
                }
            }
            private void CreateEntriesFromBinary(byte[] byteFromFile)
            {
                int lineNum = 0;
                using (MemoryStream mem = new MemoryStream(byteFromFile))
                {
                    using (BinaryReader br = new BinaryReader(mem))
                    {
                        br.ReadInt32();//binary file written with a "total frequency" int used for IO loading and previous debugging in console project
                        br.ReadInt32();//binary file was written with an extra int to indicate how many bytes the entries made up (for typical IO loading)
                        byte[] bytes;
                        while ((bytes = br.ReadBytes(BigramEntry.byteSize)) != null && bytes.Length > 0)
                        {
                            BigramEntry e = BigramEntry.FromBytes(bytes);
                            entries.Add(e);
                            lineNum++;
                            if (lineNum % 100 == 0)
                            {
                                //Debug.Log("\r{0} entries loaded     ", lineNum);
                            }
                        }
                    }
                }
            }
        }
    }
}