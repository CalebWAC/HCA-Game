namespace FSharpScripts

open Godot
open GlobalFunctions

module GlassesFS =
    let mutable self = Unchecked.defaultof<Node3D>
    
    let ready () = self <- getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("Glasses").GetNode<Node3D>("Model")
        
    let physicsProcess delta = self.RotateY(degToRad 45f * delta)