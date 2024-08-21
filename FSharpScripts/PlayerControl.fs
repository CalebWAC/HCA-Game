namespace FSharpScripts

open System.Collections.Generic
open Godot
open GlobalFunctions
open WorldFS

module PlayerControlFS =
    type Direction = Left | Right | Forward | Backward
    
    let mutable left = Unchecked.defaultof<Button>
    let mutable right = Unchecked.defaultof<Button>
    let mutable forward = Unchecked.defaultof<Button>
    let mutable backward = Unchecked.defaultof<Button>
    let mutable rotateL = Unchecked.defaultof<Button>
    let mutable rotateR = Unchecked.defaultof<Button>
    let mutable player = Unchecked.defaultof<Node3D>
    
    // For smooth motions
    let mutable originalPos = Unchecked.defaultof<Vector3>
    let mutable midpoint = Unchecked.defaultof<Vector3>
    let mutable placeToBe = Unchecked.defaultof<Vector3>
    let mutable t = 0f
    
    let mutable onBlock : Node3D option = None

    let floatingBlockFront () =
        let convert (b : Node3D) = { position = b.Position; rotation = Vector3.Zero; material = Cave }
        let req1 = fun (b : Block) -> b.position = roundVec placeToBe
        let req2 = fun (b : Block) -> b.position.X = round placeToBe.X && b.position.Z = round placeToBe.Z && b.position.Y = round placeToBe.Y + 1f // not
        let req3 = fun (b : Block) -> b.position.X = round placeToBe.X && b.position.Z = round placeToBe.Z && b.position.Y = round placeToBe.Y + 2f // not
        let req4 = fun (b : Block) -> b.position.X = round player.Position.X && b.position.Z = round player.Position.Z && b.position.Y = round player.Position.Y + 2f // not
        let notReqs = [| req2; req3; req4 |]
        
        let notReqPasses = [ for r in notReqs -> convert >> r |> TerrainManipulatorFS.destructibleBlocks.Exists |> not ]
        let desPasses = TerrainManipulatorFS.destructibleBlocks.Exists (convert >> req1) :: notReqPasses
        
        let notReqPasses = [ for r in notReqs -> Array.exists r worlds[level] |> not ]
        let cavePasses = Array.exists req1 worlds[level] :: notReqPasses
        
        List.forall (fun r -> r = true) desPasses || List.forall (fun r -> r = true) cavePasses
        
    let floatingBlockDown () =
        let convert (b : Node3D) = { position = b.Position; rotation = Vector3.Zero; material = Cave }
        let req1 = fun (b : Block) -> b.position.X = round placeToBe.X && b.position.Z = round placeToBe.Z && b.position.Y - round placeToBe.Y = -2f
        let req2 = fun (b : Block) -> b.position.X = round placeToBe.X && b.position.Z = round placeToBe.Z && b.position.Y - round placeToBe.Y = -1f // not
        let req3 = fun (b : Block) -> b.position = roundVec placeToBe // not
        let notReqs = [| req2; req3 |] 
        
        let notReqPasses = [ for r in notReqs -> convert >> r |> TerrainManipulatorFS.destructibleBlocks.Exists |> not ]
        let desPasses = TerrainManipulatorFS.destructibleBlocks.Exists (convert >> req1) :: notReqPasses
        
        let notReqPasses = [ for r in notReqs -> Array.exists r worlds[level] |> not ]
        let cavePasses = Array.exists req1 worlds[level] :: notReqPasses
        
        List.forall (fun r -> r = true) desPasses || List.forall (fun r -> r = true) cavePasses
         
    let floatingBlockFlat () =
        let convert (b : Node3D) = { position = b.Position; rotation = Vector3.Zero; material = Cave }
        let req1 = fun (b : Block) ->
            if b.position.X = round placeToBe.X && b.position.Z = round placeToBe.Z && b.position.Y - round placeToBe.Y = -1f && b.material = Cave then
                GD.Print $"{placeToBe.X} {round placeToBe.X}        {placeToBe.Z} {round placeToBe.Z}"
                GD.Print b
                true
            else false
        let req2 = fun (b : Block) -> b.position = roundVec placeToBe && b.material = Cave // not
        
        let desPasses = [ TerrainManipulatorFS.destructibleBlocks.Exists (convert >> req1); TerrainManipulatorFS.destructibleBlocks.Exists (convert >> req2) |> not ]
        let cavePasses = [ Array.exists req1 worlds[level]; Array.exists req2 worlds[level] |> not ]
        
        List.forall (fun r -> r = true) desPasses || List.forall (fun r -> r = true) cavePasses
    
    let dirVec direction = 
       match direction with
       | Forward -> player.Transform.Basis.Z
       | Backward -> -player.Transform.Basis.Z
       | Left -> player.Transform.Basis.X
       | Right -> -player.Transform.Basis.X
    
    let withinBoundaries direction =
       let dir = dirVec direction
       ((dir + player.Position).Z < 7f && (dir + player.Position).Z > -7f &&
       (dir + player.Position).X < 7f && (dir + player.Position).X > -7f) ||
       floatingBlockFlat()
    
    let tryMoveBridge () =
        try
            let bridge = elements[level] |> Array.find (fun e -> e.etype = Bridge && e.position.Y = placeToBe.Y - 1f &&
                                                                 (((round placeToBe.X + 0.5f = e.position.X && round placeToBe.Z = e.position.Z) || (round placeToBe.X - 0.5f = e.position.X && round placeToBe.Z = e.position.Z)) ||
                                                                 ((round placeToBe.X = e.position.X && round placeToBe.Z + 0.5f = e.position.Z) || (round placeToBe.X = e.position.X && round placeToBe.Z - 0.5f = e.position.Z))))
            if WorldGeneratorFS.bridges.Find(fun b -> b.Position = bridge.position).Visible = true then
                Result.Ok "All good"
            else Result.Error "Not good"
        with | :? KeyNotFoundException -> Result.Error "Not good"
    
    let tryMoveAquatically place =
        try
            let rounded = if onBlock = None then place else roundVec place
            let bubble = elements[level] |> Array.find (fun e -> e.etype = Bubble && e.position.Y = rounded.Y && 
                                                                 ((rounded.X + 0.5f = e.position.X && rounded.Z = e.position.Z) || (rounded.X - 0.5f = e.position.X && rounded.Z = e.position.Z) ||
                                                                 (rounded.X = e.position.X && rounded.Z + 0.5f = e.position.Z) || (rounded.X = e.position.X && rounded.Z - 0.5f = e.position.Z) ||
                                                                 (rounded.Z - 1f = e.position.Z && rounded.Z - 1f <> player.Position.Z) || (rounded.Z + 1f = e.position.Z && rounded.Z + 1f <> player.Position.Z)))
            placeToBe <- bubble.position
            onBlock <- None
            Result.Ok "All good"
        with | :? KeyNotFoundException -> Result.Error "Not good"
    
    let inBubble () = elements[level] |> Array.exists (fun e -> e.etype = Bubble && e.position = player.Position)
    
    let move dir =
        if withinBoundaries dir && originalPos = placeToBe && player.Position = originalPos then             
            t <- 0f
            placeToBe <- placeToBe + dirVec dir
                
            let block = worlds[level] |> Array.tryFind (fun b -> b.position.X = round placeToBe.X && b.position.Z = round placeToBe.Z)
            
            match block with
            | Some block ->
                // Moving block movement                                          
                if WorldGeneratorFS.movingBlocks.Exists(fun e -> roundVec e.Position = roundVec placeToBe || floorVec e.Position = floorVec placeToBe) then
                    onBlock <- WorldGeneratorFS.movingBlocks.Find(fun e -> roundVec e.Position = roundVec placeToBe || floorVec e.Position = floorVec placeToBe) |> Some
                    placeToBe.Y <- placeToBe.Y + 1f
                    midpoint <- Vector3(player.Position.X, player.Position.Y + 1.5f, player.Position.Z)
                elif WorldGeneratorFS.movingBlocks.Exists(fun e -> roundVec e.Position = roundVec placeToBe - Vector3(0f, 2f, 0f)) then
                    onBlock <- WorldGeneratorFS.movingBlocks.Find(fun e -> roundVec e.Position = roundVec placeToBe - Vector3(0f, 2f, 0f)) |> Some
                    placeToBe.Y <- placeToBe.Y - 1f
                    midpoint <- Vector3(player.Position.X, player.Position.Y + 0.3f, player.Position.Z) + dirVec dir
                elif WorldGeneratorFS.movingBlocks.Exists(fun e -> roundVec e.Position = roundVec placeToBe - Vector3(0f, 1f, 0f)) then
                    onBlock <- WorldGeneratorFS.movingBlocks.Find(fun e -> roundVec e.Position = roundVec placeToBe - Vector3(0f, 1f, 0f)) |> Some
                else
                    let canMoveUp () =
                       ((block.position.Y = round placeToBe.Y && (block.material = Ground || (block.material = Invisible && Array.contains Glasses PlayerFS.powerUps)) || floatingBlockFront()) &&
                       WorldGeneratorFS.companionCubes.Exists(fun c -> roundVec c.Position = roundVec placeToBe + Vector3.Up) |> not) ||
                       WorldGeneratorFS.companionCubes.Exists(fun c -> roundVec c.Position = roundVec placeToBe) ||
                       (Array.contains MoonBoots PlayerFS.powerUps && block.position.Y - round placeToBe.Y >= 1f && block.position.Y - round placeToBe.Y <= 3f)
                    
                    let canMoveDown () = 
                        (((block.position.Y - round placeToBe.Y = -2f && (block.material = Ground || (block.material = Invisible && Array.contains Glasses PlayerFS.powerUps)) || floatingBlockDown()) &&
                          WorldGeneratorFS.companionCubes.Exists(fun c -> roundVec c.Position = roundVec placeToBe - Vector3(0f, 1f, 0f)) |> not) ||
                          WorldGeneratorFS.companionCubes.Exists(fun c -> roundVec c.Position = roundVec placeToBe - Vector3(0f, 2f, 0f)) ||
                          (Array.contains MoonBoots PlayerFS.powerUps && round placeToBe.Y - block.position.Y <= 5f && placeToBe.Y - block.position.Y >= 1f)) &&
                        (floatingBlockFlat() |> not && TerrainManipulatorFS.destructibleBlocks.Exists(fun b -> b.Position = roundVec placeToBe) |> not)
                        
                    // Height compensation
                    if canMoveUp() then
                        if block.material <> Water then
                            onBlock <- None
                        else
                            onBlock <- WorldGeneratorFS.companionCubes.Find(fun c -> roundVec c.Position = roundVec placeToBe) |> Some
                        
                        
                        let cube = WorldGeneratorFS.companionCubes.Find(fun c -> roundVec c.Position = roundVec placeToBe)
                        placeToBe.Y <- if cube = null then block.position.Y + 1f else cube.Position.Y + 1f
                        midpoint <- Vector3(player.Position.X,
                                            (if cube = null then block.position.Y + 1.5f else cube.Position.Y + 1.5f),
                                            player.Position.Z)
                    elif canMoveDown() then
                        onBlock <- None
                        placeToBe.Y <- block.position.Y + 1f
                        midpoint <- Vector3(player.Position.X, player.Position.Y + 0.3f, player.Position.Z) + dirVec dir
                    else
                        // Water bubble movement
                        if tryMoveAquatically placeToBe = Result.Error "Not good" && tryMoveBridge () = Result.Error "Not good" then
                            if inBubble() then
                                if tryMoveAquatically(placeToBe + dirVec dir * 0.5f) = Result.Error "Not good" then
                                    try if block.position.Y = player.Position.Y - 1f || (worlds[level] |> Array.find (fun b ->
                                            let pos = placeToBe + dirVec dir * 0.5f - Vector3(0f, 1f, 0f) |> roundVec
                                            b.position.X = pos.X && b.position.Z = pos.Z)).position.Y = player.Position.Y - 1f then
                                            placeToBe <- placeToBe + dirVec dir * 0.5f |> roundVec
                                        else
                                            placeToBe <- player.Position
                                    with | _ -> placeToBe <- player.Position
                            elif not(WorldGeneratorFS.companionCubes.Exists(fun c -> roundVec c.Position = roundVec placeToBe - Vector3(0f, 1f, 0f))) &&
                                 block.position.Y > placeToBe.Y || block.position.Y - round placeToBe.Y < -2f || block.material = Water || block.material = RushingWater ||
                                 (CompanionCubeFS.somethingHeld && worlds[level] |> Array.exists (fun b -> b.position.X = round placeToBe.X && b.position.Z = round placeToBe.Z && b.position.Y = round player.Position.Y + 2f)) ||
                                 TerrainManipulatorFS.destructibleBlocks.Exists(fun b -> b.Position.X = round placeToBe.X && b.Position.Z = round placeToBe.Z && b.Position.Y = round placeToBe.Y + 1f) then
                                placeToBe <- player.Position
                                
                            midpoint <- (player.Position + placeToBe) / 2f
                            midpoint.Y <- midpoint.Y + 0.25f
                            if placeToBe <> player.Position then onBlock <- None          
            | None ->
                if not(floatingBlockFlat()) then
                    placeToBe <- player.Position
                    midpoint <- (player.Position + placeToBe) / 2f
                    midpoint.Y <- midpoint.Y + 0.25f
                    if placeToBe <> player.Position then onBlock <- None
            
            
    
    let moveLeft () = move Left
    let moveRight () = move Right
    let moveForward () = move Forward
    let moveBackward () = move Backward
    let rotateLeft () = degToRad 90f |> player.RotateY
    let rotateRight () = degToRad -90f |> player.RotateY
    
    let ready () =
        left <- getRoot().GetNode<Control>("Control").GetNode<Button>("Left")
        right <- getRoot().GetNode<Control>("Control").GetNode<Button>("Right")
        forward <- getRoot().GetNode<Control>("Control").GetNode<Button>("Forward")
        backward <- getRoot().GetNode<Control>("Control").GetNode<Button>("Backward")
        rotateL <- getRoot().GetNode<Control>("Control").GetNode<Button>("RotateLeft")
        rotateR <- getRoot().GetNode<Control>("Control").GetNode<Button>("RotateRight")
        
        left.add_Pressed moveLeft
        right.add_Pressed moveRight
        forward.add_Pressed moveForward
        backward.add_Pressed moveBackward
        rotateL.add_Pressed rotateLeft
        rotateR.add_Pressed rotateRight
        
        player <- getRoot().GetNode<Node3D>("Player")
        originalPos <- player.Position
        midpoint <- player.Position
        placeToBe <- player.Position
        
    let physicsProcess delta =
        match onBlock with
        | Some block when t >= 1f ->
            try player.Position <- block.Position + Vector3.Up with | _ -> ()
            placeToBe <- player.Position
            originalPos <- placeToBe
        | _ ->
            t <- if Array.contains MoonBoots PlayerFS.powerUps && Mathf.Abs(round placeToBe.Y - round originalPos.Y) > 1f then
                    Mathf.Min(1f, t + delta * 2f)
                 else Mathf.Min(1f, t + delta * 3f)
            
            if t >= 1f then
                if onBlock.IsSome then
                    player.Position <- roundVec player.Position
                    placeToBe <- roundVec placeToBe
                originalPos <- player.Position
           
            let q1 = originalPos.Lerp(midpoint, t)
            let q2 = midpoint.Lerp(placeToBe, t)
            
            player.Position <- q1.Lerp(q2, t)
    
    let input (event : InputEvent) =
        if not PlayerFS.terrainOn then
            match event with
            | :? InputEventKey ->
                let keyEvent = event :?> InputEventKey
                if keyEvent.Pressed then
                    match keyEvent.Keycode with
                    | Key.W -> move Forward
                    | Key.S -> move Backward
                    | Key.A -> move Left
                    | Key.D -> move Right
                    | Key.Q -> rotateLeft()
                    | Key.E -> rotateRight()
                    | _ -> ()
            | _ -> ()