using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        public class BigramDictionaryLookup
        {
            private WordDictionaryLookup wordDictLookup;
            public BigramDictionaryLookup(WordDictionaryLookup wordDictLookup, int seed)
            {
                this.wordDictLookup = wordDictLookup;
                rand = new System.Random(seed);
            }
            private System.Random rand;

            #region Public Bigram Lookup Methods
            /// <summary>
            /// Get a random second word weighted as likely to occur between a specified first and third word. Essentially acts as a pseudo Trigram lookup.
            /// </summary>
            /// <param name="firstWord">The word before the word being queried for.</param>
            /// <param name="secondWordPOS">The part of speech of the word being queried for.</param>
            /// <param name="thirdWord">The word after the word being queried for.</param>
            /// <returns>A word most likely to appear between the specified first and third words.</returns>
            public Word GetRandomBetweenTwoWords(Word firstWord, PartOfSpeech secondWordPOS, Word thirdWord)
            {
                Word randomWord = null;
                CachedBigramList firstBigramList = BigramDictionary.Instance.GetBigramListByPOS(firstWord.PoS, secondWordPOS);
                CachedBigramList secondBigramList = BigramDictionary.Instance.GetBigramListByPOS(secondWordPOS, thirdWord.PoS);
                if ((firstBigramList != null && secondBigramList != null) && (firstBigramList.entries.Count > 0 && secondBigramList.entries.Count > 0))
                {
                    CachedBigramList narrowedFirstBigramList = NarrowListToWord(firstBigramList, firstWord, true);
                    CachedBigramList narrowedSecondBigramList = NarrowListToWord(secondBigramList, thirdWord, false);
                    if (narrowedFirstBigramList != null && narrowedSecondBigramList != null)
                    {
                        firstBigramList = narrowedFirstBigramList;
                        secondBigramList = narrowedSecondBigramList;
                        List<Word> intersectingWords = IntersectedBigramLists(firstBigramList, secondBigramList);
                        if (intersectingWords != null && intersectingWords.Count != 0)
                        {
                            randomWord = wordDictLookup.GetWeightedRandomWordFromList(intersectingWords, intersectingWords.Sum(w => w.Frequency));
                        }
                    }
                }
                if (randomWord == null)
                {
                    if ((firstBigramList == null || secondBigramList == null) || (firstBigramList.entries.Count == 0 || secondBigramList.entries.Count == 0))
                    {
                        randomWord = wordDictLookup.GetRandomWordByPOS(secondWordPOS);
                    }
                    else
                    {
                        randomWord = ((float)rand.NextDouble() > 0.5f) ?
                            GetWeightedRandomEntryFromList(firstBigramList.entries, firstBigramList.totalFrequency).Second
                            : GetWeightedRandomEntryFromList(secondBigramList.entries, secondBigramList.totalFrequency).First;
                    }
                }

                return randomWord;
            }
            /// <summary>
            /// Get a random word weighted to occur after a specified first word.
            /// </summary>
            /// <param name="firstWord">The word before the word being queried for.</param>
            /// <param name="secondWordPOS">The part of speech of the word being queried for.</param>
            /// <returns>A word most likely to occur after the specified first word.</returns>
            public Word GetRandomSecondWord(Word firstWord, PartOfSpeech secondWordPOS)
            {
                CachedBigramList bigramList = BigramDictionary.Instance.GetBigramListByPOS(firstWord.PoS, secondWordPOS);
                CachedBigramList listToSearch = bigramList;
                if (bigramList.entries.Any(e => e.First.Equals(firstWord)))
                {
                    List<BigramEntry> entriesWithWord = bigramList.entries.Where(e => e.First.Equals(firstWord)).ToList();
                    int frequency = (int)entriesWithWord.Sum(e => e.Frequency);
                    listToSearch = new CachedBigramList() { entries = entriesWithWord, totalFrequency = frequency };
                }
                BigramEntry randomEntry = GetWeightedRandomEntryFromList(listToSearch.entries, listToSearch.totalFrequency);
                Word randomWord = null;
                if (randomEntry == null)
                {
                    randomWord = wordDictLookup.GetRandomWordByPOS(secondWordPOS);
                }
                else
                {
                    randomWord = randomEntry.Second;
                }
                return randomWord;
            }
            /// <summary>
            /// Get a random word weighted to occur after any word with a specified part of speech.
            /// </summary>
            /// <param name="firstWordPOS">The part of speech of the word that would come before the word being queried for.</param>
            /// <param name="secondWordPOS">The part of speech of the word being queried for.</param>
            /// <returns></returns>
            public Word GetRandomSecondWord(PartOfSpeech firstWordPOS, PartOfSpeech secondWordPOS)
            {
                CachedBigramList bigramList = BigramDictionary.Instance.GetBigramListByPOS(firstWordPOS, secondWordPOS);
                BigramEntry randomEntry = GetWeightedRandomEntryFromList(bigramList.entries, bigramList.totalFrequency);
                Word randomWord = null;
                if (randomEntry == null)
                {
                    randomWord = wordDictLookup.GetRandomWordByPOS(secondWordPOS);
                }
                else
                {
                    randomWord = randomEntry.Second;
                }
                return randomWord;
            }
            /// <summary>
            /// Get a random word weighted to before after a specified second word.
            /// </summary>
            /// <param name="firstWordPOS">The part of speech of the word being queried for.</param>
            /// <param name="secondWord">The word after the word being queried for.</param>
            /// <returns></returns>
            public Word GetRandomFirstWord(PartOfSpeech firstWordPOS, Word secondWord)
            {
                CachedBigramList bigramList = BigramDictionary.Instance.GetBigramListByPOS(firstWordPOS, secondWord.PoS);
                CachedBigramList listToSearch = bigramList;
                if (bigramList.entries.Any(e => e.Second.Equals(secondWord)))
                {
                    List<BigramEntry> entriesWithWord = bigramList.entries.Where(e => e.Second.Equals(secondWord)).ToList();
                    int frequency = (int)entriesWithWord.Sum(e => e.Frequency);
                    listToSearch = new CachedBigramList() { entries = entriesWithWord, totalFrequency = frequency };
                }
                BigramEntry randomEntry = GetWeightedRandomEntryFromList(listToSearch.entries, listToSearch.totalFrequency);
                Word randomWord = null;
                if (randomEntry == null)
                {
                    randomWord = wordDictLookup.GetRandomWordByPOS(firstWordPOS);
                }
                else
                {
                    randomWord = randomEntry.First;
                }
                return randomWord;
            }
            /// <summary>
            /// Get a random word weighted to occur before a specified second word.
            /// </summary>
            /// <param name="firstWordPOS">The part of speech of the word being queried for.</param>
            /// <param name="secondWordPOS">The part of speech of the word that would come after the word being queried for.</param>
            /// <returns></returns>
            public Word GetRandomFirstWord(PartOfSpeech firstWordPOS, PartOfSpeech secondWordPOS)
            {
                CachedBigramList bigramList = BigramDictionary.Instance.GetBigramListByPOS(firstWordPOS, secondWordPOS);
                BigramEntry randomEntry = GetWeightedRandomEntryFromList(bigramList.entries, bigramList.totalFrequency);
                Word randomWord = null;
                if (randomEntry == null)
                {
                    randomWord = wordDictLookup.GetRandomWordByPOS(firstWordPOS);
                }
                else
                {
                    randomWord = randomEntry.First;
                }
                return randomWord;
            }
            #endregion

            #region Private Helper Methods
            /// <summary>
            /// Gets a list of words common between two lists of bigram entries. 
            /// The intersecting words are found as the second word of entries in the first list, and as the first word in the entries of the second list.
            /// The frequencies of the words are modified to reflect their weight of showing up in both lists.
            /// </summary>
            /// <param name="first">A list of entries from which the second words are examined to retrieve intersecting words.</param>
            /// <param name="second">A list of entries from which the first words are examined to retrieve intersecting words.</param>
            /// <returns>A list of all words that the first list contains as a second word in any entry, and that the second list contains as a first word in any entry.</returns>
            private List<Word> IntersectedBigramLists(CachedBigramList first, CachedBigramList second)
            {
                List<Word> intersectedWords = new List<Word>();
                Dictionary<Word, float> wordToIntersectOn = first.entries.ToDictionary(e => e.Second, e => (float)e.Frequency / (float)first.totalFrequency);
                foreach (BigramEntry entry in second.entries)
                {
                    float firstFrequency;
                    if (wordToIntersectOn.TryGetValue(entry.First, out firstFrequency))
                    {
                        float secondFrequency = (float)entry.Frequency / (float)second.totalFrequency;
                        Word intersectedWord = entry.First.Copy();
                        intersectedWord.Frequency = secondFrequency;
                        intersectedWords.Add(intersectedWord);
                    }
                }
                return intersectedWords;
            }
            /// <summary>
            /// Takes a CachedBigramList and narrows it down to only those entries that have the specified word as either a first or second word in the entr, as specified by the parameters.
            /// </summary>
            /// <param name="originalList">The original list to narrow.</param>
            /// <param name="word">The word the list is being narrowed down to.</param>
            /// <param name="firstWord">True if entries are being narrowed down according to the first word in the entries, or false for the second words.</param>
            /// <returns>Those entries in the CachedBigramList that contain the specified word, or null if that word was not found.</returns>
            private CachedBigramList NarrowListToWord(CachedBigramList originalList, Word word, bool firstWord)
            {
                CachedBigramList narrowedList = null;
                if (firstWord)
                {
                    if (originalList.entries.Any(e => e.First.Equals(word)))
                    {
                        List<BigramEntry> entriesWithWord = originalList.entries.Where(e => e.First.Equals(word)).ToList();
                        int frequency = (int)entriesWithWord.Sum(e => e.Frequency);
                        narrowedList = new CachedBigramList() { entries = entriesWithWord, totalFrequency = frequency };
                    }
                }
                else
                {
                    if (originalList.entries.Any(e => e.Second.Equals(word)))
                    {
                        List<BigramEntry> entriesWithWord = originalList.entries.Where(e => e.Second.Equals(word)).ToList();
                        int frequency = (int)entriesWithWord.Sum(e => e.Frequency);
                        narrowedList = new CachedBigramList() { entries = entriesWithWord, totalFrequency = frequency };
                    }
                }
                return narrowedList;
            }
            /// <summary>
            /// Gets a random BigramEntry, using the frequencies of the entries as a weight.
            /// </summary>
            /// <param name="entryList">The list of entries to select an entry from</param>
            /// <param name="totalFrequency">The sum of every entry's frequency in the list.</param>
            /// <returns>A weighted random bigram entry</returns>
            private BigramEntry GetWeightedRandomEntryFromList(List<BigramEntry> entryList, int totalFrequency)
            {
                float randomIndex = (float)rand.NextDouble();
                bool foundWord = false;
                float currentIndex = 0;
                BigramEntry randomEntry = null;
                if (entryList != null && entryList.Count > 0)
                {
                    randomEntry = entryList[0];
                    for (int i = 0; i < entryList.Count && !foundWord; i++)
                    {
                        float weightedIndex = (float)entryList[i].Frequency / (float)totalFrequency;
                        currentIndex += weightedIndex;
                        if (currentIndex + 0.00001f >= randomIndex)
                        {
                            foundWord = true;
                            randomEntry = entryList[i];
                        }
                    }
                }
                return randomEntry;
            }
            #endregion
        }
    }
}