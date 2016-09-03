# DupeRemover

An utility for finding and removing duplicate files implemented in C#

Copyright (c) 2016 [Tony Wu], All Right Reserved

This software is licensed under the GNU GPL v3.0

DupeRemover is a lightweight utility for finding and removing duplicate files. It verifies duplicate files by cross-referencing SHA1 hashes and file sizes, resulting in an extremely low likelyhood of error. It includes both manual and automatic removal modes. Automatic mode can be set to keep the newer or the older copy of a duplicate pair.

usage: DupeRemover.exe -man|-auto|-autonew|-help [directory]

This software was developed and tested under a Linux environment through Mono with .NET Framework 4.4.2.

Due to the use of System.Linq, versions of .NET older than 3.5 will not work.

Depending on the sizes of files to be analysed, the amount of time needed for execution to complete can vary greatly.
