using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Player : LivingCreature
    {

        public int Gold { get; set; }
        public int ExperiencePoints { get; set; }
        public int Level { get; set; }
        public List<InventoryItem> Inventory { get; set; }
        public List<PlayerQuest> Quests { get; set; }
        public Location CurrentLocation { get; set; }



        public Player(int gold, int experiencePoints, int level,
            int currentHitPoints, int maximumHitPoints) :
            base(currentHitPoints, maximumHitPoints)
        {

            Gold = gold;
            ExperiencePoints = experiencePoints;
            Level = level;
            Inventory = new List<InventoryItem>();
            Quests = new List<PlayerQuest>();
        }

        public bool HasRequiredItemToEnterLocation(Location location)
        {
            if (location.ItemRequiredToEnter == null)
            {
                //there is no item needed for this location, so return "true"
                return true;
            }

            //See if player has item in inventory
            foreach (InventoryItem ii in Inventory)
            {
                if (ii.Details.ID == location.ItemRequiredToEnter.ID)
                {
                    //we found the required item 
                    return true;
                }
            }

            //we didn't find the item, so return false
            return false;
        }

        public bool HasThisQuest(Quest quest)
        {
            foreach (PlayerQuest pq in Quests)
            {
                if (pq.Details.ID == quest.ID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CompletedThisQuest(Quest quest)
        {
            foreach (PlayerQuest pq in Quests)
            {
                if (pq.Details.ID == quest.ID)
                {
                    return pq.IsCompleted;
                }
            }

            return false;
        }

        public bool HasAllQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                bool foundItemInPlayersInventory = false;

                //Check each item to see if they have it, and have enough of it
                foreach (InventoryItem ii in Inventory)
                {
                    //The player has this item in Inventory 
                    if (ii.Details.ID == qci.Details.ID)
                    {
                        foundItemInPlayersInventory = true;

                        if (ii.Quantity < qci.Quantity)
                        {
                            //Player does not have enough of this item 
                            return false;
                        }
                    }

                }

                //if we didn't find the required item, set our variable and stop looking for other items
                if (!foundItemInPlayersInventory)
                {
                    //player does not have this item in inventory
                    return false;
                }
            }

            //if we got here, then the player must have all the items needed, and the right quantity, to complete the quest
            return true;
        }

        public void RemoveQuestCompletionItems(Quest quest)
        {
            //Remove quest items from player inventory 
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                foreach (InventoryItem ii in Inventory)
                {
                    if (ii.Details.ID == qci.Details.ID)
                    {
                        //subtract quantity from player's inventory that was needed to complete the quest

                        ii.Quantity -= qci.Quantity; ;
                        break;
                    }
                }
            }
        }

        public void AddItemToInventory (Item itemToAdd)
        {
            foreach (InventoryItem ii in Inventory)
            {
                if (ii.Details.ID == itemToAdd.ID)
                {
                    //they already have the item, so increase the quantity
                    ii.Quantity++;

                    return;

                }
            }

            //They didn't have the item, so we need to add it
            Inventory.Add(new InventoryItem(itemToAdd, 1));
           
        }

        public void MarkQuestCompleted (Quest quest)
        {
            foreach (PlayerQuest pq in Quests)
            {
                if (pq.Details.ID == quest.ID)
                {
                    //mark completed
                    pq.IsCompleted = true;

                    return;
                }
            }
        }


    }
}

