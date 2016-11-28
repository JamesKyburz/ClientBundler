﻿namespace ClientBundler
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
      get { return Setting("Assets.CoffeeScriptUrl") ?? "http://coffeescript.org/v1/extras/coffee-script.js"; }
    }

    static string Setting(string key)
    {
      return System.Configuration.ConfigurationManager.AppSettings[key];
    }
  }
}
