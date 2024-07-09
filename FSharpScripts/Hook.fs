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
                        if match level with
                           | 0 -> true
                           | 2 ->
                               let pos = (self.Position.X, self.Position.Y, self.Position.Z)
                               match pos with
                               | 2.5f, 5f, -4f -> seq { for x in 3f..6f do for z in -6f..3f do for y in 1f..6f -> Vector3(x, y, z) }  |> Seq.contains player.Position
                               | 5.5f, 5f, 0f -> seq { for x in 2f..5f do for z in -6f..5f do for y in 5f..6f -> Vector3(x, y, z) }  |> Seq.contains player.Position
                               | 3f, 5f, 4.5f -> seq { for x in -1f..6f do for z in 1f..4f do for y in 5f..6f -> Vector3(x, y, z) }  |> Seq.contains player.Position
                               | 0f, 5f, 2.5f -> seq { for x in -3f..4f do for z in 3f..5f do for y in 5f..6f -> Vector3(x, y, z) }  |> Seq.contains player.Position
                               | -2f, 5f, 4.5f -> seq { for x in -5f..2f do for z in 2f..4f do for y in 5f..6f -> Vector3(x, y, z) }  |> Seq.contains player.Position
                               | -4.5f, 5f, 3f -> seq { for x in -4f.. -2f do for z in -1f..5f do for y in 1f..6f -> Vector3(x, y, z) }  |> Seq.contains player.Position
                               | -5f, 6f, -0.5f -> seq { for x in -5f.. -4f do for z in 0f..4f do for y in 1f..6f -> Vector3(x, y, z) }  |> Seq.contains player.Position
                               | -3.5f, 5f, -5f -> seq { for x in -2f.. -1f do for z in -5f.. -4f do for y in 1f..6f -> Vector3(x, y, z) }  |> Seq.contains player.Position
                               | _ -> true
                           | _ -> false
                        then 
                            placeToBe <- self.Position
                            placeToBe.Y <- placeToBe.Y + 2f
                           
                            placeToBe <- placeToBe - self.Transform.Basis.Z * 0.5f
                            placeToBe <- roundVec placeToBe
                            if getHeightAt placeToBe.X placeToBe.Z < placeToBe.Y - 2f then
                                placeToBe <- placeToBe + self.Transform.Basis.Z
                                
                            placeToBe <- roundVec placeToBe
                            midpoint <- (originalPos + placeToBe) / 2f
                            t <- 0f
            with | _ -> ()
        
        member this.ready (thing : Node3D) =
            self <- thing // getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("Hook")
            self.GetNode<Area3D>("Area3D").add_InputEvent (fun _ event _ _ _ -> onInputEvent event)