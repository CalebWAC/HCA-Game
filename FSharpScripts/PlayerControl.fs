namespace FSharpScripts

open Godot
open GlobalFunctions

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
       (dir + player.Position).Z < 5f && (dir + player.Position).Z > -5f &&
       (dir + player.Position).X < 5f && (dir + player.Position).X > -5f
    
    let move dir =
        if withinBoundaries dir && originalPos = placeToBe then
            t <- 0f
            placeToBe <- placeToBe + dirVec dir
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
        
        if t >= 1f then originalPos <- player.Position
        
        let q1 = originalPos.Lerp(midpoint, t)
        let q2 = midpoint.Lerp(placeToBe, t)
        
        player.Position <- q1.Lerp(q2, t)
       