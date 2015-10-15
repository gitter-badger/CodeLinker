﻿using System;

namespace CodeRecycler
{
  static class Help
  {
    internal static string SourceCodeUrl = "https://github.com/CADbloke/CodeRecycler";
    public static void Write()
    {
      WriteLine("Recycles Source code between CSPROJ files");
      WriteLine("Usages...");
      WriteLine("CODECLONER /?");
      WriteLine("CODECLONER [Folder [/s]]");
      WriteLine("CODECLONER [Source.csproj Destination.csproj]");
      WriteLine("CODECLONER init [Source Solution Root] [Destination Solution Folder]");
      WriteLine();
      WriteLine("/?       This help text.");
      WriteLine();
      WriteLine("Folder   Recycles the source(s) into all CSPROJ files in the folder");
      WriteLine("         This is the destination folder that has recycled projects.");
      WriteLine("         The recycled projects need to have the source in their placeholder.");
      WriteLine("/s       Also iterates all subfolders. You just forgot this, right?");
      WriteLine();
      WriteLine("Source.csproj        Path to the CSPROJ with the source to be recycled.");
      WriteLine("Destination.csproj   Path to the existing Destination project.");
      WriteLine();
      WriteLine("init     Creates new recycled projects from all CSPROJ in the Solution folders");
      WriteLine("         Iterates all subfolders by default. Skips destination recycled projects.");
      WriteLine("Source Solution Root  (optional)");
      WriteLine("         The root of the solution containing the projects to recycle.");
      WriteLine("         Default is the current directory.");
      WriteLine("Destination Solution Folder  (optional)");
      WriteLine("         The new Folder Name for the recycled projects. Default is \"_Builds\".");
      WriteLine("         If only one Folder is specifed then it is the destination folder.");
      WriteLine();
      WriteLine("Wrap paths with spaces in double quotes.");
      WriteLine("Paths can (probably should!) be relative.");
      WriteLine("Source.csproj is optional,");
      WriteLine("in which case it needs to have the source project in the placeholder.");
      WriteLine("If you specify one CSPROJ file it is the destination,");
      WriteLine();
      WriteLine();
      WriteLine("The Destination CSPROJ file needs this XML comment placeholder...");
      WriteLine();
      WriteLine("<!-- CodeRecycler");
      WriteLine("Source: PathTo\\NameOfProject.csproj     <== this is optional");
      WriteLine("Exclude: PathTo\\FileToBeExcluded.cs     <== this is optional");
      WriteLine("-->");
      WriteLine();
      WriteLine("<!-- EndCodeRecycler -->");
      WriteLine();
      WriteLine("You may specify multiple Source: projects. No wildcards.");
      WriteLine("If you don't specify a source in the command call it better be here.");
      WriteLine("You may specify multiple Exclude: items, file or path. No wildcards.");
      WriteLine("Exclusions are a simple String.Contains() filter list.");
      WriteLine("If you specify multiple items then they must be on separate lines.");
      WriteLine("Every Code Recycle will re-recycle the source CSPROJ");
      WriteLine("into the space between the XML comment placeholders.");
      WriteLine("ALL code inside these placeholders is refreshed every time. OK?");
      WriteLine();
      WriteLine("More Info & Source at " + SourceCodeUrl);
      WriteLine("Code Recycler by CADbloke");
      WriteLine();
    }


    private static void WriteLine(string line = "")
    {
      Console.WriteLine(line);
      Log.WriteLine(line);
    }
  }
}