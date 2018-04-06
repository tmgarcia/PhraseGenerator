using System.Collections.Generic;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        /// <summary>
        /// Contains cached lists of bigram entries as queried from the BigramDictionary, to speed up multiple lookups of the same bigram queries
        /// </summary>
        class CachedBigramListContainer
        {
            Dictionary<CachedBigramByPOSListKey, CachedBigramList> cachedBigramByPOSLists;
            public CachedBigramListContainer()
            {
                cachedBigramByPOSLists = new Dictionary<CachedBigramByPOSListKey, CachedBigramList>();
            }
            public CachedBigramList GetCachedBigramByPOSList(PartOfSpeech firstPos, PartOfSpeech secondPos)
            {
                CachedBigramList list = null;
                cachedBigramByPOSLists.TryGetValue(new CachedBigramByPOSListKey() { firstPos = firstPos, secondPos = secondPos }, out list);
                return list;
            }
            public CachedBigramList CacheBigramByPOSList(List<BigramEntry> bigramList, int totalFrequency, PartOfSpeech first, PartOfSpeech second)
            {
                CachedBigramList cachedList = new CachedBigramList() { entries = bigramList, totalFrequency = totalFrequency };
                CachedBigramByPOSListKey cachedListKey = new CachedBigramByPOSListKey() { firstPos = first, secondPos = second };
                cachedBigramByPOSLists.Add(cachedListKey, cachedList);
                return cachedList;
            }

            protected class CachedBigramByPOSListKey
            {
                public PartOfSpeech firstPos;
                public PartOfSpeech secondPos;
                public override bool Equals(object obj)
                {
                    if (this == obj) return true;
                    if (obj == null || this.GetType() != obj.GetType()) return false;
                    CachedBigramByPOSListKey that = (CachedBigramByPOSListKey)obj;
                    if (this.firstPos != that.firstPos || this.secondPos != that.secondPos)
                    {
                        return false;
                    }
                    return true;
                }
                public override int GetHashCode()
                {
                    int result = firstPos.GetHashCode();
                    result = 31 * result + secondPos.GetHashCode();
                    return result;
                }
            }
        }

        public class CachedBigramList
        {
            public int totalFrequency;
            public List<BigramEntry> entries;
        }
    }
}
