[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FSharp.Http.HttpRequest

open System
open System.IO
open System.Net
open System.Text

open Optics

let head (url : string) =
  {
    Url = url
    Method = HttpMethod.Head
    Headers = []
  }

let get (url : string) =
  {
    Url = url
    Method = HttpMethod.Get
    Headers = []
  }

let withUserAgent (ua : string) (request : HttpRequest) =
  {
    request with
      Headers =
        request.Headers
        |> (snd (HttpHeaders.header HttpHeaders.UserAgent)) [ ua ]
  }

let withAuthorization (auth : string) (request : HttpRequest) =
  {
    request with
      Headers =
        request.Headers
        |> (snd (HttpHeaders.header HttpHeaders.Authorization)) [ auth ]
  }

let rec private unpackException (exn : Exception) =
  match exn with
  | :? AggregateException as exn ->
    match Seq.tryExactlyOne exn.InnerExceptions with
    | Some exn -> unpackException exn
    | _ -> raise exn
  | :? WebException as exn -> exn
  | _ -> raise exn

let private unpackHeaders (response : HttpWebResponse) =
  response.Headers.AllKeys
  |> Seq.map (fun key ->
    let value =
      (response.Headers.Get key).Split(';')
      |> Seq.map (fun x -> x.Trim())
      |> Seq.toList

    key, value
  )
  |> Seq.toList

let send (req : HttpRequest) =
  async {
    let httpWebRequest = HttpWebRequest.CreateHttp (req.Url)

    httpWebRequest.Method <- req.Method

    for k, v in req.Headers do
      httpWebRequest.Headers.Add (k, String.concat "; " v)

    let! response =
      async {
        try
          return!
            httpWebRequest.GetResponseAsync ()
            |> Async.AwaitTask
        with exn ->
          let webException = unpackException exn
          return webException.Response
      }

    let response = response :?> HttpWebResponse

    return
      {
        Status = int response.StatusCode
        Headers = unpackHeaders response
        Stream = response.GetResponseStream ()
      }
  }
