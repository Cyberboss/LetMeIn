# LetMeIn

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that automatically requests invites from the hosts of sessions in your `autoJoinSessions` config list when you log in.

This requires the `autoJoinSessions` entries to specify sessions using the `sessionId` field in `S-U-XXX:YYY` format so the user ID to request off of can be deduced.

This can be useful for users of Headless servers that remain private but automatically respond to invite requests from specific users.

## Installation
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
1. Place [LetMeIn.dll](https://github.com/Cyberboss/LetMeIn/releases/latest/download/LetMeIn.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default Windows install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create this folder for you.
1. Start the game. If you want to verify that the mod is working you can check your Resonite logs.
