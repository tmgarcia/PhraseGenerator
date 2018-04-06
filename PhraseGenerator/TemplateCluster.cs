using System.Collections.Generic;
using System.Linq;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        //The structure of where these classes are and how they are set up is going to be changed as how the mad libs system is going to be used gets developed
        //They will probably each be in their own .cs documents
        class TemplateParser
        {
            public static char[] separators = new char[] { ' ', ',', '\t', '-' };
            public static char[] phraseSeparators = new char[] { '.', ';', '?', '!', '\n', '\"', ')', '(','\r' };
            public static TemplateCluster ParseTemplate(string template)
            {
                TemplateCluster cluster = new TemplateCluster(template);

                TemplatePhrase currentPhrase = new TemplatePhrase();

                int startIndex = 0;
                bool indexOnSeparator = true;
                for (int i = 0; i <= template.Length; i++)
                {
                    if (i == template.Length || separators.Contains(template[i]) || phraseSeparators.Contains(template[i]))
                    {
                        if (!indexOnSeparator)
                        {
                            currentPhrase.AddWord(new TemplateWord(template, startIndex, i - startIndex));
                            if (i == template.Length || (phraseSeparators.Contains(template[i]) && currentPhrase.WordCount() > 0))
                            {
                                cluster.AddPhrase(currentPhrase);
                                currentPhrase = new TemplatePhrase();
                            }
                        }

                        startIndex = i;
                        indexOnSeparator = true;
                    }
                    else
                    {
                        if (indexOnSeparator)
                        {
                            indexOnSeparator = false;
                            startIndex = i;
                        }
                    }
                }

                return cluster;
            }
        }

        public class TemplateCluster
        {
            List<TemplatePhrase> phrases;
            string template;

            public TemplateCluster(string template)
            {
                phrases = new List<TemplatePhrase>();
                this.template = template;
            }

            public void AddPhrase(TemplatePhrase phrase)
            {
                phrases.Add(phrase);
            }

            public string Populate(int seed)
            {
                return Populate(seed, false);
            }
            public string Populate(int seed, bool checkGrammar)
            {
                Dictionary<int, Word> definedGroupWords = new Dictionary<int, Word>();
                WordDictionaryLookup wordLookup = new WordDictionaryLookup(seed);
                BigramDictionaryLookup bigramLookup = new BigramDictionaryLookup(wordLookup, seed);
                foreach (TemplatePhrase phrase in phrases)
                {
                    phrase.SetSeed(seed);
                    phrase.Populate(definedGroupWords,bigramLookup,wordLookup);
                }

                string result = template;
                int offset = 0;

                if (checkGrammar)
                {
                    foreach (TemplatePhrase phrase in phrases)
                    {
                        List<TemplateWord> editedWords = GrammarEditor.EditPhrase(phrase.words);
                        foreach (TemplateWord word in editedWords)
                        {
                            result = result.Substring(0, word.start + offset) + word.Word.Value + result.Substring(word.start + word.length + offset);
                            offset += word.Word.Value.Length - word.ToString().Length;
                        }
                    }
                    result = GrammarEditor.Filter(result);
                }
                else
                {
                    foreach (TemplatePhrase phrase in phrases)
                    {
                        foreach (TemplateWord word in phrase.words)
                        {
                            result = result.Substring(0, word.start + offset) + word.Word.Value + result.Substring(word.start + word.length + offset);
                            offset += word.Word.Value.Length - word.ToString().Length;
                        }
                    }
                }
                return result;
            }

            public override string ToString()
            {
                string result = "";
                foreach (TemplatePhrase phrase in phrases)
                {
                    result += "<";
                    for (int i = 0; i < phrase.words.Count; i++)
                    {
                        TemplateWord word = phrase.words[i];
                        result += word.ToString() + "{" + word.PoS + "}";
                        if (i < phrase.words.Count - 1)
                            result += "|";
                    }
                    result += ">";
                }
                return result;
            }
        }

        public class TemplatePhrase
        {
            public List<TemplateWord> words = new List<TemplateWord>();
            private static System.Random rand = new System.Random();
            public void SetSeed(int seed)
            {
                rand = new System.Random(seed);
            }

            public void AddWord(TemplateWord word)
            {
                words.Add(word);
            }

            public void Populate(Dictionary<int, Word> populatedGroupWords, BigramDictionaryLookup bigramLookup, WordDictionaryLookup wordLookup)
            {
                foreach (TemplateWord word in words)
                {
                    if (word.IsHook)
                        word.Word = null;
                }

                List<int> indicesOfCurrentPassToExecute = new List<int>();
                do
                {
                    foreach (int index in indicesOfCurrentPassToExecute)
                    {
                        TemplateWord currentWord = words[index];
                        TemplateWord wordBefore = (index > 0 && !words[index - 1].NeedsHookPopulated) ? words[index - 1] : null;
                        TemplateWord wordAfter = (index < words.Count - 1 && !words[index + 1].NeedsHookPopulated) ? words[index + 1] : null;

                        bool populatedByGroup = false;
                        if (currentWord.group != 0 && populatedGroupWords.Count > 0)
                        {
                            Word wordForGroup = null;
                            if (populatedGroupWords.TryGetValue(currentWord.group, out wordForGroup))
                            {
                                populatedByGroup = true;
                                currentWord.Word = wordForGroup;
                            }
                        }
                        if (!populatedByGroup)
                        {
                            if (wordBefore != null && wordAfter != null)
                            {
                                currentWord.Word = bigramLookup.GetRandomBetweenTwoWords(wordBefore.Word, currentWord.PoS, wordAfter.Word);
                            }
                            else if (wordBefore != null)
                            {
                                currentWord.Word = bigramLookup.GetRandomSecondWord(wordBefore.Word, currentWord.PoS);
                            }
                            else if (wordAfter != null)
                            {
                                currentWord.Word = bigramLookup.GetRandomFirstWord(currentWord.PoS, wordAfter.Word);
                            }
                            else
                            {
                                currentWord.Word = wordLookup.GetRandomWordByPOS(currentWord.PoS);
                            }

                            if (currentWord.group != 0)
                            {
                                populatedGroupWords.Add(currentWord.group, currentWord.Word);
                            }
                        }
                        //TODO: yield return for coroutine implementation if needed
                    }
                    indicesOfCurrentPassToExecute.Clear();

                    bool areStillHooks = false;
                    bool lastWordWasHook = false;
                    for (int i = 0; i < words.Count; i++)
                    {
                        TemplateWord tWord = words[i];

                        bool currentWordIsHook = tWord.NeedsHookPopulated;
                        areStillHooks |= currentWordIsHook;

                        if (currentWordIsHook ^ lastWordWasHook)
                        {
                            if (currentWordIsHook && i > 0)
                                indicesOfCurrentPassToExecute.Add(i);

                            if (lastWordWasHook)
                                indicesOfCurrentPassToExecute.Add(i - 1);
                        }

                        lastWordWasHook = currentWordIsHook;
                    }

                    if (areStillHooks && indicesOfCurrentPassToExecute.Count == 0)
                    {
                        int times = (int)(rand.NextDouble() * (words.Count - 1)) + 1;
                        for (int i = 0; i < times; i++)
                        {
                            indicesOfCurrentPassToExecute.Add((int)(rand.NextDouble() * words.Count));
                        }
                    }
                }
                while (indicesOfCurrentPassToExecute.Count != 0);
            }

            public int WordCount()
            {
                return words.Count;
            }
        }

        public class TemplateWord
        {
            private string content;
            public int start, length;
            public int group { get; private set; }

            public bool IsHook { get; private set; }
            public bool NeedsHookPopulated { get { return IsHook && Word == null; } }

            public PartOfSpeech PoS { get; private set; }
            public Word Word { get; set; }

            public TemplateWord(string fullTemplate, int startIndex, int length)
            {
                content = fullTemplate.Substring(startIndex, length);
                start = startIndex;
                this.length = length;
                group = 0;

                PoS = null;
                string contentCopy = content;

                if (contentCopy[0] == '@')
                {
                    int tempGroup;
                    int endGroupIndex = contentCopy.LastIndexOfAny("0123456789".ToCharArray(), 3);
                    if (endGroupIndex > 0)
                    {
                        if (int.TryParse(contentCopy.Substring(1, endGroupIndex), out tempGroup))
                        {
                            group = tempGroup;
                        }
                    }
                    else
                    {
                        endGroupIndex = 0;
                    }
                    contentCopy = contentCopy.Substring(endGroupIndex + 1);
                }
                if (contentCopy[0] == '[' && contentCopy[contentCopy.Length - 1] == ']')
                {
                    PoS = PartOfSpeech.FromTag(contentCopy.Substring(1, contentCopy.Length - 2));
                    if (PoS != null)
                    {
                        IsHook = true;
                    }
                }
                else if (contentCopy[0] == '<' && contentCopy[contentCopy.Length - 1] == '>')
                {
                    int indexOfPoSStart = contentCopy.IndexOf('[');
                    if (indexOfPoSStart != -1 && contentCopy[contentCopy.Length - 2] == ']')
                    {
                        string posTag = contentCopy.Substring(indexOfPoSStart + 1, contentCopy.Length - indexOfPoSStart - 3);
                        PoS = PartOfSpeech.FromTag(posTag);
                        if (PoS != null)
                        {
                            Word = new Word(contentCopy.Substring(1, indexOfPoSStart - 1), PoS, 1);
                            IsHook = false;
                        }
                    }
                    if (PoS == null)
                    {
                        Word = WordDictionary.Instance.GetWordByValue(contentCopy.Substring(1, contentCopy.Length - 2));
                        IsHook = false;
                    }
                }
                else
                {
                    Word tempWord = WordDictionary.Instance.GetWordByValue(contentCopy);
                    if (tempWord.PoS != PartOfSpeech.Unclassified)
                    {
                        PoS = tempWord.PoS;
                        IsHook = true;
                    }
                }

                if (PoS == null && Word == null)
                {
                    Word = WordDictionary.Instance.GetWordByValue(contentCopy);
                    IsHook = false;
                }
            }

            public override string ToString()
            {
                return content;
            }
        }
    }
}
