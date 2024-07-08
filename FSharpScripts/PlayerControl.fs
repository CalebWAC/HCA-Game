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
    
    let tryMoveAquatically () =
        try
            let bubble = elements[level] |> Array.find (fun e -> e.etype = Bubble && e.position.Y = placeToBe.Y && 
                                                          (((placeToBe.X + 0.5f = e.position.X && placeToBe.Z = e.position.Z) || (placeToBe.X - 0.5f = e.position.X && placeToBe.Z = e.position.Z)) ||
                                                          ((placeToBe.X = e.position.X && placeToBe.Z + 0.5f = e.position.Z) || (placeToBe.X = e.position.X && placeToBe.Z - 0.5f = e.position.Z)) ||
                                                          ((placeToBe.Z - 1f = e.position.Z && placeToBe.Z - 1f <> player.Position.Z) || (placeToBe.Z + 1f = e.position.Z && placeToBe.Z + 1f <> player.Position.Z))))
            placeToBe <- bubble.position
            Result.Ok "All good"
        with | :? KeyNotFoundException -> Result.Error "Not good"
    
    let inBubble () = elements[level] |> Array.exists (fun e -> e.etype = Bubble && e.position = player.Position)
    
    let move dir =
        if withinBoundaries dir && originalPos = placeToBe then             
            t <- 0f
            placeToBe <- placeToBe + dirVec dir
                
            let block = worlds[level] |> Array.find (fun b -> b.position.X = Mathf.Round(placeToBe.X) && b.position.Z = Mathf.Round(placeToBe.Z))
            
            // Height compensation
            if block.position.Y = placeToBe.Y && block.material = Ground then
                placeToBe.Y <- placeToBe.Y + 1f
                midpoint <- Vector3(player.Position.X, player.Position.Y + 1.5f, player.Position.Z)
            elif block.position.Y - Mathf.Round(placeToBe.Y) = -2f && block.material = Ground then
                placeToBe.Y <- placeToBe.Y - 1f
                midpoint <- Vector3(player.Position.X, player.Position.Y + 0.3f, player.Position.Z) + dirVec dir
            else
                // Water bubble movement
                if tryMoveAquatically () = Result.Error "Not good" && tryMoveBridge () = Result.Error "Not good" then
                    if inBubble() then
                        placeToBe <- placeToBe + dirVec dir * 0.5f
                    elif block.position.Y > placeToBe.Y || block.position.Y - Mathf.Round(placeToBe.Y) < -2f || block.material <> Ground then
                        placeToBe <- player.Position
                        
                midpoint <- (player.Position + placeToBe) / 2f
                midpoint.Y <- midpoint.Y + 0.25f
    
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
        t <- Mathf.Min(1f, t + delta * 3f)
        
        if t >= 1f then
            originalPos <- player.Position
            placeToBe <- Vector3(placeToBe.X, Mathf.Floor player.Position.Y, placeToBe.Z)
            player.Position <- Vector3(player.Position.X, Mathf.Floor player.Position.Y, player.Position.Z)
        
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
                    | Key.Up -> move Forward
                    | Key.Down -> move Backward
                    | Key.Left -> move Left
                    | Key.Right -> move Right
                    | Key.Q -> rotateLeft()
                    | Key.E -> rotateRight()
                    | _ -> ()
            | _ -> ()