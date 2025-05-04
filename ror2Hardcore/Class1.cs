using BepInEx; //mod loader library
using UnityEngine; //engine library that ror2 uses
using RoR2;//use game's code
using RoR2.Navigation;
using System.Linq;
using System;

//Mod: No healing from items,drones or shrines, only from kills

[BepInPlugin("com.yourname.hardcore", "Hardcore mode", "1.0.0")]//mod unique id, mod name, version number

public class ror2Hardcore : BaseUnityPlugin {

    public void Awake() {//runs when mod starts (when game loads)
        SceneDirector.onGenerateInteractableCardSelection += RemoveHealingShrine;
        SceneDirector.onGenerateInteractableCardSelection += RemoveHealingDrone;
        SceneDirector.onGenerateInteractableCardSelection += LogAllInteractables;
}

   
    //SceneDirector -> thing in charge of setting up level, DirectorCardCategorySelection dccs -> a list of spawnable interactables
    private void RemoveHealingShrine(SceneDirector director, DirectorCardCategorySelection dccs) {
        if (dccs == null) {//if the list is empty, stop the function (error catching)
            Logger.LogInfo("Unable to remove Shrine of the Woods from spawn pool.");
            return;
        }
        for (int i = 0; i < dccs.categories.Length; i++){//loops through each category 
            var category = dccs.categories[i]; //saves current category to a variable so it can be utilised
           
            category.cards = category.cards. //replace the cards in this category and keep only cards that doesnt have "iscShrineHealing", converts updated list into array
                Where(card => !card.spawnCard.name.Equals("iscShrineHealing", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            dccs.categories[i] = category; //Apply the updated category back
        }

        Logger.LogInfo("Shrine of the Woods removed from spawn pool.");//Debugger
    }

    private void RemoveHealingDrone(SceneDirector director, DirectorCardCategorySelection dccs) {
        if (dccs == null) {
            Logger.LogInfo("Unable to remove healing drones from spawn pool.");
            return;
        }
        for (int i = 0; i < dccs.categories.Length; i++) { 
            var category = dccs.categories[i];

            category.cards = category.cards.Where(card => !card.spawnCard.name.Equals("iscDrone2", StringComparison.OrdinalIgnoreCase)).ToArray();
            dccs.categories[i] = category;
       
        }
        
        Logger.LogInfo("Healing drones removed from spawn pool."); //Debugger
    }

    private void LogAllInteractables(SceneDirector director, DirectorCardCategorySelection dccs){//DEBUGGING METHOD
        if (dccs == null)
        {
            Logger.LogInfo("No interactables to log.");
            return;
        }

        Logger.LogInfo("----Interactables in Current Stage----");
        foreach (var category in dccs.categories) {
            foreach (var card in category.cards) {
                Logger.LogInfo("Interactables: " + card.spawnCard.name);
            }
        }

        Logger.LogInfo("--Chests in Current stage--");
        foreach (var category in dccs.categories) {
            foreach (var card in category.cards) {
                if (card.spawnCard != null && card.spawnCard.name.ToLower().Contains("chest")) {
                    Logger.LogInfo("Chest Types: "+card.spawnCard.name);
                }
            }
        }

    }
}

