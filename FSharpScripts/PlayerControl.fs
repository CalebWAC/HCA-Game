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

    let dirVec direction = 
       match direction with
       | Forward -> player.Transform.Basis.Z
       | Backward -> -player.Transform.Basis.Z
       | Left -> player.Transform.Basis.X
       | Right -> -player.Transform.Basis.X
    
    let withinBoundaries direction =
       let dir = dirVec direction
       (dir + player.Position).Z < 7f && (dir + player.Position).Z > -7f &&
       (dir + player.Position).X < 7f && (dir + player.Position).X > -7f
    
    let tryMoveBridge () =
        try
            let bridge = elements[level] |> Array.find (fun e -> e.etype = Bridge && e.position.Y = placeToBe.Y - 1f &&
                                                                 (((round placeToBe.X + 0.5f = e.position.X && round placeToBe.Z = e.position.Z) || (round placeToBe.X - 0.5f = e.position.X && round placeToBe.Z = e.position.Z)) ||
                                                                 ((round placeToBe.X = e.position.X && round placeToBe.Z + 0.5f = e.position.Z) || (round placeToBe.X = e.position.X && round placeToBe.Z - 0.5f = e.position.Z))))
            Result.Ok "All good"
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
        if withinBoundaries dir && originalPos = placeToBe then             
            t <- 0f
            placeToBe <- placeToBe + dirVec dir
                
            let block = worlds[level] |> Array.find (fun b -> b.position.X = round placeToBe.X && b.position.Z = round placeToBe.Z)
            
            // Moving block movement                                          
            if MovingBlockFS.movingBlocks.Exists(fun e -> roundVec e.Position = roundVec placeToBe || floorVec e.Position = floorVec placeToBe) then
                onBlock <- MovingBlockFS.movingBlocks.Find(fun e -> roundVec e.Position = roundVec placeToBe || floorVec e.Position = floorVec placeToBe) |> Some
                placeToBe.Y <- placeToBe.Y + 1f
                midpoint <- Vector3(player.Position.X, player.Position.Y + 1.5f, player.Position.Z)
            elif MovingBlockFS.movingBlocks.Exists(fun e -> roundVec e.Position = roundVec placeToBe - Vector3(0f, 2f, 0f)) then
                onBlock <- MovingBlockFS.movingBlocks.Find(fun e -> roundVec e.Position = roundVec placeToBe - Vector3(0f, 2f, 0f)) |> Some
                placeToBe.Y <- placeToBe.Y - 1f
                midpoint <- Vector3(player.Position.X, player.Position.Y + 0.3f, player.Position.Z) + dirVec dir
            else
                // Height compensation
                if block.position.Y = round placeToBe.Y && (block.material = Ground || (block.material = Invisible && Array.contains Glasses PlayerFS.powerUps)) then
                    onBlock <- None
                    placeToBe.Y <- placeToBe.Y + 1f
                    midpoint <- Vector3(player.Position.X, player.Position.Y + 1.5f, player.Position.Z)
                elif block.position.Y - round placeToBe.Y = -2f && (block.material = Ground || (block.material = Invisible && Array.contains Glasses PlayerFS.powerUps)) then
                    onBlock <- None
                    placeToBe.Y <- placeToBe.Y - 1f
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
                        elif block.position.Y > placeToBe.Y || block.position.Y - round placeToBe.Y < -2f || block.material = Water ||
                             elements[level] |> Array.exists (fun e -> e.etype = CompanionCube && e.position = roundVec placeToBe) then
                            placeToBe <- player.Position
                            
                    midpoint <- (player.Position + placeToBe) / 2f
                    midpoint.Y <- midpoint.Y + 0.25f
                    onBlock <- None
    
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
            t <- Mathf.Min(1f, t + delta * 3f)
            
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