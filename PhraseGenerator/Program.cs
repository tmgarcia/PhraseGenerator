using System;
using System.IO;

namespace PhraseGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var programRunning = true;
            while (programRunning)
            {
                Console.WriteLine("> Enter template file name or X to exit.\n\t");
                var input = Console.ReadLine();
                if (input.ToLower() == "x")
                {
                    programRunning = false;
                    return;
                }

                var template = input;
                if (!template.StartsWith("Templates\\"))
                    template = "Templates\\" + template;
                if (!Path.HasExtension(template))
                    template = template + ".txt";


                if (!File.Exists(template))
                {
                    Console.WriteLine(string.Format("> Could not find the template specified: {0} does not exist.", template));
                    Console.WriteLine();
                    continue;
                }

                Console.WriteLine("\n> Generating text from template...");
                string phrase = PhraseGenerator.Instance.GeneratePhraseFromTemplateFile(template);
                Console.WriteLine("> Generation complete.\n");
                Console.WriteLine(phrase);
                Console.WriteLine();
            }
            
        }

    }
}
