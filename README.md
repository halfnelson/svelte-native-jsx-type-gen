# Svelte Native JSX type Generator - DEPRECATED

> This has been replaced by the much more useful and accurate https://github.com/halfnelson/nativescript-source-to-jsx-def It turns out the XML wasn't specific enough to generate good type defs.

Parses the NativeScript XML schema and uses it to generate the JSX types for use with `svelte-type-checker-vscode` etc.

## Usage

```bash
# to get paket
dotnet tool restore
# to get deps
dotnet packet restore
# to generate types
dotnet fsi generate.fsx <output-file>
```

## TODO
 - Override generated event types. The event attributes are string typed in the XML schema.


