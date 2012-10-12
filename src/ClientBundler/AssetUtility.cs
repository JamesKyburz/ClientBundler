using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClientBundler
{
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
}