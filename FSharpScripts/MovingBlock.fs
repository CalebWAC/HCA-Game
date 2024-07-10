namespace FSharpScripts

open System.Collections.Generic
open Godot
open GlobalFunctions

module MovingBlockFS =
    let movingBlocks = List<Node3D>()

    type MovingBlock() =
        let mutable self = Unchecked.defaultof<Node3D>
        let mutable origin = Vector3.Zero
        
        let onAreaEntered () =
            self.RotateY(degToRad 180f)
        
        member this.ready thing =
            self <- thing
            origin <- self.Position
            self.GetNode<Area3D>("Border").add_AreaEntered(fun _ -> onAreaEntered())
            
        member this.process (delta : float32) =
           self.Position <- self.Position + self.Transform.Basis.Z * delta
           
           if self.Position.DistanceTo origin > 3f then self.RotateY(degToRad 180f)