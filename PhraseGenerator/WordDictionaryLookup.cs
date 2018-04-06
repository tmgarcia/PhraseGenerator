using System.Collections.Generic;
using System.Linq;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        public class WordDictionaryLookup
        {
            public WordDictionaryLookup(int seed)
            {
                rand = new System.Random(seed);
            }
            private System.Random rand;

            /// <summary>
            /// Gets a weighted random word by its part of speech
            /// </summary>
            /// <param name="pos">The part of speech of the random word</param>
            /// <returns>A random word of the specified part of speech</returns>
            public Word GetRandomWordByPOS(PartOfSpeech pos)
            {
                CachedWordList wordList = WordDictionary.Instance.GetWordListByPOS(pos);
                return GetWeightedRandomWordFromList(wordList.words, wordList.totalFrequency);
            }

            /// <summary>
            /// Gets a random word from a specified list of words. Weighs each word's frequency against the total frequency of the list.
            /// </summary>
            /// <param name="wordList">The list of words to select from</param>
            /// <param name="totalFrequency">The sum of every word's frequency</param>
            /// <returns>A weighted random word.</returns>
            public Word GetWeightedRandomWordFromList(List<Word> wordList, float totalFrequency)
            {
                float randomIndex = (float)rand.NextDouble();
                bool foundWord = false;
                float currentIndex = 0;
                Word randomWord = wordList[0];
                for (int i = 0; i < wordList.Count && !foundWord; i++)
                {
                    float weightedIndex = (float)wordList[i].Frequency / (float)totalFrequency;
                    currentIndex += weightedIndex;
                    if (currentIndex + 0.000000001f >= randomIndex)
                    {
                        foundWord = true;
                        randomWord = wordList[i];
                    }
                }
                return randomWord;
            }


        }
    }
}