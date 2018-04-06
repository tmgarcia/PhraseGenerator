using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PhraseGenerator
{
    namespace PhraseGeneration
    {
        /// <summary>
        /// Part of speech tags as defined by the UCREL CLAWS7 Tagset
        /// </summary>
        public class PartOfSpeech
        {
            //All of this is set up so that I can (soon) make it easier for others to use parts of speech tags with the MadLibs system via UI using the categories 
            //& descriptions so that they don't have to memorize/look back to figure out which part of speech they need
            private static int nextId;
            public int Id { private set; get; }
            public string Tag { private set; get; }
            public string Description { private set; get; }
            public PoSCategory Category { private set; get; }
            public enum PoSCategory { Article, Noun, Adjective, Verb, Pronoun, Adverb, Determiner, Preposition, Number, Conjunction, Misc }
            
            public static PartOfSpeech FromTag(string tag)
            {
                if (AllPartsOfSpeech == null)
                {
                    initialize();
                }
                PartOfSpeech pos = null;
                AllPartsOfSpeech.TryGetValue(tag.ToUpper(), out pos);
                return pos;
            }
            public static PartOfSpeech FromId(int id)
            {
                if (AllPartsOfSpeech == null)
                {
                    initialize();
                }
                return AllPartsOfSpeech.Values.FirstOrDefault(p => p.Id == id);
            }
            public static PartOfSpeech Unclassified
            {
                get
                {
                    //Yes this is the actual tag for unclassified words
                    return AllPartsOfSpeech["FU"];
                }
            }

            public override string ToString()
            {
                return Tag;
            }
            private PartOfSpeech(string tag, string description, PoSCategory category)
            {
                this.Tag = tag;
                this.Description = description;
                this.Id = nextId++;
            }

            public static Dictionary<string, PartOfSpeech> AllPartsOfSpeech;
            public static void initialize()
            {
                AllPartsOfSpeech = new Dictionary<string, PartOfSpeech>();
                AllPartsOfSpeech.Add("APPGE", new PartOfSpeech("APPGE", "possessive pronoun, pre-nominal (e.g. my, your, our)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("AT", new PartOfSpeech("AT", "article (e.g. the, no)", PoSCategory.Article));
                AllPartsOfSpeech.Add("AT1", new PartOfSpeech("AT1", "singular article (e.g. a, an, every)", PoSCategory.Article));

                AllPartsOfSpeech.Add("BCL", new PartOfSpeech("BCL", "before-clause marker (e.g. in order (that),in order (to))", PoSCategory.Misc));
                AllPartsOfSpeech.Add("CC", new PartOfSpeech("CC", "coordinating conjunction (e.g. and, or)", PoSCategory.Conjunction));
                AllPartsOfSpeech.Add("CCB", new PartOfSpeech("CCB", "adversative coordinating conjunction ( but)", PoSCategory.Conjunction));
                AllPartsOfSpeech.Add("CS", new PartOfSpeech("CS", "subordinating conjunction (e.g. if, because, unless, so, for)", PoSCategory.Conjunction));
                AllPartsOfSpeech.Add("CSA", new PartOfSpeech("CSA", "as (as conjunction)", PoSCategory.Conjunction));
                AllPartsOfSpeech.Add("CSN", new PartOfSpeech("CSN", "than (as conjunction)", PoSCategory.Conjunction));
                AllPartsOfSpeech.Add("CST", new PartOfSpeech("CST", "that (as conjunction)", PoSCategory.Conjunction));
                AllPartsOfSpeech.Add("CSW", new PartOfSpeech("CSW", "whether (as conjunction)", PoSCategory.Conjunction));

                AllPartsOfSpeech.Add("DA", new PartOfSpeech("DA", "after-determiner or post-determiner capable of pronominal function (e.g. such, former, same)", PoSCategory.Determiner));
                AllPartsOfSpeech.Add("DA1", new PartOfSpeech("DA1", "singular after-determiner (e.g. little, much)", PoSCategory.Determiner));
                AllPartsOfSpeech.Add("DA2", new PartOfSpeech("DA2", "plural after-determiner (e.g. few, several, many)", PoSCategory.Determiner));
                AllPartsOfSpeech.Add("DAR", new PartOfSpeech("DAR ", "comparative after-determiner (e.g. more, less, fewer)", PoSCategory.Determiner));
                AllPartsOfSpeech.Add("DAT", new PartOfSpeech("DAT", "superlative after-determiner (e.g. most, least, fewest)", PoSCategory.Determiner));
                AllPartsOfSpeech.Add("DB", new PartOfSpeech("DB", "before determiner or pre-determiner capable of pronominal function (all, half)", PoSCategory.Determiner));
                AllPartsOfSpeech.Add("DB2", new PartOfSpeech("DB2", "plural before-determiner ( both)", PoSCategory.Determiner));
                AllPartsOfSpeech.Add("DD", new PartOfSpeech("DD", "determiner (capable of pronominal function) (e.g any, some)", PoSCategory.Determiner));
                AllPartsOfSpeech.Add("DD1", new PartOfSpeech("DD1", "singular determiner (e.g. this, that, another)", PoSCategory.Determiner));
                AllPartsOfSpeech.Add("DD2", new PartOfSpeech("DD2", "plural determiner ( these,those)", PoSCategory.Determiner));
                AllPartsOfSpeech.Add("DDQ", new PartOfSpeech("DDQ", "wh-determiner (which, what)", PoSCategory.Determiner));
                AllPartsOfSpeech.Add("DDQGE", new PartOfSpeech("DDQGE", "wh-determiner, genitive (whose)", PoSCategory.Determiner));
                AllPartsOfSpeech.Add("DDQV", new PartOfSpeech("DDQV", "wh-ever determiner, (whichever, whatever)", PoSCategory.Determiner));

                AllPartsOfSpeech.Add("EX", new PartOfSpeech("EX", "existential there", PoSCategory.Misc));
                AllPartsOfSpeech.Add("FO", new PartOfSpeech("FO", "formula", PoSCategory.Misc));
                AllPartsOfSpeech.Add("FU", new PartOfSpeech("FU", "unclassified word", PoSCategory.Misc));
                AllPartsOfSpeech.Add("FW", new PartOfSpeech("FW", "foreign word", PoSCategory.Misc));
                AllPartsOfSpeech.Add("GE", new PartOfSpeech("GE", "germanic genitive marker - (' or's)", PoSCategory.Misc));

                AllPartsOfSpeech.Add("IF", new PartOfSpeech("IF", "for (as preposition)", PoSCategory.Preposition));
                AllPartsOfSpeech.Add("II", new PartOfSpeech("II", "general preposition", PoSCategory.Preposition));
                AllPartsOfSpeech.Add("IO", new PartOfSpeech("IO", "of (as preposition)", PoSCategory.Preposition));
                AllPartsOfSpeech.Add("IW", new PartOfSpeech("IW", "with, without (as prepositions)", PoSCategory.Preposition));

                AllPartsOfSpeech.Add("JJ", new PartOfSpeech("JJ", "general adjective", PoSCategory.Adjective));
                AllPartsOfSpeech.Add("JJR", new PartOfSpeech("JJR", "general comparative adjective (e.g. older, better, stronger)", PoSCategory.Adjective));
                AllPartsOfSpeech.Add("JJT", new PartOfSpeech("JJT", "general superlative adjective (e.g. oldest, best, strongest)", PoSCategory.Adjective));
                AllPartsOfSpeech.Add("JK", new PartOfSpeech("JK", "catenative adjective (able in be able to, willing in be willing to)", PoSCategory.Adjective));

                AllPartsOfSpeech.Add("MC", new PartOfSpeech("MC", "cardinal number,neutral for number (two, three..)", PoSCategory.Number));
                AllPartsOfSpeech.Add("MC1", new PartOfSpeech("MC1", "singular cardinal number (one)", PoSCategory.Number));
                AllPartsOfSpeech.Add("MC2", new PartOfSpeech("MC2", "plural cardinal number (e.g. sixes, sevens)", PoSCategory.Number));
                AllPartsOfSpeech.Add("MCGE", new PartOfSpeech("MCGE", "genitive cardinal number, neutral for number (two's, 100's)", PoSCategory.Number));
                AllPartsOfSpeech.Add("MCMC", new PartOfSpeech("MCMC", "hyphenated number (40-50, 1770-1827)", PoSCategory.Number));
                AllPartsOfSpeech.Add("MD", new PartOfSpeech("MD", "ordinal number (e.g. first, second, next, last)", PoSCategory.Number));
                AllPartsOfSpeech.Add("MF", new PartOfSpeech("MF", "fraction,neutral for number (e.g. quarters, two-thirds)", PoSCategory.Number));

                AllPartsOfSpeech.Add("ND1", new PartOfSpeech("ND1", "singular noun of direction (e.g. north, southeast)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NN", new PartOfSpeech("NN", "common noun, neutral for number (e.g. sheep, cod, headquarters)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NN1", new PartOfSpeech("NN1", "singular common noun (e.g. book, girl)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NN2", new PartOfSpeech("NN2", "plural common noun (e.g. books, girls)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NNA", new PartOfSpeech("NNA", "following noun of title (e.g. M.A.)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NNB", new PartOfSpeech("NNB", "preceding noun of title (e.g. Mr., Prof.)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NNL1", new PartOfSpeech("NNL1", "singular locative noun (e.g. Island, Street)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NNL2", new PartOfSpeech("NNL2", "plural locative noun (e.g. Islands, Streets)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NNO", new PartOfSpeech("NNO", "numeral noun, neutral for number (e.g. dozen, hundred)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NNO2", new PartOfSpeech("NNO2", "numeral noun, plural (e.g. hundreds, thousands)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NNT1", new PartOfSpeech("NNT1", "temporal noun, singular (e.g. day, week, year)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NNT2", new PartOfSpeech("NNT2", "temporal noun, plural (e.g. days, weeks, years)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NNU", new PartOfSpeech("NNU", "unit of measurement, neutral for number (e.g. in, cc)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NNU1", new PartOfSpeech("NNU1", "singular unit of measurement (e.g. inch, centimetre)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NNU2", new PartOfSpeech("NNU2", "plural unit of measurement (e.g. ins., feet)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NP", new PartOfSpeech("NP", "proper noun, neutral for number (e.g. IBM, Andes)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NP1", new PartOfSpeech("NP1", "singular proper noun (e.g. London, Jane, Frederick)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NP2", new PartOfSpeech("NP2", "plural proper noun (e.g. Browns, Reagans, Koreas)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NPD1", new PartOfSpeech("NPD1", "singular weekday noun (e.g. Sunday)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NPD2", new PartOfSpeech("NPD2", "plural weekday noun (e.g. Sundays)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NPM1", new PartOfSpeech("NPM1", "singular month noun (e.g. October)", PoSCategory.Noun));
                AllPartsOfSpeech.Add("NPM2", new PartOfSpeech("NPM2", "plural month noun (e.g. Octobers)", PoSCategory.Noun));

                AllPartsOfSpeech.Add("PN", new PartOfSpeech("PN", "indefinite pronoun, neutral for number (none)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PN1", new PartOfSpeech("PN1", "indefinite pronoun, singular (e.g. anyone, everything, nobody, one)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PNQO", new PartOfSpeech("PNQO", "objective wh-pronoun (whom)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PNQS", new PartOfSpeech("PNQS", "subjective wh-pronoun (who)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PNQV", new PartOfSpeech("PNQV", "wh-ever pronoun (whoever)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PNX1", new PartOfSpeech("PNX1", "reflexive indefinite pronoun (oneself)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPGE", new PartOfSpeech("PPGE", "nominal possessive personal pronoun (e.g. mine, yours)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPH1", new PartOfSpeech("PPH1", "3rd person sing. neuter personal pronoun (it)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPHO1", new PartOfSpeech("PPHO1", "3rd person sing. objective personal pronoun (him, her)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPHO2", new PartOfSpeech("PPHO2", "3rd person plural objective personal pronoun (them)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPHS1", new PartOfSpeech("PPHS1", "3rd person sing. subjective personal pronoun (he, she)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPHS2", new PartOfSpeech("PPHS2", "3rd person plural subjective personal pronoun (they)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPIO1", new PartOfSpeech("PPIO1", "1st person sing. objective personal pronoun (me)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPIO2", new PartOfSpeech("PPIO2", "1st person plural objective personal pronoun (us)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPIS1", new PartOfSpeech("PPIS1", "1st person sing. subjective personal pronoun (I)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPIS2", new PartOfSpeech("PPIS2", "1st person plural subjective personal pronoun (we)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPX1", new PartOfSpeech("PPX1", "singular reflexive personal pronoun (e.g. yourself, itself)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPX2", new PartOfSpeech("PPX2", "plural reflexive personal pronoun (e.g. yourselves, themselves)", PoSCategory.Pronoun));
                AllPartsOfSpeech.Add("PPY", new PartOfSpeech("PPY", "2nd person personal pronoun (you)", PoSCategory.Pronoun));

                AllPartsOfSpeech.Add("RA", new PartOfSpeech("RA", "adverb, after nominal head (e.g. else, galore)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("REX", new PartOfSpeech("REX", "adverb introducing appositional constructions (namely, e.g.)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RG", new PartOfSpeech("RG", "degree adverb (very, so, too)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RGQ", new PartOfSpeech("RGQ", "wh- degree adverb (how)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RGQV", new PartOfSpeech("RGQV", "wh-ever degree adverb (however)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RGR", new PartOfSpeech("RGR", "comparative degree adverb (more, less)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RGT", new PartOfSpeech("RGT", "superlative degree adverb (most, least)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RL", new PartOfSpeech("RL", "locative adverb (e.g. alongside, forward)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RP", new PartOfSpeech("RP", "prep. adverb, particle (e.g about, in)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RPK", new PartOfSpeech("RPK", "prep. adv., catenative (about in be about to)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RR", new PartOfSpeech("RR", "general adverb", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RRQ", new PartOfSpeech("RRQ", "wh- general adverb (where, when, why, how)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RRQV", new PartOfSpeech("RRQV", "wh-ever general adverb (wherever, whenever)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RRR", new PartOfSpeech("RRR", "comparative general adverb (e.g. better, longer)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RRT", new PartOfSpeech("RRT", "superlative general adverb (e.g. best, longest)", PoSCategory.Adverb));
                AllPartsOfSpeech.Add("RT", new PartOfSpeech("RT", "quasi-nominal adverb of time (e.g. now, tomorrow)", PoSCategory.Adverb));

                AllPartsOfSpeech.Add("TO", new PartOfSpeech("TO", "infinitive marker (to)", PoSCategory.Misc));
                AllPartsOfSpeech.Add("UH", new PartOfSpeech("UH", "interjection (e.g. oh, yes, um)", PoSCategory.Misc));

                AllPartsOfSpeech.Add("VB0", new PartOfSpeech("VB0", "be, base form (finite i.e. imperative, subjunctive)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VBDR", new PartOfSpeech("VBDR", "were", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VBDZ", new PartOfSpeech("VBDZ", "was", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VBG", new PartOfSpeech("VBG", "being", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VBI", new PartOfSpeech("VBI", "be, infinitive (To be or not... It will be ..)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VBM", new PartOfSpeech("VBM", "am", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VBN", new PartOfSpeech("VBN", "been", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VBR", new PartOfSpeech("VBR", "are", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VBZ", new PartOfSpeech("VBZ", "is", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VD0", new PartOfSpeech("VD0", "do, base form (finite)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VDD", new PartOfSpeech("VDD", "did", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VDG", new PartOfSpeech("VDG", "doing", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VDI", new PartOfSpeech("VDI", "do, infinitive (I may do... To do...)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VDN", new PartOfSpeech("VDN", "done", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VDZ", new PartOfSpeech("VDZ", "does", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VH0", new PartOfSpeech("VH0", "have, base form (finite)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VHD", new PartOfSpeech("VHD", "had (past tense)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VHG", new PartOfSpeech("VHG", "having", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VHI", new PartOfSpeech("VHI", "have, infinitive", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VHN", new PartOfSpeech("VHN", "had (past participle)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VHZ", new PartOfSpeech("VHZ", "has", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VM", new PartOfSpeech("VM", "modal auxiliary (can, will, would, etc.)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VMK", new PartOfSpeech("VMK", "modal catenative (ought, used)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VV0", new PartOfSpeech("VV0", "base form of lexical verb (e.g. give, work)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VVD", new PartOfSpeech("VVD", "past tense of lexical verb (e.g. gave, worked)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VVG", new PartOfSpeech("VVG", "-ing participle of lexical verb (e.g. giving, working)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VVGK", new PartOfSpeech("VVGK", "-ing participle catenative (going in be going to)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VVI", new PartOfSpeech("VVI", "infinitive (e.g. to give... It will work...)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VVN", new PartOfSpeech("VVN", "past participle of lexical verb (e.g. given, worked)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VVNK", new PartOfSpeech("VVNK", "past participle catenative (e.g. bound in be bound to)", PoSCategory.Verb));
                AllPartsOfSpeech.Add("VVZ", new PartOfSpeech("VVZ", "-s form of lexical verb (e.g. gives, works)", PoSCategory.Verb));

                AllPartsOfSpeech.Add("XX", new PartOfSpeech("XX", "not, n't", PoSCategory.Misc));
                AllPartsOfSpeech.Add("ZZ1", new PartOfSpeech("ZZ1", "singular letter of the alphabet (e.g. A,b)", PoSCategory.Misc));
                AllPartsOfSpeech.Add("ZZ2", new PartOfSpeech("ZZ2", "plural letter of the alphabet (e.g. A's, b's)", PoSCategory.Misc));
            }
        }
    }
}