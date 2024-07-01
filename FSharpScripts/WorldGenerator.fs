namespace FSharpScripts

open Godot
open GlobalFunctions

module WorldFS =
    type Block = {
        position : Vector3
    }
    
    type Element =
        | Goal of Vector3
    
    let block x y z = { position = Vector3(x, y, z) }
    
    let world = [|
        block -4f 0f -4f; block -4f 1f -3f; block -4f 2f -2f; block -4f 2f -1f; block -4f 3f 0f; block -4f 3f 1f; block -4f 1f 2f; block -4f 0f 3f; block -4f 3f 4f;
        block -3f 0f -4f; block -3f 1f -3f; block -3f 2f -2f; block -3f 2f -1f; block -3f 2f 0f; block -3f 3f 1f; block -3f 1f 2f; block -3f 0f 3f; block -3f 3f 4f
        block -2f 0f -4f; block -2f 0f -3f; block -2f 1f -2f; block -2f 1f -1f; block -2f 2f 0f; block -2f 2f 1f; block -2f 1f 2f; block -2f 0f 3f; block -2f 3f 4f
        block -1f 0f -4f; block -1f 0f -3f; block -1f 0f -2f; block -1f 0f -1f; block -1f 1f 0f; block -1f 2f 1f; block -1f 1f 2f; block -1f 0f 3f; block -1f 3f 4f;
        block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 1f 0f; block 0f 2f 1f; block 0f 1f 2f; block 0f 2f 3f; block 0f 3f 4f;
        block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 1f 0f; block 1f 2f 1f; block 1f 1f 2f; block 1f 2f 3f; block 1f 3f 4f;
        block 2f 0f -4f; block 2f 0f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 1f 0f; block 2f 2f 1f; block 2f 1f 2f; block 2f 2f 3f; block 2f 3f 4f
        block 3f 0f -4f; block 3f 0f -3f; block 3f 0f -2f; block 3f 0f -1f; block 3f 1f 0f; block 3f 2f 1f; block 3f 1f 2f; block 3f 2f 3f; block 3f 3f 4f;
        block 4f 0f -4f; block 4f 0f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 1f 0f; block 4f 2f 1f; block 4f 1f 2f; block 4f 2f 3f; block 4f 3f 4f;
    |]
    
    let elements = [| Goal(Vector3(-4f, 4f, 4f)) |]
    
    let ready () =
        GD.Print "Beginning world initialization"
        
        world
        |> Array.iter (fun data ->
            for i in 0f .. data.position.Y do
                let blockScene = GD.Load<PackedScene>("res://Block.tscn")
                let block = blockScene.Instantiate() :?> Node3D
                block.Position <- Vector3(data.position.X, i, data.position.Z)
                getRoot().GetNode<Node3D>("WorldGenerator").AddChild(block)
        )
        
        elements
        |> Array.iter (fun element ->
            match element with
            | Goal pos ->
                let goalScene = GD.Load<PackedScene>("res://Goal.tscn")
                let goal = goalScene.Instantiate() :?> Node3D
                goal.Position <- pos
                getRoot().GetNode<Node3D>("WorldGenerator").AddChild(goal)
        )
        
    let process delta =
        ()