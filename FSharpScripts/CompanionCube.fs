namespace FSharpScripts

open Godot
open WorldFS
open GlobalFunctions

module CompanionCubeFS =
    type CompanionCube() =
        let mutable self = Unchecked.defaultof<Node3D>
        let mutable held = false
        
        let mutable t = 1f
        let mutable originalPos = Vector3(0f, 0f, 0f)
        let mutable endPos = Vector3(0f, 0f, 0f)
        let mutable midpoint = Vector3(0f, 0f, 0f)
        
        member this.ready thing =
            self <- thing
            originalPos <- self.Position
        
        member this.physicsProcess (delta : float32) =
            t <- Mathf.Min(1f, t + delta * 3f)
            
            if t < 1f then 
                let q1 = originalPos.Lerp(midpoint, t)
                let q2 = midpoint.Lerp(endPos, t)
                
                self.Position <- q1.Lerp(q2, t)
            elif held then self.Position <- getRoot().GetNode<Node3D>("Player").Position + Vector3.Up * 2f
            else originalPos <- endPos; self.Position <- roundVec self.Position
            
        member this.input (event : InputEvent) =
            match event with
            | :? InputEventKey ->
                let keyEvent = event :?> InputEventKey
                if keyEvent.Pressed then
                    match keyEvent.Keycode with
                    | Key.Space ->
                        let playerPos = getRoot().GetNode<Node3D>("Player").Position
                        let next = playerPos + getRoot().GetNode<Node3D>("Player").Transform.Basis.Z
                        let nextBlock = (worlds[level] |> Array.find (fun b -> b.position.X = round next.X && b.position.Z = round next.Z)).position
                        
                        if not held && self.Position = roundVec next ||
                           held && nextBlock.Y <= playerPos.Y + 1f then
                            held <- not held
                            t <- 0f
                        
                            if held then
                                originalPos <- self.Position
                                endPos <- playerPos + Vector3.Up * 2f
                                midpoint <- Vector3(self.Position.X, self.Position.Y + 1.5f, self.Position.Z)
                            else
                                originalPos <- self.Position
                                endPos <- nextBlock + Vector3.Up
                                midpoint <- Vector3(self.Position.X, self.Position.Y + 0.3f, self.Position.Z)
                    | _ -> ()
            | _ -> ()