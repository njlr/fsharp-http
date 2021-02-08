namespace FSharp.Http

open System
open System.IO

type HttpHeaders = (string * (string list)) list

type HttpRequest =
  {
    Url : string
    Method : string
    Headers : HttpHeaders
  }

type HttpResponse =
  {
    Status : int
    Headers : HttpHeaders
    Stream : Stream
  }
  with
    interface IDisposable
      with
        member this.Dispose () =
          this.Stream.Dispose ()

module Optics =

  module HttpRequest =

    let url =
      (fun (x : HttpRequest) -> x.Url),
      (fun v (x : HttpRequest) -> { x with Url = v })

    let method =
      (fun (x : HttpRequest) -> x.Method),
      (fun v (x : HttpRequest) -> { x with Method = v })

  module HttpHeaders =

    let header (name : string) =
      let get (x : HttpHeaders) =
        x
        |> Seq.tryPick (fun (k, v) -> if k = name then Some v else None)

      let set v (x : HttpHeaders) =
        x
        |> List.filter (fun (k, _) -> k <> name)
        |> (fun xs -> (name, v) :: xs)

      get, set
