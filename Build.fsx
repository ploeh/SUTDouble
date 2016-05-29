#r @"packages/FAKE.4.27.0/tools/FakeLib.dll"

open Fake

Target "Clean" <| fun _ ->
    directExec <| fun info ->
        info.FileName <- "git"
        info.Arguments <- "clean -xdf"
    |> ignore

Target "Build" <| fun _ ->
    !! "**/BookingApi.sln"
    |> MSBuildRelease "" "Rebuild"
    |> ignore

"Clean"
==> "Build"

RunTargetOrDefault "Build"