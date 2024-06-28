namespace FSharpScripts

open Godot
open GlobalFunctions

module WorldFS =
    type Block = {
        position : Vector3
    }
    
    let block x y z = { position = Vector3(x, y, z) }
    
    let world = [|
        block -2f 0f -2f; block -2f 1f -1f; block -2f 2f 0f; block -2f 3f 1f; block -2f 3f 2f 
        block -1f 0f -2f; block -1f 1f -1f; block -1f 1f 0f; block -1f 1f 1f; block -1f 2f 2f
        block 0f 0f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 0f 2f
        block 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 1f 1f; block 1f 1f 2f
        block 2f 0f -2f; block 2f 0f -1f; block 2f 1f 0f; block 2f 1f 1f; block 2f 2f 2f
    |]
    
    let ready () =
        GD.Print "Beginning world initialization"
        
        world
        |> Array.iter (fun data ->
            for i in 0f .. data.position.Y do
                let block = new CsgBox3D()
                block.Position <- Vector3(data.position.X, i, data.position.Z)
                getRoot().GetNode<Node3D>("WorldGenerator").AddChild(block)
        )
        
    let process delta =
        ()