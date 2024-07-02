module FSharpScripts.GlobalFunctions

open Godot

let getRoot () =
    let mainLoop = Engine.GetMainLoop()
    (match mainLoop with
    | :? SceneTree as sceneTree -> sceneTree
    | _ -> null).Root.GetNode<Node3D>("Node3D")
    
let getScreenRoot () =
    let mainLoop = Engine.GetMainLoop()
    (match mainLoop with
    | :? SceneTree as sceneTree -> sceneTree
    | _ -> null).Root.GetNode<Control>("Control")
    
// Mathematical functions                
let degToRad deg = deg * Mathf.Pi / 180f

let round num = 0.5f + num |> Mathf.Floor