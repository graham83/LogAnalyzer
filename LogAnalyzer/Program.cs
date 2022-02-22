using LogAnalyzer.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using LogAnalyzer.Helpers;

string TEMPDBFILENAME = "logging.db";
string DEFAULT_LOG_FILE = "programming-task-example-data.log";

var logFilePath = "";

// Program accepts command line argument for log path to analyze
// TO DO: Test this
if (Environment.GetCommandLineArgs().Length > 1)
{
    logFilePath = Environment.GetCommandLineArgs()[1];
}

// Create new context using sqlite DB.
// Log file could be large so program writes line by line to the DB before analyzing.
var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
var dbPath = System.IO.Path.Join(path, TEMPDBFILENAME);

var optionsBuilder = new DbContextOptionsBuilder<LogDbContext>();
optionsBuilder.UseSqlite($"Data Source={dbPath}");

// Reset local DB for each analyze log file
var context = new LogDbContext(optionsBuilder.Options);
context.Database.EnsureDeleted();
context.Database.EnsureCreated();

using (var repository = new LogRepository(context))
{
    var fileHelper = new FileHelper(repository);

    // If no command line argument then get user input for file or use default file.
    if (logFilePath == "" || !fileHelper.CheckFileExists(logFilePath))
    {
        Console.WriteLine("Enter filename or path or PRESS ENTER to use default log file.");
        logFilePath = Console.ReadLine();
        if (string.IsNullOrEmpty(logFilePath)) logFilePath = DEFAULT_LOG_FILE;
    }

    if (!fileHelper.CheckFileExists(logFilePath))
    {
        Console.WriteLine($"File {logFilePath} could not be found. Press return to exit.");
        Console.ReadLine();
        return;
    }

    // Do initial sanity check on provide log file

    using StreamReader reader = new StreamReader(logFilePath);
    var firstLine = reader.ReadLine() ?? "";

    if (!fileHelper.CheckFileLineFormatOk(firstLine))
    {
        Console.WriteLine("Log file in unexpected format. Press any key to exit");
        Console.ReadLine();
        return;
    }

    // Write log file to local db and then analyze using filehelper if records successfully written

    if (fileHelper.WriteLogFileToRepository(logFilePath) > 0)
    {
        Console.WriteLine($"Number of Unique IP Addresses: {fileHelper.GetNumberUniqueUrls()}");

        Console.WriteLine();
        Console.WriteLine();

        Console.WriteLine($"Top 3 most visited Urls:");

        foreach (var url in fileHelper.GetTop3Visited())
        {
            Console.WriteLine(url);
        }

        Console.WriteLine();
        Console.WriteLine();

        Console.WriteLine($"Top 3 most active IP Addresses:");

        foreach (var ip in fileHelper.GetTop3ActiveIps())
        {
            Console.WriteLine(ip);
        }
    }

    Console.WriteLine();
    Console.WriteLine();

    // Print bad lines to console for debugging
    if (fileHelper.BadLines.Count > 0)
    {
        Console.WriteLine("***** UNEXPECTED DATA *****");

        foreach (var line in fileHelper.BadLines)
        {
            Console.WriteLine(line);
        }
    }
}

Console.WriteLine("Press return to exit.");

Console.ReadLine();