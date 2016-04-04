using Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private Player _player;
        private Monster _currentMonster;

        public SuperAdventure()
        {
            InitializeComponent();

            Location location = new Engine.Location(1, "Home", "This is your house");

            //Using named parameters in this constructor since all params of integer value
            _player = new Player(currentHitPoints: 10, maximumHitPoints: 10, gold: 20, experiencePoints: 0, level: 1);

            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));

            _player.Inventory.Add(new InventoryItem(
                World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));

            lblHitPoints.Text = _player.CurrentHitPoints.ToString();
            lblGold.Text = _player.Gold.ToString();
            lblExperience.Text = _player.ExperiencePoints.ToString();
            lblLevel.Text = _player.Level.ToString();
            
        }

        private void SuperAdventure_Load(object sender, EventArgs e)
        {
           
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToNorth);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToSouth);
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {

        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {

        }

        private void MoveTo(Location newLocation)
        {
            //Does the location have any required items?
            if(newLocation.ItemRequiredToEnter != null)
                {
                    //see if the player has the required item in inventory
                    bool playerHasRequiredItem = false; 

                    foreach(InventoryItem ii in _player.Inventory)
                    {
                        if(ii.Details.ID == newLocation.ItemRequiredToEnter.ID)
                        {
                            //player has required item 
                            playerHasRequiredItem = true;
                            break; // Exit out of loop
                        }
                    }

                    if(!playerHasRequiredItem)
                    {
                        //player does not have item in inventory, display message to player and stop moving
                        rtbMessages.Text += "You must have a " + 
                        newLocation.ItemRequiredToEnter.Name + 
                        " to enter this location." + Environment.NewLine;
                        return;
                    }
                }

            //update player's current location
            _player.CurrentLocation = newLocation;

            //Show/hide available movement buttons
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);

            //Display location info 
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;

            //Completely heal the player 
            _player.CurrentHitPoints = _player.MaximumHitPoints;

            //Update HP in UI 
            lblHitPoints.Text = _player.CurrentHitPoints.ToString();

            //Does the location have a quest?
            if(newLocation.QuestAvailableHere != null)
            {
                //does player have quest, and is it completed?
                bool playerAlreadyHasQuest = false;
                bool playerAlreadyCompletedQuest = false;

                foreach(PlayerQuest pq in _player.Quests)
                {
                    if(pq.Details.ID == newLocation.QuestAvailableHere.ID)
                    {
                        playerAlreadyHasQuest = true;

                        if(pq.IsCompleted)
                        {
                            playerAlreadyCompletedQuest = true;

                        }
                    }
                }

                //check if player already has quest 
                if(playerAlreadyHasQuest)
                {
                    //if player has not completed quest yet
                    if(!playerAlreadyCompletedQuest)
                    {
                        //See if the player has all items needed to complete the quest
                        bool playerHasAllItemsForQuest = true;

                        foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                        {
                            bool foundItemInPlayersInventory = false;

                            //Check each item to see if they have it, and have enough of it
                            foreach(InventoryItem ii in _player.Inventory)
                            {
                                //The player has this item in Inventory 
                                if (ii.Details.ID == qci.Details.ID)
                                {
                                    foundItemInPlayersInventory = true;

                                    if(ii.Quantity < qci.Quantity)
                                    {
                                        //Player does not have enough of this item 
                                        playerHasAllItemsForQuest = false;

                                        //No need to keep checking other items
                                        break;
                                    }

                                    //we found the item, so don't check the remaining items
                                    break;
                                }
                               
                            }

                            //if we didn't find the required item, set our variable and stop looking for other items
                            if(!foundItemInPlayersInventory)
                            {
                                //player does not have this item in inventory
                                playerHasAllItemsForQuest = false;

                                //There is no need to keep checking other items
                                break;
                            }
                        }

                        if(playerHasAllItemsForQuest)
                        {
                            //Display Message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "You complete the " +
                                newLocation.QuestAvailableHere.Name +
                                " quest." + Environment.NewLine;

                            //Remove quest items from player inventory 
                            foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                            {
                                foreach(InventoryItem ii in _player.Inventory)
                                {
                                    if(ii.Details.ID == qci.Details.ID)
                                    {
                                        //subtract quantity from player's inventory that was needed to complete the quest

                                        ii.Quantity -= qci.Quantity; ;
                                        break;
                                    }
                                }
                            }

                            //give quest rewards
                            rtbMessages.Text += "You receive: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() +
                                " experience points" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() +
                                " gold" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name.ToString() +
                                " item" + Environment.NewLine;
                            rtbMessages.Text += Environment.NewLine;

                            _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            //add reward item to player inventory
                            bool addedItemToPlayerInventory = false;

                            foreach(InventoryItem ii in _player.Inventory)
                            {
                                if(ii.Details.ID == newLocation.QuestAvailableHere.RewardItem.ID)
                                {
                                    //they already have the item, so increase the quantity
                                    ii.Quantity++;

                                    addedItemToPlayerInventory = true;

                                    break;

                                }
                            }

                            //they don't have the item at all, so we need to add it
                            if(!addedItemToPlayerInventory)
                            {
                                _player.Inventory.Add(new InventoryItem(newLocation.QuestAvailableHere.RewardItem, 1));
                            }

                            //Mark the quest as completed 
                            //Find the quest in player's quest list

                            foreach (PlayerQuest pq in _player.Quests)
                            {
                                if (pq.Details.ID == newLocation.QuestAvailableHere.ID)
                                {
                                    //mark completed
                                    pq.IsCompleted = true;

                                    break;
                                }
                            }
                        }
                    }
                }

                else
                {
                    //the player does not yet have the quest

                    //display messages
                    rtbMessages.Text += "You receive the " +
                            newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "To complete it, return with: " + Environment.NewLine;
                    foreach(QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if(qci.Quantity == 1)
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " +
                                qci.Details.Name + Environment.NewLine;
                        }
                        else
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " +
                                qci.Details.NamePlural + Environment.NewLine;
                        }
                    }
                    rtbMessages.Text += Environment.NewLine;

                    //Add the quest to the player quest list
                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));

                }
            }

            //Does the location have a monster?
            if(newLocation.MonsterLivingHere != null)
            {
                //display message
                rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;

                //Make a new monster, using values from standard monster in World.Monster List
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage,
                    standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints,
                    standardMonster.MaximumHitPoints);
                // ****
                //Why are we having to assign each property like this above, can we not just say _currentMonster = standardMonster??
                //****

                foreach(LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);

                }

                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
 
            }

            else
            {
                _currentMonster = null;

                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;

            }

            //refresh player inventory list
            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Name";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Quantity";

            dgvInventory.Rows.Clear();

            foreach (InventoryItem ii in _player.Inventory)
            {
                if(ii.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { ii.Details.Name, ii.Quantity.ToString() });
                }
            }

            //Refresh player quest list
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Name";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Done?";

            dgvQuests.Rows.Clear();

            foreach(PlayerQuest pq in _player.Quests)
            {
                dgvQuests.Rows.Add(new[] { pq.Details.Name, pq.IsCompleted.ToString() });
            }

            //Refresh player weapon combobox
            List<Weapon> weapons = new List<Weapon>();

            foreach(InventoryItem ii in _player.Inventory)
            {
                if(ii.Details is Weapon)
                {
                    if(ii.Quantity > 0)
                    {
                        weapons.Add((Weapon)ii.Details);
                    }
                }
            }

            if(weapons.Count == 0)
            {
                //the player has no weapons, so hide the weapon combobox and "Use" button
                cboWeapons.Visible = false;
                btnUseWeapon.Visible = false;

            }

            else
            {
                cboWeapons.DataSource = weapons;
                cboWeapons.DisplayMember = "Name";
                cboWeapons.ValueMember = "ID";

                cboWeapons.SelectedIndex = 0;
            }

            //Refresh player healing potions combobox 

            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach(InventoryItem ii in _player.Inventory)
            {
                if(ii.Details is HealingPotion)
                {
                    if(ii.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)ii.Details);
                    }
                }
            }

            if(healingPotions.Count == 0)
            {
                //the player has no healing potions so hide the UI elements
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";

                cboPotions.SelectedIndex = 0;
            }
            
        }

    }
}
