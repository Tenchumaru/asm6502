using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asm6502
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
                Usage();
            using(TextReader reader = new StreamReader(args[0]))
            {
                Parser parser = new Parser(new Scanner(reader));
                if(parser.Parse())
                {
                    int startAddress;
                    byte[] span = parser.GetMinimalFullSpan(out startAddress);
                    if(args.Length > 1)
                    {
                        using(FileStream stream = new FileStream(args[1], FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            Console.WriteLine("Load program at {0}.", startAddress);
                            Console.WriteLine("Entry points:");
                            foreach(KeyValuePair<string, int> kvp in parser.EntryPoints)
                                Console.WriteLine("\t{0}: {1}", kvp.Key, kvp.Value);
                            foreach(byte b in span)
                                stream.WriteByte(b);
                        }
                    }
                    else
                    {
                        Console.WriteLine("{0}:", startAddress);
                        foreach(byte b in span)
                            Console.WriteLine(b);
                    }
                }
            }
        }

        private static void Usage()
        {
            string programName = typeof(Program).Assembly.ManifestModule.Name;
            Console.Error.WriteLine("usage: {0} inputFile [outputFile]", programName);
            Environment.Exit(2);
        }
    }
}
