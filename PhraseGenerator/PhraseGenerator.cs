using System.Collections;
using PhraseGenerator.PhraseGeneration;
using System.Collections.Generic;
using System.IO;
using PhraseGenerator.Utils;
using System.Linq;

namespace PhraseGenerator
{
    public class PhraseGenerator
    {
        private string BigramDictionaryBinaryFilename = "bigramBinary.bytes";
        private string WordDictionaryBinaryFilename = "wordBinary.bytes";
        List<PhraseGenerationJob> jobs;
        public int NumJobs
        {
            get
            {
                return jobs.Count;
            }
        }

        void Update()
        {
            if (jobs != null && jobs.Count > 0)
            {
                List<PhraseGenerationJob> finishedJobs = new List<PhraseGenerationJob>();
                foreach (PhraseGenerationJob job in jobs)
                {
                    if (job.Update())
                    {
                        finishedJobs.Add(job);
                    }
                }
                foreach (PhraseGenerationJob job in finishedJobs)
                {
                    jobs.Remove(job);
                }
            }
        }

        /// <summary>
        /// Generates a phrase from a template synchroniously. Use of this method is discouraged as it will cause the game to wait for it to finish and, depending on the length of the template,
        /// this can cause a noticable pause. Instead, use GeneratePhraseAsync if possible.
        /// </summary>
        /// <param name="filename">The TextAsset containing the phrase template the new phrase will be generated from.</param>
        /// <param name="seed">(Optional) A seed to use in generation. If no seed is passed, a new one will be generated based on system time.</param>
        /// <returns>The newly generated phrase</returns>
        public string GeneratePhraseFromTemplateFile(string filename, int? seed = null)
        {
            return GeneratePhraseFromTemplateString(FileHelper.FileToString(filename));
        }

        /// <summary>
        /// Generates a phrase from a template synchroniously. Use of this method is discouraged as it will cause the game to wait for it to finish and, depending on the length of the template,
        /// this can cause a noticable pause. Instead, use GeneratePhraseAsync if possible.
        /// </summary>
        /// <param name="template">The string value of the phrase template the new phrase will be generated from.</param>
        /// <param name="seed">(Optional) A seed to use in generation. If no seed is passed, a new one will be generated based on system time.</param>
        /// <returns>The newly generated phrase</returns>
        public string GeneratePhraseFromTemplateString(string template, int? seed = null)
        {
            TemplateCluster cluster = TemplateParser.ParseTemplate(template);
            int _seed = seed.HasValue ? seed.Value : System.Environment.TickCount;
            string phrase = cluster.Populate(_seed,true);
            return phrase;
        }

        /// <summary>
        /// Generates a phrase on a separate thread and sends the generated phrase and the seed used to generate it to a callback once complete.
        /// </summary>
        /// <param name="filename">A TextAsset containing the template phrase for the newly generated phrase.</param>
        /// <param name="callback">A callback to which the string phrase and int seed will be passed.</param>
        /// <param name="seed">(Optional) The seed that will be used to generate the phrase.
        /// <para>Use this if you are wanting to recieve a phrase generated previously.</para>
        /// <para>If no seed is passed, a new one will be created and that int is what will be passed to the callback. Otherwise, the seed passed in is the one that will go to the callback.</para></param>
        /// <returns>The Id of the PhraseGenerationJob (thread) generating the phrase, can be passed to CancelGeneration to abort the thread later.</returns>
        public int GeneratePhraseAsyncFromTemplateFile(string filename, System.Action<string, int> callback, int? seed = null)
        {
            return GeneratePhraseAsyncFromTemplateString(FileHelper.FileToString(filename), callback, seed);
        }
        /// <summary>
        /// Generates a phrase on a separate thread and sends the generated phrase and the seed used to generate it to a callback once complete.
        /// </summary>
        /// <param name="text">A TextAsset containing the template phrase for the newly generated phrase.</param>
        /// <param name="callback">A callback to which the string phrase and int seed will be passed.</param>
        /// <param name="seed">(Optional) The seed that will be used to generate the phrase.
        /// <para>Use this if you are wanting to recieve a phrase generated previously.</para>
        /// <para>If no seed is passed, a new one will be created and that int is what will be passed to the callback. Otherwise, the seed passed in is the one that will go to the callback.</para></param>
        /// <returns>The Id of the PhraseGenerationJob (thread) generating the phrase, can be passed to CancelGeneration to abort the thread later.</returns>
        public int GeneratePhraseAsyncFromTemplateString(string template, System.Action<string, int> callback, int? seed = null)
        {
            PhraseGenerationJob generationJob = new PhraseGenerationJob();
            TemplateCluster cluster = TemplateParser.ParseTemplate(template);
            jobs.Add(generationJob);
            int _seed = seed.HasValue ? seed.Value : System.Environment.TickCount;
            generationJob.Start(_seed, cluster, callback);
            return generationJob.Id;
        }

        /// <summary>
        /// Cancels a Phrase Generation Job thread. If the job isn't canceled before it is completed, the callback that was passed when the job was started may still get invoked.
        /// </summary>
        /// <param name="phraseGenerationJobId">The job id of the job that is being canceled. The id is the number that was returned from calling GeneratePhraseAsync</param>
        public void CancelPhraseGeneration(int phraseGenerationJobId)
        {
            PhraseGenerationJob jobToAbort = jobs.FirstOrDefault(j => j.Id == phraseGenerationJobId);
            if (jobToAbort != null)
            {
                jobToAbort.Abort();
                jobs.Remove(jobToAbort);
            }
        }

        private static PhraseGenerator _instance = null;
        public static PhraseGenerator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PhraseGenerator();
                    BigramProcessor processor = new BigramProcessor();
                    processor.LoadInFromBinary(_instance.BigramDictionaryBinaryFilename, _instance.WordDictionaryBinaryFilename);
                    _instance.jobs = new List<PhraseGenerationJob>();
                }
                return _instance;
            }
        }
        private PhraseGenerator() { }
    }

    
}