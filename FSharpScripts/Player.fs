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
            if WorldFS.level = 0 then
                getRoot().GetTree().Paused <- true
                getRoot().GetNode<Control>("TutorialGrapplingHook").Visible <- true
        | "TerrainManipulator" ->
            powerUps <- Array.append powerUps [|WorldFS.TerrainManipulator|]
            other.GetParent().QueueFree()
            if WorldFS.level = 1 then
                getRoot().GetTree().Paused <- true
                getRoot().GetNode<Control>("TutorialTerrainManipulator").Visible <- true
        | "Glasses" ->
            powerUps <- Array.append powerUps [|WorldFS.Glasses|]
            other.GetParent().QueueFree()
            if WorldFS.level = 5 then
                getRoot().GetTree().Paused <- true
                getRoot().GetNode<Control>("TutorialInvisibleGlasses").Visible <- true
        | _ -> ()
    
    let ready () =
        self <- getRoot().GetNode<Node3D>("Player")
        self.GetNode<Area3D>("Area3D").add_AreaEntered (fun other -> onAreaEntered other)
        
    let process delta =
        ()