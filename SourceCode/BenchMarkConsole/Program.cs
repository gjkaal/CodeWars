using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace BenchMarkConsole
{
    internal class Program
    {
        private static string[] _args = Array.Empty<string>();

        private static void Main(string[] args)
        {
            _args = args;
            if (_args.Length != 0) { 
                // do something with arguments
            }

            _ = BenchmarkRunner.Run<StringReplaceBenchmarks>();
        }
    }
}