namespace FSharpScripts

open Godot
open GlobalFunctions

module WorldFS =
    type BlockMaterial =
        | Ground
        | Water
    
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
        
    type PowerUpType =
        | GrapplingHook
        | TerrainManipulator
        
    type Element = { etype: ElementType; position: Vector3; rotation: Vector3 }
    
    type PowerUp = { ptype: PowerUpType; position: Vector3 }
    
    let block x y z = { position = Vector3(x, y, z); material = Ground }
    let waterBlock x y z = { position = Vector3(x, y, z); material = Water }
    
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
           block 0f 0f -6f; block 0f 0f -5f; block 0f 0f -4f; block 0f 0f -3f; block 0f 0f -2f; block 0f 0f -1f; block 0f 0f 0f; block 0f 0f 1f; block 0f 1f 2f; block 0f 0f 3f; block 0f 0f 4f; block 0f 0f 5f; block 0f 0f 6f
           block 1f 0f -6f; block 1f 0f -5f; block 1f 0f -4f; block 1f 0f -3f; block 1f 0f -2f; block 1f 0f -1f; block 1f 0f 0f; block 1f 0f 1f; block 1f 1f 2f; block 1f 0f 3f; block 1f 0f 4f; block 1f 0f 5f; block 1f 0f 6f
           block 2f 0f -6f; block 2f 4f -5f; block 2f 4f -4f; block 2f 5f -3f; block 2f 0f -2f; block 2f 0f -1f; block 2f 0f 0f; block 2f 0f 1f; block 2f 1f 2f; block 2f 0f 3f; block 2f 0f 4f; block 2f 0f 5f; block 2f 0f 6f
           block 3f 0f -6f; block 3f 3f -5f; block 3f 6f -4f; block 3f 6f -3f; block 3f 0f -2f; block 3f 0f -1f; block 3f 0f 0f; block 3f 0f 1f; block 3f 1f 2f; block 3f 0f 3f; block 3f 0f 4f; block 3f 0f 5f; block 3f 0f 6f
           block 4f 0f -6f; block 4f 3f -5f; block 4f 6f -4f; block 4f 6f -3f; block 4f 0f -2f; block 4f 0f -1f; block 4f 0f 0f; block 4f 0f 1f; block 4f 1f 2f; block 4f 0f 3f; block 4f 0f 4f; block 4f 0f 5f; block 4f 0f 6f
           block 5f 0f -6f; block 5f 2f -5f; block 5f 2f -4f; block 5f 1f -3f; block 5f 0f -2f; block 5f 0f -1f; block 5f 0f 0f; block 5f 0f 1f; block 5f 1f 2f; block 5f 0f 3f; block 5f 0f 4f; block 5f 0f 5f; block 5f 1f 6f
           block 6f 0f -6f; block 6f 0f -5f; block 6f 0f -4f; block 6f 0f -3f; block 6f 0f -2f; block 6f 0f -1f; block 6f 0f 0f; block 6f 0f 1f; block 6f 1f 2f; block 6f 0f 3f; block 6f 0f 4f; block 6f 1f 5f; block 6f 2f 6f
        |]
    |]
    
    let elements = [|
        [| // Level 1
            { etype = Goal; position = Vector3(-5f, 9f, -6f); rotation = Vector3.Zero }
            { etype = Bubble; position = Vector3(-5f, 7f, 3.5f); rotation = Vector3.Zero }
            { etype = Bubble; position = Vector3(-5f, 7f, 1.5f); rotation = Vector3.Zero }
            { etype = Bridge; position = Vector3(-2.5f, 6f, 0f); rotation = Vector3.Zero }
            { etype = Hook; position = Vector3(-3f, 7f, -5.5f); rotation = Vector3.Zero }
        |]
        [| // Level 2
            { etype = Goal; position = Vector3(-3f, 8f, 3f); rotation = Vector3.Zero }
            { etype = Bridge; position = Vector3(-5f, 1f, -3.5f); rotation = Vector3(0f, degToRad 90f, 0f) }
            { etype = LavaWall; position = Vector3(3f, 1.5f, 2f); rotation = Vector3(0f, degToRad 90f, 0f) }
        |]
    |]
    
    let powerUps = [|
        [| // Level 1
            { ptype = GrapplingHook; position = Vector3(-2f, 1f, 0f) }
        |]
        [| // Level 2
            { ptype = TerrainManipulator; position = Vector3(4f, 7f, -4f) }
        |]
    |]
    
    let ready () = level <- (getRoot().GetTree().CurrentScene.SceneFilePath.ToString()[11]).ToString().ToInt() - 1