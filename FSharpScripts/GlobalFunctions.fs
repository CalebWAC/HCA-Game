module FSharpScripts.GlobalFunctions

open Godot

let getRoot () =
    let mainLoop = Engine.GetMainLoop()
    (match mainLoop with
    | :? SceneTree as sceneTree -> sceneTree
    | _ -> null).Root.GetNode<Node3D>("Node3D")
                    
let degToRad deg = deg * Mathf.Pi / 180f