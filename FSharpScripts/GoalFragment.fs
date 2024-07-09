namespace FSharpScripts

open Godot
open GlobalFunctions

module GoalFragmentFS =
    let mutable frags = 0
    let mutable change = 0.15f
    
    type Fragment() =
        let mutable self = Unchecked.defaultof<Node3D>
        
        let onAreaEntered () =
            frags <- frags + 1
            if frags = 5 then
                Array.set WorldFS.completedLevels WorldFS.level true
                getRoot().GetTree().ChangeSceneToFile("res://LevelSelect.tscn") |> ignore
            self.QueueFree()
        
        member this.ready thing =
            self <- thing
            self.GetNode<Area3D>("Area3D").add_AreaEntered(fun _ -> onAreaEntered())
            
        member this.physicsProcess delta =
            self.RotateX(degToRad 45f * delta)
            self.RotateZ(degToRad 45f * delta)