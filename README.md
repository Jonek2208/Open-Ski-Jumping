# Open Ski Jumping
Open Ski Jumping is a free and open source ski jumping game.
More specific information you can find on project wiki.

<img src="https://jonek2208.github.io/Open-Ski-Jumping/img/screenshot2.jpg" height="500" > 

# Setting up your computer for development
## Git
### Windows
Download and install Windows version of git from [https://git-scm.com/](https://git-scm.com/)
### Linux
Install git using package manager like `sudo apt install git` on Ubuntu
## Blender 
<img src="https://download.blender.org/branding/blender_logo_socket.svg" height="50" > 

Download and install Blender from [https://www.blender.org/](https://www.blender.org/)
## Graphics editor
If you do not have any favorite one, we strongly recommend to use GIMP, it is free and open source. You can download it from [https://www.gimp.org/](https://www.gimp.org/).
## Unity
<img src="https://upload.wikimedia.org/wikipedia/commons/1/19/Unity_Technologies_logo.svg" height="50" > 


Download and install the newest version of Unity Hub from [https://forum.unity.com/threads/unity-hub-v2-0-0-release.677485/](https://forum.unity.com/threads/unity-hub-v2-0-0-release.677485/).
Then install the newest version of Unity 2019 using Unity Hub.
## Code editor
If you do not have favorite one, we recommend use Visual Studio Code - free, open source and lightweight. You can download version that suits your platform from: [https://code.visualstudio.com/Download](https://code.visualstudio.com/Download)

# Git commands
## Download project to your computer
* Open git bash console or terminal
* Clone git repository to your computer `git clone https://github.com/Jonek2208/Open-Ski-Jumping.git`
* Enter project folder `cd Open-Ski-Jumping`

## Add changes to project
* Add changes to commit `git add .`
* Create a commit (with short and brief description of changes you made) `git commit`
* Pull changes from remote repository `git pull`
* Push changes `git push`

## You have not done anything important with project and you want to fetch changes from remote repository without pushing your changes
It is very common situation when you did some "experiments" with project and you want to fetch changes made by someone else. Then you probably would have an error. Try this:
 
`git reset --hard`

`git pull`


