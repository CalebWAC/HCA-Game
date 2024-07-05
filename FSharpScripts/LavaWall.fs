namespace FSharpScripts

open System
open System.Threading.Tasks
open Godot
open WorldFS
open GlobalFunctions

module LavaWallFS =
    let mutable self = Unchecked.defaultof<Node3D>
    
    let onAreaEntered (other : Area3D) =
        if other.GetParent().Name.ToString() = "Player" then
            GD.Print "Collided with player"
            
            for i in 0 .. 35 do
                GD.Print $"Rotation: {(other.GetParent() :?> Node3D).Rotation}"
                Task.Delay(TimeSpan.FromSeconds(2)).WaitAsync(TimeSpan.FromSeconds(10)) |> ignore
                (other.GetParent() :?> Node3D).RotateY(degToRad 10f)
            
            PlayerControlFS.placeToBe <- Vector3(4f, 1f, 4f)
            (other.GetParent() :?> Node3D).Position <- Vector3(4f, 1f, 4f)
    
    let ready () =
        self <- getRoot().GetNode<Node3D>("WorldGenerator").GetNode<Node3D>("LavaWall")
        self.GetNode<Area3D>("Area3D").add_AreaEntered (fun other -> onAreaEntered other)
    
    let process (delta : float32) =
       try if (worlds[level] |> Array.find (fun b -> b.position.X = round self.Position.X && b.position.Z = round self.Position.Z)).position.Y < self.Position.Y - 1f then
            self.RotateY(degToRad 180f)
       with | _ -> self.RotateY(degToRad 180f)
    
       self.Position <- self.Position + self.Transform.Basis.Z * delta