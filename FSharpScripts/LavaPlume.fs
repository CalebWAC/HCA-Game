namespace FSharpScripts

open Godot
open GlobalFunctions

module LavaPlumeFS =
    let lavaPlumes = System.Collections.Generic.List()
    
    type Direction =
        | Up
        | Down
        | Stagnant
    
    type LavaPlume() =
        let mutable tChange = 0f
        let mutable direction = Stagnant
        let mutable startingPoint = Unchecked.defaultof<Vector3>

        let onAreaEntered (other: Area3D) =
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
                
                
        member val self = Unchecked.defaultof<Node3D> with get, set
        member val active = 1f with get, set
                
        member this.ready (thing : Node3D) =
            this.self <- thing.GetNode<Node3D>("Model")
            this.self.GetNode<Area3D>("BodyArea3D").add_AreaEntered onAreaEntered
            startingPoint <- getRoot().GetNode<Node3D>("Player").Position
            lavaPlumes.Add this
            
        member this.process delta =
            tChange <- tChange + delta * this.active
            
            if tChange > 7f then
                tChange <- 0f
                if this.self.Scale.Y > 1f then direction <- Down else direction <- Up
           
            if direction = Up && this.self.Scale.Y < 20f then
                this.self.Scale <- Vector3(1f, this.self.Scale.Y + delta * 5f, 1f)
                this.self.GetParent().GetNode<Node3D>("Particles").Translate(Vector3(0f, delta, 0f))
            elif direction = Down && this.self.Scale.Y > 1f then
                this.self.Scale <- Vector3(1f, this.self.Scale.Y - delta * 5f, 1f)
                this.self.GetParent().GetNode<Node3D>("Particles").Translate(Vector3(0f, -delta, 0f))
                
            if this.self.Scale.Y <= 1f || this.self.Scale.Y >= 20f then direction <- Stagnant