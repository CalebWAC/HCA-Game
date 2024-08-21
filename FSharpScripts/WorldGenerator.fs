namespace FSharpScripts

open Godot
open WorldFS
open GlobalFunctions
open System.Collections.Generic

module WorldGeneratorFS =
    let movingBlocks = List<Node3D>()
    let companionCubes = List<Node3D>()
    let cubeTriggers = List<Node3D>()
    let bridges = List<Node3D>()
    let goalFragments = List<Node3D>()
    
    let mutable audioPlayer = Unchecked.defaultof<AudioStreamPlayer>
    
    let ready () =
        // World generation
        worlds[level]
        |> Array.iter (fun data ->
            if data.material <> Cave then
                let range = if level = 16 then [| -5f .. data.position.Y |] else [| 0f .. data.position.Y |]
                for i in range do
                    let blockScene = GD.Load<PackedScene>("res://Elements/Block.tscn")
                    let block = blockScene.Instantiate() :?> Node3D
                    
                    block.Position <- Vector3(data.position.X, i, data.position.Z)
                    block.Rotation <- data.rotation
                    block.GetNode<CsgBox3D>("CSGBox3D").MaterialOverride <-
                        match data.material with
                        | Ground ->
                            if level >= 12 then
                                block.GetNode("Model").QueueFree()
                                let desModel = GD.Load<PackedScene>("res://Elements/VolcanicBlock.tscn")
                                let desBlock = desModel.Instantiate() :?> Node3D
                                block.AddChild desBlock
                                ResourceLoader.Load("res://Materials/VolcanicBlock.tres") :?> Material
                            else ResourceLoader.Load("res://Materials/Blue.tres") :?> Material
                        | Water | RushingWater ->
                            block.GetNode("Model").QueueFree()
                            block.GetNode<Node3D>("CSGBox3D").Visible <- true
                            if level < 12 then ResourceLoader.Load("res://Materials/WaterBubble.tres") :?> Material
                            else ResourceLoader.Load("res://Materials/LavaBlock.tres") :?> Material
                        | Invisible ->
                            if i <> 0f then block.Visible <- false
                            ResourceLoader.Load("res://Materials/Blue.tres") :?> Material
                        | Cave -> System.Exception "This should not be possible" |> raise
                     
                    block.GetNode<Area3D>("Area3D").add_InputEvent (fun _ event position _ _ -> TerrainManipulatorFS.Block.onInputEvent event position)
                    getRoot().GetNode<Node3D>("WorldGenerator").AddChild(block)
            else
                let blockScene = GD.Load<PackedScene>("res://Elements/Block.tscn")
                let block = blockScene.Instantiate() :?> Node3D
                if level >= 12 then
                    block.GetNode("Model").QueueFree()
                    let desModel = GD.Load<PackedScene>("res://Elements/VolcanicBlock.tscn")
                    let desBlock = desModel.Instantiate() :?> Node3D
                    block.AddChild desBlock
                block.Position <- data.position
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
                                                                 | MovingBlock ->
                                                                     if level < 12 then "MovingBlock.tscn" else "MovingBlockLava.tscn"
                                                                 | MovingBlockWithHook -> "MovingBlockWithHook.tscn"
                                                                 | CompanionCube -> "CompanionCube.tscn"
                                                                 | CubeTrigger -> "CubeTrigger.tscn"
                                                                 | DestructibleBlock -> "DestructibleBlock.tscn"
                                                                 | LavaPlume -> "LavaPlume.tscn")
            
            if element.etype = DestructibleBlock then
                for i in getHeightAt element.position.X element.position.Z .. element.position.Y do
                    if elements[level] |> Array.exists (fun e -> e.position = Vector3(element.position.X, i, element.position.Z) && e.etype <> DestructibleBlock) |> not then
                        let block = scene.Instantiate() :?> Node3D
                        block.Position <- Vector3(element.position.X, i, element.position.Z)
                        getRoot().GetNode<Node3D>("WorldGenerator").AddChild block
                        TerrainManipulatorFS.destructibleBlocks.Add block
            else
                let emt = scene.Instantiate() :?> Node3D
                emt.Position <- element.position
                emt.Rotation <- element.rotation
                if element.visible = false then emt.Visible <- false
                getRoot().GetNode<Node3D>("WorldGenerator").AddChild emt
                
                match element.etype with
                | MovingBlock | MovingBlockWithHook -> movingBlocks.Add emt
                | CompanionCube -> companionCubes.Add emt
                | CubeTrigger -> cubeTriggers.Add emt
                | Bridge -> bridges.Add emt
                | GoalFragment -> goalFragments.Add emt
                | _ -> ()
        )
        
        // Power up placement
        powerUps[level]
        |> Array.iter (fun powerUp ->
            let scene = match powerUp.ptype with
                        | GrapplingHook -> GD.Load<PackedScene>("res://Power Ups/GrapplingHook.tscn")
                        | TerrainManipulator -> GD.Load<PackedScene>("res://Power Ups/TerrainManipulator.tscn")
                        | Glasses -> GD.Load<PackedScene>("res://Power Ups/Glasses.tscn")
                        | Bomb -> GD.Load<PackedScene>("res://Power Ups/Bomb.tscn")
                        | MoonBoots -> GD.Load<PackedScene>("res://Power Ups/MoonBoots.tscn")
            let power = scene.Instantiate() :?> Node3D
            power.Position <- powerUp.position
            getRoot().GetNode<Node3D>("WorldGenerator").AddChild(power)
        )
        
        // Music playing
        try getRoot().GetNode<AudioStreamPlayer>("IntroMusic").QueueFree() with | _ -> ()
        audioPlayer <- new AudioStreamPlayer()
        audioPlayer.ProcessMode <- Node.ProcessModeEnum.Always
        GD.Print audioPlayer.ProcessMode
        getRoot().GetNode<Node3D>("WorldGenerator").AddChild(audioPlayer)
        audioPlayer.Stream <- GD.Load<AudioStream>("res://Assets/Music/ExpeditionTheme.wav")
        
    let process delta =
        if audioPlayer <> null && not audioPlayer.Playing then audioPlayer.Playing <- true