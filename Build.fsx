#r @"packages/FAKE.4.27.0/tools/FakeLib.dll"

open Fake
open Fake.Testing

Target "Clean" <| fun _ ->
    directExec <| fun info ->
        info.FileName <- "git"
        info.Arguments <- "clean -xdf"
    |> ignore

Target "Build" <| fun _ ->
    !! "**/BookingApi.sln"
    |> MSBuildRelease "" "Rebuild"
    |> ignore

Target "Test" <| fun _ ->
    !! "**/bin/Release/*Ploeh.Samples.BookingApi.*Tests*.dll"
    |> xUnit2 (fun p -> { p with Parallel = ParallelMode.All })

"Clean"
==> "Build"
==> "Test"

RunTargetOrDefault "Test"