namespace FSharpScripts

open Godot
open GlobalFunctions
open WorldFS
open PlayerFS

module BridgeFS =
    type Bridge() =
        let mutable self = Unchecked.defaultof<Node3D>
        
        member this.ready thing =
            self <- thing
            
        member this.process delta =
            if elements[level] |> Array.exists (fun b -> b.position = self.Position && b.visible = false) then
                if Array.contains Glasses powerUps && getRoot().GetNode<Node3D>("Player").Position.DistanceTo self.Position <= 3f then
                    self.Visible <- true
                else self.Visible <- false