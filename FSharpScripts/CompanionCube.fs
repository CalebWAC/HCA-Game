namespace FSharpScripts

open FSharpScripts.WorldFS
open Godot
open GlobalFunctions
open System.Collections.Generic

module CompanionCubeFS =
    type CompanionCube() =
        let mutable self = Unchecked.defaultof<Node3D>
        
        let mutable t = 1f
        let mutable originalPos = Vector3(0f, 0f, 0f)
        let mutable endPos = Vector3(0f, 0f, 0f)
        let mutable midpoint = Vector3(0f, 0f, 0f)      
        
        member val held = false with get, set
        
        member this.fall () =
            this.held <- false
            originalPos <- self.Position
            endPos <- self.Position - Vector3(0f, 2f, 0f)
            midpoint <- (originalPos + endPos) / 2f
            t <- 0f
        
        member this.ready thing =
            self <- thing
            originalPos <- self.Position
        
        member this.physicsProcess (delta : float32) =
            t <- Mathf.Min(1f, t + delta * 3f)
            
            if t < 1f then 
                let q1 = originalPos.Lerp(midpoint, t)
                let q2 = midpoint.Lerp(endPos, t)
                
                self.Position <- q1.Lerp(q2, t)
            elif this.held then self.Position <- getRoot().GetNode<Node3D>("Player").Position + Vector3.Up * 2f
            else
                originalPos <- endPos; self.Position <- roundVec self.Position
                
                let block = worlds[level] |> Array.find (fun b -> b.position.X = round self.Position.X && b.position.Z = round self.Position.Z)
                if block.material = RushingWater then
                    let forward = match block.rotation.Y with
                                  | pi2 when pi2 = degToRad 90f -> Vector3(1f, 0f, 0f)
                                  | pi when pi = degToRad 180f -> Vector3(0f, 0f, -1f)
                                  | pi32 when pi32 = degToRad 270f -> Vector3(-1f, 0f, 0f)
                                  | _ -> Vector3(0f, 0f, 1f)
                    
                    endPos <- endPos + forward
                    midpoint <- (originalPos + endPos) / 2f - Vector3(0f, 0.5f, 0f)
                    t <- 0f
                    
            // For invisble blocks
            if Array.contains Glasses PlayerFS.powerUps then
                if self.Position.DistanceTo PlayerFS.self.Position <= 3f then
                    self.Visible <- true
                else self.Visible <- false
            
            // For lava plumes
            let exists predicate (array: Collections.Array<Area3D>) =
                let rec inner predicate (array: Collections.Array<Area3D>) i =
                    if i < array.Count then 
                        if predicate array[i] then true
                        else inner predicate array (i + 1)
                    else false
                    
                inner predicate array 0
                
            let find predicate (array: Collections.Array<Area3D>) =
                let rec inner predicate (array: Collections.Array<Area3D>) i =
                    if i < array.Count then
                        if predicate array[i] then array[i]
                        else inner predicate array (i + 1)
                    else System.Exception "Item not found" |> raise
                    
                inner predicate array 0
            
            let areas = self.GetNode<Area3D>("Area3D").GetOverlappingAreas()
            if not this.held && areas.Count > 0 && areas |> exists (fun a -> a.Name.ToString().Contains "TopArea3D") then
                t <- 0f
                originalPos <- (areas |> find (fun a -> a.Name.ToString().Contains "TopArea3D")).GlobalPosition
                midpoint <- (areas |> find (fun a -> a.Name.ToString().Contains "TopArea3D")).GlobalPosition
                endPos <- (areas |> find (fun a -> a.Name.ToString().Contains "TopArea3D")).GlobalPosition
            elif not this.held && areas.Count > 0 && areas |> exists (fun a -> a.Name.ToString().Contains "BodyArea3D") then
                t <- 0f
                originalPos <- self.Position
                endPos <- Vector3(areas[0].GlobalPosition.X, self.Position.Y + 0.5f, areas[0].GlobalPosition.Z)
                midpoint <- (originalPos + endPos) / 2f
            
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
                        
                        if not this.held && roundVec self.Position = roundVec next ||
                           this.held && nextBlock.Y <= playerPos.Y then
                            this.held <- not this.held
                            t <- 0f
                        
                            if this.held then
                                originalPos <- self.Position
                                endPos <- playerPos + Vector3.Up * 2f
                                midpoint <- Vector3(self.Position.X, self.Position.Y + 1.5f, self.Position.Z)
                            else
                                originalPos <- self.Position
                                endPos <- nextBlock + Vector3.Up
                                midpoint <- Vector3(self.Position.X, self.Position.Y + 0.3f, self.Position.Z)
                    | _ -> ()
            | _ -> ()
            
        
    let companionCubesCode = List<CompanionCube>()