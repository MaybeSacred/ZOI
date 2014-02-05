ZOI - Zone of Inaccessibility
=============================================================================

IMPORTANT:

	Please export all 3D models into .fbx format.
	Unity can natively read/import those files while other file formats require their respective programs to import, e.g. .blend files for Blender, .mb files for Maya, etc.
	
=============================================================================

Git workflow.
  Try not to work on your local master but instead on a seperate local branch.

Using GitHub's GUI.
  - Make sure you have your branch set to master. (Found on top right corner)
  - Sync this branch to the remote repository.
  - To work on things.
    - Create or use a different branch from master. Either create another or select another.
      - Make sure this other branch is synched with master.
      - To synch branches, you merge your 2 branches together.
      - Press the branches drop down and press manage/merge, then drag the master with your local
        and make sure that the new branch is your local. [Branch 1]>[Branch 2] > [new branch].
        (make sure new branch is your local branch.)
    - Perform work on your local branch.
    - Commit changes to your local branch.
    - Merge your local branch to your master branch.
    - Once master has all of your committed changes from your local branch, push them
      to the remote repository.
  - Resynch your master if necessary, although it should always be synched at this point since
    a commit was just pushed from you.
    
Using Git's terminal commands. ($ [command] [words and words] means it's a terminal command in this readme, don't type the "$ ")
  - $ cd ZOI to get into the correct repository.
  - Make sure you are in the master branch.
  
    $ git checkout master
  - If master has not been touched since the last time it was up-to-date.
    - Pull changes from the repository.
	
      $ git pull
  - If master has changes from the last time it was up-to-date.
    - Make Git track changes.
	
      $ git status
    - Commit changes or reset your changes.
      - Commit them.
	  
        $ git commit
      - Undo commit.
	  
        $ git reset --soft HEAD^
      - Reset your repo.
	  
        $ git reset --hard HEAD^
    - Push changes to remote repository (origin).
	
      $ git push
    - Now try to pull commit back to master
	
      $ git pull
  - At this point, master needs to be up-to-date. Local master commit has to be the same as the remote master.
  - Create or merge master changes onto another branch.
    - Create branch.
	
      $ git branch [other branch name]
    - Merge branch.
	
      $ git checkout [other branch name]
	  
      $ git merge master          <-- this command merges changes FROM branch, so make sure you were able to checkout [branch name]
  - Go about your work on your local branch and you can forget about Git until you are done working.
  - Once you are done working on your stuff, tell Git to track changes, and then commit them.
  
    $ git status
    - Now add files to be commited. "$ git add ." adds all files to the commit. Pretty sure "$ git commit -a" adds all too.
	
    $ git commit
    - If a text editor appears, you write the name of your commit at the top line, save the file, and exit it.
  - Now that your local branch has your changes committed, time to merge it to your master and commit them to the remote.
  - Checkout your master and merge the changes from your other branch to master.
  
    $ git checkout master
	
    $ git merge [other branch name]
  - I'm pretty sure that after that merge, master has the other branch's commit.
  - Lastly, push your changes to the remote repository.
  
    $ git push
    
================================================================================

Other important data about the project... something

================================================================================

2014 Feb 02 - Daniel Dias - First Draft 
