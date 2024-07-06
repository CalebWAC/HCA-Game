namespace FSharpScripts

open System.Collections.Generic
open FSharpScripts.WorldFS
open Godot
open WorldFS
open PlayerFS
open GlobalFunctions

module TerrainManipulatorFS =
    let mutable self = Unchecked.defaultof<Node3D>
    let mutable selected = Vector3(0f, 0f, 0f)
    let mutable t = 0f
    
    let newBlocks = List<Node3D>()
    
    let onInputEvent (event : InputEvent) (position : Vector3) =
        if Array.contains TerrainManipulator powerUps then
            try
                let mouseClick = event :?> InputEventMouseButton
                if mouseClick.ButtonIndex = MouseButton.Left && mouseClick.Pressed then
                    terrainOn <- not terrainOn
                    selected <- Vector3(round position.X, round position.Y, round position.Z)
            with | _ -> ()
    
    let process delta =
        t <- t + delta * 4f
    
    let input (event : InputEvent) =
        if terrainOn && t > 1f && (worlds[level] |> Array.find(fun b -> b.position = selected - Vector3(0f, 1f, 0f))).material = Ground then
            match event with
            | :? InputEventKey ->
                let keyEvent = event :?> InputEventKey
                if keyEvent.Pressed then
                    match keyEvent.Keycode with
                    | Key.Up ->
                        t <- 0f
                        let blockScene = GD.Load<PackedScene>("res://Elements/Block.tscn")
                        let block = blockScene.Instantiate() :?> Node3D
                        block.Position <- selected
                        block.GetNode<Area3D>("Area3D").add_InputEvent (fun _ event position _ _ -> onInputEvent event position)
                        getRoot().GetNode<Node3D>("WorldGenerator").AddChild(block)
                        Array.set worlds[level] (worlds[level] |> Array.findIndex (fun b -> b.position = selected - Vector3(0f, 1f, 0f))) { position = selected; material = Ground }
                        selected <- selected + Vector3(0f, 1f, 0f)
                        newBlocks.Add block
                    | Key.Down ->
                        let block = newBlocks.Find (fun b -> b.Position = selected - Vector3(0f, 1f, 0f))
                        if block <> null then block.QueueFree()
                        selected <- selected - Vector3(0f, 1f, 0f)
                        Array.set worlds[level] (worlds[level] |> Array.findIndex (fun b -> b.position = selected)) { position = selected - Vector3(0f, 1f, 0f); material = Ground }
                        waitThen 0.25 ()
                    | _ -> ()
            | _ -> ()