# Player Stat Display

Controls the UI display of player stats such as Health and Mana.

Setup:

For this script to work accordingly, you need to create a prefab with the following:

    • Parent Container (PlayerHealthUI) - attach the script to this object
        
        • Outer Image (HeartOuter) - Background image / Empty heart
            
            • Rect Mask - Attach a Rect Mask 2D Component
                
                • Inner Image (Heart Inner) - Full Heart image
        
        • Stat Text (Health Text) - This will display the number of the stat value

![image](https://github.com/alyoctavian/PlayerStatDisplay/assets/33526573/50fdc42b-3cb4-42a5-9273-9ebfb3bb990d)

The RectMask and InnerImage UI objects should stretch to cover their parents fully:

![image](https://github.com/alyoctavian/PlayerStatDisplay/assets/33526573/d8ab4985-0dc4-4d14-81f0-b2bfdaa78524)

These will be the values assigned in the inspector:

![image](https://github.com/alyoctavian/PlayerStatDisplay/assets/33526573/ddcc9f7d-44a9-4ad7-b32d-41c03b48d537)
