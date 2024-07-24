## What is this?

This editor tool adds a graph editor to Unity to create procedural noise using the [FastNoise2](https://github.com/Auburn/FastNoise2) library. This is intended to work like the built-in graph editor of FastNoise2 but inside Unity.

## Compatibility

This editor tool has been tested only with [FastNoise2 v0.10.0-alpha](https://github.com/Auburn/FastNoise2/releases/tag/v0.10.0-alpha) and Unity 2022 LTS.

## Features

- Noise tree assets.
- Nodes are created using the metadata from FastNoise2.
- One editor window per graph for easy editing.
- Copy and paste support.

# How to install

- First you have to import the FastNoise2 library into your Unity project. To do that go to the [tags page in the FastNoise2 repository](https://github.com/Auburn/FastNoise2/tags), select a version (I recommend v0.10.0-alpha), and download the zip file according to your platoform. Now:
  - If you are on windows, copy the FastNoise.dll and paste it in your Unity Assets folder.
- You don't need to install the C# bindings of FastNoise2 because they are included in this package.
- Now you need to install this package. In Unity, go to Window -> Package Manager
- Click the "+" icon in the top left
- Choose  "Add package from git URL"
- Paste https://github.com/carlo697/FastNoise2GraphEditor.git

# How to use

- Create a **Fast Noise tree** asset by right clicking on the Project window -> Create -> Fast Noise Tree.
- Double click the asset to open the editor window.
- Add new nodes and connect them to the output node.
- Now in a MonoBehaviour:
  - Add: ```using FastNoise2Graph;```
  - Add a public field: ```public NoiseTree noiseTree;```
  - Save your script and now you can drag and drop the previously created noise tree into the field.
- Once you have a reference to the noise tree in a script, now you can call: ```noiseTree.GetFastNoise()``` to get a **FastNoise** instance.
- With a **FastNoise** instance now you can generate procedural noise by using its methods:
  - GenSingle2D
  - GenSingle3D
  - GenSingle4D
  - GenUniformGrid2D
  - GenUniformGrid3D
  - GenUniformGrid4D
  - GenPositionArray2D
  - GenPositionArray3D
  - GenPositionArray4D

## Images
![image](https://github.com/user-attachments/assets/fa2905c1-86e0-45cc-8fb1-995bbc0b50d6)
![image](https://github.com/user-attachments/assets/f05717c9-5e7a-472d-b593-9e2762671aba)
![image](https://github.com/user-attachments/assets/20a6eb5f-d0bc-4d46-8d74-3a0409dc9d97)
