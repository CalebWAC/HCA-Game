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
        
    type PowerUpType =
        | GrapplingHook
        
    type Element = { etype: ElementType; position: Vector3; rotation: Vector3 }
    
    type PowerUp = { ptype: PowerUpType; position: Vector3 }
    
    let block x y z = { position = Vector3(x, y, z) }
    
    let world = [|
        block -4f 0f -4f; block -4f 1f -3f; block -4f 2f -2f; block -4f 2f -1f; block -4f 3f 0f; block -4f 3f 1f; block -4f 1f 2f; block -4f 0f 3f; block -4f 3f 4f;
        block -3f 0f -4f; block -3f 1f -3f; block -3f 2f -2f; block -3f 2f -1f; block -3f 2f 0f; block -3f 3f 1f; block -3f 1f 2f; block -3f 0f 3f; block -3f 3f 4f
        block -2f 0f -4f; block -2f 0f -3f; block -2f 1f -2f; block -2f 1f -1f; block -2f 2f 0f; block -2f 2f 1f; block -2f 1f 2f; block -2f 0f 3f; block -2f 3f 4f
        block -1f 0f -4f; block -1f 0f -3f; block -1f 0f -2f; block -1f 0f -1f; block -1f 1f 0f; block -1f 2f 1f; block -1f 1f 2f; block -1f 0f 3f; block -1f 3f 4f;
        block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 1f 0f; block 0f 2f 1f; block 0f 1f 2f; block 0f 2f 3f; block 0f 3f 4f;
        block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 1f 0f; block 1f 2f 1f; block 1f 1f 2f; block 1f 2f 3f; block 1f 3f 4f;
        block 2f 0f -4f; block 2f 0f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 1f 0f; block 2f 2f 1f; block 2f 0f 2f; block 2f 0f 3f; block 2f 2f 4f
        block 3f 0f -4f; block 3f 0f -3f; block 3f 0f -2f; block 3f 0f -1f; block 3f 1f 0f; block 3f 2f 1f; block 3f 0f 2f; block 3f 0f 3f; block 3f 2f 4f;
        block 4f 0f -4f; block 4f 0f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 1f 0f; block 4f 2f 1f; block 4f 0f 2f; block 4f 0f 3f; block 4f 2f 4f;
    |]
    
    let elements = [|
        { etype = Goal; position = Vector3(-4f, 4f, 4f); rotation = Vector3.Zero }
        { etype = Bubble; position = Vector3(-3f, 4f, 2.5f); rotation = Vector3.Zero }
        { etype = Bridge; position = Vector3(3f, 2f, 2.5f); rotation = Vector3(0f, degToRad 90f, 0f) }
    |]
    
    let powerUps = [|
        { ptype = GrapplingHook; position = Vector3(-1f, 1f, -3f) }
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
            let scene = match element.etype with
                        | Goal -> GD.Load<PackedScene>("res://Elements/Goal.tscn")
                        | Bubble -> GD.Load<PackedScene>("res://Elements/WaterBubble.tscn")
                        | Bridge _ -> GD.Load<PackedScene>("res://Elements/Bridge.tscn")
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