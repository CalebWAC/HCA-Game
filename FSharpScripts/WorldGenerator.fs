namespace FSharpScripts

open Godot
open GlobalFunctions

module WorldFS =
    type Block = {
        position : Vector3
    }
    
    type ElementType =
        | Goal
        | Bubble
        | Bridge
        | Hook
        
    type PowerUpType =
        | GrapplingHook
        
    type Element = { etype: ElementType; position: Vector3; rotation: Vector3 }
    
    type PowerUp = { ptype: PowerUpType; position: Vector3 }
    
    let block x y z = { position = Vector3(x, y, z) }
    
    let world = [|
        block -6f 5f -6f; block -6f 0f -5f; block -6f 0f -4f; block -6f 0f -3f; block -6f 0f -2f; block -6f 0f -1f; block -6f 0f 0f; block -6f 0f 1f; block -6f 0f 2f; block -6f 0f 3f; block -6f 4f 4f; block -6f 5f 5f; block -6f 6f 6f
        block -5f 8f -6f; block -5f 0f -5f; block -5f 0f -4f; block -5f 0f -3f; block -5f 0f -2f; block -5f 0f -1f; block -5f 6f 0f; block -5f 3f 1f; block -5f 0f 2f; block -5f 0f 3f; block -5f 3f 4f; block -5f 6f 5f; block -5f 6f 6f
        block -4f 7f -6f; block -4f 0f -5f; block -4f 0f -4f; block -4f 0f -3f; block -4f 0f -2f; block -4f 0f -1f; block -4f 6f 0f; block -4f 4f 1f; block -4f 0f 2f; block -4f 0f 3f; block -4f 0f 4f; block -4f 3f 5f; block -4f 5f 6f
        block -3f 7f -6f; block -3f 0f -5f; block -3f 0f -4f; block -3f 0f -3f; block -3f 0f -2f; block -3f 0f -1f; block -3f 0f 0f; block -3f 0f 1f; block -3f 0f 2f; block -3f 0f 3f; block -3f 0f 4f; block -3f 3f 5f; block -3f 4f 6f
        block -2f 7f -6f; block -2f 0f -5f; block -2f 0f -4f; block -2f 0f -3f; block -2f 0f -2f; block -2f 0f -1f; block -2f 0f 0f; block -2f 0f 1f; block -2f 0f 2f; block -2f 0f 3f; block -2f 0f 4f; block -2f 3f 5f; block -2f 3f 6f
        block -1f 0f -6f; block -1f 0f -5f; block -1f 0f -4f; block -1f 0f -3f; block -1f 0f -2f; block -1f 5f -1f; block -1f 6f 0f; block -1f 0f 1f; block -1f 0f 2f; block -1f 0f 3f; block -1f 0f 4f; block -1f 2f 5f; block -1f 3f 6f
        block 0f 0f -6f; block 0f 0f -5f; block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 0f 2f; block 0f 0f 3f; block 0f 0f 4f; block 0f 2f 5f; block 0f 3f 6f
        block 1f 0f -6f; block 1f 0f -5f; block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 0f 1f; block 1f 0f 2f; block 1f 0f 3f; block 1f 0f 4f; block 1f 1f 5f; block 1f 2f 6f
        block 2f 0f -6f; block 2f 0f -5f; block 2f 0f -4f; block 2f 0f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 0f 0f; block 2f 0f 1f; block 2f 0f 2f; block 2f 0f 3f; block 2f 0f 4f; block 2f 1f 5f; block 2f 1f 6f
        block 3f 0f -6f; block 3f 0f -5f; block 3f 0f -4f; block 3f 0f -3f; block 3f 0f -2f; block 3f 0f -1f; block 3f 1f 0f; block 3f 2f 1f; block 3f 0f 2f; block 3f 0f 3f; block 3f 0f 4f; block 3f 0f 5f; block 3f 1f 6f
        block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 1f 0f; block 4f 1f 1f; block 4f 0f 2f; block 4f 0f 3f; block 4f 0f 4f; block 4f 0f 5f; block 4f 0f 6f
        block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 0f -3f; block 5f 0f -2f; block 5f 0f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 0f 2f; block 5f 0f 3f; block 5f 0f 4f; block 5f 0f 5f; block 5f 0f 6f
        block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; block 6f 0f 3f; block 6f 0f 4f; block 6f 0f 5f; block 6f 0f 6f
    |]
    
    let elements = [|
        { etype = Goal; position = Vector3(-5f, 9f, -6f); rotation = Vector3.Zero }
        { etype = Bubble; position = Vector3(-5f, 7f, 3.5f); rotation = Vector3.Zero }
        { etype = Bubble; position = Vector3(-5f, 7f, 1.5f); rotation = Vector3.Zero }
        { etype = Bridge; position = Vector3(-2.5f, 6f, 0f); rotation = Vector3.Zero }
        { etype = Hook; position = Vector3(-3f, 7f, -5.5f); rotation = Vector3.Zero }
    |]
    
    let powerUps = [|
        { ptype = GrapplingHook; position = Vector3(-2f, 1f, 0f) }
    |]
    
    let ready () =
        GD.Print "Beginning world initialization"
        
        // World generation
        world
        |> Array.iter (fun data ->
            for i in 0f .. data.position.Y do
                let blockScene = GD.Load<PackedScene>("res://Elements/Block.tscn")
                let block = blockScene.Instantiate() :?> Node3D
                block.Position <- Vector3(data.position.X, i, data.position.Z)
                getRoot().GetNode<Node3D>("WorldGenerator").AddChild(block)
        )
        
        // Platformer element placement
        elements
        |> Array.iter (fun element ->
            let scene = GD.Load<PackedScene>(match element.etype with
                                            | Goal -> "res://Elements/Goal.tscn"
                                            | Bubble -> "res://Elements/WaterBubble.tscn"
                                            | Bridge -> "res://Elements/Bridge.tscn"
                                            | Hook -> "res://Elements/Hook.tscn")
            
            let emt = scene.Instantiate() :?> Node3D
            emt.Position <- element.position
            emt.Rotation <- element.rotation
            getRoot().GetNode<Node3D>("WorldGenerator").AddChild(emt)
        )
        
        // Power up placement
        powerUps
        |> Array.iter (fun powerUp ->
            let scene = match powerUp.ptype with
                        | GrapplingHook -> GD.Load<PackedScene>("res://Power Ups/GrapplingHook.tscn")
            let power = scene.Instantiate() :?> Node3D
            power.Position <- powerUp.position
            getRoot().GetNode<Node3D>("WorldGenerator").AddChild(power)
        )
        
    let process delta =
        ()