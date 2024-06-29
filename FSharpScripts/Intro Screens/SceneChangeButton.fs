namespace FSharpScripts

open Godot
open GlobalFunctions

module SceneChangeButtonFS =
    type SceneChanger(name: string, scene: string) = 
        let mutable button = Unchecked.defaultof<Button>
       
        let startScene () = button.GetTree().ChangeSceneToFile(scene) |> ignore
        
        member this.Ready () =
            button <- getScreenRoot().GetNode<Button>(name)
            button.add_Pressed startScene