# Style-of-Life
Style Of Life (SOL) is an open source program with the goal of providing excessive choice in 'style of life' - utility - programs, all bundled in one place to make life all the more easier. The program is built with a highly in-depth commenting style with the goal to also have SOL be used as a 'playground' for both new and experienced programmers to come learn, practice, teach and/or participate.
---
If you're a user:

##**Currently the installer is not working properly (because the manifests visual stuio generates are being wack)** so there currently isn't any.
Instead go into the "SOL (dev files)\SOL\bin\Debug" folder and use the executable that's there for now.


---
If you're a developer:

Unless you've updated **existing code**, do **not** request to merge with the **master** as the program will 'softly' add your addition *if the user want it* (kappa) in a later update (the 'additions' update, which will effectively let branches be treated as mods rather than perminate changes). non-update merge requests will just be ignored (with some leaneancy). This is just so that SOL stays small and doesn't eventually become bloatware.

If you're adding new stuff onto someone elses branch (that you can't add as it's own branch from the master, for compatibility sakes) feel free to merge, however the update will also hande 'mod mods' so you should only merge if you feel it's absolutely a nessasary addition to the main mod.

The program has a relatively easy to use update logging system, **make sure you read the _'programming logs'_ text file** on how to use the updates system - it also explains my commenting style if you can't wrap your head around how it works for some reason, no you don't have to comment in the same style.

The installer creates the executable here:
%AppData%\..\Local\Apps\2.0\Y0KZ27J0.QYW\VDABDBGK.RO3\sol...tion_0000000000000000_0001.0000_42e09a81965087f8
The resources folder it reads from doesn't have all the data in it (for some reason it's stored in some other random folder within the '2.0' folder) once you put it in there, it seems to work fine.
Just to get it to install I had to exclude everything from having a hash.
