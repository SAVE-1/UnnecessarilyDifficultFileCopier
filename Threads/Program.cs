// See https://aka.ms/new-console-template for more information
using Threads;

#if DEV
string source = @"";
string target = @"";
#else

Console.WriteLine(args);

if(args.Length != 2)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("copier 'source' 'target'");
    Console.WriteLine("arg 1. = source folder");
    Console.WriteLine("arg 2. = target folder");
}

string source = args[0];
string target = args[1];

#endif

FileCopier copier = new FileCopier(source, target);

copier.Start();
