using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        class GrammarEditor
        {
            private static string[] capitalizedPosTags = { "fu", "np", "np1", "np2", "nn1", "npd1", "npd2", "npm1", "npm2", "nna", "nnb", "nnl1", "nnl2", "zz1", "zz2", "ppis1" };

            public static List<TemplateWord> EditPhrase(List<TemplateWord> phrase)
            {
                return EditCapitalization(phrase);
            }
            /// <summary>
            /// Capitalizes the first letter of the first word of the phrase, and converts any words that shouldn't be caplitalized to full lowercase.
            /// </summary>
            public static List<TemplateWord> EditCapitalization(List<TemplateWord> phrase)
            {
                List<TemplateWord> phraseCopy = phrase;

                if (phraseCopy.Count > 1)
                {
                    bool foundFirstWord = false;
                    TemplateWord firstWord = null;
                    for (int i = 0; i < phraseCopy.Count && !foundFirstWord; i++)
                    {
                        Word word = phraseCopy[i].Word;
                        if (!(word.Value.Length == 1 && (TemplateParser.separators.Contains(word.Value[0]) || TemplateParser.phraseSeparators.Contains(word.Value[0]))))
                        {
                            foundFirstWord = true;
                            firstWord = phraseCopy[i];
                        }
                    }
                    if (firstWord != null)
                    {
                        firstWord.Word = new Word(CapitalizeFirstLetter(firstWord.Word.Value), firstWord.Word.PoS, firstWord.Word.Frequency);
                    }
                }

                for (int i = 1; i < phraseCopy.Count; i++)
                {
                    if (!capitalizedPosTags.Contains(phraseCopy[i].Word.PoS.Tag.ToLower()))
                    {
                        phraseCopy[i].Word = new Word(phraseCopy[i].Word.Value.ToLowerInvariant(), phraseCopy[i].Word.PoS, phraseCopy[i].Word.Frequency);
                    }
                }
                return phraseCopy;
            }
            /// <summary>
            /// Capitalizes the first letter of the word
            /// </summary>
            private static string CapitalizeFirstLetter(string str)
            {
                if (char.IsLower(str[0]))
                {
                    return char.ToUpper(str[0]) + str.Substring(1, str.Length - 1);
                }
                return str;
            }
            /// <summary>
            /// Runs the string through a series of filters to remove things like strong language and any "strange words"/mistakes common in the BigramDictionary
            /// </summary>
            public static string Filter(string stringToFilter)
            {
                string filteredString = stringToFilter;
                foreach (PatternFilter filter in filters)
                {
                    filteredString = filter.Replace(filteredString);
                }
                return filteredString;
            }

            private static PatternFilter[] filters =
        {
            //I'm going to replace this filter with an external list of strong language that will be loaded in instead of having all of this hard coded in here.
            //Mostly because I hate seeing it every time I open this file, and also because the external list is much more extensive.
            new PatternFilter("\\ssnatch\\s|\\spussy\\s|\\sdick\\s|\\spenis\\s|\\swang\\s|\\sshit\\s|\\spiss\\s|\\sfuck\\s|\\scunt\\s|\\scocksucker\\s|\\smotherfucker\\s|\\stits\\s|\\sbastard\\s|\\sdamn\\s|\\sbitch\\s|\\sass\\s|\\sasshole\\s|\\ssex\\s|\\ssexual\\s|\\sfucker\\s","thing"),
            new PatternFilter("\\sca\\s","can"),
            new PatternFilter("\\s'nt\\s", "not")
        };
            private class PatternFilter
            {
                public string pattern { get; private set; }
                public string replacement { get; private set; }
                public Regex regex { get; private set; }
                public PatternFilter(string pattern, string replacement)
                {
                    this.pattern = pattern;
                    this.replacement = replacement;
                    this.regex = new Regex(pattern);
                }
                public string Replace(string inputString)
                {
                    return regex.Replace(inputString, replacement);
                }
            }
        }
    }
}
