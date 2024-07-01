namespace FSharpScripts

open Godot
open GlobalFunctions

module GoalFS =
    let mutable position = Vector3(0f, 0f, 0f)
    let mutable change = 0.15f
    
    let onAreaEntered () = getRoot().GetTree().ChangeSceneToFile("res://LevelSelect.tscn") |> ignore
    
    let ready () =
        let goal = getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("Goal")
        goal.GetNode<Area3D>("Area3D").add_AreaEntered (fun _ -> onAreaEntered())
        position <- goal.Position
        
    let physicsProcess delta =
        let goal = getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("Goal")
        goal.RotateY(degToRad 45f * delta)
        
        goal.Translate(Vector3(0f, change * delta, 0f))
        
        if goal.Position.Y - position.Y > 0.27f || position.Y - goal.Position.Y > 0f then change <- -change