using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace FileDropper
{
  class Program
  {
    static void Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration().WriteTo.File($"{Environment.UserName}.{typeof(Program).Assembly.GetName().Name}.log").CreateLogger();
      Settings settings = GetSettings();
      Log.Information($"Settings on launch: {Newtonsoft.Json.JsonConvert.SerializeObject(settings)}");

      DropRandomFiles(settings);
    }

    private static void DropRandomFiles(Settings settings)
    {
      Random rng = new Random();
      var filesWritten = 0;

      for (var i = 0; i < settings.NumberOfFilesToCreate; i++)
      {
        var lCode = RandomLCode(rng); // get random LCode (L001 - L499)
        var randomFileName = GetNewFileName(rng, settings.FileNameBase, lCode); // randomly generate filename formatted $"{settings.FileNameBase}_{some RNG}_{lcode}.txt"
        var randomBytes = GetRandomByteArray(rng, settings.MaxFileSizeInKb); // creates a byte array of a random size ranging from 1KB - MaxFileSizeInKb
        var dropOffDirectory = Path.Combine(settings.OutputPath, string.Format(settings.SubdirectoryPathTemplate, lCode)); // drop off is an LCode subdirectory
        var fullFileName = Path.Combine(dropOffDirectory, randomFileName); 

        Directory.CreateDirectory(dropOffDirectory);

        try
        {
          using (var fs = new FileStream(fullFileName, FileMode.Create, FileAccess.Write, FileShare.None))
          {
            fs.Write(randomBytes, 0, randomBytes.Length);
          }
          filesWritten++;
          Log.Information($"Created file: {fullFileName}");
        }
        catch (Exception ex)
        {
          Log.Error(ex, "Error writing file.");
        }
        Thread.Sleep(settings.SleepTimeBetweenDrops); // configurable sleep time for testing different rates of drop off
      }

      bool allFilesCreated = filesWritten == settings.NumberOfFilesToCreate;
      Log.Information($"File creation complete, {filesWritten} of {settings.NumberOfFilesToCreate} files were created successfully.");
      if (!allFilesCreated) Log.Information($"Not all files created successfully, please check log for file errors.");
    }

    private static Settings GetSettings()
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

      IConfigurationRoot configuration = builder.Build();
      return configuration.GetSection(nameof(Settings)).Get<Settings>();
    }

    private class Settings
    {
      public string OutputPath { get; set;}
      public string SubdirectoryPathTemplate { get; set; }
      public string FileNameBase { get; set;}
      public int SleepTimeBetweenDrops { get; set;}
      public int NumberOfFilesToCreate{ get; set;}
      public int MaxFileSizeInKb { get; set; }
    }

    private static byte[] GetRandomByteArray(Random rng, int maxFileSizeInKb)
    {
      var randomlySizedByteArray = new byte[GetRandomFileSize(rng, maxFileSizeInKb)];
      rng.NextBytes(randomlySizedByteArray);
      return randomlySizedByteArray;
    }

    private static int GetRandomFileSize(Random rng, int maxFileSizeInKb) => rng.Next(1, maxFileSizeInKb + 1) * 1024;

    private static string GetNewFileName(Random rng, string fileNameBase, string lCode) => $"{fileNameBase}_{rng.Next()}_{lCode}.txt";

    private static string RandomLCode(Random rng) => $"L{rng.Next(1, 500).ToString("D3")}";
  }
}
