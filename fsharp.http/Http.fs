namespace FSharp.Http

open System
open System.IO
open System.Net
open System.Text

open Optics

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module HttpStatus =

  let isSuccess (status : int) =
    status >= 200
    && status < 300

  let isFailure = isSuccess >> not

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module HttpResponse =

  let private wellKnownTextEncodings =
    [
      "utf-8", Encoding.UTF8
    ]
    |> Map.ofSeq

  let private tryFindTextEncoding (res : HttpResponse) =
    res.Headers
    |> Seq.tryFind (fun (k, _) -> k = HttpHeaders.ContentType)
    |> Option.bind (fun (_, v) ->
      v
      |> Seq.tryPick (fun x ->
        match x.Split('=') with
        | [| "charset"; x |] ->
          wellKnownTextEncodings
          |> Map.tryFind x
        | _ -> None
      )
    )

  let fetchContent (res : HttpResponse) =
    async {
      let encoding =
        tryFindTextEncoding res
        |> Option.defaultValue Encoding.UTF8

      use reader = new StreamReader (res.Stream, encoding)

      return!
        reader.ReadToEndAsync ()
        |> Async.AwaitTask
    }
