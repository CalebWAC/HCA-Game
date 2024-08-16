namespace FSharpScripts

open Godot
open GlobalFunctions

module WorldFS =
    type BlockMaterial =
        | Ground
        | Water
        | RushingWater
        | Invisible
        | Cave
    
    type Block = {
        position : Vector3
        rotation : Vector3
        material : BlockMaterial
    }
    
    type ElementType =
        | Goal
        | Bubble
        | Bridge
        | Hook
        | LavaWall
        | GoalFragment
        | MovingBlock
        | MovingBlockWithHook
        | CompanionCube
        | CubeTrigger
        | DestructibleBlock
        | LavaPlume
        
    type PowerUpType =
        | GrapplingHook
        | TerrainManipulator
        | Glasses
        | Bomb
        
    type Element = { etype: ElementType; position: Vector3; rotation: Vector3; visible: bool }
    
    type PowerUp = { ptype: PowerUpType; position: Vector3 }
    
    let block x y z = { position = Vector3(x, y, z); rotation = Vector3.Zero; material = Ground }
    let waterBlock x y z = { position = Vector3(x, y, z); rotation = Vector3.Zero; material = Water }
    let hiddenBlock x y z = { position = Vector3(x, y, z); rotation = Vector3.Zero; material = Invisible }
    let riverBlock x y z rot = { position = Vector3(x, y, z); material = RushingWater; 
                                    rotation = (match rot with
                                                | "r" -> Vector3(0f, degToRad 180f, 0f)
                                                | "f" -> Vector3(0f, degToRad 90f, 0f)
                                                | "b" -> Vector3(0f, degToRad 270f, 0f)
                                                | _ -> Vector3(0f, 0f, 0f))  }
    let desBlock x y z = { etype = DestructibleBlock; position = Vector3(x, y, z); rotation = Vector3.Zero; visible = true }
    let caveBlock x y z = { position = Vector3(x, y, z); rotation = Vector3.Zero; material = Cave }
                
    let mutable currentWorld = 1
    let mutable level = 0
    
    let completedLevelsW1 = Array.create 12 false
    let completedLevelsW2 = Array.create 12 false
    
    let worlds = [|
        //// World 1 \\\\
        
        [| // Level 1
           block -6f 5f -6f; block -6f 0f -5f; block -6f 0f -4f; block -6f 0f -3f; block -6f 0f -2f; block -6f 0f -1f; block -6f 0f 0f; block -6f 0f 1f; block -6f 0f 2f; block -6f 0f 3f; block -6f 4f 4f; block -6f 5f 5f; block -6f 6f 6f
           block -5f 8f -6f; block -5f 5f -5f; block -5f 3f -4f; block -5f 0f -3f; block -5f 0f -2f; block -5f 0f -1f; block -5f 6f 0f; block -5f 3f 1f; block -5f 0f 2f; block -5f 0f 3f; block -5f 3f 4f; block -5f 6f 5f; block -5f 6f 6f
           block -4f 7f -6f; block -4f 4f -5f; block -4f 2f -4f; block -4f 0f -3f; block -4f 0f -2f; block -4f 0f -1f; block -4f 6f 0f; block -4f 4f 1f; block -4f 0f 2f; block -4f 0f 3f; block -4f 0f 4f; block -4f 3f 5f; block -4f 5f 6f
           block -3f 7f -6f; block -3f 4f -5f; block -3f 3f -4f; block -3f 0f -3f; block -3f 0f -2f; block -3f 0f -1f; block -3f 0f 0f; block -3f 0f 1f; block -3f 0f 2f; block -3f 0f 3f; block -3f 0f 4f; block -3f 3f 5f; block -3f 4f 6f
           block -2f 7f -6f; block -2f 3f -5f; block -2f 3f -4f; block -2f 0f -3f; block -2f 0f -2f; block -2f 0f -1f; block -2f 0f 0f; block -2f 0f 1f; block -2f 0f 2f; block -2f 0f 3f; block -2f 0f 4f; block -2f 3f 5f; block -2f 3f 6f
           block -1f 5f -6f; block -1f 3f -5f; block -1f 2f -4f; block -1f 0f -3f; block -1f 0f -2f; block -1f 5f -1f; block -1f 6f 0f; block -1f 0f 1f; block -1f 0f 2f; block -1f 0f 3f; block -1f 0f 4f; block -1f 2f 5f; block -1f 3f 6f
           block 0f 2f -6f; block 0f 2f -5f; block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 0f 2f; block 0f 0f 3f; block 0f 0f 4f; block 0f 2f 5f; block 0f 3f 6f
           block 1f 0f -6f; block 1f 0f -5f; block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 0f 1f; block 1f 0f 2f; block 1f 0f 3f; block 1f 0f 4f; block 1f 1f 5f; block 1f 2f 6f
           block 2f 0f -6f; block 2f 0f -5f; block 2f 0f -4f; block 2f 0f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 0f 0f; block 2f 0f 1f; block 2f 0f 2f; block 2f 0f 3f; block 2f 0f 4f; block 2f 1f 5f; block 2f 1f 6f
           block 3f 0f -6f; block 3f 0f -5f; block 3f 0f -4f; block 3f 0f -3f; block 3f 0f -2f; block 3f 0f -1f; block 3f 1f 0f; block 3f 2f 1f; block 3f 0f 2f; block 3f 0f 3f; block 3f 0f 4f; block 3f 0f 5f; block 3f 1f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 1f 0f; block 4f 1f 1f; block 4f 0f 2f; block 4f 0f 3f; block 4f 0f 4f; block 4f 0f 5f; block 4f 0f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 0f -3f; block 5f 0f -2f; block 5f 0f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 0f 2f; block 5f 0f 3f; block 5f 0f 4f; block 5f 0f 5f; block 5f 0f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; block 6f 0f 3f; block 6f 0f 4f; block 6f 0f 5f; block 6f 0f 6f
        |]
        [| // Level 2
           waterBlock -6f 0f -6f; waterBlock -6f 0f -5f; waterBlock -6f 0f -4f; waterBlock -6f 0f -3f; block -6f 0f -2f; block -6f 0f -1f; block -6f 0f 0f; block -6f 0f 1f; block -6f 0f 2f; block -6f 0f 3f; block -6f 0f 4f; block -6f 0f 5f; block -6f 0f 6f
           block -5f 1f -6f; block -5f 1f -5f; waterBlock -5f 0f -4f; waterBlock -5f 0f -3f; block -5f 1f -2f; block -5f 0f -1f; block -5f 0f 0f; waterBlock -5f 0f 1f; waterBlock -5f 0f 2f; waterBlock -5f 0f 3f; waterBlock -5f 0f 4f; waterBlock -5f 0f 5f; block -5f 0f 6f
           block -4f 0f -6f; block -4f 0f -5f; waterBlock -4f 0f -4f; waterBlock -4f 0f -3f; block -4f 0f -2f; block -4f 0f -1f; block -4f 0f 0f; waterBlock -4f 0f 1f; block -4f 0f 2f; block -4f 0f 3f; block -4f 0f 4f; waterBlock -4f 0f 5f; block -4f 0f 6f
           waterBlock -3f 0f -6f; waterBlock -3f 0f -5f; waterBlock -3f 0f -4f; waterBlock -3f 0f -3f; block -3f 0f -2f; block -3f 0f -1f; block -3f 0f 0f; waterBlock -3f 0f 1f; block -3f 0f 2f; block -3f 7f 3f; block -3f 0f 4f; waterBlock -3f 0f 5f; block -3f 0f 6f
           block -2f 0f -6f; block -2f 0f -5f; block -2f 0f -4f; block -2f 0f -3f; block -2f 0f -2f; block -2f 0f -1f; block -2f 0f 0f; waterBlock -2f 0f 1f; block -2f 0f 2f; block -2f 0f 3f; block -2f 0f 4f; waterBlock -2f 0f 5f; block -2f 0f 6f
           block -1f 0f -6f; block -1f 0f -5f; block -1f 0f -4f; block -1f 0f -3f; block -1f 0f -2f; block -1f 0f -1f; block -1f 0f 0f; waterBlock -1f 0f 1f; waterBlock -1f 0f 2f; block -1f 0f 3f; waterBlock -1f 0f 4f; waterBlock -1f 0f 5f; block -1f 0f 6f
           block 0f 0f -6f; block 0f 0f -5f; block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 0f 2f; block 0f 0f 3f; block 0f 0f 4f; block 0f 0f 5f; block 0f 0f 6f
           block 1f 0f -6f; block 1f 0f -5f; block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 0f 1f; block 1f 0f 2f; block 1f 0f 3f; block 1f 0f 4f; block 1f 0f 5f; block 1f 0f 6f
           block 2f 0f -6f; block 2f 4f -5f; block 2f 4f -4f; block 2f 5f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 0f 0f; block 2f 0f 1f; block 2f 0f 2f; block 2f 0f 3f; block 2f 0f 4f; block 2f 0f 5f; block 2f 0f 6f
           block 3f 0f -6f; block 3f 3f -5f; block 3f 6f -4f; block 3f 6f -3f; block 3f 0f -2f; block 3f 0f -1f; block 3f 0f 0f; block 3f 0f 1f; block 3f 0f 2f; block 3f 0f 3f; block 3f 0f 4f; block 3f 0f 5f; block 3f 0f 6f
           block 4f 0f -6f; block 4f 3f -5f; block 4f 6f -4f; block 4f 6f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 0f 0f; block 4f 0f 1f; block 4f 0f 2f; block 4f 0f 3f; block 4f 0f 4f; block 4f 0f 5f; block 4f 0f 6f
           block 5f 0f -6f; block 5f 2f -5f; block 5f 2f -4f; block 5f 1f -3f; block 5f 0f -2f; block 5f 0f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 0f 2f; block 5f 0f 3f; block 5f 0f 4f; block 5f 0f 5f; block 5f 1f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; block 6f 0f 3f; block 6f 0f 4f; block 6f 1f 5f; block 6f 2f 6f
        |]
        [| // Level 3
           block -6f 5f -6f; block -6f 5f -5f; block -6f 5f -4f; waterBlock -6f 0f -3f; block -6f 0f -2f; block -6f 5f -1f; block -6f 4f 0f; waterBlock -6f 0f 1f; waterBlock -6f 0f 2f; waterBlock -6f 0f 3f; waterBlock -6f 0f 4f; waterBlock -6f 0f 5f; waterBlock -6f 0f 6f
           block -5f 5f -6f; block -5f 6f -5f; block -5f 5f -4f; waterBlock -5f 0f -3f; block -5f 1f -2f; block -5f 6f -1f; block -5f 3f 0f; waterBlock -5f 0f 1f; block -5f 5f 2f; block -5f 5f 3f; block -5f 5f 4f; waterBlock -5f 0f 5f; waterBlock -5f 0f 6f
           block -4f 5f -6f; block -4f 5f -5f; block -4f 5f -4f; waterBlock -4f 0f -3f; block -4f 1f -2f; block -4f 1f -1f; block -4f 2f 0f; waterBlock -4f 0f 1f; waterBlock -4f 0f 2f; waterBlock -4f 0f 3f; waterBlock -4f 0f 4f; waterBlock -4f 0f 5f; waterBlock -4f 0f 6f
           waterBlock -3f 0f -6f; waterBlock -3f 0f -5f; waterBlock -3f 0f -4f; waterBlock -3f 0f -3f; block -3f 0f -2f; block -3f 1f -1f; waterBlock -3f 0f 0f; waterBlock -3f 0f 1f; waterBlock -3f 0f 2f; waterBlock -3f 0f 3f; waterBlock -3f 0f 4f; block -3f 5f 5f; waterBlock -3f 0f 6f
           waterBlock -2f 0f -6f; waterBlock -2f 0f -5f; waterBlock -2f 0f -4f; waterBlock -2f 0f -3f; block -2f 0f -2f; block -2f 0f -1f; waterBlock -2f 0f 0f; waterBlock -2f 0f 1f; waterBlock -2f 0f 2f; waterBlock -2f 0f 3f; waterBlock -2f 0f 4f; block -2f 5f 5f; waterBlock -2f 0f 6f
           waterBlock -1f 0f -6f; block -1f 3f -5f; block -1f 3f -4f; waterBlock -1f 0f -3f; block -1f 0f -2f; waterBlock -1f 0f -1f; waterBlock -1f 0f 0f; waterBlock -1f 0f 1f; block -1f 5f 2f; waterBlock -1f 0f 3f; waterBlock -1f 0f 4f; waterBlock -1f 0f 5f; waterBlock -1f 0f 6f
           waterBlock 0f 0f -6f; block 0f 2f -5f; block 0f 1f -4f; block 0f 0f -3f; block 0f 0f -2f; waterBlock 0f 0f -1f; waterBlock 0f 0f 0f; waterBlock 0f 0f 1f; block 0f 5f 2f; waterBlock 0f 0f 3f; waterBlock 0f 0f 4f; waterBlock 0f 0f 5f; waterBlock 0f 0f 6f
           waterBlock 1f 0f -6f; waterBlock 1f 0f -5f; waterBlock 1f 0f -4f; waterBlock 1f 0f -3f; waterBlock 1f 0f -2f; waterBlock 1f 0f -1f; waterBlock 1f 0f 0f; waterBlock 1f 0f 1f; block 1f 5f 2f; waterBlock 1f 0f 3f; waterBlock 1f 0f 4f; block 1f 5f 5f; waterBlock 1f 0f 6f
           block 2f 5f -6f; block 2f 5f -5f; block 2f 5f -4f; block 2f 5f -3f; block 2f 5f -2f; block 2f 5f -1f; block 2f 5f 0f; waterBlock 2f 0f 1f; block 2f 5f 2f; waterBlock 2f 0f 3f; waterBlock 2f 0f 4f; block 2f 5f 5f; waterBlock 2f 0f 6f
           block 3f 0f -6f; block 3f 0f -5f; block 3f 0f -4f; block 3f 0f -3f; waterBlock 3f 0f -2f; waterBlock 3f 0f -1f; waterBlock 3f 0f 0f; waterBlock 3f 0f 1f; waterBlock 3f 0f 2f; waterBlock 3f 0f 3f; waterBlock 3f 0f 4f; block 3f 5f 5f; waterBlock 3f 0f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; waterBlock 4f 0f -2f; waterBlock 4f 0f -1f; waterBlock 4f 0f 0f; waterBlock 4f 0f 1f; waterBlock 4f 0f 2f; waterBlock 4f 0f 3f; waterBlock 4f 0f 4f; block 4f 5f 5f; waterBlock 4f 0f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 0f -3f; waterBlock 5f 0f -2f; waterBlock 5f 0f -1f; waterBlock 5f 0f 0f; waterBlock 5f 0f 1f; waterBlock 5f 0f 2f; waterBlock 5f 0f 3f; waterBlock 5f 0f 4f; block 5f 5f 5f; waterBlock 5f 0f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 5f -3f; block 6f 5f -2f; block 6f 5f -1f; block 6f 5f 0f; block 6f 5f 1f; block 6f 5f 2f; block 6f 5f 3f; waterBlock 6f 0f 4f; waterBlock 6f 0f 5f; waterBlock 6f 0f 6f
        |]
        [| // Level 4
           block -6f 0f -6f; block -6f 0f -5f; block -6f 1f -4f; block -6f 0f -3f; block -6f 0f -2f; block -6f 1f -1f; block -6f 2f 0f; block -6f 3f 1f; block -6f 5f 2f; block -6f 5f 3f; block -6f 6f 4f; block -6f 6f 5f; block -6f 0f 6f
           block -5f 0f -6f; block -5f 0f -5f; block -5f 0f -4f; block -5f 0f -3f; block -5f 0f -2f; block -5f 1f -1f; block -5f 1f 0f; block -5f 3f 1f; block -5f 4f 2f; block -5f 5f 3f; block -5f 6f 4f; block -5f 7f 5f; block -5f 0f 6f
           block -4f 0f -6f; block -4f 0f -5f; block -4f 0f -4f; block -4f 0f -3f; block -4f 0f -2f; block -4f 0f -1f; block -4f 0f 0f; block -4f 2f 1f; block -4f 3f 2f; block -4f 3f 3f; block -4f 6f 4f; block -4f 7f 5f; block -4f 0f 6f
           block -3f 0f -6f; block -3f 0f -5f; block -3f 0f -4f; block -3f 0f -3f; block -3f 0f -2f; block -3f 0f -1f; block -3f 0f 0f; block -3f 0f 1f; block -3f 0f 2f; block -3f 0f 3f; waterBlock -3f 0f 4f; waterBlock -3f 0f 5f; waterBlock -3f 0f 6f
           block -2f 0f -6f; block -2f 0f -5f; block -2f 0f -4f; block -2f 0f -3f; block -2f 0f -2f; block -2f 0f -1f; block -2f 0f 0f; block -2f 0f 1f; block -2f 0f 2f; block -2f 0f 3f; waterBlock -2f 0f 4f; block -2f 0f 5f; waterBlock -2f 0f 6f
           block -1f 0f -6f; block -1f 0f -5f; block -1f 0f -4f; block -1f 0f -3f; block -1f 0f -2f; block -1f 0f -1f; block -1f 0f 0f; block -1f 0f 1f; block -1f 0f 2f; block -1f 0f 3f; waterBlock -1f 0f 4f; block -1f 0f 5f; waterBlock -1f 0f 6f
           block 0f 0f -6f; block 0f 0f -5f; block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 0f 2f; block 0f 0f 3f; waterBlock 0f 0f 4f; block 0f 0f 5f; waterBlock 0f 0f 6f
           block 1f 0f -6f; block 1f 0f -5f; block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 0f 1f; block 1f 0f 2f; block 1f 0f 3f; waterBlock 1f 0f 4f; waterBlock 1f 0f 5f; waterBlock 1f 0f 6f
           block 2f 0f -6f; block 2f 0f -5f; block 2f 0f -4f; block 2f 0f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 0f 0f; block 2f 1f 1f; block 2f 1f 2f; block 2f 0f 3f; waterBlock 2f 0f 4f; block 2f 7f 5f; waterBlock 2f 0f 6f
           block 3f 0f -6f; block 3f 0f -5f; block 3f 0f -4f; block 3f 0f -3f; block 3f 0f -2f; block 3f 0f -1f; block 3f 1f 0f; block 3f 2f 1f; block 3f 1f 2f; block 3f 0f 3f; waterBlock 3f 0f 4f; waterBlock 3f 0f 5f; waterBlock 3f 0f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 0f 0f; block 4f 1f 1f; block 4f 0f 2f; block 4f 0f 3f; waterBlock 4f 0f 4f; waterBlock 4f 0f 5f; waterBlock 4f 0f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 0f -3f; block 5f 0f -2f; block 5f 0f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 0f 2f; waterBlock 5f 0f 3f; waterBlock 5f 0f 4f; waterBlock 5f 0f 5f; waterBlock 5f 0f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; waterBlock 6f 0f 2f; waterBlock 6f 0f 3f; waterBlock 6f 0f 4f; block 6f 7f 5f; block 6f 7f 6f
        |]
        [| // Level 5
           block -6f 7f -6f; block -6f 7f -5f; block -6f 0f -4f; block -6f 0f -3f; block -6f 0f -2f; block -6f 0f -1f; block -6f 0f 0f; block -6f 0f 1f; block -6f 0f 2f; block -6f 0f 3f; block -6f 0f 4f; block -6f 0f 5f; waterBlock -6f 0f 6f
           waterBlock -5f 6f -6f; waterBlock -5f 7f -5f; block -5f 0f -4f; waterBlock -5f 0f -3f; block -5f 0f -2f; waterBlock -5f 0f -1f; waterBlock -5f 0f 0f; waterBlock -5f 0f 1f; waterBlock -5f 0f 2f; waterBlock -5f 0f 3f; waterBlock -5f 0f 4f; block -5f 0f 5f; waterBlock -5f 0f 6f
           block -4f 5f -6f; waterBlock -4f 0f -5f; block -4f 0f -4f; block -4f 0f -3f; block -4f 0f -2f; block -4f 0f -1f; block -4f 5f 0f; waterBlock -4f 0f 1f; waterBlock -4f 0f 2f; block -4f 5f 3f; waterBlock -4f 0f 4f; block -4f 0f 5f; block -4f 5f 6f
           waterBlock -3f 0f -6f; waterBlock -3f 0f -5f; block -3f 0f -4f; block -3f 0f -3f; waterBlock -3f 0f -2f; waterBlock -3f 0f -1f; waterBlock -3f 0f 0f; waterBlock -3f 0f 1f; waterBlock -3f 0f 2f; waterBlock -3f 0f 3f; waterBlock -3f 0f 4f; block -3f 0f 5f; waterBlock -3f 0f 6f
           waterBlock -2f 0f -6f; waterBlock -2f 0f -5f; block -2f 0f -4f; waterBlock -2f 0f -3f; block -2f 0f -2f; block -2f 0f -1f; block -2f 0f 0f; block -2f 0f 1f; block -2f 0f 2f; block -2f 0f 3f; waterBlock -2f 0f 4f; block -2f 0f 5f; waterBlock -2f 0f 6f
           block -1f 5f -6f; waterBlock -1f 0f -5f; block -1f 0f -4f; block -1f 5f -3f; block -1f 0f -2f; waterBlock -1f 0f -1f; block -1f 5f 0f; waterBlock -1f 0f 1f; waterBlock -1f 0f 2f; block -1f 0f 3f; waterBlock -1f 0f 4f; block -1f 0f 5f; block -1f 5f 6f
           block 0f 0f -6f; waterBlock 0f 0f -5f; block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; waterBlock 0f 0f -1f; waterBlock 0f 0f 0f; waterBlock 0f 0f 1f; waterBlock 0f 0f 2f; block 0f 0f 3f; waterBlock 0f 0f 4f; block 0f 0f 5f; waterBlock 0f 0f 6f
           block 1f 0f -6f; waterBlock 1f 0f -5f; block 1f 0f -4f; waterBlock 1f 0f -3f; waterBlock 1f 0f -2f; waterBlock 1f 0f -1f; block 1f 0f 0f; block 1f 0f 1f; block 1f 0f 2f; block 1f 0f 3f; waterBlock 1f 0f 4f; block 1f 0f 5f; waterBlock 1f 0f 6f
           block 2f 0f -6f; block 2f 0f -5f; block 2f 0f -4f; block 2f 5f -3f; waterBlock 2f 0f -2f; waterBlock 2f 0f -1f; block 2f 0f 0f; waterBlock 2f 0f 1f; waterBlock 2f 0f 2f; block 2f 5f 3f; waterBlock 2f 0f 4f; block 2f 0f 5f; block 2f 5f 6f
           waterBlock 3f 0f -6f; waterBlock 3f 0f -5f; waterBlock 3f 0f -4f; waterBlock 3f 0f -3f; waterBlock 3f 0f -2f; waterBlock 3f 0f -1f; block 3f 0f 0f; block 3f 0f 1f; block 3f 0f 2f; waterBlock 3f 0f 3f; block 3f 0f 4f; block 3f 0f 5f; waterBlock 3f 0f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 0f 0f; waterBlock 4f 0f 1f; block 4f 0f 2f; block 4f 0f 3f; waterBlock 4f 0f 4f; block 4f 1f 5f; waterBlock 4f 0f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 5f -3f; block 5f 0f -2f; waterBlock 5f 0f -1f; block 5f 5f 0f; waterBlock 5f 0f 1f; block 5f 0f 2f; block 5f 5f 3f; waterBlock 5f 0f 4f; block 5f 2f 5f; block 5f 5f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; waterBlock 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; waterBlock 6f 0f 3f; waterBlock 6f 0f 4f; block 6f 3f 5f; block 6f 4f 6f
        |]
        [| // Level 6
           block -6f 0f -6f; block -6f 0f -5f; block -6f 0f -4f; block -6f 0f -3f; block -6f 0f -2f; block -6f 0f -1f; block -6f 0f 0f; block -6f 0f 1f; block -6f 0f 2f; hiddenBlock -6f 1f 3f; hiddenBlock -6f 2f 4f; hiddenBlock -6f 3f 5f; hiddenBlock -6f 4f 6f
           block -5f 0f -6f; block -5f 0f -5f; block -5f 0f -4f; hiddenBlock -5f 4f -3f; hiddenBlock -5f 4f -2f; hiddenBlock -5f 4f -1f; hiddenBlock -5f 3f 0f; block -5f 0f 1f; block -5f 0f 2f; hiddenBlock -5f 1f 3f; hiddenBlock -5f 2f 4f; hiddenBlock -5f 3f 5f; hiddenBlock -5f 3f 6f
           block -4f 0f -6f; block -4f 0f -5f; block -4f 0f -4f; hiddenBlock -4f 3f -3f; block -4f 0f -2f; hiddenBlock -4f 2f -1f; hiddenBlock -4f 3f 0f; hiddenBlock -4f 3f 1f; block -4f 0f 2f; hiddenBlock -4f 1f 3f; hiddenBlock -4f 2f 4f; hiddenBlock -4f 2f 5f; hiddenBlock -4f 2f 6f
           block -3f 0f -6f; block -3f 0f -5f; block -3f 0f -4f; hiddenBlock -3f 4f -3f; block -3f 0f -2f; hiddenBlock -3f 1f -1f; hiddenBlock -3f 2f 0f; hiddenBlock -3f 1f 1f; block -3f 0f 2f; hiddenBlock -3f 1f 3f; hiddenBlock -3f 1f 4f; hiddenBlock -3f 1f 5f; hiddenBlock -3f 1f 6f
           hiddenBlock -2f 6f -6f; hiddenBlock -2f 6f -5f; hiddenBlock -2f 5f -4f; hiddenBlock -2f 3f -3f; block -2f 0f -2f; hiddenBlock -2f 1f -1f; hiddenBlock -2f 1f 0f; block -2f 0f 1f; block -2f 0f 2f; block -2f 0f 3f; block -2f 0f 4f; block -2f 0f 5f; block -2f 0f 6f
           block -1f 0f -6f; block -1f 0f -5f; hiddenBlock -1f 5f -4f; hiddenBlock -1f 3f -3f; block -1f 0f -2f; block -1f 0f -1f; block -1f 0f 0f; block -1f 0f 1f; block -1f 0f 2f; block -1f 0f 3f; block -1f 0f 4f; block -1f 0f 5f; block -1f 0f 6f
           block 0f 0f -6f; block 0f 0f -5f; hiddenBlock 0f 4f -4f; hiddenBlock 0f 3f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 0f 2f; block 0f 0f 3f; block 0f 0f 4f; block 0f 0f 5f; block 0f 0f 6f
           block 1f 0f -6f; hiddenBlock 1f 1f -5f; hiddenBlock 1f 2f -4f; hiddenBlock 1f 2f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 0f 1f; block 1f 0f 2f; block 1f 0f 3f; block 1f 0f 4f; block 1f 0f 5f; block 1f 0f 6f
           block 2f 0f -6f; hiddenBlock 2f 1f -5f; hiddenBlock 2f 1f -4f; block 2f 0f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 0f 0f; block 2f 0f 1f; block 2f 0f 2f; block 2f 0f 3f; block 2f 0f 4f; block 2f 0f 5f; hiddenBlock 2f 5f 6f
           block 3f 0f -6f; block 3f 0f -5f; block 3f 0f -4f; block 3f 0f -3f; block 3f 0f -2f; block 3f 0f -1f; block 3f 0f 0f; block 3f 0f 1f; block 3f 0f 2f; hiddenBlock 3f 1f 3f; block 3f 0f 4f; block 3f 0f 5f; hiddenBlock 3f 5f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; block 4f 0f -2f; hiddenBlock 4f 2f -1f; block 4f 0f 0f; block 4f 0f 1f; block 4f 0f 2f; hiddenBlock 4f 2f 3f; hiddenBlock 4f 2f 4f; block 4f 0f 5f; hiddenBlock 4f 5f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 0f -3f; block 5f 0f -2f; hiddenBlock 5f 1f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 0f 2f; hiddenBlock 5f 3f 3f; hiddenBlock 5f 3f 4f; hiddenBlock 5f 4f 5f; hiddenBlock 5f 5f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; hiddenBlock 6f 4f 3f; hiddenBlock 6f 4f 4f; hiddenBlock 6f 4f 5f; hiddenBlock 6f 5f 6f
        |]
        [| // Level 7
           block -6f 0f -6f; block -6f 0f -5f; block -6f 0f -4f; block -6f 0f -3f; block -6f 0f -2f; block -6f 0f -1f; block -6f 0f 0f; block -6f 0f 1f; block -6f 0f 2f; block -6f 6f 3f; block -6f 5f 4f; block -6f 6f 5f; block -6f 7f 6f
           block -5f 5f -6f; block -5f 5f -5f; block -5f 5f -4f; block -5f 4f -3f; block -5f 0f -2f; block -5f 0f -1f; block -5f 0f 0f; block -5f 0f 1f; block -5f 0f 2f; block -5f 5f 3f; block -5f 5f 4f; block -5f 5f 5f; block -5f 5f 6f
           block -4f 6f -6f; block -4f 1f -5f; block -4f 1f -4f; block -4f 3f -3f; block -4f 0f -2f; block -4f 0f -1f; block -4f 0f 0f; block -4f 0f 1f; block -4f 0f 2f; block -4f 5f 3f; block -4f 5f 4f; block -4f 4f 5f; block -4f 4f 6f
           block -3f 6f -6f; block -3f 1f -5f; block -3f 1f -4f; block -3f 2f -3f; block -3f 0f -2f; block -3f 0f -1f; block -3f 0f 0f; block -3f 0f 1f; block -3f 0f 2f; block -3f 4f 3f; block -3f 4f 4f; block -3f 4f 5f; block -3f 4f 6f
           block -2f 6f -6f; block -2f 1f -5f; block -2f 2f -4f; block -2f 2f -3f; block -2f 0f -2f; block -2f 0f -1f; block -2f 0f 0f; block -2f 0f 1f; block -2f 0f 2f; block -2f 0f 3f; block -2f 2f 4f; block -2f 3f 5f; block -2f 3f 6f
           block -1f 7f -6f; block -1f 0f -5f; block -1f 0f -4f; block -1f 0f -3f; block -1f 0f -2f; block -1f 0f -1f; block -1f 0f 0f; block -1f 0f 1f; block -1f 0f 2f; block -1f 0f 3f; block -1f 2f 4f; block -1f 3f 5f; block -1f 3f 6f
           block 0f 7f -6f; block 0f 7f -5f; block 0f 8f -4f; block 0f 8f -3f; block 0f 1f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 0f 2f; block 0f 0f 3f; block 0f 0f 4f; block 0f 3f 5f; block 0f 4f 6f
           block 1f 0f -6f; block 1f 0f -5f; block 1f 0f -4f; block 1f 0f -3f; block 1f 1f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 0f 1f; block 1f 0f 2f; block 1f 0f 3f; block 1f 0f 4f; block 1f 0f 5f; block 1f 0f 6f
           block 2f 0f -6f; block 2f 0f -5f; block 2f 0f -4f; block 2f 0f -3f; block 2f 1f -2f; block 2f 0f -1f; block 2f 0f 0f; block 2f 0f 1f; block 2f 0f 2f; block 2f 0f 3f; block 2f 0f 4f; block 2f 0f 5f; block 2f 0f 6f
           block 3f 0f -6f; block 3f 0f -5f; block 3f 0f -4f; block 3f 0f -3f; block 3f 1f -2f; block 3f 0f -1f; block 3f 0f 0f; block 3f 0f 1f; block 3f 0f 2f; block 3f 0f 3f; block 3f 0f 4f; block 3f 0f 5f; block 3f 0f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; block 4f 1f -2f; block 4f 0f -1f; block 4f 0f 0f; block 4f 0f 1f; block 4f 0f 2f; block 4f 0f 3f; block 4f 0f 4f; block 4f 0f 5f; block 4f 0f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 0f -3f; block 5f 1f -2f; block 5f 0f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 0f 2f; block 5f 0f 3f; block 5f 0f 4f; block 5f 0f 5f; block 5f 0f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 1f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; block 6f 0f 3f; block 6f 0f 4f; block 6f 0f 5f; block 6f 0f 6f
        |]
        [| // Level 8
           block -6f 6f -6f; block -6f 5f -5f; block -6f 4f -4f; block -6f 3f -3f; block -6f 2f -2f; block -6f 1f -1f; block -6f 1f 0f; block -6f 1f 1f; block -6f 2f 2f; block -6f 2f 3f; block -6f 4f 4f; block -6f 6f 5f; block -6f 7f 6f
           block -5f 5f -6f; block -5f 4f -5f; block -5f 4f -4f; block -5f 2f -3f; block -5f 0f -2f; block -5f 0f -1f; block -5f 0f 0f; block -5f 0f 1f; block -5f 0f 2f; block -5f 2f 3f; block -5f 5f 4f; block -5f 5f 5f; block -5f 7f 6f
           block -4f 5f -6f; block -4f 4f -5f; block -4f 3f -4f; block -4f 0f -3f; block -4f 0f -2f; block -4f 0f -1f; block -4f 0f 0f; block -4f 0f 1f; block -4f 0f 2f; block -4f 2f 3f; block -4f 5f 4f; block -4f 5f 5f; block -4f 6f 6f
           block -3f 5f -6f; block -3f 4f -5f; block -3f 3f -4f; block -3f 0f -3f; block -3f 0f -2f; block -3f 0f -1f; block -3f 0f 0f; block -3f 0f 1f; block -3f 0f 2f; block -3f 1f 3f; block -3f 5f 4f; block -3f 5f 5f; block -3f 6f 6f
           block -2f 5f -6f; block -2f 3f -5f; block -2f 2f -4f; block -2f 0f -3f; block -2f 0f -2f; block -2f 0f -1f; block -2f 0f 0f; block -2f 0f 1f; block -2f 0f 2f; block -2f 0f 3f; block -2f 5f 4f; block -2f 6f 5f; block -2f 6f 6f
           block -1f 5f -6f; block -1f 3f -5f; block -1f 2f -4f; block -1f 0f -3f; block -1f 0f -2f; block -1f 0f -1f; block -1f 0f 0f; block -1f 0f 1f; block -1f 0f 2f; block -1f 0f 3f; block -1f 5f 4f; block -1f 6f 5f; block -1f 6f 6f
           block 0f 4f -6f; block 0f 3f -5f; block 0f 3f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 0f 2f; block 0f 0f 3f; block 0f 5f 4f; block 0f 6f 5f; block 0f 7f 6f
           block 1f 4f -6f; block 1f 3f -5f; block 1f 3f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 0f 1f; block 1f 0f 2f; block 1f 0f 3f; block 1f 4f 4f; block 1f 6f 5f; block 1f 7f 6f
           block 2f 4f -6f; block 2f 4f -5f; block 2f 3f -4f; block 2f 0f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 0f 0f; block 2f 0f 1f; block 2f 0f 2f; block 2f 0f 3f; block 2f 4f 4f; block 2f 6f 5f; block 2f 7f 6f
           block 3f 5f -6f; block 3f 4f -5f; block 3f 2f -4f; block 3f 0f -3f; block 3f 0f -2f; block 3f 0f -1f; block 3f 0f 0f; block 3f 0f 1f; block 3f 0f 2f; block 3f 0f 3f; block 3f 3f 4f; block 3f 6f 5f; block 3f 7f 6f
           block 4f 5f -6f; block 4f 4f -5f; block 4f 2f -4f; block 4f 2f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 0f 0f; block 4f 0f 1f; block 4f 0f 2f; block 4f 2f 3f; block 4f 3f 4f; block 4f 5f 5f; block 4f 7f 6f
           block 5f 5f -6f; block 5f 4f -5f; block 5f 3f -4f; block 5f 2f -3f; block 5f 0f -2f; block 5f 0f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 0f 2f; block 5f 1f 3f; block 5f 3f 4f; block 5f 5f 5f; block 5f 7f 6f
           block 6f 5f -6f; block 6f 4f -5f; block 6f 3f -4f; block 6f 2f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; block 6f 1f 3f; block 6f 4f 4f; block 6f 5f 5f; block 6f 6f 6f
        |]
        [| // Level 9
           block -6f 0f -6f; block -6f 0f -5f; block -6f 0f -4f; block -6f 0f -3f; block -6f 0f -2f; block -6f 0f -1f; block -6f 1f 0f; block -6f 1f 1f; block -6f 2f 2f; block -6f 1f 3f; block -6f 1f 4f; block -6f 0f 5f; block -6f 0f 6f
           block -5f 0f -6f; block -5f 0f -5f; block -5f 0f -4f; block -5f 0f -3f; block -5f 0f -2f; block -5f 0f -1f; block -5f 1f 0f; block -5f 2f 1f; block -5f 3f 2f; block -5f 3f 3f; block -5f 2f 4f; block -5f 0f 5f; block -5f 0f 6f
           block -4f 0f -6f; block -4f 0f -5f; block -4f 0f -4f; block -4f 0f -3f; block -4f 0f -2f; block -4f 0f -1f; block -4f 2f 0f; block -4f 2f 1f; block -4f 3f 2f; block -4f 3f 3f; block -4f 1f 4f; block -4f 0f 5f; block -4f 0f 6f
           block -3f 0f -6f; block -3f 0f -5f; block -3f 0f -4f; block -3f 0f -3f; block -3f 0f -2f; block -3f 0f -1f; block -3f 2f 0f; block -3f 3f 1f; block -3f 4f 2f; block -3f 2f 3f; block -3f 1f 4f; block -3f 0f 5f; block -3f 0f 6f
           block -2f 0f -6f; block -2f 0f -5f; block -2f 0f -4f; block -2f 0f -3f; block -2f 0f -2f; block -2f 0f -1f; block -2f 4f 0f; block -2f 4f 1f; block -2f 4f 2f; block -2f 0f 3f; block -2f 0f 4f; block -2f 0f 5f; block -2f 0f 6f
           block -1f 0f -6f; block -1f 0f -5f; block -1f 0f -4f; block -1f 0f -3f; block -1f 0f -2f; block -1f 0f -1f; block -1f 5f 0f; block -1f 0f 1f; block -1f 0f 2f; block -1f 0f 3f; block -1f 0f 4f; block -1f 5f 5f; block -1f 0f 6f
           block 0f 0f -6f; block 0f 0f -5f; block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 5f 0f; block 0f 0f 1f; block 0f 0f 2f; block 0f 0f 3f; block 0f 0f 4f; block 0f 2f 5f; block 0f 2f 6f
           block 1f 0f -6f; block 1f 0f -5f; block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 3f 0f; block 1f 0f 1f; block 1f 0f 2f; block 1f 1f 3f; block 1f 1f 4f; block 1f 1f 5f; block 1f 6f 6f
           block 2f 0f -6f; block 2f 0f -5f; block 2f 0f -4f; block 2f 0f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 3f 0f; block 2f 0f 1f; block 2f 0f 2f; block 2f 1f 3f; block 2f 2f 4f; block 2f 2f 5f; block 2f 2f 6f
           block 3f 5f -6f; block 3f 5f -5f; block 3f 5f -4f; block 3f 5f -3f; block 3f 7f -2f; block 3f 6f -1f; block 3f 5f 0f; block 3f 4f 1f; block 3f 4f 2f; block 3f 1f 3f; block 3f 4f 4f; block 3f 4f 5f; block 3f 4f 6f
           block 4f 0f -6f; block 4f 1f -5f; block 4f 1f -4f; block 4f 2f -3f; block 4f 3f -2f; block 4f 4f -1f; block 4f 4f 0f; block 4f 4f 1f; block 4f 0f 2f; block 4f 2f 3f; block 4f 4f 4f; block 4f 5f 5f; block 4f 6f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 1f -3f; block 5f 1f -2f; block 5f 2f -1f; block 5f 2f 0f; block 5f 3f 1f; block 5f 0f 2f; block 5f 2f 3f; block 5f 4f 4f; block 5f 6f 5f; block 5f 6f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; block 6f 2f 3f; block 6f 5f 4f; block 6f 6f 5f; block 6f 6f 6f
        |]
        [| // Level 10
           block -6f 0f -6f; block -6f 0f -5f; block -6f 1f -4f; block -6f 2f -3f; block -6f 0f -2f; block -6f 2f -1f; block -6f 1f 0f; block -6f 0f 1f; block -6f 0f 2f; block -6f 0f 3f; block -6f 0f 4f; block -6f 0f 5f; block -6f 0f 6f
           block -5f 0f -6f; block -5f 0f -5f; block -5f 1f -4f; block -5f 2f -3f; block -5f 0f -2f; block -5f 2f -1f; block -5f 1f 0f; block -5f 0f 1f; block -5f 0f 2f; block -5f 0f 3f; block -5f 0f 4f; block -5f 0f 5f; block -5f 0f 6f
           block -4f 0f -6f; block -4f 0f -5f; block -4f 1f -4f; block -4f 2f -3f; block -4f 0f -2f; block -4f 2f -1f; block -4f 1f 0f; block -4f 0f 1f; block -4f 0f 2f; block -4f 0f 3f; block -4f 0f 4f; block -4f 0f 5f ; block -4f 0f 6f
           block -3f 0f -6f; block -3f 0f -5f; block -3f 1f -4f; block -3f 2f -3f; block -3f 0f -2f; block -3f 2f -1f; block -3f 1f 0f; block -3f 0f 1f; block -3f 0f 2f; riverBlock -3f 0f 3f "f"; riverBlock -3f 0f 4f "r"; riverBlock -3f 0f 5f "r"; block -3f 1f 6f
           block -2f 0f -6f; block -2f 0f -5f; block -2f 1f -4f; block -2f 2f -3f; block -2f 0f -2f; block -2f 2f -1f; block -2f 1f 0f; block -2f 0f 1f; block -2f 0f 2f; riverBlock -2f 0f 3f "f"; block -2f 0f 4f; riverBlock -2f 0f 5f "b"; block -2f 2f 6f
           block -1f 0f -6f; block -1f 0f -5f; block -1f 1f -4f; block -1f 2f -3f; block -1f 0f -2f; block -1f 2f -1f; block -1f 1f 0f; block -1f 0f 1f; block -1f 0f 2f; riverBlock -1f 0f 3f "f"; riverBlock -1f 0f 4f "b"; riverBlock -1f 0f 5f "b"; block -1f 3f 6f
           block 0f 0f -6f; block 0f 0f -5f; block 0f 1f -4f; block 0f 2f -3f; block 0f 0f -2f; block 0f 2f -1f; block 0f 1f 0f; block 0f 0f 1f; block 0f 0f 2f; riverBlock 0f 0f 3f "f"; riverBlock 0f 0f 4f "b"; riverBlock 0f 0f 5f "b"; block 0f 3f 6f
           riverBlock 1f 0f -6f "l"; riverBlock 1f 0f -5f "l"; riverBlock 1f 0f -4f "l"; riverBlock 1f 0f -3f "l"; riverBlock 1f 0f -2f "l"; riverBlock 1f 0f -1f "l"; riverBlock 1f 0f 0f "l"; riverBlock 1f 0f 1f "l"; riverBlock 1f 0f 2f "l"; riverBlock 1f 0f 3f "l"; riverBlock 1f 0f 4f "b"; riverBlock 1f 0f 5f "b"; riverBlock 1f 0f 6f "r"
           riverBlock 2f 0f -6f "l"; riverBlock 2f 0f -5f "l"; riverBlock 2f 0f -4f "l"; riverBlock 2f 0f -3f "l"; riverBlock 2f 0f -2f "l"; riverBlock 2f 0f -1f "l"; riverBlock 2f 0f 0f "l"; riverBlock 2f 0f 1f "l"; riverBlock 2f 0f 2f "l"; riverBlock 2f 0f 3f "l"; riverBlock 2f 0f 4f "b"; riverBlock 2f 0f 5f "b"; riverBlock 2f 0f 6f "r"
           block 3f 3f -6f; block 3f 3f -5f; block 3f 3f -4f; block 3f 3f -3f; block 3f 3f -2f; block 3f 3f -1f; block 3f 3f 0f; block 3f 3f 1f; block 3f 3f 2f; block 3f 3f 3f; block 3f 3f 4f; block 3f 3f 5f; block 3f 3f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 0f 0f; block 4f 1f 1f; block 4f 1f 2f; block 4f 1f 3f; block 4f 1f 4f; block 4f 1f 5f; block 4f 1f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 0f -3f; block 5f 0f -2f; block 5f 0f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 0f 2f; block 5f 0f 3f; block 5f 0f 4f; block 5f 0f 5f; block 5f 0f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; block 6f 0f 3f; block 6f 0f 4f; block 6f 0f 5f; block 6f 0f 6f
        |]
        [| // Level 11
           block -6f 0f -6f; block -6f 0f -5f; block -6f 0f -4f; block -6f 0f -3f; block -6f 0f -2f; block -6f 0f -1f; block -6f 0f 0f; block -6f 0f 1f; block -6f 0f 2f; block -6f 0f 3f; block -6f 0f 4f; block -6f 0f 5f; block -6f 0f 6f
           block -5f 0f -6f; block -5f 6f -5f; block -5f 0f -4f; block -5f 0f -3f; block -5f 0f -2f; block -5f 0f -1f; block -5f 0f 0f; block -5f 0f 1f; block -5f 0f 2f; block -5f 0f 3f; block -5f 0f 4f; block -5f 0f 5f; block -5f 0f 6f
           block -4f 0f -6f; block -4f 0f -5f; block -4f 0f -4f; block -4f 0f -3f; block -4f 0f -2f; block -4f 0f -1f; block -4f 3f 0f; block -4f 3f 1f; block -4f 3f 2f; block -4f 3f 3f; block -4f 3f 4f; block -4f 0f 5f; block -4f 0f 6f
           block -3f 0f -6f; block -3f 0f -5f; block -3f 0f -4f; block -3f 0f -3f; block -3f 0f -2f; block -3f 0f -1f; block -3f 3f 0f; block -3f 0f 1f; block -3f 0f 2f; block -3f 0f 3f; block -3f 3f 4f; block -3f 0f 5f; block -3f 0f 6f
           block -2f 0f -6f; block -2f 0f -5f; block -2f 0f -4f; block -2f 0f -3f; block -2f 0f -2f; block -2f 0f -1f; block -2f 3f 0f; block -2f 0f 1f; block -2f 0f 2f; block -2f 0f 3f; block -2f 3f 4f; block -2f 0f 5f; block -2f 0f 6f
           block -1f 3f -6f; block -1f 3f -5f; block -1f 3f -4f; block -1f 3f -3f; block -1f 3f -2f; block -1f 3f -1f; block -1f 3f 0f; block -1f 0f 1f; block -1f 0f 2f; block -1f 0f 3f; block -1f 3f 4f; block -1f 0f 5f; block -1f 0f 6f
           block 0f 3f -6f; block 0f 0f -5f; block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 0f 2f; block 0f 0f 3f; block 0f 3f 4f; block 0f 0f 5f; block 0f 0f 6f
           block 1f 3f -6f; block 1f 0f -5f; block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 0f 1f; block 1f 0f 2f; block 1f 0f 3f; block 1f 3f 4f; block 1f 0f 5f; block 1f 0f 6f
           block 2f 3f -6f; block 2f 0f -5f; block 2f 0f -4f; block 2f 0f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 0f 0f; block 2f 0f 1f; block 2f 0f 2f; block 2f 0f 3f; block 2f 3f 4f; block 2f 0f 5f; block 2f 0f 6f
           block 3f 3f -6f; block 3f 0f -5f; block 3f 0f -4f; block 3f 3f -3f; block 3f 3f -2f; block 3f 3f -1f; block 3f 3f 0f; block 3f 3f 1f; block 3f 3f 2f; block 3f 3f 3f; block 3f 3f 4f; block 3f 0f 5f; block 3f 0f 6f
           block 4f 3f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 0f 0f; block 4f 0f 1f; block 4f 0f 2f; block 4f 0f 3f; block 4f 0f 4f; block 4f 0f 5f; block 4f 0f 6f
           block 5f 3f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 0f -3f; block 5f 0f -2f; block 5f 0f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 0f 2f; block 5f 0f 3f; block 5f 0f 4f; block 5f 0f 5f; block 5f 0f 6f
           block 6f 3f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; block 6f 0f 3f; block 6f 0f 4f; block 6f 0f 5f; block 6f 0f 6f
        |]
        [| // Level 12
           block -6f 0f -6f; block -6f 0f -5f; block -6f 0f -4f; block -6f 0f -3f; block -6f 3f -2f; block -6f 0f -1f; block -6f 0f 0f; block -6f 0f 1f; block -6f 0f 2f; block -6f 0f 3f; block -6f 0f 4f; block -6f 0f 5f; block -6f 0f 6f
           block -5f 0f -6f; block -5f 0f -5f; block -5f 0f -4f; block -5f 0f -3f; block -5f 2f -2f; block -5f 0f -1f; block -5f 0f 0f; block -5f 0f 1f; block -5f 0f 2f; block -5f 0f 3f; block -5f 0f 4f; block -5f 0f 5f; block -5f 0f 6f
           block -4f 0f -6f; block -4f 0f -5f; block -4f 0f -4f; block -4f 0f -3f; block -4f 2f -2f; block -4f 0f -1f; block -4f 0f 0f; block -4f 0f 1f; block -4f 0f 2f; block -4f 0f 3f; block -4f 0f 4f; block -4f 0f 5f; block -4f 0f 6f
           block -3f 0f -6f; block -3f 0f -5f; block -3f 0f -4f; block -3f 0f -3f; block -3f 1f -2f; block -3f 0f -1f; block -3f 0f 0f; block -3f 0f 1f; block -3f 0f 2f; block -3f 0f 3f; block -3f 0f 4f; block -3f 0f 5f; block -3f 0f 6f
           block -2f 0f -6f; block -2f 0f -5f; block -2f 0f -4f; block -2f 0f -3f; block -2f 1f -2f; block -2f 0f -1f; block -2f 0f 0f; block -2f 0f 1f; block -2f 0f 2f; block -2f 0f 3f; block -2f 0f 4f; block -2f 0f 5f; block -2f 0f 6f
           block -1f 0f -6f; block -1f 0f -5f; block -1f 0f -4f; block -1f 0f -3f; block -1f 1f -2f; block -1f 0f -1f; block -1f 0f 0f; block -1f 0f 1f; block -1f 0f 2f; block -1f 0f 3f; block -1f 0f 4f; block -1f 0f 5f; block -1f 0f 6f
           block 0f 1f -6f; block 0f 1f -5f; block 0f 1f -4f; block 0f 1f -3f; block 0f 1f -2f; block 0f 1f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 0f 2f; block 0f 0f 3f; block 0f 0f 4f; block 0f 0f 5f; block 0f 4f 6f
           block 1f 0f -6f; block 1f 0f -5f; block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 1f -1f; block 1f 0f 0f; block 1f 0f 1f; block 1f 0f 2f; block 1f 2f 3f; block 1f 2f 4f; block 1f 3f 5f; block 1f 4f 6f
           block 2f 0f -6f; block 2f 0f -5f; block 2f 0f -4f; block 2f 0f -3f; block 2f 0f -2f; block 2f 1f -1f; block 2f 1f 0f; block 2f 1f 1f; block 2f 1f 2f; block 2f 2f 3f; block 2f 0f 4f; block 2f 0f 5f; block 2f 0f 6f
           block 3f 0f -6f; block 3f 0f -5f; block 3f 1f -4f; block 3f 0f -3f; block 3f 0f -2f; block 3f 1f -1f; block 3f 0f 0f; block 3f 0f 1f; block 3f 0f 2f; block 3f 0f 3f; block 3f 0f 4f; block 3f 0f 5f; block 3f 0f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; block 4f 0f -2f; block 4f 1f -1f; block 4f 0f 0f; block 4f 0f 1f; block 4f 0f 2f; block 4f 0f 3f; block 4f 0f 4f; block 4f 0f 5f; block 4f 0f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 0f -3f; block 5f 0f -2f; block 5f 1f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 0f 2f; block 5f 0f 3f; block 5f 0f 4f; block 5f 0f 5f; block 5f 0f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 1f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; block 6f 0f 3f; block 6f 0f 4f; block 6f 0f 5f; block 6f 0f 6f
        |]
        
        //// World 2 \\\\
        
        [| // Level 13
           waterBlock -6f 0f -6f; block -6f 3f -5f; block -6f 3f -4f; block -6f 4f -3f; block -6f 3f -2f; block -6f 3f -1f; riverBlock -6f 0f 0f "b"; riverBlock -6f 0f 1f "b"; block -6f 3f 2f; block -6f 2f 3f; block -6f 2f 4f; block -6f 0f 5f; block -6f 0f 6f
           waterBlock -5f 0f -6f; block -5f 2f -5f; block -5f 2f -4f; block -5f 3f -3f; waterBlock -5f 0f -2f; waterBlock -5f 0f -1f; riverBlock -5f 0f 0f "b"; riverBlock -5f 0f 1f "b"; block -5f 2f 2f; block -5f 2f 3f; block -5f 1f 4f; block -5f 0f 5f; block -5f 0f 6f
           waterBlock -4f 0f -6f; block -4f 1f -5f; block -4f 1f -4f; waterBlock -4f 0f -3f; waterBlock -4f 0f -2f; waterBlock -4f 0f -1f; riverBlock -4f 0f 0f "b"; riverBlock -4f 0f 1f "b"; block -4f 1f 2f; block -4f 1f 3f; block -4f 1f 4f; block -4f 0f 5f; block -4f 0f 6f
           waterBlock -3f 0f -6f; waterBlock -3f 0f -5f; block -3f 0f -4f; waterBlock -3f 0f -3f; waterBlock -3f 0f -2f; waterBlock -3f 0f -1f; riverBlock -3f 0f 0f "b"; riverBlock -3f 0f 1f "b"; block -3f 0f 2f; block -3f 0f 3f; block -3f 0f 4f; block -3f 0f 5f; block -3f 0f 6f
           waterBlock -2f 0f -6f; waterBlock -2f 0f -5f; waterBlock -2f 0f -4f; waterBlock -2f 0f -3f; block -2f 0f -2f; block -2f 0f -1f; riverBlock -2f 0f 0f "b"; riverBlock -2f 0f 1f "b"; block -2f 0f 2f; block -2f 0f 3f; block -2f 0f 4f; block -2f 0f 5f; block -2f 0f 6f
           block -1f 1f -6f; block -1f 1f -5f; block -1f 0f -4f; block -1f 0f -3f; block -1f 0f -2f; block -1f 0f -1f; riverBlock -1f 0f 0f "b"; riverBlock -1f 0f 1f "b"; block -1f 0f 2f; block -1f 0f 3f; block -1f 0f 4f; block -1f 0f 5f; block -1f 0f 6f
           block 0f 0f -6f; block 0f 0f -5f; block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; riverBlock 0f 0f 0f "b"; riverBlock 0f 0f 1f "b"; block 0f 0f 2f; block 0f 0f 3f; block 0f 0f 4f; block 0f 0f 5f; block 0f 0f 6f
           block 1f 0f -6f; block 1f 0f -5f; block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; riverBlock 1f 0f 0f "b"; block 1f 0f 1f; block 1f 0f 2f; block 1f 0f 3f; block 1f 0f 4f; block 1f 0f 5f; block 1f 0f 6f
           block 2f 0f -6f; block 2f 0f -5f; block 2f 0f -4f; block 2f 0f -3f; block 2f 0f -2f; block 2f 4f -1f; riverBlock 2f 0f 0f "b"; block 2f 0f 1f; block 2f 0f 2f; block 2f 0f 3f; block 2f 0f 4f; block 2f 0f 5f; block 2f 0f 6f
           block 3f 0f -6f; block 3f 0f -5f; block 3f 0f -4f; block 3f 0f -3f; block 3f 0f -2f; block 3f 5f -1f; riverBlock 3f 0f 0f "b"; block 3f 0f 1f; block 3f 0f 2f; block 3f 0f 3f; block 3f 0f 4f; block 3f 0f 5f; block 3f 0f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; block 4f 1f -2f; block 4f 5f -1f; riverBlock 4f 0f 0f "b"; block 4f 0f 1f; block 4f 0f 2f; block 4f 0f 3f; block 4f 0f 4f; block 4f 0f 5f; block 4f 0f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 0f -3f; block 5f 3f -2f; block 5f 4f -1f; riverBlock 5f 0f 0f "b"; block 5f 0f 1f; block 5f 0f 2f; block 5f 0f 3f; block 5f 0f 4f; block 5f 0f 5f; block 5f 0f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 3f -2f; block 6f 4f -1f; riverBlock 6f 0f 0f "b"; block 6f 0f 1f; block 6f 0f 2f; block 6f 0f 3f; block 6f 0f 4f; block 6f 0f 5f; block 6f 0f 6f
        |]
        [| // Level 14
           waterBlock -6f 0f -6f; waterBlock -6f 0f -5f; waterBlock -6f 0f -4f; waterBlock -6f 0f -3f; waterBlock -6f 0f -2f; waterBlock -6f 0f -1f; waterBlock -6f 0f 0f; waterBlock -6f 0f 1f; waterBlock -6f 0f 2f; waterBlock -6f 0f 3f; waterBlock -6f 0f 4f; waterBlock -6f 0f 5f; waterBlock -6f 0f 6f
           waterBlock -5f 0f -6f; waterBlock -5f 0f -5f; waterBlock -5f 0f -4f; waterBlock -5f 0f -3f; waterBlock -5f 0f -2f; block -5f 0f -1f; block -5f 0f 0f; block -5f 0f 1f; block -5f 0f 2f; waterBlock -5f 0f 3f; waterBlock -5f 0f 4f; waterBlock -5f 0f 5f; waterBlock -5f 0f 6f
           waterBlock -4f 0f -6f; block -4f 0f -5f; block -4f 0f -4f; block -4f 0f -3f; block -4f 0f -2f; waterBlock -4f 0f -1f; waterBlock -4f 0f 0f; waterBlock -4f 0f 1f; block -4f 0f 2f; block -4f 0f 3f; block -4f 0f 4f; block -4f 0f 5f; waterBlock -4f 0f 6f
           waterBlock -3f 0f -6f; waterBlock -3f 0f -5f; waterBlock -3f 0f -4f; waterBlock -3f 0f -3f; block -3f 0f -2f; block -3f 0f -1f; block -3f 0f 0f; block -3f 0f 1f; block -3f 0f 2f; waterBlock -3f 0f 3f; block -3f 0f 4f; block -3f 0f 5f; waterBlock -3f 0f 6f
           block -2f 0f -6f; waterBlock -2f 0f -5f; waterBlock -2f 0f -4f; waterBlock -2f 0f -3f; waterBlock -2f 0f -2f; waterBlock -2f 0f -1f; waterBlock -2f 0f 0f; block -2f 0f 1f; block -2f 0f 2f; waterBlock -2f 0f 3f; waterBlock -2f 0f 4f; block -2f 0f 5f; waterBlock -2f 0f 6f
           block -1f 1f -6f; waterBlock -1f 0f -5f; waterBlock -1f 0f -4f; waterBlock -1f 0f -3f; waterBlock -1f 0f -2f; waterBlock -1f 0f -1f; waterBlock -1f 0f 0f; block -1f 1f 1f; block -1f 1f 2f; waterBlock -1f 0f 3f; waterBlock -1f 0f 4f; waterBlock -1f 0f 5f; waterBlock -1f 0f 6f
           block 0f 1f -6f; block 0f 1f -5f; waterBlock 0f 0f -4f; waterBlock 0f 0f -3f; waterBlock 0f 0f -2f; waterBlock 0f 0f -1f; waterBlock 0f 0f 0f; block 0f 2f 1f; block 0f 2f 2f; waterBlock 0f 0f 3f; waterBlock 0f 0f 4f; waterBlock 0f 0f 5f; waterBlock 0f 0f 6f
           block 1f 3f -6f; block 1f 3f -5f; block 1f 1f -4f; waterBlock 1f 0f -3f; waterBlock 1f 0f -2f; waterBlock 1f 0f -1f; waterBlock 1f 0f 0f; waterBlock 1f 0f 1f; waterBlock 1f 0f 2f; waterBlock 1f 0f 3f; waterBlock 1f 0f 4f; waterBlock 1f 0f 5f; waterBlock 1f 0f 6f
           block 2f 5f -6f; block 2f 3f -5f; block 2f 1f -4f; block 2f 1f -3f; block 2f 1f -2f; block 2f 1f -1f; block 2f 1f 0f; block 2f 1f 1f; block 2f 1f 2f; block 2f 1f 3f; block 2f 1f 4f; block 2f 1f 5f; block 2f 2f 6f
           block 3f 6f -6f; block 3f 5f -5f; block 3f 4f -4f; block 3f 3f -3f; block 3f 3f -2f; block 3f 3f -1f; block 3f 3f 0f; block 3f 2f 1f; block 3f 2f 2f; block 3f 2f 3f; block 3f 2f 4f; block 3f 3f 5f; block 3f 3f 6f
           block 4f 6f -6f; block 4f 5f -5f; block 4f 4f -4f; block 4f 4f -3f; block 4f 4f -2f; block 4f 4f -1f; block 4f 4f 0f; block 4f 4f 1f; block 4f 5f 2f; block 4f 5f 3f; block 4f 5f 4f; block 4f 5f 5f; block 4f 5f 6f
           block 5f 6f -6f; block 5f 6f -5f; block 5f 6f -4f; block 5f 6f -3f; block 5f 6f -2f; block 5f 6f -1f; block 5f 6f 0f; block 5f 6f 1f; block 5f 6f 2f; block 5f 6f 3f; block 5f 6f 4f; block 5f 6f 5f; block 5f 6f 6f
           block 6f 6f -6f; block 6f 6f -5f; block 6f 6f -4f; block 6f 6f -3f; block 6f 6f -2f; block 6f 6f -1f; block 6f 6f 0f; block 6f 6f 1f; block 6f 6f 2f; block 6f 6f 3f; block 6f 6f 4f; block 6f 6f 5f; block 6f 6f 6f
        |]
        [| // Level 15
           block -6f 0f -6f; block -6f 0f -5f; block -6f 0f -4f; block -6f 0f -3f; block -6f 0f -2f; waterBlock -6f 0f -1f; waterBlock -6f 0f 0f; waterBlock -6f 0f 1f; block -6f 0f 2f; block -6f 0f 3f; block -6f 0f 4f; block -6f 0f 5f; block -6f 0f 6f
           block -5f 0f -6f; block -5f 1f -5f; block -5f 1f -4f; block -5f 1f -3f; block -5f 0f -2f; waterBlock -5f 0f -1f; waterBlock -5f 0f 0f; waterBlock -5f 0f 1f; block -5f 0f 2f; block -5f 0f 3f; block -5f 0f 4f; block -5f 0f 5f; block -5f 0f 6f
           block -4f 1f -6f; block -4f 1f -5f; block -4f 2f -4f; block -4f 2f -3f; block -4f 0f -2f; waterBlock -4f 0f -1f; waterBlock -4f 0f 0f; waterBlock -4f 0f 1f; block -4f 0f 2f; block -4f 1f 3f; block -4f 3f 4f; block -4f 2f 5f; block -4f 1f 6f
           block -3f 3f -6f; block -3f 3f -5f; block -3f 3f -4f; block -3f 3f -3f; block -3f 0f -2f; waterBlock -3f 0f -1f; waterBlock -3f 0f 0f; waterBlock -3f 0f 1f; block -3f 0f 2f; block -3f 1f 3f; block -3f 5f 4f; block -3f 4f 5f; block -3f 1f 6f
           block -2f 2f -6f; block -2f 3f -5f; block -2f 3f -4f; block -2f 3f -3f; block -2f 1f -2f; waterBlock -2f 0f -1f; waterBlock -2f 0f 0f; waterBlock -2f 0f 1f; waterBlock -2f 0f 2f; block -2f 2f 3f; block -2f 5f 4f; block -2f 3f 5f; block -2f 1f 6f
           block -1f 2f -6f; block -1f 2f -5f; block -1f 2f -4f; block -1f 1f -3f; block -1f 1f -2f; waterBlock -1f 0f -1f; waterBlock -1f 0f 0f; waterBlock -1f 0f 1f; waterBlock -1f 0f 2f; waterBlock -1f 0f 3f; block -1f 4f 4f; block -1f 2f 5f; block -1f 1f 6f
           block 0f 1f -6f; block 0f 1f -5f; block 0f 1f -4f; block 0f 1f -3f; waterBlock 0f 0f -2f; waterBlock 0f 0f -1f; waterBlock 0f 0f 0f; waterBlock 0f 0f 1f; waterBlock 0f 0f 2f; block 0f 1f 3f; block 0f 3f 4f; block 0f 2f 5f; block 0f 2f 6f
           waterBlock 1f 0f -6f; waterBlock 1f 0f -5f; waterBlock 1f 0f -4f; waterBlock 1f 0f -3f; waterBlock 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; waterBlock 1f 0f 1f; waterBlock 1f 0f 2f; waterBlock 1f 0f 3f; waterBlock 1f 0f 4f; waterBlock 1f 0f 5f; waterBlock 1f 0f 6f
           waterBlock 2f 0f -6f; waterBlock 2f 0f -5f; waterBlock 2f 0f -4f; waterBlock 2f 0f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 1f 0f; block 2f 1f 1f; block 2f 0f 2f; block 2f 0f 3f; block 2f 0f 4f; block 2f 0f 5f; block 2f 0f 6f
           block 3f 1f -6f; block 3f 1f -5f; block 3f 0f -4f; waterBlock 3f 0f -3f; block 3f 0f -2f; block 3f 0f -1f; block 3f 0f 0f; block 3f 1f 1f; block 3f 2f 2f; block 3f 2f 3f; block 3f 0f 4f; block 3f 0f 5f; block 3f 0f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; waterBlock 4f 0f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 0f 0f; block 4f 1f 1f; block 4f 1f 2f; block 4f 1f 3f; block 4f 0f 4f; block 4f 0f 5f; block 4f 0f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; waterBlock 5f 0f -3f; block 5f 0f -2f; block 5f 0f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 0f 2f; block 5f 0f 3f; block 5f 0f 4f; block 5f 0f 5f; block 5f 0f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; waterBlock 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; block 6f 0f 3f; block 6f 0f 4f; block 6f 5f 5f; block 6f 0f 6f
        |]
        [| // Level 16
           block -6f 0f -6f; block -6f 0f -5f; block -6f 0f -4f; block -6f 0f -3f; block -6f 0f -2f; block -6f 0f -1f; block -6f 0f 0f; block -6f 0f 1f; block -6f 0f 2f; block -6f 0f 3f; block -6f 0f 4f; block -6f 0f 5f; block -6f 0f 6f
           block -5f 0f -6f; block -5f 0f -5f; block -5f 0f -4f; block -5f 0f -3f; block -5f 0f -2f; block -5f 0f -1f; block -5f 0f 0f; block -5f 0f 1f; block -5f 0f 2f; block -5f 0f 3f; block -5f 0f 4f; block -5f 0f 5f; block -5f 0f 6f
           block -4f 0f -6f; block -4f 0f -5f; block -4f 0f -4f; block -4f 0f -3f; block -4f 0f -2f; block -4f 0f -1f; block -4f 0f 0f; block -4f 1f 1f; block -4f 1f 2f; block -4f 1f 3f; block -4f 0f 4f; block -4f 0f 5f; block -4f 0f 6f
           block -3f 0f -6f; block -3f 0f -5f; block -3f 0f -4f; block -3f 0f -3f; block -3f 0f -2f; block -3f 0f -1f; block -3f 2f 0f; block -3f 3f 1f; block -3f 3f 2f; block -3f 1f 3f; block -3f 0f 4f; block -3f 0f 5f; block -3f 0f 6f
           block -2f 2f -6f; block -2f 1f -5f; block -2f 0f -4f; block -2f 0f -3f; block -2f 0f -2f; block -2f 0f -1f; block -2f 2f 0f; block -2f 3f 1f; block -2f 1f 2f; block -2f 1f 3f; block -2f 0f 4f; block -2f 0f 5f; block -2f 0f 6f
           block -1f 2f -6f; block -1f 1f -5f; block -1f 0f -4f; block -1f 0f -3f; block -1f 0f -2f; block -1f 0f -1f; block -1f 3f 0f; block -1f 3f 1f; block -1f 1f 2f; block -1f 0f 3f; block -1f 0f 4f; block -1f 0f 5f; block -1f 0f 6f
           block 0f 1f -6f; block 0f 0f -5f; block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 3f 1f; block 0f 1f 2f; block 0f 0f 3f; block 0f 0f 4f; block 0f 0f 5f; block 0f 0f 6f
           block 1f 0f -6f; block 1f 0f -5f; block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 2f 1f; waterBlock 1f 0f 2f; waterBlock 1f 0f 3f; block 1f 2f 4f; block 1f 2f 5f; block 1f 2f 6f
           block 2f 0f -6f; block 2f 0f -5f; block 2f 0f -4f; block 2f 0f -3f; block 2f 0f -2f; waterBlock 2f 0f -1f; block 2f 3f 0f; block 2f 2f 1f; waterBlock 2f 0f 2f; waterBlock 2f 0f 3f; block 2f 4f 4f; block 2f 4f 5f; block 2f 2f 6f
           block 3f 0f -6f; block 3f 0f -5f; block 3f 0f -4f; block 3f 0f -3f; block 3f 0f -2f; waterBlock 3f 0f -1f; block 3f 4f 0f; block 3f 4f 1f; waterBlock 3f 0f 2f; waterBlock 3f 0f 3f; block 3f 5f 4f; block 3f 4f 5f; block 3f 2f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; block 4f 0f -2f; waterBlock 4f 0f -1f; block 4f 4f 0f; block 4f 4f 1f; waterBlock 4f 0f 2f; waterBlock 4f 0f 3f; block 4f 3f 4f; block 4f 4f 5f; block 4f 3f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 0f -3f; block 5f 0f -2f; block 5f 2f -1f; block 5f 4f 0f; block 5f 2f 1f; block 5f 1f 2f; waterBlock 5f 0f 3f; block 5f 2f 4f; block 5f 2f 5f; block 5f 2f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 1f -1f; block 6f 2f 0f; block 6f 2f 1f; block 6f 1f 2f; waterBlock 6f 0f 3f; block 6f 2f 4f; block 6f 2f 5f; block 6f 2f 6f
        |]
        [| // Level 17
           block -6f 0f -6f; block -6f 0f -5f; block -6f 0f -4f; block -6f 0f -3f; block -6f 0f -2f; block -6f 0f -1f; block -6f 0f 0f; block -6f 0f 1f; block -6f 0f 2f; block -6f 0f 3f; block -6f 0f 4f; block -6f 0f 5f; block -6f 0f 6f
           block -5f 0f -6f; block -5f 0f -5f; block -5f -1f -4f; block -5f -1f -3f; block -5f -1f -2f; block -5f -1f -1f; block -5f -1f 0f; block -5f -1f 1f; block -5f -1f 2f; block -5f -1f 3f; block -5f -1f 4f; block -5f -1f 5f; block -5f 0f 6f
           block -4f 0f -6f; block -4f 0f -5f; block -4f -2f -4f; block -4f -2f -3f; block -4f -2f -2f; block -4f -2f -1f; block -4f -2f 0f; block -4f -2f 1f; block -4f -2f 2f; block -4f -2f 3f; block -4f -2f 4f; block -4f -1f 5f; block -4f 0f 6f
           block -3f 0f -6f; block -3f 0f -5f; block -3f -2f -4f; block -3f -3f -3f; block -3f -3f -2f; block -3f -3f -1f; block -3f -3f 0f; block -3f -3f 1f; block -3f -3f 2f; block -3f -3f 3f; block -3f -2f 4f; block -3f -1f 5f; block -3f 0f 6f
           block -2f 0f -6f; block -2f 0f -5f; block -2f -2f -4f; block -2f -3f -3f; block -2f -4f -2f; block -2f -4f -1f; block -2f -4f 0f; block -2f -4f 1f; block -2f -4f 2f; block -2f -3f 3f; block -2f -2f 4f; block -2f -1f 5f; block -2f 0f 6f
           block -1f 1f -6f; block -1f 0f -5f; block -1f -2f -4f; block -1f -3f -3f; block -1f -4f -2f; waterBlock -1f -5f -1f; waterBlock -1f -5f 0f; waterBlock -1f -5f 1f; block -1f -4f 2f; block -1f -3f 3f; block -1f -2f 4f; block -1f -1f 5f; block -1f 0f 6f
           block 0f 2f -6f; block 0f 0f -5f; block 0f 3f -4f; block 0f -3f -3f; block 0f -4f -2f; waterBlock 0f -5f -1f; waterBlock 0f -5f 0f; waterBlock 0f -5f 1f; block 0f -4f 2f; block 0f -3f 3f; block 0f -2f 4f; block 0f -1f 5f; block 0f 0f 6f
           block 1f 0f -6f; block 1f 0f -5f; block 1f 3f -4f; block 1f -3f -3f; block 1f -4f -2f; waterBlock 1f -5f -1f; waterBlock 1f -5f 0f; waterBlock 1f -5f 1f; block 1f -4f 2f; block 1f -3f 3f; block 1f -2f 4f; block 1f -1f 5f; block 1f 0f 6f
           block 2f 0f -6f; block 2f 0f -5f; block 2f 3f -4f; block 2f -3f -3f; block 2f -4f -2f; block 2f -4f -1f; block 2f -4f 0f; block 2f -4f 1f; block 2f -4f 2f; block 2f -3f 3f; block 2f -2f 4f; block 2f -1f 5f; block 2f 0f 6f
           block 3f 0f -6f; block 3f 0f -5f; block 3f 3f -4f; block 3f -3f -3f; block 3f -3f -2f; block 3f -3f -1f; block 3f -3f 0f; block 3f -3f 1f; block 3f -3f 2f; block 3f -3f 3f; block 3f -2f 4f; block 3f -1f 5f; block 3f 0f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 3f -4f; block 4f -2f -3f; block 4f -2f -2f; block 4f -2f -1f; block 4f -2f 0f; block 4f -2f 1f; block 4f -2f 2f; block 4f -2f 3f; block 4f -2f 4f; block 4f -1f 5f; block 4f 0f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 3f -4f; block 5f -1f -3f; block 5f -1f -2f; block 5f -1f -1f; block 5f -1f 0f; block 5f -1f 1f; block 5f -1f 2f; block 5f -1f 3f; block 5f -1f 4f; block 5f -1f 5f; block 5f 0f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 3f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; block 6f 0f 3f; block 6f 0f 4f; block 6f 0f 5f; block 6f 0f 6f
        
           caveBlock 0f 3f -5f;
           caveBlock 1f 3f 6f; caveBlock 2f 3f 6f; caveBlock 3f 3f 6f
           caveBlock 1f 3f 7f; caveBlock 2f 3f 7f; caveBlock 3f 3f 7f
           caveBlock 1f 3f 8f; caveBlock 2f 3f 8f; caveBlock 3f 3f 8f
           caveBlock 2f 3f 9f
        |]
    |]
    
    let elements = [|
        //// World 1 \\\\
        
        [| // Level 1
            { etype = Goal; position = Vector3(-5f, 9f, -6f); rotation = Vector3.Zero; visible = true }
            { etype = Bubble; position = Vector3(-5f, 7f, 3.5f); rotation = Vector3.Zero; visible = true }
            { etype = Bubble; position = Vector3(-5f, 7f, 1.5f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(-2.5f, 6f, 0f); rotation = Vector3.Zero; visible = true }
            { etype = Hook; position = Vector3(-3f, 7f, -5.5f); rotation = Vector3.Zero; visible = true }
        |]
        [| // Level 2
            { etype = Goal; position = Vector3(-3f, 8f, 3f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(-5f, 1f, -3.5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
        |]
        [| // Level 3
            { etype = Goal; position = Vector3(-5f, 7f, -5f); rotation = Vector3.Zero; visible = true }
            { etype = LavaWall; position = Vector3(6f, 5.5f, 0f); rotation = Vector3.Zero; visible = true }
            { etype = LavaWall; position = Vector3(0.5f, 5.5f, 2f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = Hook; position = Vector3(2.5f, 5f, -4f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = Hook; position = Vector3(5.5f, 5f, 0f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = Hook; position = Vector3(3f, 5f, 4.5f); rotation = Vector3.Zero; visible = true }
            { etype = Hook; position = Vector3(0f, 5f, 2.5f); rotation = Vector3.Zero; visible = true }
            { etype = Hook; position = Vector3(-2f, 5f, 4.5f); rotation = Vector3.Zero; visible = true }
            { etype = Hook; position = Vector3(-4.5f, 5f, 3f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = Hook; position = Vector3(-5f, 6f, -0.5f); rotation = Vector3.Zero; visible = true }
            { etype = Hook; position = Vector3(-3.5f, 5f, -5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
        |]
        [| // Level 4
            { etype = Goal; position = Vector3(6f, 8f, 6f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(-2.5f, 7f, 5f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(0.5f, 7f, 5f); rotation = Vector3.Zero; visible = true }
            { etype = Hook; position = Vector3(5.5f, 7f, 5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
        |]
        [| // Level 5
            { etype = Goal; position = Vector3(-4f, 6f, 3f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(3.5f, 5f, 6f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(0.5f, 5f, 6f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(-2.5f, 5f, 6f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(2f, 5f, 4.5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = Bridge; position = Vector3(3.5f, 5f, 3f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(5f, 5f, 1.5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = Bridge; position = Vector3(5f, 5f, -1.5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = Bridge; position = Vector3(3.5f, 5f, -3f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(0.5f, 5f, -3f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(-1f, 5f, -1.5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = Bridge; position = Vector3(-2.5f, 5f, 0f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(-4f, 5f, 1.5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = Bridge; position = Vector3(-1f, 5f, -4.5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = Bridge; position = Vector3(-2.5f, 5f, -6f); rotation = Vector3.Zero; visible = true }
        |]
        [| // Level 6
            { etype = Goal; position = Vector3(0f, 6f, 0f); rotation = Vector3.Zero; visible = false }
            { etype = GoalFragment; position = Vector3(-6f, 5f, 6f); rotation = Vector3.Zero; visible = true }
            { etype = GoalFragment; position = Vector3(2f, 6f, 6f); rotation = Vector3.Zero; visible = true }
            { etype = GoalFragment; position = Vector3(-2f, 7f, -6f); rotation = Vector3.Zero; visible = true }
            { etype = GoalFragment; position = Vector3(-5f, 4f, 0f); rotation = Vector3.Zero; visible = true }
            { etype = GoalFragment; position = Vector3(4f, 3f, -1f); rotation = Vector3.Zero; visible = true }
        |]
        [| // Level 7
            { etype = Goal; position = Vector3(-3f, 2f, -4f); rotation = Vector3.Zero; visible = true }
            { etype = MovingBlock; position = Vector3(3f, 2f, -1f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = MovingBlock; position = Vector3(3f, 3f, -0f); rotation = Vector3(0f, degToRad 270f, 0f); visible = true }
            { etype = Bubble; position = Vector3(1f, 4f, 1.5f); rotation = Vector3.Zero; visible = true }
            { etype = Bubble; position = Vector3(1f, 4f, 3.5f); rotation = Vector3.Zero; visible = true }
            { etype = Bubble; position = Vector3(-1f, 4f, 3.5f); rotation = Vector3.Zero; visible = true }
            { etype = MovingBlock; position = Vector3(-3f, 6f, 2f); rotation = Vector3(0f, degToRad 270f, 0f); visible = true }
            { etype = MovingBlock; position = Vector3(-2f, 7f, 1f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = MovingBlock; position = Vector3(-1f, 8f, 0f); rotation = Vector3(0f, degToRad 270f, 0f); visible = true }
            { etype = Bubble; position = Vector3(0f, 9f, -1.5f); rotation = Vector3.Zero; visible = true }
        |]
        [| // Level 8
            { etype = Goal; position = Vector3(5f, 6f, -6f); rotation = Vector3.Zero; visible = true }
            { etype = LavaWall; position = Vector3(3f, 0.5f, 0f); rotation = Vector3.Zero; visible = false }
            { etype = LavaWall; position = Vector3(2f, 0.5f, 1f); rotation = Vector3(0f, degToRad 180f, 0f); visible = false }
            { etype = LavaWall; position = Vector3(1f, 0.5f, -2f); rotation = Vector3.Zero; visible = false }
            { etype = LavaWall; position = Vector3(0f, 0.5f, -1f); rotation = Vector3(0f, degToRad 180f, 0f); visible = false }
            { etype = LavaWall; position = Vector3(-1f, 0.5f, 0f); rotation = Vector3.Zero; visible = false }
            { etype = LavaWall; position = Vector3(-2f, 0.5f, 3f); rotation = Vector3(0f, degToRad 180f, 0f); visible = false }
            { etype = LavaWall; position = Vector3(-6f, 1.5f, 0f); rotation = Vector3(0f, degToRad 180f, 0f); visible = false }
        |]
        [| // Level 9
            { etype = Goal; position = Vector3(6f, 1f, -6f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(2.5f, 6f, 6f); rotation = Vector3.Zero; visible = true }
            { etype = MovingBlock; position = Vector3(0f, 5f, 3f); rotation = Vector3.Zero; visible = true }
            { etype = MovingBlock; position = Vector3(-1f, 4f, 1f); rotation = Vector3.Zero; visible = true }
            { etype = MovingBlockWithHook; position = Vector3(3f, 7f, -3f); rotation = Vector3.Zero; visible = true }
        |]
        [| // Level 10
            { etype = CompanionCube; position = Vector3(5f, 1f, -2f); rotation = Vector3.Zero; visible = true }
            { etype = CompanionCube; position = Vector3(3f, 4f, -5f); rotation = Vector3.Zero; visible = true }
            { etype = CompanionCube; position = Vector3(-5f, 1f, 2f); rotation = Vector3.Zero; visible = true }
            { etype = CubeTrigger; position = Vector3(-2f, 1f, 4f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(1.5f, 3f, 6f); rotation = Vector3.Zero; visible = false }
            { etype = Goal; position = Vector3(-3f, 1f, -6f); rotation = Vector3.Zero; visible = true }
        |]
        [| // Level 11
            { etype = Goal; position = Vector3(-5f, 7f, -5f); rotation = Vector3.Zero; visible = true }
            { etype = CompanionCube; position = Vector3(-2f, 1f, 2f); rotation = Vector3.Zero; visible = true }
            { etype = CubeTrigger; position = Vector3(3f, 1f, 6f); rotation = Vector3.Zero; visible = true }
            { etype = LavaWall; position = Vector3(5f, 0.5f, 4f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = LavaWall; position = Vector3(6f, 0.5f, 3f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = LavaWall; position = Vector3(4f, 0.5f, 2f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = LavaWall; position = Vector3(6f, 0.5f, 1f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = LavaWall; position = Vector3(5f, 0.5f, 0f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = LavaWall; position = Vector3(4f, 0.5f, -1f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = LavaWall; position = Vector3(6f, 0.5f, -2f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = LavaWall; position = Vector3(5f, 0.5f, -3f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = LavaWall; position = Vector3(2f, 0.5f, 0f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = LavaWall; position = Vector3(1f, 0.5f, -1f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = LavaWall; position = Vector3(0f, 0.5f, -2f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = LavaWall; position = Vector3(2f, 0.5f, -3f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
        |]
        [| // Level 12
            { etype = Goal; position = Vector3(0f, 6f, 0f); rotation = Vector3.Zero; visible = false }
            { etype = CompanionCube; position = Vector3(1f, 1f, -4f); rotation = Vector3.Zero; visible = false }
            { etype = GoalFragment; position = Vector3(-6f, 2f, -6f); rotation = Vector3.Zero; visible = false }
            { etype = CubeTrigger; position = Vector3(-6f, 1f, -6f); rotation = Vector3.Zero; visible = false }
            { etype = GoalFragment; position = Vector3(0f, 6f, 6f); rotation = Vector3.Zero; visible = false }
            { etype = CubeTrigger; position = Vector3(0f, 5f, 6f); rotation = Vector3.Zero; visible = false }
            { etype = GoalFragment; position = Vector3(-3f, 2f, 0f); rotation = Vector3.Zero; visible = false }
            { etype = CubeTrigger; position = Vector3(-3f, 1f, 0f); rotation = Vector3.Zero; visible = false }
            { etype = GoalFragment; position = Vector3(5f, 2f, 4f); rotation = Vector3.Zero; visible = false }
            { etype = CubeTrigger; position = Vector3(5f, 1f, 4f); rotation = Vector3.Zero; visible = false }
            { etype = GoalFragment; position = Vector3(-6f, 2f, 6f); rotation = Vector3.Zero; visible = false }
            { etype = CubeTrigger; position = Vector3(-6f, 1f, 6f); rotation = Vector3.Zero; visible = false }
        |]
        
        //// World 2 \\\\
        
        [| // Level 13
            { etype = Goal; position = Vector3(4f, 1f, 4f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(-2.5f, 1f, -5f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(-6f, 3f, 0.5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            
            // Destructible blocks
            desBlock 2f 6f -6f; desBlock 2f 5f -5f ;desBlock 2f 5f -4f; desBlock 2f 4f -3f; desBlock 2f 4f -2f
            desBlock 1f 2f 1f; desBlock 2f 3f 1f; desBlock 3f 4f 1f; desBlock 4f 4f 1f; desBlock 5f 4f 1f; desBlock 6f 4f 1f
            desBlock 1f 3f 2f; desBlock 2f 4f 2f; desBlock 3f 4f 2f; desBlock 4f 6f 2f; desBlock 5f 6f 2f; desBlock 6f 5f 2f
            desBlock 1f 4f 3f; desBlock 2f 6f 3f; desBlock 3f 6f 3f; desBlock 4f 5f 3f; desBlock 5f 5f 3f; desBlock 6f 5f 3f
            desBlock 1f 4f 4f; desBlock 2f 6f 4f; desBlock 3f 5f 4f; desBlock 5f 4f 4f; desBlock 6f 4f 4f
            desBlock 1f 3f 5f; desBlock 2f 5f 5f; desBlock 3f 4f 5f; desBlock 4f 4f 5f; desBlock 5f 5f 5f; desBlock 6f 4f 5f
            desBlock 1f 3f 6f; desBlock 2f 5f 6f; desBlock 3f 5f 6f; desBlock 4f 4f 6f; desBlock 5f 3f 6f; desBlock 6f 3f 6f
        |]
        [| // Level 14
            { etype = Goal; position = Vector3(-4f, 1f, -5f); rotation = Vector3.Zero; visible = true }
            { etype = MovingBlock; position = Vector3(1f, 2f, 3f); rotation = Vector3.Zero; visible = true }
        
            // Destructible Blocks
            desBlock 0f 6f -6f; desBlock 0f 6f -5f; desBlock 0f 6f -4f; desBlock 0f 6f -3f; desBlock 0f 6f -2f; desBlock 0f 6f -1f; desBlock 0f 6f 0f; desBlock 0f 6f 1f; desBlock 0f 6f 2f; desBlock 0f 6f 3f; desBlock 0f 6f 4f; desBlock 0f 6f 5f; desBlock 0f 6f 6f
            desBlock 1f 6f -6f; desBlock 1f 6f -5f; desBlock 1f 6f -4f; desBlock 1f 6f -3f; desBlock 1f 6f -2f; desBlock 1f 6f -1f; desBlock 1f 6f 0f; desBlock 1f 6f 1f; desBlock 1f 6f 2f; desBlock 1f 6f 3f; desBlock 1f 6f 4f; desBlock 1f 6f 5f; desBlock 1f 6f 6f
            desBlock 2f 6f -6f; desBlock 2f 6f -5f; desBlock 2f 6f -4f; desBlock 2f 6f -3f; desBlock 2f 6f -2f; desBlock 2f 6f -1f; desBlock 2f 6f 0f; desBlock 2f 6f 1f; desBlock 2f 6f 2f; desBlock 2f 6f 3f; desBlock 2f 6f 4f; desBlock 2f 6f 5f; desBlock 2f 6f 6f
            desBlock 3f 6f -5f; desBlock 3f 6f -4f; desBlock 3f 6f -3f; desBlock 3f 6f -2f; desBlock 3f 6f -1f; desBlock 3f 6f 0f; desBlock 3f 6f 1f; desBlock 3f 6f 2f; desBlock 3f 6f 3f; desBlock 3f 6f 4f; desBlock 3f 6f 5f; desBlock 3f 6f 6f
            desBlock 4f 6f -5f; desBlock 4f 6f -4f; desBlock 4f 6f -3f; desBlock 4f 6f -2f; desBlock 4f 6f -1f; desBlock 4f 6f 0f; desBlock 4f 6f 1f; desBlock 4f 6f 2f; desBlock 4f 6f 3f; desBlock 4f 6f 4f; desBlock 4f 6f 5f; desBlock 4f 6f 6f
        |]
        [| // Level 15
            { etype = Goal; position = Vector3(6f, 6f, 5f); rotation = Vector3.Zero; visible = true }
            { etype = LavaPlume; position = Vector3(-3f, 0.5f, 0f); rotation = Vector3.Zero; visible = true }
            { etype = LavaPlume; position = Vector3(-2f, 0.5f, 0f); rotation = Vector3.Zero; visible = true }
            { etype = LavaPlume; position = Vector3(1f, 0.5f, -5f); rotation = Vector3.Zero; visible = true }
            { etype = LavaPlume; position = Vector3(2f, 0.5f, -5f); rotation = Vector3.Zero; visible = true }
            { etype = LavaPlume; position = Vector3(-1f, 0.5f, 2f); rotation = Vector3.Zero; visible = true }
            { etype = LavaPlume; position = Vector3(-1f, 0.5f, 3f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(1.5f, 1f, -5f); rotation = Vector3.Zero; visible = true }
            { etype = Hook; position = Vector3(-3f, 5f, 3.5f); rotation = Vector3.Zero; visible = true }
            { etype = Hook; position = Vector3(2.5f, 2f, 3f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = Hook; position = Vector3(6f, 5f, 4.5f); rotation = Vector3.Zero; visible = true }
        |]
        [| // Level 16
            { etype = Goal; position = Vector3(6f, 3f, 5f); rotation = Vector3.Zero; visible = true }
            { etype = CompanionCube; position = Vector3(-5f, 1f, -5f); rotation = Vector3.Zero; visible = true }
            { etype = LavaWall; position = Vector3(0f, 0.5f, -4f); rotation = Vector3.Zero; visible = true }
            { etype = LavaWall; position = Vector3(-1f, 0.5f, -2f); rotation = Vector3.Zero; visible = true }
            { etype = LavaPlume; position = Vector3(3f, 0.5f, -1f); rotation = Vector3.Zero; visible = true }
            { etype = CubeTrigger; position = Vector3(-2f, 4f, 1f); rotation = Vector3.Zero; visible = true }
            { etype = LavaPlume; position = Vector3(3f, 0.5f, 2f); rotation = Vector3.Zero; visible = false }
            { etype = LavaPlume; position = Vector3(3f, 0.5f, 3f); rotation = Vector3.Zero; visible = false }
        |]
        [| // Level 17
            { etype = Goal; position = Vector3(0f, 6f, 0f); rotation = Vector3.Zero; visible = false }
            { etype = GoalFragment; position = Vector3(6f, 1f, 6f); rotation = Vector3.Zero; visible = true }
            { etype = GoalFragment; position = Vector3(-1f, 1f, 0f); rotation = Vector3.Zero; visible = true }
            { etype = GoalFragment; position = Vector3(-6f, 1f, -6f); rotation = Vector3.Zero; visible = true }
            { etype = GoalFragment; position = Vector3(1f, 4f, 5f); rotation = Vector3.Zero; visible = true }
            { etype = GoalFragment; position = Vector3(2f, 4f, 9f); rotation = Vector3.Zero; visible = true }
            { etype = CompanionCube; position = Vector3(3f, 1f, -6f); rotation = Vector3.Zero; visible = true }
            { etype = CubeTrigger; position = Vector3(-4f, 1f, -6f); rotation = Vector3.Zero; visible = true }
            { etype = LavaPlume; position = Vector3(-1f, -4.5f, 0f); rotation = Vector3.Zero; visible = true }
            { etype = MovingBlock; position = Vector3(0f, 0f, 5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = MovingBlock; position = Vector3(1f, 1f, 5f); rotation = Vector3(0f, degToRad 270f, 0f); visible = true }
            { etype = MovingBlock; position = Vector3(2f, 2f, 5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = true }
            { etype = MovingBlock; position = Vector3(-1f, 3f, 5f); rotation = Vector3(0f, degToRad 270f, 0f); visible = true }
        |]
    |]
    
    let powerUps = [|
        //// World 1 \\\\
        
        [| // Level 1
            { ptype = GrapplingHook; position = Vector3(-2f, 1f, 0f) }
        |]
        [| // Level 2
            { ptype = TerrainManipulator; position = Vector3(4f, 7f, -4f) }
        |]
        [| // Level 3
            { ptype = GrapplingHook; position = Vector3(4f, 1f, -5f) }
        |]
        [| // Level 4
            { ptype = TerrainManipulator; position = Vector3(3f, 3f, 1f) }
            { ptype = GrapplingHook; position = Vector3(-1f, 6f, -3f) }
        |]
        [| // Level 5
        |]
        [| // Level 6
            { ptype = Glasses; position = Vector3(0f, 1f, 0f) }
        |]
        [| // Level 7
        |]
        [| // Level 8
            { ptype = Glasses; position = Vector3(-5f, 8f, 6f) }
        |]
        [| // Level 9
            { ptype = GrapplingHook; position = Vector3(-6f, 1f, 6f) }
        |]
        [| // Level 10
        |]
        [| // Level 11
        |]
        [| // Level 12
            { ptype = Glasses; position = Vector3(3f, 2f, -4f) }
        |]
        
        
        //// World 2 \\\\
        
        [| // Level 13
            { ptype = Bomb; position = Vector3(4f, 1f, -4f) }
        |]
        [| // Level 14
            { ptype = Bomb; position = Vector3(6f, 7f, 0f) }
        |]
        [| // Level 15
            { ptype = GrapplingHook; position = Vector3(-3f, 4f, -4f) }
        |]
        [| // Level 16
        |]
        [| // Level 17
        |]
    |]
    
    let ready () =
        level <- try (getRoot().GetTree().CurrentScene.SceneFilePath.ToString()[18..19]).ToString().ToInt() - 1
                 with | _ -> (getRoot().GetTree().CurrentScene.SceneFilePath.ToString()[18]).ToString().ToInt() - 1
    
    let getHeightAt x z = (worlds[level] |> Array.find (fun b -> b.position.X = x && b.position.Z = z)).position.Y
    
    let getMaterialAt x z = (worlds[level] |> Array.find (fun b -> b.position.X = x && b.position.Z = z)).material