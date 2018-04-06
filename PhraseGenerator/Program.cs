using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhraseGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string phrase = PhraseGenerator.Instance.GeneratePhraseFromTemplateFile("HotelCalifornia.txt");
            Console.WriteLine(phrase);
        }
    }
}
