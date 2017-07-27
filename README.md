TurnSignal (New 2.0!)
=

![TurnSignal Icon](/Assets/_Res/turnsignal-v2.png)
A minimalistic intuitive visual IN-GAME utility for keeping track of Cord Wrapping/Tangling in SteamVR.

---

### Please Note (!):
 TurnSignal 2.0 (and up) is a much needed optimization upgrade, it uses proper SteamVR auto-startup, less CPU cycles, and is all around better! Please Upgrade if you have not!

---
## ScreenShots - Updated for 2.0!

[How It Looks](https://gfycat.com/ImpressiveUnnaturalAmericansaddlebred)

[How It Works](https://gfycat.com/DemandingAdorableAntbear)

[The Menu](https://gfycat.com/EarlySnoopyChihuahua)

['Scale' Setting](https://gfycat.com/WhichSnoopyKite)

['Opacity' Setting](https://gfycat.com/GleamingGivingArchaeopteryx)

['Twist Rate' Setting](https://gfycat.com/FrequentHeartfeltAsianwaterbuffalo)

['Pedals' Setting](https://gfycat.com/CompetentPlainCormorant)

['Use Chaperone Color' Setting](https://gfycat.com/ContentBlueBlueshark) 
(This one got straight up BORK'ed by the video compression, but the color, it really does match, I promise, if you could see it. Really.)

['Link Opacity and Twist' Setting](https://gfycat.com/HeartfeltLimpGoitered)

[Some Fun Stuff ;3](https://gfycat.com/ThoroughThunderousAmurminnow)

---
## Download:

[Click here to go to releases!](https://github.com/benotter/TurnSignal/releases)

---

## Installation:
To 'Install', please download the 'latest' release from [here](https://github.com/benotter/TurnSignal/releases), extract from the .zip file to where-ever you want to keep the utility on your computer, and run TurnSignal.exe! 
 
 It should automatically register itself to start with SteamVR, and auto start/close with it from first launch on, but this is a toggle-able option, and can be freely disabled from the Dashboard Menu!

---
## Usage:

After launching the program, the window should be minimized automatically, if not, you can minimize it at your liking!

The settings menu to control Aspects of TurnSignal should be included in your SteamVR Dashboard as an Overlay, and the utility itself should be projected onto the floor of your environment.

The Newest Settings menu includes:

- A Button Toggle to enable/disable the tools in-game display
- A Button to Reset the utilities Rotation Tracking
- An Opacity Setting 
- A Scale Setting 
- A Twist Rate Setting for manipulating the twist-i-ness levels 
- A Toggle to auto-starting and closing the utility with SteamVR
- A Toggle to use Chaperone Color instead of white (real time too!) 
- A Toggle To Link Opacity and Twist'ed-ness together, so it shows clearer the more wrapped up your cord is.

All settings are saved between app sessions, and shouldn't need to be set every time its launched.

The utility will still track rotations even when its toggled off from the dashboard menu, so if/when its toggled back on, it will still show turns as if was left on, with the turns intact.

-- Note -- The way Twist Rate affects the ammount of rotation the floor reacts to is actually measured from 0 - 10 (now user set Max) turns in either direction, and given as a 0.0 - 1.0 floating point, with 10 (or user number) turns used as 'max', but it can exceed this without problem, resulting in 'fun' floor shapes that have higher then 1.0 twist values. Just an FYI.

---
## About:

I had the idea for the utility (which originally used large arrows and a counter in the center, but That was Terrible) after playing, and watching people play, GORN on my HMD. 

Every time, when action got intense, the cord would inevitably get wrapped up and tangled, and get caught on legs, the controller, the like.

I figured it couldn't hurt to have a visual, in-game tool, to subtly alert you to your ammount of turns, without pausing to dashboard. It was also a very simple idea, with a very fixed feature set to complete it, so why not, eh?

### Part 2

After that, I posted the tool on reddit, and somone cross posted THAT on geogaf, and I got a whole lot of positive feedback, (which I thank everyone for <3!), and that drove me to dedicate a bit more time to it (Another Week), and now its basically rewritten from scatch.

---
## About - OpenVR Overlays in Newest Version of Unity

Now this was a minor chunk of a task, but after messing around with the provided overlays in the SteamVR plugin, finding out Unity only sends the 'Scene' App type to OpenVR, further finding out others who use unity for overlays mitigate this issue by using older Unity Versions;

I finally just went ahead and spun my own OpenVR Handler that initializes SteamVR, handles getting HMD/Left/Right controller positions and input, and outputting 2D texures as overlays in relation to Unity Worldspace.

I Built the OVR_Overlay to be generic, and be used in arbitrary projects, which evolved (only in concept, rewrote from scratch) from TurnSignal 1.0's implementation, which was god-awful. Still is pretty bad, but now a bit more, self contained and trusty?

Anyway, It technically relies on 'having' the SteamVR plugin in a project, but requires you 100% disable the built in SteamVR/UnityVR stuff, as having both running in the same proccess REALLY screws with things.

As for actually rendering an image, I just 1950's it, and set up 2D things in a 3D scene, and render-to-texture a flat UI image. Really saves time when compared to spinning a texture generation script.

For the UI interaction in the Menu, I wrote a generic UnityUI handler that takes in 2D screen coords from SteamVR, and uses them as screen-coords for the menu-camera, then I fire GraphicRays against UI, and manually create a fake pointer-data event, submit it, and Bam, UnityUI works drop in, and I now mean Actually drop in, because in 1.x, Sliders and some stuff wouldn't work, but now its all pretty and does!

---

I think thats that, Credit/CopyRight to Valve for OpenVR/SteamVR stuff. 

Thanks and credit to my friend Mason Halstead (141art) for The Icon asset!

Special thanks to [matzman666](https://github.com/matzman666) for giving me super-helpful resources for the SteamVR AutoStart App-Manifest, and Chaperone Color Retrivial Stuff, which he posted [Here!](https://www.reddit.com/r/Vive/comments/6oqspt/turnsignal_a_small_ingame_vr_utility_i_built_to/dkn6pj7/)

Feel free to message me for any questions/comments/feature-requests!