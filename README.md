#Client bundler

Forget waiting for compile time, during development serve js, coffee &
less files as is and let the browser compile them with less.js and
coffeescript.js. Also support for serving as the contents of template files in text/html script tags inline.

#Before considering this package read this section.

Only support for javascript, coffeescript, css and less.
It does nothing fancy in development mode it serves up coffeescript.js and less.js, as it doesn't compile anything.
This makes it the fastest option in .NET in my experience so far. 

In production it is assumed one app*.js and one app*.css. The * is what ever scheme you decide on suffixing the files so the browser cache is invalidated.

#Usage in views

``` razor
@using ClientBundler;
@Html.RenderStyles("app") // Look for app.less(development) or app*.css (production)
@Html.RenderTemplates()
@Html.RenderScripts("app") // Look for manifest app.js/coffee file (development) or app*.js (production)
```

#Helper methods for building appcache

``` razor
@using ClientBundler;
CACHE MANIFEST

#@System.DateTime.Now.ToString()

@Html.RenderScriptSources("app")
@Html.RenderStyleSources("app")
```

#Development use

Use a manifest file that requires files in the order needed.
Same syntax as Ruby On Rails [Sprockets](https://github.com/sstephenson/sprockets) and Node's [Snockets](https://github.com/TrevorBurnham/snockets)

Example app.coffee (javascript can also be used)

``` coffeescript
#= require vendor/jquery
#= require_tree vendor
#= require helper
#= require_tree models
#= require_tree .
```


#Production use

For production use you want Precompiled to be true (see settings).

This package does not solve the minify issue use node snockets and less
for that in node. By it's not hard here is an example of how to fix it :-

``` coffeescript
fs              = require 'fs'
lessc           = require 'less'
snockets        = new (require 'Snockets')
time            = (new Date().getTime()).toString(36)
checkError      = (e, _) -> throw e if e
data            = (callback) -> (e, data) -> checkError(e); callback(data)
assetsPath      = './src/Web/Assets'
preCompiledPath = './src/Web/precompiled/Assets'

fs.readFile "#{assetsPath}/css/app.less", 'utf8', data (less) ->
  lessc.render less, { compress: true, paths: ["#{assetsPath}/css"] }, data (css) ->
    fs.writeFile "#{preCompiledPath}/css/app-#{time}.css", css, 'utf8', checkError()

snockets.getConcatenation "#{assetsPath}/js/app.coffee", { minify: true }, data (js) ->
  fs.writeFile "#{preCompiledPath}/js/app-#{time}.js", js, 'utf8', checkError()
```

Here are the settings for your web.config

##appSettings config
``` xml
<!-- Javascript & CoffeeScript path -->
<add key="Assets.ScriptPath" value="assets/js" />
<!-- Less path -->
<add key="Assets.StylePath" value="assets/css" />
<!-- Client template path if any -->
<add key="Assets.TemplatePath" value="assets/html" />
<!-- If false will serve coffeescript.js and less.js -->
<add key="Assets.Precompiled" value="false" />
```

###Nuget

``` nuget
install-package ClientBundler
```
