## Check out my game
If you want to help support the development of this project, please buy my game on Steam!
https://store.steampowered.com/app/1384030/Boppio/

# DLLValidityCheck
A simple Unity C# script for checking the integrity of DLL files at startup.

Hello there :)

Unfortunately if you've come across this project someone has probably cracked your Steam game, which is extremely frustrating as a game developer. However most of these cracks are just simple DLL subititutions, therefore they are easy to detect. This plugin does not *prevent* DLL substitution, it just detects if you're running in an environment where one or more of your DLLs have changed since the build phase.

## Requirements

Some assumptions we're making:
 - You're running Unity 2019.4, other versions are not tested but may work just fine.
 - You are building for standalone Windows, Mac OSX or Linux. Mobile or web builds are not officially supported right now.
 - All of your plugins are within the `Assets/Plugins` directory (or subdirectories therein).

## Usage

If you just want to import a Unity package, check out our [releases page](https://github.com/boppygames/DLLValidityCheck/releases) and just import the latest release into Unity. We recommend looking at the example scene before you implement this in your own app.

How to get the most out of this script:
 - Call `DllValidityCheck.CheckIntegrity()` at startup - decide what you want to do here.
 - We recommend keeping `DLLValidityCheck.computeHashesOnBuild` enabled, so that hashes are computed automatically during build.
 - We recommend keeping `DLLValidityCheck.strictChecks` enabled, although it can cause false positives if you are loading custom
        DLLs outside of the build.
 - We STRONGLY recommend disabling `DLLValidityCheck.printHashErrors` in builds - printing hash errors will make it really easy
        for a cracker to see where you are checking for validity.
        
HIGHLY Recommended asset to pair with this:
 - BeeByte's obfuscator: https://assetstore.unity.com/packages/tools/utilities/obfuscator-48919
   - If you use BeeByte's obfuscator make sure you enable class/method/property renaming.

Other helpful assets:
 - ACT: https://assetstore.unity.com/packages/tools/utilities/anti-cheat-toolkit-152334

## Cracked Environments

There seem to be quite a few opinions on the internet on what to do when you're running in a cracked environment. I think these seem to be the best options:
 - Restrict the player to demo mode, this works nicely because if you already have a free demo then the cracked versions are basically free distribution of your demo.
 - Close the app after a random interval between 5-10 minutes. If you close the app right away it could make it easier for a cracker to figure out where in the code you are closing the app.
 - Let the player play normally, just with a watermark or warning they are running cracked software

I would recommend if you are running in a cracked environment to try to prevent your app from loading any new DLLs. This means you should check to see if you're running in cracked mode before doing *any* `steam_api` calls.

## Limitations

If someone is motivated enough they will be able to crack your game reguardless of what you do. This detection code can also easily be defeated if the cracker figures out where in your DLL you are checking for validity, and then just disabling that check. Its also wise to have multiple layers of security here - meaning you should also be using a code obfuscator to make it harder for crackers to figure out where in your code you are doing these checks.
