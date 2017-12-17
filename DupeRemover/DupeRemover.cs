// -----------------------------------------------------------------------------
// DupeRemover - An utility for finding and removing duplicate files            
//                                                                              
// Repo: https://github.com/TonyWu386/DupeRemover                               
//                                                                              
// Dependencies: .NET framework 4.0                                             
//                                                                              
// License: GNU GPL v3.0                                                        
//                                                                              
// Copyright (c) 2016 [Tony Wu], All Right Reserved                             
// -----------------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace DupeRemover
{
	class MainClass
	{
		static int Main (string[] args)
		{
			string hash;
			string path;
			char userIn = '\0';
			int dupCount = 0;
			int delCount = 0;
			bool auto;
			bool newer = false;
			bool ageflag = false;
			if (args.Length == 0)
			{
				Console.Write ("flag required\n");
				PrintUsage ();
				return 1;
			}
			else if (args.Length > 2)
			{
				Console.Write ("too many args\n");
				PrintUsage ();
				return 1;
			}
			else
			{
				switch (args [0].ToLower ())
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
					PrintHelp ();
					return 0;
				default:
					Console.Write ("invalid flag\n");
					PrintUsage ();
					return 1;
				}
				if (args.Length == 2)
				{
					path = args [1];
				}
				else
				{
					path = "./";
				}
			}
			try
			{
				string[] files = Directory.GetFiles (path);
				string[] hashes = new String[files.Length];
				if (!auto)
				{
					Console.Write ("keep 1st: z, keep 2nd: x, skip: any other key\n");
				}
				for (int i = 0; i < files.Length; i++)
				{
					hash = GetSHA1 (files [i]);
					if (hashes.Contains (hash) &&
					    (GetSize (files [i]) == GetSize (files [Array.IndexOf (hashes, hash)])))
					{
						if (!auto)
						{
							Console.Write ("{0} {1} {2} {3}bytes\n>>>", files [i],
								files [Array.IndexOf (hashes, hash)], hash, GetSize (files [i]));
							userIn = (char)Console.Read ();
							Console.Write ("\n");
						}
						dupCount++;
						if (auto)
						{
							ageflag = FirstNewer (files [i], files [Array.IndexOf (hashes, hash)]);
							if (newer)
							{
								ageflag = !ageflag;
							}
						}
						if (userIn == 'z' || (auto && !ageflag))
						{
							File.Delete (files [Array.IndexOf (hashes, hash)]);
							hashes [i] = hash;
							hashes [Array.IndexOf (hashes, hash)] = "\0";
							delCount++;
						}
						else if (userIn == 'x' || (auto && ageflag))
						{
							File.Delete (files [i]);
							delCount++;
						}
					}
					else
					{
						hashes [i] = hash;
					}
				}
				Console.Write ("{0} processed, {1} deleted\n", dupCount, delCount);
				return 0;
			}
			catch (DirectoryNotFoundException)
			{
				Console.Write ("invalid directory\n");
				return 1;
			}
		}

		static string GetSHA1 (string fName)
		{
			FileStream F = new FileStream (fName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider ();
			byte[] data = hasher.ComputeHash (F);
			StringBuilder sBuilder = new StringBuilder ();
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append (data [i].ToString ("x2"));
			}
			F.Close ();
			return sBuilder.ToString ();
		}

		static long GetSize (string fName)
		{
			FileStream F = new FileStream (fName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			long size = F.Length;
			F.Close ();
			return size;
		}

		static void PrintHelp ()
		{
			Console.Write ("\nDupeRemover - an utility for finding and removing duplicate files\n");
			Console.Write ("usage: DupeRemover.exe -man|-auto|-autonew|-help [directory]\n");
			Console.Write ("duplicate files are found by cross-referencing SHA1 hashes with file sizes\n");
			Console.Write ("two manners of removal are offered:\n");
			Console.Write ("-man, manual mode: user confirmation for all deletions, ability to pick file name\n");
			Console.Write ("-auto, automatic mode: no user intervention, keeps older file\n");
			Console.Write ("-autonew, automatic mode: no user intervention, keeps newer file\n");
			Console.Write ("directory: directory (string representation) may be provided\n");
			Console.Write ("if it is not passed, the directory of the executable will be used\n\n");
		}

		static void PrintUsage ()
		{
			Console.Write ("usage: DupeRemover.exe -man|-auto|-autonew|-help [directory]\n");
		}

		static bool FirstNewer (string fName1, string fName2)
		{
			DateTime mod1 = File.GetLastWriteTime (fName1);
			DateTime mod2 = File.GetLastWriteTime (fName2);
			int result = DateTime.Compare (mod1, mod2);
			return !(result < 0);
		}

	}
}
