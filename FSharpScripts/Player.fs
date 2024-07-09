namespace FSharpScripts

open Godot
open GlobalFunctions

module PlayerFS =
    let mutable self = Unchecked.defaultof<Node3D>
    let mutable powerUps : WorldFS.PowerUpType array = [||]
    let mutable terrainOn = false
    
    let onAreaEntered (other : Area3D) =
        match other.GetParent().Name.ToString() with
        | "GrapplingHook" ->
            powerUps <- Array.append powerUps [|WorldFS.GrapplingHook|]
            other.GetParent().QueueFree()
        | "TerrainManipulator" ->
            powerUps <- Array.append powerUps [|WorldFS.TerrainManipulator|]
            other.GetParent().QueueFree()
        | "Glasses" ->
            powerUps <- Array.append powerUps [|WorldFS.Glasses|]
            other.GetParent().QueueFree()
        | _ -> ()
    
    let ready () =
        self <- getRoot().GetNode<Node3D>("Player")
        self.GetNode<Area3D>("Area3D").add_AreaEntered (fun other -> onAreaEntered other)
        
    let process delta =
        ()