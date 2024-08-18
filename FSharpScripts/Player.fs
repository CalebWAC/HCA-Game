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
            elif WorldFS.level = 17 then
                getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("Goal").Visible <- true
        | "Bomb" ->
            powerUps <- Array.append powerUps [|WorldFS.Bomb|]
            other.GetParent().QueueFree()
            if WorldFS.level = 12 then
                getRoot().GetTree().Paused <- true
                getRoot().GetNode<Control>("TutorialBomb").Visible <- true
        | "MoonBoots" ->
            powerUps <- Array.append powerUps [|WorldFS.MoonBoots|]
            other.GetParent().QueueFree()
            if WorldFS.level = 18 then
                getRoot().GetTree().Paused <- true
                getRoot().GetNode<Control>("TutorialMoonBoots").Visible <- true
        | _ -> ()
    
    let ready () =
        self <- getRoot().GetNode<Node3D>("Player")
        self.GetNode<Area3D>("Area3D").add_AreaEntered (fun other -> onAreaEntered other)
        
    let process delta = ()
    
    let input (event : InputEvent) =
        if Array.contains WorldFS.Bomb powerUps && event :? InputEventKey then
            let keyEvent = event :?> InputEventKey
            if keyEvent.IsReleased() && keyEvent.Keycode = Key.Space then
                let scene = GD.Load<PackedScene>("res://Power Ups/MiniBomb.tscn")
                let bomb = scene.Instantiate() :?> Node3D
                bomb.Position <- self.Position + self.Transform.Basis.Z / 2f + Vector3(0f, 1f, 0f)
                getRoot().AddChild(bomb)
                