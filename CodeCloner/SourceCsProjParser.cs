﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace CodeCloner
{
  internal class SourceCsProjParser
  {
    private static XNamespace MSBuild = "http://schemas.microsoft.com/developer/msbuild/2003";

    /// <summary> Gets the full pathname of the source create structure project file. </summary>
    internal string SourceCsProjPath { get; }

    internal List<XElement> ItemGroups { get; }


    internal SourceCsProjParser(string sourceCsProjAbsolutePath)
    {
      SourceCsProjPath = sourceCsProjAbsolutePath;
      if (!File.Exists(sourceCsProjAbsolutePath)) { Program.Crash("ERROR: " + sourceCsProjAbsolutePath + "  does not exist."); }
      if (!sourceCsProjAbsolutePath.IsaCsOrVbProjFile())
        Program.Crash("ERROR: " + sourceCsProjAbsolutePath + "  is not a CSPROJ.");

      try
      {
        XDocument csProjXml = XDocument.Load(sourceCsProjAbsolutePath);
        ItemGroups = new List<XElement>();

        //IEnumerable<XElement> itemGroups = from element in csProjXml.Root.Elements().DescendantsAndSelf()
        //                                   where element.Name.LocalName == "Itemgroup" // .Attribute("name").Value
        //                                   select element;

        XElement xElement = csProjXml
          .Element(MSBuild + "Project");
        if (xElement != null)
        {
            IEnumerable<XElement> itemGroups = xElement
              .Elements(MSBuild + "ItemGroup")
              .Select(elements => elements);

            ItemGroups.AddRange(itemGroups);
          }

        if (ItemGroups.Count == 0) { Log.WriteLine("Curious: " + SourceCsProjPath + " contains no ItemGroups. No Codez?"); }
      }
      catch (Exception e) { Program.Crash(e, "source CSPROJ: "+ sourceCsProjAbsolutePath); }
    }
  }
}