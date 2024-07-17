namespace FSharpScripts

open Godot
open WorldFS
open GlobalFunctions
open System.Collections.Generic

module CubeTriggerFS =
    let cubeTriggers = List<Node3D>()
    
    type CubeTrigger() =
        let mutable self = Unchecked.defaultof<Node3D>
        
        let onActivated (other : Area3D) =
            GD.Print other.Name
        
        member this.ready thing =
            self <- thing
            self.GetNode<Area3D>("Area3D").add_AreaEntered (fun other -> onActivated other)
        
        member this.process (delta : float32) = ()