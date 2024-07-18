namespace FSharpScripts

open Godot
open WorldFS
open GlobalFunctions

module LavaWallFS =
    type LavaWall() =
        let mutable self = Unchecked.defaultof<Node3D>
        let mutable startingPoint = Unchecked.defaultof<Vector3>
        let mutable invisible = false
        
        let onAreaEntered (other : Area3D) =
            if other.GetParent().Name.ToString() = "Player" then
                let shaky = getRoot().GetNode<Node3D>("Camera").GetNode("ShakyCamera3D") :?> Camera3D
                shaky.Current <- true
                CameraControlFS.camera.GetNode<Camera3D>("Camera3D").Current <- false
                
                for cube in CompanionCubeFS.companionCubesCode do 
                    if cube.held then cube.fall()
                
                PlayerControlFS.placeToBe <- startingPoint
                (other.GetParent() :?> Node3D).Position <- startingPoint
                
                let timer = new Timer()
                getRoot().AddChild timer
                timer.OneShot <- true
                timer.WaitTime <- 0.5
                timer.Start()
                timer.add_Timeout (fun _ -> shaky.Current <- false; CameraControlFS.camera.GetNode<Camera3D>("Camera3D").Current <- true)
                
        member this.ready thing =
            self <- thing
            self.GetNode<Area3D>("Area3D").add_AreaEntered (fun other -> onAreaEntered other)
            startingPoint <- getRoot().GetNode<Node3D>("Player").Position
            if self.Visible = false then invisible <- true
        
        member this.process (delta : float32) =
           try
                let blockPos = (worlds[level] |> Array.find (fun b -> b.position.X = round self.Position.X && b.position.Z = round self.Position.Z)).position.Y 
                if blockPos < self.Position.Y - 1f || blockPos >= self.Position.Y + 0.5f then
                    self.RotateY(degToRad 180f)
           with | _ -> self.RotateY(degToRad 180f)
        
           self.Position <- self.Position + self.Transform.Basis.Z * delta
           
           if invisible then
               if Array.contains Glasses PlayerFS.powerUps && getRoot().GetNode<Node3D>("Player").Position.DistanceTo self.Position <= 3f then
                    self.Visible <- true
               else self.Visible <- false