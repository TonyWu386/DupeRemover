// -----------------------------------------------------------------------------        
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
using System.Security.Cryptography;

namespace DupeRemover
{
  /// <summary>
  /// A data object that represents a file.
  /// </summary>
  class FileObject
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:DupeRemover.FileObject"/> class.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <param name="name">The file name.</param>
    public FileObject(string path, string name)
    {
      this.path = path;
      this.name = name;
      deleted = false;
      size = GetSize(name);
      hash = GetSHA1(name);
    }

    /// <summary>
    /// Deletes the file represented by this FileObject.
    /// Throws an Exception if the file has already been deleted.
    /// </summary>
    public void DeleteFile()
    {
      if (deleted)
      {
        throw new Exception("Attempted to delete an already deleted file");
      }
      File.Delete(name);
      deleted = true;
    }

    /// <summary>
    /// Returns if the file still exists.
    /// </summary>
    /// <returns>Return true if the file still exists. Else, return false</returns>
    public bool StillExists()
    {
      return !deleted;
    }

    /// <summary>
    /// Returns the file size.
    /// </summary>
    /// <returns>File size.</returns>
    /// <param name="fName">File name.</param>
    long GetSize(string fName)
    {
      FileStream F = new FileStream(fName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
      long fileSize = F.Length;
      F.Close();
      return fileSize;
    }

    /// <summary>
    /// Returns the SHA1 hash.
    /// </summary>
    /// <returns>The SHA1 hash.</returns>
    /// <param name="fName">File name.</param>
    byte[] GetSHA1(string fName)
    {
      FileStream F = new FileStream(fName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
      SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
      byte[] result = hasher.ComputeHash(F);
      F.Close();
      return result;
    }

    bool deleted;

    public string path;
    public string name;
    public long size;
    public byte[] hash;
  }
}
