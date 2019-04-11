using System;
using System.IO;
using System.Threading;
using Serilog;

namespace FileDropper
{
  public class ThingThatDropsTheFiles
  {
    Settings _settings;

    public ThingThatDropsTheFiles(Settings settings)
    {
      _settings = settings;
    }

    public void DropThemFilesLikeTheyreHawt()
    {
      Random rng = new Random();
      var filesWritten = 0;

      for (var i = 0; i < _settings.NumberOfFilesToCreate; i++)
      {
        var lCode = OmgSoRandom.GetRandomLCode(rng); // get random LCode (L001 - L499)
        var randomFileName = OmgSoRandom.GetRandomFilename(rng, _settings.FileNameBase, lCode); // randomly generate filename formatted $"{settings.FileNameBase}_{some RNG}_{lcode}.txt"
        var randomBytes = OmgSoRandom.GetRandomByteArray(rng, _settings.MaxFileSizeInKb); // creates a byte array of a random size ranging from 1KB - MaxFileSizeInKb
        var dropOffDirectory = Path.Combine(_settings.OutputPath, string.Format(_settings.SubdirectoryPathTemplate, lCode)); // drop off is an LCode subdirectory
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
        Thread.Sleep(_settings.SleepTimeBetweenDrops); // configurable sleep time for testing different rates of drop off
      }

      Log.Information(CompletionMessage(filesWritten, _settings.NumberOfFilesToCreate));
      Console.WriteLine($" {CompletionMessage(filesWritten, _settings.NumberOfFilesToCreate)}");

      if (filesWritten != _settings.NumberOfFilesToCreate)
      {
        Log.Information(NotAllFilesWrittenMessage);
        Console.WriteLine($" {NotAllFilesWrittenMessage}");
      }

      Console.WriteLine("\n Press 'q' to close application.\n");
      while (Console.Read() != 'q') ;
    }

    private string NotAllFilesWrittenMessage => "Not all files created successfully, please check log for file errors.";

    private string CompletionMessage(int filesWritten, int fileRequested) => $"File creation complete, {filesWritten} of {fileRequested} files were created.";
  }
}
