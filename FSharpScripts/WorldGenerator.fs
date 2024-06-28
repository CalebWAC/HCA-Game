namespace FSharpScripts

open Godot

module WorldFS =
    type Block = {
        position : Vector3
    }
    
    let block x y z = { position = Vector3(x, y, z) }
    
    let mutable self = Unchecked.defaultof<Node3D>
    
    let world = [|
        block -5f 0f -5f; block -5f 0f -3f; block -5f 0f 3f; block -5f 0f 5f
        block -3f 0f -5f; block -3f 0f -3f; block -3f 0f 3f; block -3f 0f 5f
        block 3f 0f -5f; block 3f 0f -3f; block 3f 0f 3f; block 3f 0f 5f
        block 5f 0f -5f; block 5f 0f -3f; block 5f 0f 3f; block 5f 0f 5f
    |]
    
    let ready () =
        GD.Print "Beginning world initialization"
        
        let mainLoop = Engine.GetMainLoop()
        let sceneTree = match mainLoop with
                        | :? SceneTree as sceneTree -> sceneTree
                        | _ -> null
        
        world
        |> Array.iter (fun data ->
            let block = new CsgBox3D()
            block.Position <- data.position
            sceneTree.Root.GetNode("Node3D").GetNode<Node3D>("WorldGenerator").AddChild(block)
            GD.Print $"{block.Position}"
        )
        
    let process delta =
        ()