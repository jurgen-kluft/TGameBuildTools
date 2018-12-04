# Verification

This command-line tool can verify the content of a folder. It uses multiple cores to
speed up the hashing. It has been tested on a 40 GB build and took roughly 7 minutes
on an Core i7 machine.

## Create a Verification Signature

`GameBuildVerification -c create -f D:\\build -o D:\\build.gbv`

After this you can your build to the client including this tool and the `.gbv` file.

## Verify using a Verification Signature

Then when they have received it they can run the following command to verify the content:

`GameBuildVerification -c verify -f D:\\build -o D:\\build.gbv`

