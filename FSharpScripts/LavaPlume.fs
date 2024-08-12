namespace FSharpScripts

open Godot
open GlobalFunctions

module LavaPlumeFS =
    type Direction =
        | Up
        | Down
        | Stagnant
    
    type LavaPlume() =
        let mutable tChange = 0f
        let mutable direction = Stagnant
        let mutable self = Unchecked.defaultof<Node3D>

        member this.ready (thing : Node3D) =
            self <- thing.GetNode<Node3D>("Model")
            
        member this.process delta =
            tChange <- tChange + delta
            
            if tChange > 10f then
                GD.Print $"At a change point. Scale: {self.Scale.Y}"
                tChange <- 0f
                if self.Scale.Y > 1f then direction <- Down else direction <- Up
           
            if direction = Up && self.Scale.Y < 20f then
                self.ScaleObjectLocal(Vector3(1f, 1f + delta, 1f))
                self.GetParent().GetNode<Node3D>("Particles").Translate(Vector3(0f, delta * 1.3f, 0f))
            elif direction = Down && self.Scale.Y > 1f then
                self.ScaleObjectLocal(Vector3(1f, 1f - delta, 1f))
                self.GetParent().GetNode<Node3D>("Particles").Translate(Vector3(0f, -delta * 1.3f, 0f))
                
            if self.Scale.Y <= 1f || self.Scale.Y >= 20f then direction <- Stagnant