namespace FSharpScripts

open System
open System.Threading.Tasks
open Godot
open WorldFS
open GlobalFunctions

module LavaWallFS =
    type LavaWall() =
        let mutable self = Unchecked.defaultof<Node3D>
        
        let onAreaEntered (other : Area3D) =
            if other.GetParent().Name.ToString() = "Player" then
                let shaky = getRoot().GetNode<Node3D>("Camera").GetNode("ShakyCamera3D") :?> Camera3D
                shaky.Current <- true
                CameraControlFS.camera.GetNode<Camera3D>("Camera3D").Current <- false
                
                PlayerControlFS.placeToBe <- Vector3(6f, 1f, -6f)
                (other.GetParent() :?> Node3D).Position <- Vector3(6f, 1f, -6f)
                
                let timer = new Timer()
                getRoot().AddChild timer
                timer.OneShot <- true
                timer.WaitTime <- 0.5
                timer.Start()
                timer.add_Timeout (fun _ -> shaky.Current <- false; CameraControlFS.camera.GetNode<Camera3D>("Camera3D").Current <- true)
                
        member this.ready thing =
            self <- thing // getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("LavaWall")
            self.GetNode<Area3D>("Area3D").add_AreaEntered (fun other -> onAreaEntered other)
        
        member this.process (delta : float32) =
           try if (worlds[level] |> Array.find (fun b -> b.position.X = round self.Position.X && b.position.Z = round self.Position.Z)).position.Y < self.Position.Y - 1f then
                self.RotateY(degToRad 180f)
           with | _ -> self.RotateY(degToRad 180f)
        
           self.Position <- self.Position + self.Transform.Basis.Z * delta