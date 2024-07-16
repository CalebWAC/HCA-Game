namespace FSharpScripts

open Godot
open WorldFS
open GlobalFunctions

module WorldGeneratorFS =
    let ready () =
        for lev in 0..7 do
            let xOffset = match lev with
                          | 0 -> 0f
                          | 1 -> 0f
                          | 2 -> 0f
                          | 3 -> 15f
                          | 4 -> 15f
                          | 5 -> 15f
                          | 6 -> -15f
                          | 7 -> -15f
                          | _ -> 0f
            let zOffset = match lev with
                          | 0 -> 0f
                          | 1 -> 15f
                          | 2 -> -15f
                          | 3 -> 0f
                          | 4 -> 15f
                          | 5 -> -15f
                          | 6 -> 0f
                          | 7 -> 15f
                          | _ -> 0f
        
            // World generation
            worlds[lev]
            |> Array.iter (fun data ->
                for i in 0f .. data.position.Y do
                    let blockScene = GD.Load<PackedScene>("res://Elements/Block.tscn")
                    let block = blockScene.Instantiate() :?> Node3D
                    
                    block.Position <- Vector3(data.position.X + xOffset, i, data.position.Z + zOffset)
                    block.GetNode<CsgBox3D>("CSGBox3D").MaterialOverride <-
                        match data.material with
                        | Ground -> ResourceLoader.Load("res://Materials/Blue.tres") :?> Material
                        | Water ->
                            block.GetNode("Model").QueueFree()
                            block.GetNode<Node3D>("CSGBox3D").Visible <- true
                            ResourceLoader.Load("res://Materials/WaterBubble.tres") :?> Material
                        | Invisible ->
                            if i <> 0f then block.Visible <- false
                            ResourceLoader.Load("res://Materials/Blue.tres") :?> Material
                    
                    block.GetNode<Area3D>("Area3D").add_InputEvent (fun _ event position _ _ -> TerrainManipulatorFS.Block.onInputEvent event position)
                    getRoot().GetNode<Node3D>("WorldGenerator").AddChild(block)
            )
            
            // Platformer element placement
            elements[lev]
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
                emt.Position <- Vector3(element.position.X + xOffset, element.position.Y, element.position.Z + zOffset)
                emt.Rotation <- element.rotation
                if element.visible = false then emt.Visible <- false
                
                getRoot().GetNode<Node3D>("WorldGenerator").AddChild emt
                if element.etype = MovingBlock then MovingBlockFS.movingBlocks.Add emt
            )
            
            // Power up placement
            powerUps[lev]
            |> Array.iter (fun powerUp ->
                let scene = match powerUp.ptype with
                            | GrapplingHook -> GD.Load<PackedScene>("res://Power Ups/GrapplingHook.tscn")
                            | TerrainManipulator -> GD.Load<PackedScene>("res://Power Ups/TerrainManipulator.tscn")
                            | Glasses -> GD.Load<PackedScene>("res://Power Ups/Glasses.tscn")
                let power = scene.Instantiate() :?> Node3D
                power.Position <- Vector3(powerUp.position.X + xOffset, powerUp.position.Y, powerUp.position.Z + zOffset)
                getRoot().GetNode<Node3D>("WorldGenerator").AddChild(power)
            )
            
            // Intermediary Blocks
            middle
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
                            block.GetNode<Node3D>("CSGBox3D").Visible <- true
                            ResourceLoader.Load("res://Materials/WaterBubble.tres") :?> Material
                        | Invisible ->
                            if i <> 0f then block.Visible <- false
                            ResourceLoader.Load("res://Materials/Blue.tres") :?> Material
                    
                    block.GetNode<Area3D>("Area3D").add_InputEvent (fun _ event position _ _ -> TerrainManipulatorFS.Block.onInputEvent event position)
                    getRoot().GetNode<Node3D>("WorldGenerator").AddChild(block))
        
        if level = 0 then
            getRoot().GetTree().Paused <- true
            getRoot().GetNode<Control>("TutorialIntro").Visible <- true
        elif level = 1 then
            getRoot().GetTree().Paused <- true
            getRoot().GetNode<Control>("TutorialWater").Visible <- true
        elif level = 2 then
            getRoot().GetTree().Paused <- true
            getRoot().GetNode<Control>("TutorialLavaWall").Visible <- true
        elif level = 5 then
            getRoot().GetTree().Paused <- true
            getRoot().GetNode<Control>("TutorialGoalFragment").Visible <- true
        
    let process delta = ()