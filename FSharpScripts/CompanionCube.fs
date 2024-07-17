namespace FSharpScripts

open Godot
open WorldFS
open GlobalFunctions

module CompanionCubeFS =
    type CompanionCube() =
        let mutable self = Unchecked.defaultof<Node3D>
        let mutable held = false
        
        let mutable t = 0f
        let mutable originalPos = Vector3(0f, 0f, 0f)
        let mutable endPos = Vector3(0f, 0f, 0f)
        let mutable midpoint = Vector3(0f, 0f, 0f)
        
        member this.ready thing =
            self <- thing
            originalPos <- self.Position
        
        member this.physicsProcess (delta : float32) =
            if held then
                t <- Mathf.Min(1f, t + delta * 3f)
                
                if t < 1f then 
                    let q1 = originalPos.Lerp(midpoint, t)
                    let q2 = midpoint.Lerp(endPos, t)
                    
                    self.Position <- q1.Lerp(q2, t)
                elif held then self.Position <- getRoot().GetNode<Node3D>("Player").Position + Vector3.Up * 2f
                else originalPos <- endPos
            
        member this.input (event : InputEvent) =
            match event with
            | :? InputEventKey ->
                let keyEvent = event :?> InputEventKey
                if keyEvent.Pressed then
                    match keyEvent.Keycode with
                    | Key.Space ->
                        held <- not held
                        t <- 0f
                        if held then
                            endPos <- getRoot().GetNode<Node3D>("Player").Position + Vector3.Up * 2f
                            midpoint <- Vector3(self.Position.X, self.Position.Y + 1.5f, self.Position.Z)
                        else
                            originalPos <- self.Position
                            endPos <- getRoot().GetNode<Node3D>("Player").Position + getRoot().GetNode<Node3D>("Player").Transform.Basis.Z
                            midpoint <- Vector3(self.Position.X, self.Position.Y + 0.3f, self.Position.Z)
                    | _ -> ()
            | _ -> ()