﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveWordFinder.WordProcessing.Strategies;

namespace FiveWordFinderConsole.Services
{
    internal static class ConsoleInputService
    {
        public static string GetWordSourceFileName(string defaultFile)
        {
            Console.WriteLine($"Enter word source file path or enter to use default: {defaultFile}");
            bool fileOkay = false;
            string fileName = string.Empty;
            do
            {
                string input = Console.ReadLine() ?? String.Empty;

                try
                {
                    if (string.IsNullOrWhiteSpace(input))
                        input = defaultFile;

                    if (!File.Exists(input))
                        throw new FileNotFoundException($"File {input} does not exist. Please try again.");

                    fileName = input;
                    fileOkay = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            } while (!fileOkay);

            return fileName;
        }

        public static IGraphEvaluationStrategy GetEvaluationStrategy()
        {
            Console.WriteLine("How should the word graph be evaluated?");
            Console.WriteLine("1 = Find Loop In Series");
            Console.WriteLine("2 = Find Parallel ForEach - (Default)");
            bool inputValid = false;
            int selection = 2;
            do
            {
                string input = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(input))
                    input = "2";

                if (!(inputValid = int.TryParse(input, out selection)))
                {
                    Console.WriteLine($"Could not parse input value: {input} Please try again.");
                }
            } while (!inputValid);

            IGraphEvaluationStrategy strategy = selection switch
            {
                2 => new EvaluateGraphParallel(),
                _ => new EvaluateGraphSeries(),
            };

            return strategy;
        }

        public static bool GetShouldWriteToScreen()
        {
            Console.WriteLine("Write found cliques to screen? (Y / N)");
            Console.WriteLine("  Select this option to display the list to the console window as well as writing to file.");

            bool result = false;
            bool inputValid = false;
            while (!inputValid)
            {
                var input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    inputValid = true;
                    result = input.ToUpper().First() == 'Y';
                }    
            }

            return result;
        }
    }
}
