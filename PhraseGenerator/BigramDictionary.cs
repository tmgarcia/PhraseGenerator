using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        class BigramDictionary
        {
            public List<BigramEntry> entries { get; private set; }
            private CachedBigramListContainer cachedLists;
            private bool bigramsLoaded;

            /// <summary>
            /// Gets a list of all bigram entries with a first word of a specified part of speech and a second word of a specified part of speech.
            /// First tries to find a cached list of entries with the two parts of speech. If one has not been cached previously, it creates the list and caches it.
            /// </summary>
            /// <param name="firstPOS">The part of speech of the first word of the entries.</param>
            /// <param name="secondPOS">The part of speech of the second word of the entries.</param>
            /// <returns>A list of all bigram entries with the specified first and second parts of speech.</returns>
            public CachedBigramList GetBigramListByPOS(PartOfSpeech firstPOS, PartOfSpeech secondPOS)
            {
                CachedBigramList cachedList = cachedLists.GetCachedBigramByPOSList(firstPOS, secondPOS);
                if (cachedList == null)
                {
                    List<BigramEntry> entryList = entries.Where(e => e.First.PoS == firstPOS && e.Second.PoS == secondPOS).ToList();
                    int frequency = (int)entryList.Sum(e => e.Frequency);
                    cachedList = cachedLists.CacheBigramByPOSList(entryList, frequency, firstPOS, secondPOS);
                }
                return cachedList;
            }

            public void LoadBigramsIntoDictionary(List<BigramEntry> entryList)
            {
                entries = entryList;
                bigramsLoaded = true;
            }

            private static BigramDictionary _instance = null;
            public static BigramDictionary Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new BigramDictionary();
                    }
                    if (!_instance.bigramsLoaded)
                    {
                        //BigramProcessor processor = new BigramProcessor();
                        //processor.LoadInFromBinary();
                    }
                    return _instance;
                }
            }
            private BigramDictionary()
            {
                entries = new List<BigramEntry>();
                bigramsLoaded = false;
                cachedLists = new CachedBigramListContainer();
            }
        }
    }
}
