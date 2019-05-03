using System;
using System.Collections.Generic;
using System.Text;

namespace FileDropper
{
  public static class OmgSoRandom
  {
    public static string GetRandomFilename(Random rng, string fileNameBase, string lCode) => $"{fileNameBase}_{rng.Next()}_{lCode}.txt";

    public static string GetRandomLCode(Random rng) => $"L{rng.Next(1, 500).ToString("D3")}";

    public static byte[] GetRandomByteArray(Random rng, int maxFileSizeInKb)
    {
      var randomlySizedByteArray = new byte[GetRandomFileSize(rng, maxFileSizeInKb)];
      rng.NextBytes(randomlySizedByteArray);
      return randomlySizedByteArray;
    }

    private static int GetRandomFileSize(Random rng, int maxFileSizeInKb) => rng.Next(1, maxFileSizeInKb + 1) * 1024;
  }
}
