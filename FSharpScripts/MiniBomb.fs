namespace FSharpScripts

open Godot
open GlobalFunctions

module MiniBombFS =
    let ready (thing : RigidBody3D) =
        let player = getRoot().GetNode<Node3D>("Player")
        GD.Print "Here"
        thing.ApplyForce(Vector3(0f, 750f, 0f) + player.Transform.Basis.Z * 300f)
        
    
    // let physicsProcess delta = ()
        