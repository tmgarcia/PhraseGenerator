using System.Collections.Generic;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        /// <summary>
        /// Contains cached lists of words as queried from the WordDictionary, to speed up multiple lookups of the same words or parts of speech.
        /// </summary>
        class CachedWordListContainer
        {
            private Dictionary<PartOfSpeech, CachedWordList> _cachedWordByPOSLists;
            private Dictionary<string, Word> _cachedWordsByValue;

            public CachedWordListContainer()
            {
                _cachedWordByPOSLists = new Dictionary<PartOfSpeech, CachedWordList>();
                _cachedWordsByValue = new Dictionary<string, Word>();
            }
            public CachedWordList GetCachedWordByPOSList(PartOfSpeech pos)
            {
                CachedWordList list = null;
                _cachedWordByPOSLists.TryGetValue(pos, out list);
                return list;
            }
            public Word GetCachedWordByValue(string value)
            {
                Word cachedWord = null;
                _cachedWordsByValue.TryGetValue(value, out cachedWord);
                return cachedWord;
            }
            public CachedWordList CacheWordByPOSList(PartOfSpeech pos, List<Word> words, float frequency)
            {
                CachedWordList l = new CachedWordList() { totalFrequency = frequency, words = words };
                _cachedWordByPOSLists.Add(pos, l);
                return l;
            }
            public void CacheWordByValue(string value, Word word)
            {
                _cachedWordsByValue.Add(value, word);
            }
        }
        public class CachedWordList
        {
            public List<Word> words;
            public float totalFrequency;
        }
    }
}
