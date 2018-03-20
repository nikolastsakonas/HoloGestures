# HoloGestures: An Evaluation of Bimanual Gestures on the Microsoft HoloLens

## Abstract
We developed and evaluated two-handed gestures on the Microsoft HoloLens to manipulate augmented reality annotations through rotation and scale operations. We explore the design space of bimanual interactions on head-worn AR platforms, with the intention of dedicating two-handed gestures to rotation and scaling manipulations while reserving one-handed interactions to drawing annotations.

In total, we implemented five techniques for rotation and scale manipulation gestures on the Microsoft HoloLens: three two-handed techniques, one technique for one-handed rotation and two-handed scale, and one baseline one-handed technique that represents standard HoloLens UI recommendations. Two of the bimanual interaction techniques involve axis separation for rotation whereas the third technique is fully 6DOF and modeled after the successful "spindle" approach from 3DUI literature. To evaluate our techniques, we conducted a study with 48 users. We recorded multiple performance metrics for each user on each technique, as well as user preferences. Results indicate that in spite of problems due to field-of-view limitations, certain two-handed techniques perform comparatively to the one-handed baseline technique in terms of accuracy and time. Furthermore, the best-performing two-handed technique outdid all other techniques in terms of overall user preference, demonstrating that bimanual gesture interactions can serve a valuable role in the UI toolbox on head-worn AR devices such as the HoloLens.

## Introduction

Augmented Reality is a convenient UI paradigm for creating annotations of real word objects. The Microsoft HoloLens is a device well suited to this task as it offers head and hand tracking, as well as spatial mapping of physical operation environments. [Previous work exists](http://ieeexplore.ieee.org/document/7893337/) for assessing the most accurate and preferred methods for creating one-handed annotations on the HoloLens. However, users commonly lack the ability to change the orientation or size of an annotation without re-drawing the annotation. Furthermore, in an environment with only one-handed interactions, the addition of one-handed scale and rotation gestures would require some method for switching between annotation-drawing and annotation-manipulation modes. Alternatively, as our work explores, two hands can be used for spatial manipulation tasks, and one hand can be reserved for drawing annotations. This way, all visual indicators needed for rotation and scale could be hidden when only one hand is in the air, allowing annotations to not be obstructed. The Microsoft HoloLens currently only recommends one-handed gestures, and discourages the development of two-handed gestures on their [developer forums](https://forums.hololens.com/discussion/1613/two-hands-gesture), with the biggest concern about two-handed gestures being the limited hand tracking area in front of the device. Despite this, countless literature has demonstrated that bimanual interactions can outperform uni-manual interactions in 3D manipulation tasks ([1](https://dl.acm.org/citation.cfm?id=993837), [2](https://dl.acm.org/citation.cfm?id=1089512), [3](https://dl.acm.org/citation.cfm?id=253316), [4](http://ieeexplore.ieee.org/document/7131738/)), providing strong motivation for the exploration of bimanual gestures on the HoloLens. This work explores the feasibility and justification of developing two-handed gestures on the HoloLens, contributing four different approaches for manipulating drawn annotations using two-handed gestures and comparing them to a standard one-handed manipulation method on the HoloLens.

To evaluate the design space of two-handed interactions on the HoloLens, we conducted a within-subjects user study with 38 participants comparing the time and accuracy of performing each gesture to complete simple reference tasks. As a baseline comparison, we also implemented a technique similar to the one-handed  Wireframe Cube technique currently in use on standard HoloLens applications. The Wireframe Cube technique as implemented, e.g., in the default Hologram viewer, only allows for rotation along one axis (yaw). In order to allow for a fair comparison between this technique and our proposed two-handed techniques, we modified the Wireframe Cube and added the possibility for rotation about any axis. In our results, we found that overall the Wireframe Cube technique afforded more accurate manipulation than the other techniques by a small margin, and that there wasn't a significant difference in timing among the best performing techniques. One two-handed technique, our novel "Hands Locking into Gesture" technique, was most preferred by users compared to all other techniques, including Wireframe Cube, and showed no significant difference in terms of performance compared to the Wireframe Cube technique. Our results indicate that the possibility for a two-handed interface on the HoloLens is not only feasible, but can indeed be a valuable UI option according to user feedback and performance.

## Compilation Instructions

Open the Unity application. The current version of the application was developed in Unity 5.6.1. Click "open" in the top corner. Navigate to the Hololens-Gestures directory, and click on Hololens Gestures.

Within Unity, navigate to file -> build settings -> Build with the following settings:

```Scenes in Build: Gestures
Platform: Windows Store
SDK: Universal 10
Target Device: HoloLens
UWP Build Type: D3D
UWP SDK: Latest Installed
Build and Run on: Local Machine
Copy References: False
Unity C# Projects: True
Development Build: False
Autoconnect Profiler: False
```

Click on the folder Hololens-Gestures/Hololens Gestures/App

Once the application has built, you can open the .sln from within Hololens-Gestures/Hololens Gestures/App and deploy to the HoloLens from within Visual Studio.


