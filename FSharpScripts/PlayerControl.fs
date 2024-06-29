namespace FSharpScripts

open Godot
open GlobalFunctions

module PlayerControlFS =
    let mutable left = Unchecked.defaultof<Button>
    let mutable right = Unchecked.defaultof<Button>
    let mutable forward = Unchecked.defaultof<Button>
    let mutable backward = Unchecked.defaultof<Button>
    let mutable rotateL = Unchecked.defaultof<Button>
    let mutable rotateR = Unchecked.defaultof<Button>
    let mutable player = Unchecked.defaultof<Node3D>

    let moveLeft () = player.Position <- Vector3(player.Position.X + 1f, player.Position.Y, player.Position.Z)
    let moveRight () = player.Position <- Vector3(player.Position.X - 1f, player.Position.Y, player.Position.Z)
    let moveForward () = player.Position <- Vector3(player.Position.X, player.Position.Y, player.Position.Z + 1f)
    let moveBackward () = player.Position <- Vector3(player.Position.X, player.Position.Y, player.Position.Z - 1f)
    let rotateLeft () =
        player.RotateY -90f
        GD.Print $"{player.Rotation.Y}"
    let rotateRight () = player.Rotate(Vector3.Up, 90f)
    
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
       