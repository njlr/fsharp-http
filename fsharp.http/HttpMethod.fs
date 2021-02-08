namespace FSharp.Http

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module HttpMethod =

  [<Literal>]
  let Head = "HEAD"

  [<Literal>]
  let Get = "GET"

  [<Literal>]
  let Post = "POST"

  [<Literal>]
  let Patch = "PATCH"

  [<Literal>]
  let Delete = "DELETE"
