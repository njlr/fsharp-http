#load "./fsharp.http/Http.fsx"

open FSharp.Http

[<Literal>]
let ChromeUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36"

async {
  let request =
    HttpRequest.get "https://www.whatismybrowser.com/detect/what-is-my-user-agent"
    |> HttpRequest.withUserAgent ChromeUserAgent

  let! response = HttpRequest.send request

  printfn "%i %A" response.Status response.Headers

  let! content =
    response
    |> HttpResponse.fetchContent

  printfn "%s" content
}
|> Async.RunSynchronously
