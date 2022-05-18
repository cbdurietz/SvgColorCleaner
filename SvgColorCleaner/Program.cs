using HtmlAgilityPack;

namespace SvgColorCleaner {
  internal class Program {
    private static string targetFolderPath { get; set; }


    static void Main(string[] args) {
      Console.WriteLine("Hello, World!");

      var sourceFolderPath = @"C:\testfiles\source";
      targetFolderPath = @"C:\testfiles\target";

      if (!Directory.Exists(targetFolderPath)) {
        Directory.CreateDirectory(targetFolderPath);
      }

      if (!Directory.Exists(sourceFolderPath)) return;
      Console.WriteLine(sourceFolderPath);
      ProcessDirectory(sourceFolderPath);
    }

    private static void ProcessDirectory(string sourceDirectory) {
      Console.WriteLine($"Processing directory '{sourceDirectory}'.");

      // Process the list of files found in the directory.
      var fileEntries = Directory.GetFiles(sourceDirectory);
      foreach (var fileName in fileEntries)
        ProcessFile(fileName);

      // Recurse into subdirectories of this directory.
      var subDirectoryEntries = Directory.GetDirectories(sourceDirectory);
      foreach (var subDirectory in subDirectoryEntries)
        ProcessDirectory(subDirectory);
    }

    private static void ProcessFile(string sourceFilePath) {
      Console.WriteLine($"Processing file '{sourceFilePath}'.");

      var svgSourceDoc = new HtmlDocument();
      svgSourceDoc.Load(sourceFilePath);

      //Console.WriteLine(svgSourceDoc.ToString());

      var svgSourceNode = svgSourceDoc.DocumentNode.SelectSingleNode("//svg");
      svgSourceNode.SetAttributeValue("stroke", "currentColor");
      //svgSourceNode.SetAttributeValue("fill", "currentColor");
      foreach (var node in svgSourceNode.ChildNodes) {
        ProcessNode(node);
      }

      var targetWriter = new StringWriter();
      svgSourceDoc.Save(targetWriter);
      //Console.WriteLine(targetWriter.ToString());

      var targetFileName = Path.GetFileName(sourceFilePath).Replace(" ", "-").ToLower().Replace("å", "a").Replace("ä", "a").Replace("ö", "o");

      using var outputFile = new StreamWriter(Path.Combine(targetFolderPath, targetFileName));
      {
        outputFile.Write(targetWriter.ToString());
        outputFile.Close();
      }
    }

    private static void ProcessNode(HtmlNode node) {

      if (node.Attributes.Contains("Fill"))
      {
        node.Attributes["fill"].Value = "currentColor";
      }
      if (node.Attributes.Contains("stroke"))
      {
        node.Attributes["stroke"].Value = "currentColor";
      }

      foreach (var subNode in node.ChildNodes) {
        ProcessNode(subNode);
      }
    }
  }
}