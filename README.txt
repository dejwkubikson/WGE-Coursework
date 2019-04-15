12/03/2019
- Created main menu containing two buttons and one input field. Buttons take the player to different scenes and the input field allows the player to input and load his own chunk of blocks from an XML file.
- Added loading voxel map from XML files.
- Added sound effects that are different for each block.
- Added a block that appears when one is destroyed. Still needs physics.

13/03/2019
- Created a 'trigger' that starts when the player is near the NPC.
- Disabled player movement when dialogue starts.
- Loading dialogue from XML file.

19/03/2019
- Instantiating a cube as a collectable.
- Added proper texture to the cube depending on the destroyed block.

20/03/2019
- Added physics to the dropped block.
- Added inventory 'prototype'.
- Picked up blocks are added to inventory.
- Droped block can be picked up now.

21/03/2019
- Inventory is properly displaying blocks.
- Placing a block removes one from inventory.

25/03/2019
- Player can draw blocks towards him when 'F' is pressed.
- When the player runs out of certain block it's removed from the inventory (the place for the block will be empty now).
- Blocks are now selected through the inventory only.
- Selected inventory item is highlighted.

28/03/2019
- Added search by name function
- Added controllers at the top of the screen that are shown for the first 10 seconds of the game

29/03/2019
- Added merge sort fully working for future use

1/04/2019
- Added sorting by number from low to high
- Added sorting by number from high to low
- Added sorting by name from low to high
- Added sorting by name from high to low
- Fixed some issues with function UpdateInventory() that didn't display the inventory properly
- When the user is searching for a block by name the inventory layer will not disapear when he types in 'E'

2/04/2019
- Added loading from file by user input in the main menu
- Scene 1 is finished
- Started working on loading the dialogue - it can properly go through one whole conversation
- Added GUI to scene 2 for dialogue display

3/04/2019
- The first dialogue conversation displays properly in the GUI
- Dialogue fully works. The player can choose different options which will result in differnt NPC reactions.
- Further improvements to the camera movement. 
- Started working on the inspector that will allow the developer to create / load dialogues.

04/04/2019
- Added text fields, buttons and popups to the new window dialogue inspector

05/04/2019
- Dialogue editor in inspector properly shows the amount of conversations, options and stores them up to second 'branch'

09/04/2019
- Added camera shake when the player lands on ground

12/04/2019
- Started new script for dialogue editor that will be class not dictionary based

15/04/2019
- Dialogue editor fully works. Loads the dialogue to the inspector and scene. Creates dialogues. Added more human error prevention 'ifs'.