# Patch

This command-line tool can create a patch of a folder using a rsync delta-compression.

## Create a Patch

`GameBuildPatcher -c create -r D:\build.yesterday -b D:\build.today -p D:\build.today.patch`

After this you can send `build.yesterday`, if they don't have it already, and `build.today.patch` 
to the client together with this tool.

## Apply a Patch

Then when they have received it they can run the following command to construct `build.today`:

`GameBuildPatcher -c apply -r D:\build.yesterday -b D:\build.today -p D:\build.today.patch`




