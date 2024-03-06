
# Projector Unity Integration

This library can be used to develop games and experiences with a simple webcam and a projector inside Unity.

This is part of my Bachelor's Thesis, which will be used to develop experiences in classrooms and museums.


## Documentation

First of all, u need to have a Unity installed with the library imported, with that done, you can now start trying thing.

You will find a folder with some prefabs, this is what is used to make the interactions between the real world and the virtual projection. 

This library has 3 great parts, here is the abstraction:

### Camera detection

In the ferst part of the execution you will be directed to a calibration scene, with green squares in the corners. 
This is used to calibrate the limits of the projection, and after that will be used to lerp the position of your interactor.

### Contour Finder

This uses OpenCV library to get the image of the webcam and process the image. This are the steps:

- The script gets the image from the camera, without any change
- After that, it gets modified with a threshold and turns out as a B/W image that is more useful and easier for the program to understand.
- With that information, the script ContourFinder analyzes the black parts and selects the one with a certain area or higher and is inside the limits of the projection calculated earlier, that will be the interactor.

### Interactables
This library has three main classes used for the interaction, this will be the base to any of the interactable objects inside the game.

It uses the Unity Collision and Trigger events to develop its functionality.

- **Interactor:** this class gets the information from *ContourFinder* and the center of the biggest black spot in the image as the position of this collider. This object is an empty GameObject with a Collider2D and its Rigidbody2D that moves around the scene.

- **InteractableObject:** this abstract class consist of an abstract method called InteractionEvent that will be called inside OnTriggerEnter2D. **Every object that wants to have some interaction should inherit from this class and be part of a GameObject with a Collider2D attached.**

- *InteractableButton:* this class inherits from InteractableObject and develops a behavior of a classic Button. Uses InteractionEvent to invoke an OnClick() Event.

## Authors

- [@david-3lm](https://david-3lm.github.io/)

