using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

/*@using ClientBundler;

@{
  ClientBundler.Options.ScriptPath = "Scripts";
  ClientBundler.Options.TemplatePath = "Templates";
  ClientBundler.Options.StylePath = "Content";
}

@Html.RenderStyles("app")
@Html.RenderScripts("app")
@Html.RenderTemplates()
*/

namespace ClientBundler
{
  public static class Options
  {
    public static string TemplatePath, ScriptPath, StylePath;
    static Options()
    {
      ScriptPath = "Scripts";
      StylePath = "Content";
      TemplatePath = "Templates";
    }
  }

  class ScriptManifest
  {
    string manifestPath, specialComment;

    public ScriptManifest(string name)
    {
      manifestPath = new AssetUtility(Options.ScriptPath, false).GetAsset(name).FilePath;
      specialComment = new Dictionary<string, string>() {
        {".js", "//="},
        {".coffee", "#="}
      }[Path.GetExtension(manifestPath)];
    }

    public IEnumerable<Asset> GetAssets()
    {
      var assetUtility = new AssetUtility(Options.ScriptPath, false);
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

  class Asset
  {
    string fileRoot, assetRoot;
    public string FilePath { get; private set; }
    bool inlineContent;

    Dictionary<string, string> scriptTypes = new Dictionary<string, string>() {
      {".js", "text/javascript"},
      {".coffee", "text/coffeescript"},
      {".serenade", "text/html"}
    };

    public Asset(string assetRoot, string fileRoot, string filePath, bool inlineContent)
    {
      this.fileRoot = fileRoot;
      this.FilePath = filePath;
      this.assetRoot = assetRoot;
      this.inlineContent = inlineContent;
    }

    public string ScriptTag()
    {
      var href = Href();
      var scriptType = "text/html";
      scriptTypes.TryGetValue(Path.GetExtension(href), out scriptType);
      return inlineContent ?
        string.Format(
         "<script type=\"{1}\" id=\"{2}\">{0}</script>", File.ReadAllText(FilePath), scriptType,
          Regex.Replace(href, "^" + assetRoot + "/", "").Replace("/", "-").Replace(Path.GetExtension(FilePath), "")
        )
        :
        string.Format(
         "<script src=\"{0}\" type=\"{1}\"></script>", href, scriptType
        );
    }

    string Href()
    {
      return (assetRoot +
            (Path.GetDirectoryName(FilePath).Replace(fileRoot, "")).Replace(@"\.", "") +
            "/" +
            Path.GetFileName(FilePath)
          ).Replace(@"\", "/");
    }
  }
  class AssetUtility
  {
    string root, assetRoot;
    bool inlineContent;

    public AssetUtility(string assetRoot, bool inlineContent)
    {
      this.assetRoot = assetRoot;
      this.root = System.Web.HttpContext.Current.Server.MapPath(assetRoot);
      this.inlineContent = inlineContent;
    }

    public Asset GetAsset(string path)
    {
      return GetAssets("", path + ".*").FirstOrDefault();
    }

    public IEnumerable<Asset> GetAssets(string path)
    {
      return GetAssets(path, "**");
    }

    IEnumerable<Asset> GetAssets(string path, string search)
    {
      return
        Directory.EnumerateFiles(root + @"\" + path, search, SearchOption.AllDirectories)
          .Select(assetPath => new Asset(assetRoot, root, assetPath, inlineContent));
    }

  }

  public static class Helpers
  {
    public static IHtmlString RenderStyles(this HtmlHelper helper, string name)
    {
      return helper.Raw(
        new StringBuilder()
          .Append("<link rel=\"stylesheet/less\" type=\"text/css\" href=\"" + Options.StylePath + "/" + name + ".less" + "\">")
          .Append("<script src=\"https://raw.github.com/cloudhead/less.js/master/dist/less-1.1.0.js\"></script>")
          .ToString()
      );
    }

    public static IHtmlString RenderScripts(this HtmlHelper helper, string name)
    {
      return helper.Raw(
        string.Join(Environment.NewLine,
          new HashSet<string>(
            new ScriptManifest(name).GetAssets()
              .Select(x => x.ScriptTag())
          )
        )
      );
    }

    public static IHtmlString RenderTemplates(this HtmlHelper helper)
    {
      return helper.Raw(
        string.Join(Environment.NewLine,
          new AssetUtility(Options.TemplatePath, true).GetAssets("")
            .Select(x => x.ScriptTag())
        )
      );
    }
  }
}