namespace FSharpScripts

open Godot
open WorldFS
open GlobalFunctions

module WorldGeneratorFS =
    let ready () =
        GD.Print "Beginning world initialization"
        
        // World generation
        worlds[level]
        |> Array.iter (fun data ->
            for i in 0f .. data.position.Y do
                let blockScene = GD.Load<PackedScene>("res://Elements/Block.tscn")
                let block = blockScene.Instantiate() :?> Node3D
                
                block.Position <- Vector3(data.position.X, i, data.position.Z)
                block.GetNode<CsgBox3D>("CSGBox3D").MaterialOverride <-
                    match data.material with
                    | Ground -> ResourceLoader.Load("res://Materials/Block.tres") :?> Material
                    | Water ->
                        block.GetNode("Model").QueueFree()
                        ResourceLoader.Load("res://Materials/WaterBubble.tres") :?> Material
                
                block.GetNode<Area3D>("Area3D").add_InputEvent (fun _ event position _ _ -> TerrainManipulatorFS.onInputEvent event position)
                getRoot().GetNode<Node3D>("WorldGenerator").AddChild(block)
                
        )
        
        // Platformer element placement
        elements[level]
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
        powerUps[level]
        |> Array.iter (fun powerUp ->
            let scene = match powerUp.ptype with
                        | GrapplingHook -> GD.Load<PackedScene>("res://Power Ups/GrapplingHook.tscn")
                        | TerrainManipulator -> GD.Load<PackedScene>("res://Power Ups/TerrainManipulator.tscn")
            let power = scene.Instantiate() :?> Node3D
            power.Position <- powerUp.position
            getRoot().GetNode<Node3D>("WorldGenerator").AddChild(power)
        )
        
    let process delta =
        ()