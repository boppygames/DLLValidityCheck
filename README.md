## Check out my game
If you want to help support the development of this project, please buy my game on Steam!
https://store.steampowered.com/app/1384030/Boppio/

# DLLValidityCheck
A simple Unity C# script for checking the integrity of DLL files during runtime.

Hello there :)

Unfortunately if you've come across this project someone has probably cracked your Steam game, which
is extremely frustrating as a game developer. The point of this project is to give you some basic
detection tools for trivial attacks, which will stop most crackers. However any game can be cracked
given enough time and enough willpower. We just want to provide all Unity game developers some
simple defense against these DLL substitution attacks. This should be good "enough" to prevent
most automated forms of attacks.

Some assumptions we're making:
 - You are building for standalone Windows, Mac OSX or Linux. Mobile or web builds are not officially supported right now.
 - All of your plugins are within the `Assets/Plugins` directory (or subdirectories therein)

How to get the most out of this script:
 - Call `DllValidityCheck.CheckIntegrity()` at startup. Think very carefully about what you want to do when the
        the integrity is invalid - if you close the application right away it will make it very easy for a cracker
        to find your logic for checking DLL integrity.
 - We recommend keeping `DLLValidityCheck.computeHashesOnBuild` enabled, so that hashes are computed automatically during build.
 - We recommend keeping `DLLValidityCheck.strictChecks` enabled, although it can cause false positives if you are loading custom
        DLLs outside of the build.
 - We STRONGLY recommend disabling `DLLValidityCheck.printHashErrors` in builds - printing hash errors will make it really easy
        for a cracker to see where you are checking for validity.
HIGHLY Recommended asset to pair with this:
 - BeeByte's obfuscator: https://assetstore.unity.com/packages/tools/utilities/obfuscator-48919
   - If you use BeeByte's obfuscator make sure you enable class/method/property renaming for this class. Also
        make sure to configure the obfuscator to generate additional "garbage" methods (~500+/class is good).

Other helpful assets:
 - ACT: https://assetstore.unity.com/packages/tools/utilities/anti-cheat-toolkit-152334


## Releases

If you just want to import a Unity package, we wil provide them as github releases.

## Off Topic

My only request is that if you use this script, do no harm to the user's machine who is running your
cracked software. Most people who download cracked software/games wouldn't have paid for your software/game
anyway and doing harm to their hardware is wrong. Also consider a large portion of people downloading 
cracked software are children or college students who have no money.

This tool is pretty much as import-and-never-touch-again as possible. I'm a game developer myself and I know
nobody wants to use a plugin that needs constant attention, so I've designed this to be as simple as possible.
