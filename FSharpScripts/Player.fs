namespace FSharpScripts

open Godot
open GlobalFunctions

module PlayerFS =
    let mutable powerUps : WorldFS.PowerUpType array = [||]
    
    let onAreaEntered (other : Area3D) =
        match other.GetParent().Name.ToString() with
        | "GrapplingHook" ->
            powerUps <- Array.append powerUps [|WorldFS.GrapplingHook|]
            other.GetParent().QueueFree()
        | _ -> ()
    
    let ready () =
        let self = getRoot().GetNode<Node3D>("Player")
        self.GetNode<Area3D>("Area3D").add_AreaEntered (fun other -> onAreaEntered other)
        
    let process delta =
        ()