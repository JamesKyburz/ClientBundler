﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ClientBundler
{

  public static class HtmlHelpers
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
            .AppendFormat("<script src=\"{0}\"></script>", Options.LessScriptUrl)
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
         string.Format("<script src=\"{0}\"></script>", Options.CoffeeScriptUrl)
      );
    }

    public static IHtmlString RenderStyleSources(this HtmlHelper helper, string name)
    {
      return helper.Raw(
        Options.PreCompiled ?
          new AssetUtility(Options.StylePath).GetAssets("")
           .Where(x=>Regex.IsMatch(x.FilePath, "(?i)css$"))
           .First()
           .Href()
          :
          new StringBuilder()
            .Append("/" + Options.StylePath + "/" + name + ".less")
            .AppendLine()
            .AppendLine(Options.LessScriptUrl)
            .ToString()
      );
    }
    public static IHtmlString RenderScriptSources(this HtmlHelper helper, string name)
    {
      return helper.Raw(
        Options.PreCompiled ?
         new AssetUtility(Options.ScriptPath).GetAssets("")
           .First()
           .Href()
         :
         string.Join(Environment.NewLine,
           new HashSet<string>(
             new ScriptManifest(name).GetAssets()
               .Select(x => x.Href())
           )
         ) +
         Environment.NewLine
         +
         Options.CoffeeScriptUrl
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
