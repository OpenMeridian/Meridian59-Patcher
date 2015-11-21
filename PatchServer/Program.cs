using System;
using System.IO;

namespace PatchListGenerator
{
    class PatchListGenerator
    {
        static string _jsonOutputPath;
        static string _clientPath;

        static void Main(string[] args)
        {
            /*
             * --client=path - Base folder to scan for patch info
             * --outfile=path - File to store patch info in
             * --type=[classic|DotNetX86|DotNetX64]
             */

            if (args.Length < 2)
            {
                DisplaySyntax();
                return;
            }

            char[] delim = { '=' };
            ClientScanner clientscanner = new ClientScanner();
            foreach (string argument in args)
            {
                string[] param = argument.Split(delim);
                switch (param[0])
                {
                    case "--client":
                        _clientPath = param[1];
                        break;
                    case "--outfile":
                        _jsonOutputPath = param[1];
                        break;
                    case "--type":
                        switch (param[1].ToLower())
                        {
                            case "classic":
                                clientscanner = new ClassicClientScanner(_clientPath);
                                break;
                            case "dotnet":
                                clientscanner = new OgreClientScanner(_clientPath);
                                break;
                            default:
                                DisplaySyntax();
                                return;
                        }
                        break;
                    default:
                        if (param.Length > 0) Console.WriteLine("Parameter: '{0}' not known.", param[0]);
                        return;
                }
            }

            if ((_clientPath == null) || (_jsonOutputPath == null))
            {
                Console.WriteLine("Missing Parameter");
                Console.WriteLine("--client=[path] - Base folder to scan for patch info");
                Console.WriteLine("--outfile=[path] - File to store patch info in");
                return;
            }

            Console.WriteLine("Scan Folder: {0}", _clientPath);
            Console.WriteLine("Output File: {0}", _jsonOutputPath);

            Console.WriteLine("Scanning...");
            //Creates list of latest file hashes
            clientscanner.ScannerSetup(_clientPath);
            clientscanner.ScanSource();
            Console.WriteLine("Scanned {0} Files", clientscanner.Files.Count);

            using (var sw = new StreamWriter(_jsonOutputPath))
            {
                sw.Write(clientscanner.ToJson());
            }

            Console.WriteLine("File Written! Goodbye!");
        }

        static void DisplaySyntax()
        {
            Console.WriteLine("Not enough parameters");
            Console.WriteLine("--client=[path] - Base folder to scan for patch info");
            Console.WriteLine("--outfile=[path] - File to store patch info in");
            Console.WriteLine("--type=[classic|dotnet]");
        }
    }
}
