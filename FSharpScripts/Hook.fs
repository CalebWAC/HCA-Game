namespace FSharpScripts

open Godot
open GlobalFunctions

module HookFS =
    
    let onInputEvent (event : InputEvent) =
        try
            let mouseClick = event :?> InputEventMouseButton
            if mouseClick.ButtonIndex = MouseButton.Left && mouseClick.Pressed then
                if Array.contains WorldFS.PowerUpType.GrapplingHook PlayerFS.powerUps then
                    PlayerControlFS.placeToBe <- getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("Hook").Position
                    PlayerControlFS.placeToBe.Y <- PlayerControlFS.placeToBe.Y + 1f
                    PlayerControlFS.placeToBe.Z <- PlayerControlFS.placeToBe.Z + 0.5f
                    PlayerControlFS.midpoint <- (PlayerControlFS.originalPos + PlayerControlFS.placeToBe) / 2f
                    PlayerControlFS.t <- 0f
        with | _ -> ()
    
    let ready () =
        let self = getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("Hook")
        self.GetNode<Area3D>("Area3D").add_InputEvent (fun _ event _ _ _ -> onInputEvent event)