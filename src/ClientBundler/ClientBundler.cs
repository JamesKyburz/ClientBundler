using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

/*@using ClientBundler;

@Html.RenderStyles("app")
@Html.RenderScripts("app")
@Html.RenderTemplates()
*/

namespace ClientBundler
{
  class Options
  {
    public static string ScriptPath
    {
      get { return Setting("Assets.ScriptPath") ?? "Scripts"; }
    }

    public static string StylePath
    {
      get { return Setting("Assets.StylePath") ?? "Content"; }
    }

    public static string TemplatePath
    {
      get { return Setting("Assets.TemplatePath") ?? "Templates"; }
    }

    public static bool PreCompiled
    {
      get { return "true" == Setting("Assets.Precompiled"); }
    }

    static string Setting(string key) {
      return System.Configuration.ConfigurationManager.AppSettings[key];
    }
  }

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

  class Asset
  {
    string fileRoot, assetRoot;
    public string FilePath { get; private set; }

    Dictionary<string, string> scriptTypes = new Dictionary<string, string>() {
      {".js", "text/javascript"},
      {".coffee", "text/coffeescript"},
      {".serenade", "text/html"}
    };

    public Asset(string assetRoot, string fileRoot, string filePath)
    {
      this.fileRoot = fileRoot;
      this.FilePath = filePath;
      this.assetRoot = assetRoot;
    }

    public string ScriptTag()
    {
      var href = Href();
      var scriptType = "text/html";
      scriptTypes.TryGetValue(Path.GetExtension(href), out scriptType);
      return scriptType == "text/javascript" ?
        string.Format(
         "<script src=\"{0}\" type=\"{1}\"></script>", href, scriptType
        )
        :
        string.Format(
         "<script type=\"{1}\" id=\"{2}\">{0}</script>", File.ReadAllText(FilePath), scriptType,
          Regex.Replace(href, "^" + "/" + assetRoot + "/", "").Replace("/", "-").Replace(Path.GetExtension(FilePath), "")
        );
    }

    public string LinkTag()
    {
      return "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Href() + "\">";
    }

    string Href()
    {
      return ("/" + assetRoot +
        (FilePath.Replace(@"\", "/").Split(new string[] { assetRoot }, StringSplitOptions.None))[1]
      ).Replace("./", "");
    }
  }
  class AssetUtility
  {
    string root, assetRoot;

    public AssetUtility(string assetRoot)
    {
      this.assetRoot = assetRoot;
      this.root = System.Web.HttpContext.Current.Server.MapPath("/" + assetRoot);
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
          .Select(assetPath => new Asset(assetRoot, root, assetPath));
    }

  }

  public static class Helpers
  {
    public static IHtmlString RenderStyles(this HtmlHelper helper, string name)
    {
      return helper.Raw(
        Options.PreCompiled ?
          new AssetUtility(Options.StylePath).GetAssets("")
           .Where(x=>Regex.IsMatch(x.FilePath, "(?i)css$"))
           .First()
           .LinkTag()
          :
          new StringBuilder()
            .Append("<link rel=\"stylesheet/less\" type=\"text/css\" href=\"/" + Options.StylePath + "/" + name + ".less" + "\">")
            .Append("<script src=\"https://raw.github.com/JamesKyburz/less.js/master/dist/less-1.3.0.js\"></script>")
            .ToString()
      );
    }

    public static IHtmlString RenderScripts(this HtmlHelper helper, string name)
    {
      return helper.Raw(
        Options.PreCompiled ?
         new AssetUtility(Options.ScriptPath).GetAssets("")
           .First()
           .ScriptTag()
         :
         string.Join(Environment.NewLine,
           new HashSet<string>(
             new ScriptManifest(name).GetAssets()
               .Select(x => x.ScriptTag())
           )
         ) +
         "<script src=\"http://coffeescript.org/extras/coffee-script.js\"></script>"
      );
    }

    public static IHtmlString RenderTemplates(this HtmlHelper helper)
    {
      return helper.Raw(
        string.Join(Environment.NewLine,
          new AssetUtility(Options.TemplatePath).GetAssets("")
            .Select(x => x.ScriptTag())
        )
      );
    }
  }
}