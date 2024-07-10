namespace FSharpScripts

open Godot
open WorldFS
open GlobalFunctions

module WorldGeneratorFS =
    let ready () =
        // World generation
        worlds[level]
        |> Array.iter (fun data ->
            for i in 0f .. data.position.Y do
                let blockScene = GD.Load<PackedScene>("res://Elements/Block.tscn")
                let block = blockScene.Instantiate() :?> Node3D
                
                block.Position <- Vector3(data.position.X, i, data.position.Z)
                block.GetNode<CsgBox3D>("CSGBox3D").MaterialOverride <-
                    match data.material with
                    | Ground -> ResourceLoader.Load("res://Materials/Blue.tres") :?> Material
                    | Water ->
                        block.GetNode("Model").QueueFree()
                        ResourceLoader.Load("res://Materials/WaterBubble.tres") :?> Material
                    | Invisible ->
                        if i <> 0f then block.Visible <- false
                        ResourceLoader.Load("res://Materials/Blue.tres") :?> Material
                
                block.GetNode<Area3D>("Area3D").add_InputEvent (fun _ event position _ _ -> TerrainManipulatorFS.Block.onInputEvent event position)
                getRoot().GetNode<Node3D>("WorldGenerator").AddChild(block)
                
        )
        
        // Platformer element placement
        elements[level]
        |> Array.iter (fun element ->
            let scene = GD.Load<PackedScene>("res://Elements/" + match element.etype with
                                                                 | Goal -> "Goal.tscn"
                                                                 | Bubble -> "WaterBubble.tscn"
                                                                 | Bridge -> "Bridge.tscn"
                                                                 | Hook -> "Hook.tscn"
                                                                 | LavaWall -> "LavaWall.tscn"
                                                                 | GoalFragment -> "GoalFragment.tscn"
                                                                 | MovingBlock -> "MovingBlock.tscn")
            let emt = scene.Instantiate() :?> Node3D
            emt.Position <- element.position
            emt.Rotation <- element.rotation
            if element.visible = false then emt.Visible <- false
            
            getRoot().GetNode<Node3D>("WorldGenerator").AddChild emt
            if element.etype = MovingBlock then MovingBlockFS.movingBlocks.Add emt
        )
        
        // Power up placement
        powerUps[level]
        |> Array.iter (fun powerUp ->
            let scene = match powerUp.ptype with
                        | GrapplingHook -> GD.Load<PackedScene>("res://Power Ups/GrapplingHook.tscn")
                        | TerrainManipulator -> GD.Load<PackedScene>("res://Power Ups/TerrainManipulator.tscn")
                        | Glasses -> GD.Load<PackedScene>("res://Power Ups/Glasses.tscn")
            let power = scene.Instantiate() :?> Node3D
            power.Position <- powerUp.position
            getRoot().GetNode<Node3D>("WorldGenerator").AddChild(power)
        )
        
    let process delta =
        ()