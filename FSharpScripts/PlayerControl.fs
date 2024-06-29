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

    let withinBoundaries direction =
           let dir = 
               match direction with
               | Forward -> player.Transform.Basis.Z
               | Backward -> -player.Transform.Basis.Z
               | Left -> player.Transform.Basis.X
               | Right -> -player.Transform.Basis.X
           
           (dir + player.Position).Z < 5f && (dir + player.Position).Z > -5f &&
           (dir+ player.Position).X < 5f && (dir + player.Position).X > -5f
    
    let moveLeft () = if withinBoundaries Left then player.Translate(Vector3(1f, 0f, 0f))
    let moveRight () = if withinBoundaries Right then player.Translate(Vector3(-1f, 0f, 0f))
    let moveForward () = if withinBoundaries Forward then player.Translate(Vector3(0f, 0f, 1f))
    let moveBackward () = if withinBoundaries Backward then player.Translate(Vector3(0f, 0f, -1f))
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
       