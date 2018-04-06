///////////////////////////////////////////////////////////////
//
// WordDictionary (c) 2015 React Games
//
// Created by Taylor Garcia on 3/23/2015
//
///////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.Linq;
//Will switch over from using LINQ to LinqUtils once I add a method to replace OrderByDescending
//using React.Utils;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        class WordDictionary
        {
            public CachedWordListContainer cachedLists { get; private set; }
            public Dictionary<int, Word> words { get; private set; }
            private bool wordsLoaded;

            /// <summary>
            /// Gets a word that has the specified value as it's literal string value.
            /// If a word with that value cannot be found, an attempt will be made to find a word with the lowercase variant.
            /// If no word of either varient of the value can be found, a new one will be created under the Unclassified part of speech.
            /// </summary>
            /// <param name="value">The literal string value of the potential word. E.G. "the" or "ham"</param>
            /// <returns>The word from the dictionary, or, if no word was found, a new word of the Unclassified part of speech.</returns>
            public Word GetWordByValue(string value)
            {
                Word word = WordDictionary.Instance.cachedLists.GetCachedWordByValue(value);
                if (word == null)//Word has not been cached to this value previously, try to find and cache it
                {
                    word = WordDictionary.Instance.words.Values.Where(w => w.Value == value).OrderByDescending(w => w.Frequency).FirstOrDefault();
                    if (word == null)//A word with this exact value does not exist, try the value lower cased
                    {
                        string lowerValue = value.ToLowerInvariant();
                        if (value != lowerValue)//The value wasn't already lower cased
                        {
                            word = WordDictionary.Instance.words.Values.Where(w => w.Value == lowerValue).OrderByDescending(w => w.Frequency).FirstOrDefault();

                        }
                    }
                    if (word == null)//A word with the lower case version of this value does not exist, create a new word as Unclassified
                    {
                        word = new Word(value, PartOfSpeech.Unclassified, 1);
                    }
                    WordDictionary.Instance.cachedLists.CacheWordByValue(value, word);
                }

                return word;
            }

            /// <summary>
            /// Gets the word with the specified id.
            /// </summary>
            /// <param name="id">Id of the word</param>
            /// <returns>The word with the specified Id, if found, or null, if not found.</returns>
            public Word GetWordById(int id)
            {
                Word word = null;
                if (!WordDictionary.Instance.words.TryGetValue(id, out word))
                {
                    //Debug.Log("Word of ID " + id + " does not exist or could not be found.");
                }
                return word;
            }

            /// <summary>
            /// Gets the word at the specified index.
            /// </summary>
            /// <param name="index"></param>
            /// <returns>The word at the specified index, or null if that index does not exist. Or if the word there is null.</returns>
            public Word GetWordByIndex(int index)
            {
                Word word = null;
                if (index < WordDictionary.Instance.words.Count && index >= 0)
                {
                    word = WordDictionary.Instance.words[index];
                }
                else
                {
                    //Debug.Log("Could not retreive Word at " + index + ", there are " + words.Count + " Words.");
                }
                return word;
            }

            /// <summary>
            /// Returns all words that start with the specified string.
            /// </summary>
            /// <param name="str">The beginging of the word being searched for. E.G. "th"</param>
            /// <returns>A list of all words in the dictionary that start with the specified string.</returns>
            public List<Word> GetWordsThatStartWith(string str)
            {
                return WordDictionary.Instance.words.Where(w => w.Value.Value.StartsWith(str)).Select(w => w.Value).ToList();
            }

            /// <summary>
            /// Gets a list of all words of a specified part of speech.
            /// First tries to retrieve a cached list of words of the part of speech.
            /// If one does not exist, it is created and cached.
            /// </summary>
            /// <param name="pos">The part of speech the words in the list will have in commons.</param>
            /// <returns>A list of words that all have the specified part of speech in common.</returns>
            public CachedWordList GetWordListByPOS(PartOfSpeech pos)
            {
                CachedWordList wordListByPOS = cachedLists.GetCachedWordByPOSList(pos);
                if (wordListByPOS == null)
                {
                    List<Word> wordList = words.Where(wkvp => wkvp.Value.PoS == pos).Select(wkvp => wkvp.Value).ToList();
                    float frequency = wordList.Sum(w => w.Frequency);
                    wordListByPOS = cachedLists.CacheWordByPOSList(pos, wordList, frequency);
                }
                return wordListByPOS;
            }

            public void LoadWordsIntoDictionary(List<Word> wordList)
            {
                foreach (Word word in wordList)
                {
                    words.Add(word.Id, word);
                }
                //sort words here
                wordsLoaded = true;
            }
            private static WordDictionary _instance;
            public static WordDictionary Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new WordDictionary();
                        if (_instance.wordsLoaded)
                        {

                        }
                    }
                    return _instance;
                }
            }
            private WordDictionary()
            {
                wordsLoaded = false;
                words = new Dictionary<int, Word>();
                cachedLists = new CachedWordListContainer();
            }
        }
    }
}
