# Simple-Funscript-Editor
A simple open-source editor for .funscript files compatible with various sex toys (I've only tested it with the Handy, so I don't know how well it works for anything else. This code is provided without any warranty whatsoever, and that includes some other masturbator taking one of these scripts and interpreting it as "engage blender mode." I don't know why your toy even has that feature).

I was taking a quick break from making games, but apparently I start to hallucinate when I've gone more than two days without writing code for fun, so this happened. If you're here from my Patreon, don't judge me. I think writing open source software is good for your karma or something. Even if it's just to help control a very elaborate masturbator. 

This is just about as simple as a funscript editor can be, but I got frustrated with the complexity of existing offerings I saw and decided to write my own (and learn WPF in the process).

Just open a video to get started. Once that's done, use keyboard shortcuts to tag keyframes. There are no on-screen buttons for that because this is faster.

This software uses FFmpeg for video playback. I'm pretty sure I'm supposed to mention that prominently, so here it is.

*Shortcuts:*

* Space: Play/Pause
* W/S: Increase/decrease playback speed
* A/D: Previous/next frame (pauses video)
* Number keys (top row or numpad): Set keyframe for (number * 11) at the current frame
* Delete: Delete keyframe at the current frame

That's all there is to it. As with most open source software, use this at your own risk. I used it to write funscripts for some of my favorite videos and don't know of any problems, but I don't take responsibility for anything that happens as a consequence of using this software.

You need to put FFmpeg in the same directory as the output files to get this to run (specifically, ffmpeg.exe has to be in bin/[debug or release]/ffmpeg/bin). The amount of compliance engineering they ask for scares me, so I'm not distributing it myself. You can get it here: https://ffmpeg.zeranoe.com/builds/

All code is under GPL V3.0
