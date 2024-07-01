namespace FSharpScripts

open Godot
open GlobalFunctions

module GoalFS =
    let onAreaEntered () = getRoot().GetTree().ChangeSceneToFile("res://LevelSelect.tscn") |> ignore
    
    let ready () =
        let area3d = getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("Goal").GetNode<Area3D>("Area3D")
        area3d.add_AreaEntered (fun x -> onAreaEntered())