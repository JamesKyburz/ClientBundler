using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ClientBundler
{
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
}
