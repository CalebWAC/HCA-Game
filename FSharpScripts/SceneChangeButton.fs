#nowarn "3391"

namespace FSharpScripts

open Godot
open GlobalFunctions

module SceneChangeButtonFS =
    type SceneChanger(name: string, scene: string) = 
        let mutable button = Unchecked.defaultof<Button>
       
        let startScene () =
            if name = "World1" || name = "BackButton" || name = "BackButtonSelect" then
                WorldFS.currentWorld <- 1
                RenderingServer.SetDefaultClearColor(Color(0f, 1f, 1f))
            elif name = "World2" then
                WorldFS.currentWorld <- 2
                RenderingServer.SetDefaultClearColor(Color(1f, 0.25f, 0f))
            
            button.GetTree().ChangeSceneToFile(scene) |> ignore
        
        member this.Ready () =
            button <- if name <> "BackButtonLevel" then getScreenRoot().GetNode<Button>(name) else getRoot().GetNode<Control>("Control").GetNode<Button>(name)
            button.add_Pressed startScene
                
            let completedLevels = if WorldFS.currentWorld = 1 then WorldFS.completedLevelsW1 else WorldFS.completedLevelsW2
            try                                                  
                if name[..4] = "Level" && completedLevels[(name[5..6].ToString() |> int) - 1] = true then
                    button.Icon <- ResourceLoader.Load($"res://Assets/Level Images/{name}Filled.png") :?> Texture2D
            with | _ -> 
                if name[..4] = "Level" && completedLevels[(name[5].ToString() |> int) - 1] = true then
                    button.Icon <- ResourceLoader.Load($"res://Assets/Level Images/{name}Filled.png") :?> Texture2D
            
            // Reset mechanics
            PlayerFS.powerUps <- [||]
            if name <> "BackButtonLevel" then
                WorldGeneratorFS.movingBlocks.Clear()
                WorldGeneratorFS.companionCubes.Clear()
                WorldGeneratorFS.cubeTriggers.Clear()
                WorldGeneratorFS.bridges.Clear()
                WorldGeneratorFS.goalFragments.Clear()
                TerrainManipulatorFS.destructibleBlocks.Clear()