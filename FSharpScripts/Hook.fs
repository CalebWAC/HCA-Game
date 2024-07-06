namespace FSharpScripts

open Godot
open WorldFS
open PlayerControlFS
open GlobalFunctions

module HookFS =
    type Hook() =
        let mutable self = Unchecked.defaultof<Node3D>
        
        let onInputEvent (event : InputEvent) =
            try
                let mouseClick = event :?> InputEventMouseButton
                if mouseClick.ButtonIndex = MouseButton.Left && mouseClick.Pressed then
                    if Array.contains PowerUpType.GrapplingHook PlayerFS.powerUps && self.Position.DistanceTo(player.Position) < 6f then
                        placeToBe <- self.Position
                        placeToBe.Y <- placeToBe.Y + 2f
                       
                        placeToBe <- placeToBe - self.Transform.Basis.Z * 0.5f
                        placeToBe <- roundVec placeToBe
                        if getHeightAt placeToBe.X placeToBe.Z < placeToBe.Y - 2f then
                            placeToBe <- placeToBe + self.Transform.Basis.Z
                            
                        midpoint <- (originalPos + placeToBe) / 2f
                        t <- 0f
            with | _ -> ()
        
        member this.ready (thing : Node3D) =
            self <- thing // getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("Hook")
            self.GetNode<Area3D>("Area3D").add_InputEvent (fun _ event _ _ _ -> onInputEvent event)