namespace FSharpScripts

open Godot
open WorldFS

module CubeTriggerFS =
    type CubeTrigger() =
        let mutable self = Unchecked.defaultof<Node3D>
        
        let onActivated (other : Area3D) =
            match other.GetParent().Name.ToString() with
            | "CompanionCube" ->
                match level with
                | 9 -> if self.Position = Vector3(0f, 1f, -1f) then WorldGeneratorFS.bridges.ForEach(fun b -> b.Visible <- true)
                | _ -> ()
            | _ -> ()
        
        let onDeactivated (other : Area3D) =
            match other.GetParent().Name.ToString() with
            | "CompanionCube" ->
                match level with
                | 9 -> if self.Position = Vector3(0f, 1f, -1f) then WorldGeneratorFS.bridges.ForEach(fun b -> b.Visible <- false)
                | _ -> ()
            | _ -> ()
        
        member this.ready thing =
            self <- thing
            self.GetNode<Area3D>("Area3D").add_AreaEntered (fun other -> onActivated other)
            self.GetNode<Area3D>("Area3D").add_AreaExited (fun other -> onDeactivated other)
        
        member this.process (delta : float32) = ()