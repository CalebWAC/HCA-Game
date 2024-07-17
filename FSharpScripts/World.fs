namespace FSharpScripts

open Godot
open GlobalFunctions

module WorldFS =
    type BlockMaterial =
        | Ground
        | Water
        | Invisible
    
    type Block = {
        position : Vector3
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
        
    type PowerUpType =
        | GrapplingHook
        | TerrainManipulator
        | Glasses
        
    type Element = { etype: ElementType; position: Vector3; rotation: Vector3; visible: bool }
    
    type PowerUp = { ptype: PowerUpType; position: Vector3 }
    
    let block x y z = { position = Vector3(x, y, z); material = Ground }
    let waterBlock x y z = { position = Vector3(x, y, z); material = Water }
    let hiddenBlock x y z = { position = Vector3(x, y, z); material = Invisible }
    
    let mutable level = 0
    
    let completedLevels = Array.create 12 false
    
    let worlds = [|
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
           block -6f 0f -6f; block -6f 0f -5f; block -6f 0f -4f; block -6f 0f -3f; block -6f 0f -2f; block -6f 0f -1f; block -6f 0f 0f; block -6f 0f 1f; block -6f 0f 2f; block -6f 0f 3f; block -6f 0f 4f; block -6f 0f 5f; block -6f 0f 6f
           block -5f 0f -6f; block -5f 0f -5f; block -5f 0f -4f; block -5f 0f -3f; block -5f 0f -2f; block -5f 0f -1f; block -5f 0f 0f; block -5f 0f 1f; block -5f 0f 2f; block -5f 0f 3f; block -5f 0f 4f; block -5f 0f 5f; block -5f 0f 6f
           block -4f 0f -6f; block -4f 0f -5f; block -4f 0f -4f; block -4f 0f -3f; block -4f 0f -2f; block -4f 0f -1f; block -4f 0f 0f; block -4f 0f 1f; block -4f 0f 2f; block -4f 0f 3f; block -4f 0f 4f; block -4f 0f 5f; block -4f 0f 6f
           block -3f 2f -6f; block -3f 0f -5f; block -3f 0f -4f; block -3f 2f -3f; block -3f 0f -2f; block -3f 0f -1f; block -3f 0f 0f; block -3f 0f 1f; block -3f 0f 2f; block -3f 0f 3f; block -3f 0f 4f; block -3f 0f 5f; block -3f 0f 6f
           block -2f 1f -6f; block -2f 0f -5f; block -2f 0f -4f; block -2f 0f -3f; block -2f 0f -2f; block -2f 0f -1f; block -2f 0f 0f; block -2f 0f 1f; block -2f 0f 2f; block -2f 0f 3f; block -2f 0f 4f; block -2f 0f 5f; block -2f 0f 6f
           block -1f 0f -6f; block -1f 0f -5f; block -1f 0f -4f; block -1f 0f -3f; block -1f 0f -2f; block -1f 0f -1f; block -1f 0f 0f; block -1f 0f 1f; block -1f 0f 2f; block -1f 0f 3f; block -1f 0f 4f; block -1f 0f 5f; block -1f 0f 6f
           block 0f 0f -6f; block 0f 0f -5f; block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 0f 2f; block 0f 0f 3f; block 0f 0f 4f; block 0f 0f 5f; block 0f 0f 6f
           block 1f 0f -6f; block 1f 0f -5f; block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 0f 1f; block 1f 0f 2f; block 1f 0f 3f; block 1f 0f 4f; block 1f 0f 5f; block 1f 0f 6f
           block 2f 0f -6f; block 2f 0f -5f; block 2f 0f -4f; block 2f 0f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 0f 0f; block 2f 0f 1f; block 2f 0f 2f; block 2f 0f 3f; block 2f 0f 4f; block 2f 0f 5f; block 2f 0f 6f
           block 3f 0f -6f; block 3f 0f -5f; block 3f 0f -4f; block 3f 0f -3f; block 3f 0f -2f; block 3f 0f -1f; block 3f 0f 0f; block 3f 0f 1f; block 3f 0f 2f; block 3f 0f 3f; block 3f 0f 4f; block 3f 0f 5f; block 3f 0f 6f
           block 4f 0f -6f; block 4f 0f -5f; block 4f 0f -4f; block 4f 0f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 0f 0f; block 4f 0f 1f; block 4f 0f 2f; block 4f 0f 3f; block 4f 0f 4f; block 4f 0f 5f; block 4f 0f 6f
           block 5f 0f -6f; block 5f 0f -5f; block 5f 0f -4f; block 5f 0f -3f; block 5f 0f -2f; block 5f 0f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 0f 2f; block 5f 0f 3f; block 5f 0f 4f; block 5f 0f 5f; block 5f 0f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 0f 2f; block 6f 0f 3f; block 6f 0f 4f; block 6f 0f 5f; block 6f 0f 6f
        |]
    |]
    
    let elements = [|
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
            { etype = CompanionCube; position = Vector3(2f, 1f, -1f); rotation = Vector3.Zero; visible = true }
            { etype = CubeTrigger; position = Vector3(0f, 1f, -1f); rotation = Vector3.Zero; visible = true }
            { etype = Bridge; position = Vector3(-3f, 2f, -4.5f); rotation = Vector3(0f, degToRad 90f, 0f); visible = false }
        |]
    |]
    
    let powerUps = [|
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
    |]
    
    let ready () = level <-
        try (getRoot().GetTree().CurrentScene.SceneFilePath.ToString()[18..19]).ToString().ToInt() - 1
        with | _ -> (getRoot().GetTree().CurrentScene.SceneFilePath.ToString()[18]).ToString().ToInt() - 1
    
    let getHeightAt x z = (worlds[level] |> Array.find (fun b -> b.position.X = x && b.position.Z = z)).position.Y