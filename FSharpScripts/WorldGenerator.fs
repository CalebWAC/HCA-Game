namespace FSharpScripts

open Godot

module WorldFS =
    type Block = {
        position : Vector3
    }
    
    let block x y z = { position = Vector3(x, y, z) }
    
    let mutable self = Unchecked.defaultof<Node3D>
    
    let world = [|
        block -2f 0f -2f; block -2f 1f -1f; block -2f 2f 0f; block -2f 3f 1f; block -2f 3f 2f 
        block -1f 0f -2f; block -1f 1f -1f; block -1f 1f 0f; block -1f 1f 1f; block -1f 2f 2f
        block 0f 0f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 0f 2f
        block 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 1f 1f; block 1f 1f 2f
        block 2f 0f -2f; block 2f 0f -1f; block 2f 1f 0f; block 2f 1f 1f; block 2f 2f 2f
    |]
    
    let ready () =
        GD.Print "Beginning world initialization"
        
        let mainLoop = Engine.GetMainLoop()
        let sceneTree = match mainLoop with
                        | :? SceneTree as sceneTree -> sceneTree
                        | _ -> null
        
        world
        |> Array.iter (fun data ->
            for i in 0f .. data.position.Y do
                let block = new CsgBox3D()
                block.Position <- Vector3(data.position.X, i, data.position.Z)
                sceneTree.Root.GetNode("Node3D").GetNode<Node3D>("WorldGenerator").AddChild(block)
        )
        
    let process delta =
        ()