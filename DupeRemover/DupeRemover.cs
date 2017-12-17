// -----------------------------------------------------------------------------
// DupeRemover - An utility for finding and removing duplicate files            
//                                                                              
// Repo: https://github.com/TonyWu386/DupeRemover                               
//                                                                              
// Dependencies: .NET framework 4.0                                             
//                                                                              
// License: GNU GPL v3.0                                                        
//                                                                              
// Copyright (c) 2016-2017 [Tony Wu], All Right Reserved                           
// -----------------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace DupeRemover
{
  /// <summary>
  /// Main class containing the command-line controller.
  /// </summary>
  class MainClass
  {
    static int Main(string[] args)
    {
      string path;
      char userIn = '\0';
      int dupCount = 0;
      int delCount = 0;
      bool auto;
      bool newer = false;
      bool ageflag = false;

      // command-line processing
      if (args.Length == 0)
      {
        Console.Write("flag required\n");
        PrintUsage();
        return 1;
      }
      if (args.Length > 2)
      {
        Console.Write("too many args\n");
        PrintUsage();
        return 1;
      }

      switch (args[0].ToLower())
      {
        case "-man":
          auto = false;
          break;
        case "-auto":
          auto = true;
          break;
        case "-autonew":
          auto = true;
          newer = true;
          break;
        case "-help":
          PrintHelp();
          return 0;
        default:
          Console.Write("invalid flag\n");
          PrintUsage();
          return 1;
      }
      if (args.Length == 2)
      {
        path = args[1];
      }
      else
      {
        path = "./";
      }

      // logic processing
      string[] fileNames;
      try
      {
        fileNames = Directory.GetFiles(path);
      }
      catch (DirectoryNotFoundException)
      {
        Console.Write("invalid directory\n");
        return 1;
      }

      Console.WriteLine(fileNames.Count());

      List<FileObject> objectsToProcess = new List<FileObject>();
      if (!auto)
      {
        Console.Write("keep 1st: z, keep 2nd: x, skip: any other key\n");
      }
      for (int i = 0; i < fileNames.Length; i++)
      {
        FileObject newObject = new FileObject(path, fileNames[i]);

        if (objectsToProcess.Where(o => o.hash.SequenceEqual(newObject.hash)
                                   && o.size == newObject.size
                                   && o.StillExists()).Count() > 0)
        {
          FileObject existingObject = objectsToProcess.First(o => o.hash.SequenceEqual(newObject.hash)
                                                             && o.size == newObject.size
                                                             && o.StillExists());
          if (auto)
          {
            ageflag = FirstNewer(newObject, existingObject);
            if (newer)
            {
              ageflag = !ageflag;
            }
          }
          else
          {
            Console.Write("{0} {1} {2} {3}bytes\n>>>", newObject.name,
                          existingObject.name, BytesToString(newObject.hash), newObject.size);
            userIn = (char)Console.Read();
            Console.Write("\n");
          }
          dupCount++;

          if (userIn == 'z' || (auto && !ageflag))
          {
            existingObject.DeleteFile();
            objectsToProcess.Add(newObject);
            delCount++;
          }
          else if (userIn == 'x' || (auto && ageflag))
          {
            newObject.DeleteFile();
            delCount++;
          }
        }
        else
        {
          objectsToProcess.Add(newObject);
        }
      }
      Console.Write("{0} processed, {1} deleted\n", dupCount, delCount);
      return 0;
    }


    /// <summary>
    /// Converts a byte array into a human readable format.
    /// Used to view hashes.
    /// </summary>
    /// <returns>String representation of the byte.</returns>
    /// <param name="data">A byte array.</param>
    static string BytesToString(byte[] data)
    {
      StringBuilder sBuilder = new StringBuilder();
      for (int i = 0; i < data.Length; i++)
      {
        sBuilder.Append(data[i].ToString("x2"));
      }
      return sBuilder.ToString();
    }


    /// <summary>
    /// Prints information about the tool.
    /// </summary>
    static void PrintHelp()
    {
      Console.Write("\nDupeRemover - an utility for finding and removing duplicate files\n");
      Console.Write("usage: DupeRemover.exe -man|-auto|-autonew|-help [directory]\n");
      Console.Write("duplicate files are found by cross-referencing SHA1 hashes with file sizes\n");
      Console.Write("two manners of removal are offered:\n");
      Console.Write("-man, manual mode: user confirmation for all deletions, ability to pick file name\n");
      Console.Write("-auto, automatic mode: no user intervention, keeps older file\n");
      Console.Write("-autonew, automatic mode: no user intervention, keeps newer file\n");
      Console.Write("directory: directory (string representation) may be provided\n");
      Console.Write("if it is not passed, the directory of the executable will be used\n\n");
    }


    /// <summary>
    /// Prints the usage string.
    /// </summary>
    static void PrintUsage()
    {
      Console.Write("usage: DupeRemover.exe -man|-auto|-autonew|-help [directory]\n");
    }


    /// <summary>
    /// Finds out which FileObject is newer.
    /// </summary>
    /// <returns>True if the first FileObject is newer, else False.</returns>
    static bool FirstNewer(FileObject fileObject1, FileObject fileObject2)
    {
      DateTime mod1 = File.GetLastWriteTime(fileObject1.name);
      DateTime mod2 = File.GetLastWriteTime(fileObject2.name);
      int result = DateTime.Compare(mod1, mod2);
      return !(result < 0);
    }
  }
}
