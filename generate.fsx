#r "packages/FSharp.Data/lib/netstandard2.0/FSharp.Data.dll"

open FSharp.Data
open System.IO

type XmlSchemaDocument = XmlProvider<"tns-example.xsd">

let data = XmlSchemaDocument.Load("http://schemas.nativescript.org/tns.xsd")

(*
    our elements are defined as
    foreach data.Elements as Element
        if ComplexType[Element.Type].AttributeGroup
            attrs = ComplexType[Element.Type].AttributeGroup
        else
            attrs = attrsForType(ComplexType[Element.Type].ComplexContent.Value.Extension.Base) + attrsForRef ComplexType[Element.Type].ComplexContent.Value.Extension.AttributeGroup.Ref

*)

let attributeGroups = data.AttributeGroups
let complexTypes = data.ComplexTypes
let elements = data.Elements

let camelToTitleCase (camel: string): string = 
    sprintf "%c%s"  (camel.ToUpper().[0]) camel.[1..]

let titleToCamelCase (title: string): string =
     sprintf "%c%s"  (title.ToLower().[0]) title.[1..]


let tsTypeFromSimpleTypeRef ( simpleTypeRef: string): string =
    match simpleTypeRef with
    | "BindingValidator" -> "string"
    | "StringValidator" -> "string"
    | "BooleanValidator" -> "boolean"
    | "NumberValidator" -> "number"
    | "ColorValidator" -> "string"
    | "LayoutValidator" -> "string"
    | _ -> "string"

// uses lowercase attribute name since svelte clobbers attribute case :( 
let tsAttributeDefFromAttribute ( attr: XmlSchemaDocument.Attribute ): string =
    sprintf "%s: %s" (attr.Name.ToLower()) (tsTypeFromSimpleTypeRef attr.Type)


let tsTypeDefFromAttributeGroup ( group: XmlSchemaDocument.AttributeGroup ): string =
    let attrLines = group.Attributes |> Array.map tsAttributeDefFromAttribute
    let attrStr = attrLines |> Array.fold (fun current e -> current + "    " + e + "\n") ""
    
    sprintf "type %s = {\n%s};" (camelToTitleCase group.Name) attrStr


let attributeGroupDefs = 
    attributeGroups |> Array.map tsTypeDefFromAttributeGroup

let attributeGroupSection = 
    let defs = attributeGroupDefs  |> Array.fold (fun current e -> current + e + "\n\n") ""
    "/*\n  Attributes\n*/\n\n" + defs;

let tsTypeDefFromComplexType (ct: XmlSchemaDocument.ComplexType): string =
    let name = ct.Name
    let tsType = 
        match ct.AttributeGroup with
        | Some(g) -> camelToTitleCase g.Ref
        | None -> 
            match ct.ComplexContent with
            | Some (c) -> c.Extension.Base + " & " + (camelToTitleCase c.Extension.AttributeGroup.Ref)
            | None -> "any"
    sprintf "type %s = %s;" name tsType

let complexTypeDefs = 
    complexTypes |> Array.map tsTypeDefFromComplexType

let complexTypeSection =
    let defs = complexTypeDefs  |> Array.fold (fun current e -> current + e + "\n\n") ""
    "/*\n  Element Types \n*/\n\n" + defs;

let intrinsicElementDef (el: XmlSchemaDocument.Element7): string =
    sprintf "%s: %s;" (titleToCamelCase el.Name) el.Type

let intrinsicElementDefs = 
    elements 
        |> Array.filter (fun e -> not (e.Name.Contains('-')))
        |> Array.distinctBy (fun e -> e.Name.ToLower())
        |> Array.map intrinsicElementDef

let elementSection = 
    let defs = intrinsicElementDefs  |> Array.fold (fun current e -> current + "    " + e + "\n") ""
    let fallback =  "    [name: string]: { [name: string]: any };\n"
    "interface IntrinsicElements {\n" + defs + fallback + "}"


let intrinsicElementTypes =
    sprintf "%s\n%s\n%s" attributeGroupSection complexTypeSection elementSection

let templateText = File.ReadAllText("template.d.ts")

let completedTemplate = 
    templateText.Replace("[[INTRINSIC_ELEMENTS]]", "\n" + intrinsicElementTypes)
   

match fsi.CommandLineArgs.Length with
| 2 -> 
    let target = fsi.CommandLineArgs.[1]
    File.WriteAllLines(target, [completedTemplate])
    printfn "Types exported at: %s" target
| _ -> 
    printfn "\nUsage: dotnet fsi %s <outputfile>\n\n" fsi.CommandLineArgs.[0]
            
