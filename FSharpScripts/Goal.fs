namespace FSharpScripts

open Godot
open GlobalFunctions

module GoalFS =
    type Goal() =
        let mutable position = Vector3(0f, 0f, 0f)
        let mutable change = 0.15f
        let mutable goal = Unchecked.defaultof<Node3D>
        
        let onAreaEntered () =
            Array.set WorldFS.completedLevels WorldFS.level true
            getRoot().GetTree().ChangeSceneToFile("res://LevelSelect.tscn") |> ignore
        
        member this.ready (thing : Node3D) =
            goal <- thing
            goal.GetNode<Area3D>("Area3D").add_AreaEntered (fun _ -> onAreaEntered())
            position <- goal.Position
            
        member this.physicsProcess delta =
            goal.RotateY(degToRad 45f * delta)
            
            goal.Translate(Vector3(0f, change * delta, 0f))
            
            if goal.Position.Y - position.Y > 0.27f || position.Y - goal.Position.Y > 0f then change <- -change