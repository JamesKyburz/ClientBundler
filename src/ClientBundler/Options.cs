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

    public static string CoffeeScriptUrl
    {
      get { return Setting("Assets.CoffeeScriptUrl") ?? "https://cdn.rawgit.com/JamesKyburz/c840e047c278ebd356245739af605432/raw/coffee-script-1.11-1.js"; }
    }

    public static string LessScriptUrl
    {
      get { return Setting("Assets.LessScriptUrl") ?? "https://cdn.rawgit.com/JamesKyburz/less.js/master/dist/less-1.3.0.js"; }
    }

    static string Setting(string key)
    {
      return System.Configuration.ConfigurationManager.AppSettings[key];
    }
  }
}
