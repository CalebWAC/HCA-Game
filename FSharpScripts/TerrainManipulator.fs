namespace FSharpScripts

open System.Collections.Generic
open FSharpScripts.WorldFS
open Godot
open PlayerFS
open GlobalFunctions

module TerrainManipulatorFS =
    let destructibleBlocks = List<Node3D>()
    
    type Block() =
        let mutable self = Unchecked.defaultof<Node3D>
        static let mutable t = 0f
        
        static let mutable selected = Vector3(0f, 0f, 0f)
        static let mutable selector = Unchecked.defaultof<CsgBox3D>
        
        static let newBlocks = List<Node3D>()
        
        static member onInputEvent (event : InputEvent) (position : Vector3) =
            if Array.contains TerrainManipulator powerUps then
                try
                    let mouseClick = event :?> InputEventMouseButton
                    if mouseClick.ButtonIndex = MouseButton.Left && mouseClick.Pressed &&
                       roundVec position <> roundVec(getRoot().GetNode("WorldGenerator").GetNode<Node3D>("Goal").Position) &&
                       roundVec position <> roundVec(getRoot().GetNode<Node3D>("Player").Position) then
                        terrainOn <- not terrainOn
                        selected <- roundVec position
                        
                        if terrainOn then
                            selector.Position <- selected - Vector3(0f, 0.9f, 0f)
                        else
                            selector.Position <- Vector3(0f, 50f, 0f)
                with | _ -> ()
        
        member this.onCollided (_: Node3D) =
            if destructibleBlocks.Contains self then
                destructibleBlocks.Remove self |> ignore
                self.QueueFree()
        
        member this.ready thing =
            self <- thing
            selector <- getRoot().GetNode<CsgBox3D>("Selector")
            self.GetNode<Area3D>("Area3D").add_BodyEntered (fun x -> this.onCollided x)
        
        member this.process delta =
            t <- t + delta * 0.5f
            
            // For hidden blocks
            if Array.contains Glasses powerUps && getMaterialAt self.Position.X self.Position.Z = Invisible
               && self.Position.Y <> 0f then
                if getRoot().GetNode<Node3D>("Player").Position.DistanceTo self.Position <= 3f then
                    self.Visible <- true
                else self.Visible <- false
                
            // For rushing water
            if getMaterialAt self.Position.X self.Position.Z = RushingWater then
                self.RotateObjectLocal(Vector3.Right, delta * 3f)
        
        static member input (event : InputEvent) =
            if terrainOn && t > 1f && (worlds[level] |> Array.find(fun b -> b.position = selected - Vector3.Up)).material = Ground then
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
                            block.GetNode<Area3D>("Area3D").add_InputEvent (fun _ event position _ _ -> Block.onInputEvent event position)
                            getRoot().GetNode<Node3D>("WorldGenerator").AddChild(block)
                            Array.set worlds[level] (worlds[level] |> Array.findIndex (fun b -> b.position = selected - Vector3.Up)) { position = selected; rotation = Vector3.Zero; material = Ground }
                            selected <- selected + Vector3.Up
                            newBlocks.Add block
                            selector.Position <- selected - Vector3(0f, 0.9f, 0f)
                        | Key.Down ->
                            t <- 0f
                            let block = newBlocks.Find (fun b -> b.Position = selected - Vector3.Up)
                            if block <> null then
                                block.QueueFree()
                                newBlocks.Remove block |> ignore
                                selected <- selected - Vector3.Up
                                Array.set worlds[level] (worlds[level] |> Array.findIndex (fun b -> b.position = selected)) { position = selected - Vector3.Up; rotation = Vector3.Zero; material = Ground }
                                selector.Position <- selected - Vector3(0f, 0.9f, 0f)
                        | _ -> ()
                | _ -> ()