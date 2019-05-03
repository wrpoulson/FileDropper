using System;
using System.Collections.Generic;
using System.Text;

namespace FileDropper
{
  public class Settings
  {
    public string OutputPath { get; set; }
    public string SubdirectoryPathTemplate { get; set; }
    public string FileNameBase { get; set; }
    public int SleepTimeBetweenDrops { get; set; }
    public int NumberOfFilesToCreate { get; set; }
    public int MaxFileSizeInKb { get; set; }
  }
}
