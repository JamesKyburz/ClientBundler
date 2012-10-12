using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ClientBundler
{
  class ScriptManifest
  {
    string manifestPath, specialComment;

    public ScriptManifest(string name)
    {
      manifestPath = new AssetUtility(Options.ScriptPath).GetAsset(name).FilePath;
      specialComment = new Dictionary<string, string>() {
        {".js", "//="},
        {".coffee", "#="}
      }[Path.GetExtension(manifestPath)];
    }

    public IEnumerable<Asset> GetAssets()
    {
      var assetUtility = new AssetUtility(Options.ScriptPath);
      foreach (var line in File.ReadAllLines(manifestPath))
      {
        var match = Regex.Split(line, specialComment + " require ");
        if (match.Length > 1)
        {
          yield return assetUtility.GetAsset(match[1] + ".*");
        }
        else
        {
          match = Regex.Split(line, specialComment + " require_tree ");
          foreach (var asset in assetUtility.GetAssets(match[1]))
            yield return asset;
        }
      }
    }

  }
}
