#Client bundler

Forget waiting for compile time, during development serve js, coffee &
less files as is and let the browser compile them with less.js and
coffeescript.js.

For production use you want Precompiled to be true.

This package does not solve the minify issue use node snockets and less
for that in node.

Here are the settings

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
