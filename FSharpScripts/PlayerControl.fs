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

    let moveLeft () = player.Translate(Vector3(1f, 0f, 0f))
    let moveRight () = player.Translate(Vector3(-1f, 0f, 0f))
    let moveForward () = player.Translate(Vector3(0f, 0f, 1f))
    let moveBackward () = player.Translate(Vector3(0f, 0f, -1f))
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
       