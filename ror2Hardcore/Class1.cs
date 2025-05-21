using BepInEx; //mod loader library
using UnityEngine; //engine library that ror2 uses
using RoR2;//use game's code
using System.Linq;
using System;
using System.Collections.Generic;

//Mod: No healing from items,drones or shrines, only from kills

[BepInPlugin("com.meepmorp.ror2hardcore", "Hardcore mode", "1.0.0")]//mod unique id, mod name, version number

public class ror2Hardcore : BaseUnityPlugin {

    private static readonly HashSet<String> excludedItems = new HashSet<String>(StringComparer.OrdinalIgnoreCase) {
            //TIER 1
            "Mushroom", //Bustling Fungus
            "Medkit", //Medkit
            "FlatHealth", //Cautious Slug
            "HealWhileSafe", //Monster tooth (necklace)
            
            //shield items: BarrierOnKill, Bear, PersonalShield

            //TIER 2
            "HealOnCrit", //Harvester's scythe
            "infusion", //infusion
            "Seed", //leeching seed
            "TPHealingNova" //lepton daisy
        };

    public void Awake() {//runs when mod starts (when game loads)
        On.RoR2.PickupPickerController.SetOptionsFromPickupForCommandArtifact += FilterCommandOptions; //Hook into Command artifact item selection to filter items

        SceneDirector.onGenerateInteractableCardSelection += RemoveHealingShrine;
        SceneDirector.onGenerateInteractableCardSelection += RemoveHealingDrone;
        SceneDirector.onGenerateInteractableCardSelection += RemoveHealingChest;
        SceneDirector.onGenerateInteractableCardSelection += LogAllInteractables;

        Run.onRunStartGlobal += (run) => StartCoroutine(DelayedRemove());



    }

    private System.Collections.IEnumerator DelayedRemove()
    {
        yield return new WaitForSeconds(0.5f); // Wait to ensure drop lists are populated

        RemoveAllExcludedItemsFromDropLists();
        LogAllDroppableItems(Run.instance);     // Debug log
    }

    private void RemoveAllExcludedItemsFromDropLists()
    {
        void Filter(List<PickupIndex> list)
        {
            foreach (var pickup in list.ToList())
            {
                var def = PickupCatalog.GetPickupDef(pickup);
                if (def != null && def.itemIndex != ItemIndex.None)
                {
                    var itemDef = ItemCatalog.GetItemDef(def.itemIndex);
                    if (itemDef != null && excludedItems.Contains(itemDef.name))
                    {
                        list.Remove(pickup);
                        Logger.LogInfo($"Removed {itemDef.name} from drop list (Command filtered).");
                    }
                }
            }
        }

        Filter(Run.instance.availableTier1DropList);
        Filter(Run.instance.availableTier2DropList);
        Filter(Run.instance.availableTier3DropList);
       // Filter(Run.instance.availableLunarDropList);
        Filter(Run.instance.availableVoidTier1DropList);
        Filter(Run.instance.availableVoidTier2DropList);
        Filter(Run.instance.availableVoidTier3DropList);
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

            category.cards = category.cards.Where(card => !card.spawnCard.name.Equals("iscBrokenDrone2", StringComparison.OrdinalIgnoreCase)).ToArray();
            dccs.categories[i] = category;
       
        }
        
        Logger.LogInfo("Healing drones removed from spawn pool."); //Debugger
    }

    private void RemoveHealingChest(SceneDirector director, DirectorCardCategorySelection dccs) {
        if (dccs == null) {
            Logger.LogInfo("Unable to remove healing chests from spawn pool.");
            return;
        }
        for (int i = 0; i < dccs.categories.Length; i++) {
            var category = dccs.categories[i];

            category.cards = category.cards.Where(card => !card.spawnCard.name.Equals("iscCategoryChestHealing",StringComparison.OrdinalIgnoreCase)).ToArray();
            dccs.categories[i] = category; 
        }

        Logger.LogInfo("Healing chests removed from spawn pool.");//debugger
    }

    private void LogAllDroppableItems(Run gamestart) {
        foreach (var pickup in Run.instance.availableTier1DropList) {//tier 1 droplist
            var def = PickupCatalog.GetPickupDef(pickup);
            if (def != null && def.itemIndex != ItemIndex.None) { 
                var itemDef=ItemCatalog.GetItemDef(def.itemIndex); 
                Logger.LogInfo("Tier1 item: " + itemDef.name);
            }
        }
        foreach (var pickup in Run.instance.availableTier2DropList) {
            var def = PickupCatalog.GetPickupDef(pickup);
            if (def != null && def.itemIndex != ItemIndex.None) {
                var itemDef = ItemCatalog.GetItemDef(def.itemIndex);
                Logger.LogInfo("Tier2 item: " +itemDef.name);
            }
        }
        foreach (var pickup in Run.instance.availableTier3DropList)
        {
            var def = PickupCatalog.GetPickupDef(pickup);
            if (def != null && def.itemIndex != ItemIndex.None)
            {
                var itemDef = ItemCatalog.GetItemDef(def.itemIndex);
                Logger.LogInfo("Tier3 item: " + itemDef.name);
            }
        }



    }
    private void LogAllInteractables(SceneDirector director, DirectorCardCategorySelection dccs){//DEBUGGING METHOD FOR INTERACTABLES
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

    //Hook method to filter out specific healing items from the Artifact of Command item selection UI
    private void FilterCommandOptions(On.RoR2.PickupPickerController.orig_SetOptionsFromPickupForCommandArtifact orig, PickupPickerController self, PickupIndex pickupIndex) { 
        
        //Execute the original method first to populate the 'options' array
        orig(self, pickupIndex);

        //use reflection to access the private 'options' field from the PickupPickerController instance
        var optionsField = typeof(PickupPickerController).GetField("options", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        //Retrieve the current array of selectable item options
        var options = (PickupPickerController.Option[])optionsField.GetValue(self);

        //filter out Bustling fungus (mushroom)
        var filtered = options.Where(opt =>{ 

            var def = PickupCatalog.GetPickupDef(opt.pickupIndex); // Get pickup definition (metadata)
            if (def != null && def.itemIndex != ItemIndex.None) {
                var itemDef = ItemCatalog.GetItemDef(def.itemIndex); // Get actual item definition
                if (itemDef != null && excludedItems.Contains(itemDef.name)) {// Check if it's the Bustling Fungus
                    Logger.LogInfo($"[Command] Removed {itemDef.name} from Command options."); //debug log
                    return false;// Exclude this option
                }
            }
            return true; // Keep this option
        }).ToArray();

        // Apply the filtered options back into the private 'options' field
        optionsField.SetValue(self, filtered);

    }
}

