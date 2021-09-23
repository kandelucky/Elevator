Elevator

Features:

Create floors:
Assign first, standard and last floors prefabs in FloorCreator script and set how many floors you need (min 3 - max 100)
Set floors height.
Floors will by created automatically.
Numbers at floors will be written automatically.
(ToDo: Assign not automatically created (static) floors)

Buttons:

Hall buttons:
There are two buttons (if You are at standard floors) for upstair and for downstair directions.
You can call elevator, or if door is opened and started closing - opening again.
If You going upstair and was clicked up destination button - the elevator will not stop if it goes down (it will come only when it reaches its final destination down). And vice versa.
Also, there are button lights controller. bool if doors are closed and timer if doors are opened.

Elevator buttons:
"Open doors" (if door is opened and started closing - opening again.), "close doors" (doors will by closing immediately).
"DoubleNumers" - if You need 10 or more floors, just click first of all in button ">9". for ex. 1 and 2 will by 12.
                 (You can see this button if there are more than 10 floors)
"Numbers" - if we have as many floors as the number clicked on,
            or if don't already have registered this number,
            or the elevator don't is at the current hall - the number will by set in destinatins lists.

Doors: 
Timer: set how long will by stay door opened

Elevator speed control:
There are two speed variables: normal speed and max speed.
If next floor is upstair than 2 floors, elevator will by accelerated (also moving sound will by speed up).
If next floor is close than 2 floors elevator speed will by returned at normal.
Elevator stoped slowly (with endMoving sound).

Texts and arrows:

Elevator display:
Will showed current floor number.
Will showed all destination numbers.
Will showed direction arrows.

Floors displays:
Will showed elevator current floor number.
Will showed direction arrows.

Photocell:
If You are in photocell box collider - door is not going to closed / or if doors started closing - its will by opened again.
You have display info at floors and in elevator. You can see elevator going up or down and which floor is it on.

ChildDetector:
For correct moving people in elevator, when They are in the elevator, Their gameObjects temporarily will moved in elevator parent object.


used free assets:

First Person All-in-One
https://assetstore.unity.com/packages/tools/input-management/first-person-all-in-one-135316

Sci-Fi Construction Kit
https://assetstore.unity.com/packages/3d/environments/sci-fi/sci-fi-construction-kit-modular-159280
