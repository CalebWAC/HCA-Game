namespace FSharpScripts

open Godot
open GlobalFunctions

module MiniBombFS =
    let ready (thing : RigidBody3D) =
        let player = getRoot().GetNode<Node3D>("Player")
        thing.ApplyForce(Vector3(0f, 500f, 0f) + player.Transform.Basis.Z * 400f)
        
    let process delta =
        if getRoot().GetNode<Node3D>("MiniBomb").Position.Y < -5f then
            getRoot().GetNode<Node3D>("MiniBomb").QueueFree()