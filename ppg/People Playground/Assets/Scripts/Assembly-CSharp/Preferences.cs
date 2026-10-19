using System;
using UnityEngine;

public class Preferences
{
	[Setting(SettingCategory.General, "Collision quality", "Determines the quality of the collision detection. Discrete mode is much faster, but less accurate. Continuous mode prevents objects from passing through each other at high velocities. Dynamic mode switches between the two based on velocity.")]
	[Sort(0)]
	public CollisionQuality CollisionQuality = CollisionQuality.Dynamic;

	[Setting(SettingCategory.General, "Stop animation on damage", "When enabled, entities will reset their animation override if they are damaged. For example, a human will stop walking when shot.")]
	public bool StopAnimationOnDamage;

	[Setting(SettingCategory.General, "Drop on death", "When enabled, humans and androids will drop objects when their arms aren't capable of holding on anymore. This is usually when they die or when their arms are shot. Default is on.")]
	public bool DropOnDeath = true;

	[Setting(SettingCategory.General, "Screenshake intensity", "Adjust the intensity of the screenshake. Default is 1x.")]
	[Range(0f, 2f)]
	[Format("{0}x", 1f, 2)]
	[Step(0.01f)]
	public float ShakeIntensity = 1f;

	[Setting(SettingCategory.General, "Show hovering highlights", "Toggle whether selection, freezing, and wire highlights are drawn or not. Useful for recording.")]
	public bool ShowOutlines = true;

	[Setting(SettingCategory.General, "Slow motion speed modifier", "Determines how much slower the time will pass in slow motion mode. Default is 20%.")]
	[Range(1f, 100f)]
	[Format("{0}%", 1f, 0)]
	[Step(1f)]
	public float SlowMotionSpeed = 20f;

	[Setting(SettingCategory.General, "Ambient temperature transfer", "Heat will travel through the air when this is enabled.\nIf disabled, heat can only be transferred through heat pipes and direct contact.\nThis setting may decrease performance when enabled. It is disabled by default.")]
	[Sort(4)]
	public bool AmbientTemperatureTransfer;

	[Setting(SettingCategory.General, "Physics iteration count", "Determines the amount of physics iterations for the physics simulation. Higher values cause the simulation to be more accurate at the cost of performance. Values greater than 128 may have diminishing returns. Default is 32.")]
	[Range(8f, 256f)]
	[Sort(1)]
	[Format("{0} iterations", 1f, 0)]
	public int PhysicsIterations = 32;

	[Setting(SettingCategory.General, "Snap grid size", "The distance between grid points used for translational snapping. This helps in aligning objects more precisely by snapping their position to the nearest grid point. Default is 0.25 m")]
	[Range(0.1f, 2f)]
	[Step(0.05f)]
	[Format("{0} m", 1f, 2)]
	public float GridSnap = 0.25f;

	[Setting(SettingCategory.General, "Angle snap size", "The angle increment used for snapping during rotation. This helps in aligning objects more precisely by snapping their rotation to the nearest multiple of this value. Default is 15 degrees")]
	[Range(5f, 90f)]
	[Format("{0} degrees", 1f, 0)]
	public int AngleSnap = 15;

	[Setting(SettingCategory.General, "Alert when mods significantly affect loading times", "When enabled, an alert will appear when mods cause increased loading times.")]
	public bool ShowModLoadingFreeze = true;

	[Setting(SettingCategory.General, "Reject suspicious mods", "When enabled, the game will reject mods with suspicious code. Takes effect after restart. \n<color=#F55><b>It is recommended to keep this enabled. Mods that ask you to disable this setting should be reported.")]
	public bool RejectShadyCode = true;

	[Setting(SettingCategory.General, "Maximum mod compilation time", "The maximum amount of seconds that a mod can compile for until it's skipped.")]
	[Range(2f, 120f)]
	[Format("{0} seconds", 1f, 0)]
	public int MaxModCompilationTime = 30;

	[Setting(SettingCategory.Gore, "Goreless", "No blood or obvious gore will be rendered when this is enabled.")]
	[Sort(-1)]
	public bool GorelessMode;

	[Setting(SettingCategory.Gore, "Procedural fragments", "Enabling this will cause the generation of procedural flesh and bone fragments on full limb crush. This may cause performance issues when lots of limbs are destroyed.")]
	[Sort(-1)]
	public bool ProceduralFragments;

	[Setting(SettingCategory.Gore, "Fragility multiplier", "The higher this value is the more fragile entities become.")]
	[Range(0.05f, 2f)]
	[Format("{0}x", 1f, 3)]
	[Step(0.05f)]
	public float FragilityMultiplier = 1f;

	[Setting(SettingCategory.Gore, "Limb crushing", "Toggle whether limbs are fully destroyable or not. When enabled, limbs will turn into red mist under extreme pressure.")]
	public bool LimbCrushing = true;

	[Setting(SettingCategory.Gore, "Brain damage", "Toggle whether brain damage can occur. It causes organisms to occasionally pass out, fail to stand upright, and have seizures.")]
	public bool BrainDamage = true;

	[Obsolete]
	[HideInSettingsMenu]
	[Setting(SettingCategory.Gore, "Liquid overflow limb exploding", "Toggle whether limbs explode when too full of liquid. Disabled by default. Will only take effect if limb crushing is enabled.")]
	public bool LimbOverflowExplosion;

	[Setting(SettingCategory.Gore, "Gore shader", "Choose between different gore shaders.")]
	public GoreShaderMode GoreMode;

	[Setting(SettingCategory.Gore, "Slowly heal injuries", "Wounds and injuries will slowly heal on living organisms when this is enabled.")]
	public bool AutoHealWounds = true;

	[Setting(SettingCategory.Gore, "Extra shot impact particles", "Limbs with enough flesh on them may eject chunky particles when shot.")]
	public bool ChunkyShotParticles = true;

	[Setting(SettingCategory.Gore, "Dismemberment loose tissue", "Dismemberment of limbs may expose loose stringy tissue.")]
	public bool DismembermentLooseTissue = true;

	[Setting(SettingCategory.Gore, "Limb crushing sensitivity", "Determines how much force is needed to crush a limb. Default is 100%.")]
	[Range(0f, 2f)]
	[Format("{0}%", 100f, 0)]
	[Step(0.05f)]
	public float CrushForceMultiplier = 1f;

	[Setting(SettingCategory.VisualEffects, "Bloom", "Bloom is a post processing effect that gives the illusion of objects being extremely bright.")]
	[Sort(7)]
	public BloomMode BloomMode = BloomMode.Fancy;

	[Setting(SettingCategory.VisualEffects, "Bullet tracers", "Toggle whether bullets have travel time and display tracers. This degrades performance. Off by default.")]
	public bool TracerBullets = true;

	[Setting(SettingCategory.VisualEffects, "Fancy effects", "This will toggle relatively performance-heavy effects.")]
	public bool FancyEffects = true;

	[Setting(SettingCategory.VisualEffects, "Decals", "Toggle whether decals are created. Disabling decals may improve performance.")]
	public bool Decals = true;

	[Setting(SettingCategory.VisualEffects, "Lighting", "The lighting system makes dark areas actually dark, and makes light-emitting objects useful. Disabling lighting will make everything \"full bright\". Lighting has very little impact on performance.")]
	public bool Lighting = true;

	[Setting(SettingCategory.Audio, "Master", "The volume of the entire application.")]
	[Range(0f, 1f)]
	[Sort(0)]
	[Format("{0}%", 100f, 0)]
	[Step(0.01f)]
	public float MasterVolume = 1f;

	[Setting(SettingCategory.Audio, "Sound effects", "The volume of all items and effects in the game.")]
	[Range(0f, 1f)]
	[Sort(1)]
	[Format("{0}%", 100f, 0)]
	[Step(0.01f)]
	public float SfxVolume = 1f;

	[Setting(SettingCategory.Audio, "User interface", "The volume of UI elements.")]
	[Range(0f, 1f)]
	[Sort(3)]
	[Format("{0}%", 100f, 0)]
	[Step(0.01f)]
	public float UserInterfaceVolume = 1f;

	[Setting(SettingCategory.Audio, "Ambience", "The volume of the ambient hum and other background noises.")]
	[Range(0f, 1f)]
	[Sort(2)]
	[Format("{0}%", 100f, 0)]
	[Step(0.01f)]
	public float AmbienceVolume = 1f;

	[Setting(SettingCategory.Audio, "Ambience Highpass", "Highpass cutoff. Ambient audio will not play any audio below this frequency. This is useful if you suffer from crackling audio issues. 90 Hz is the default and recommended value here.")]
	[Range(50f, 500f)]
	[Sort(3)]
	[Format("{0} Hz", 1f, 0)]
	[Step(1f)]
	public float AmbienceHighpassCutoff = 90f;

	[Setting(SettingCategory.Audio, "Clamp audio volume", "Audio can play at volumes that exceed set limits. It is recommended you keep this turned on to prevent hearing damage. You enable this at your own risk!")]
	public bool ClampVolume = true;

	[Setting(SettingCategory.Audio, "Distant sound effects", "When enabled, things like explosions and guns can be heard reflecting and echoing in the distance. Disabling this setting makes audio processing a bit less demanding.")]
	public bool DistantSoundEffects = true;

	[Setting(SettingCategory.Audio, "Bullet cracks and whizzes", "When enabled, supersonic bullets can make whip crack and whiz-by sounds.")]
	public bool BulletCrackWhiz = true;

	[Setting(SettingCategory.UserInterface, "Log debug messages", "Debug messages will be logged as notifications in-game. Messages include warnings, errors, and unhandled exceptions. It is recommended to keep this off.")]
	public bool LogDebugMessages;

	[Setting(SettingCategory.UserInterface, "Show framerate", "Displays the current framerate in the top-right of the screen.")]
	public bool ShowFramerate;

	[Setting(SettingCategory.UserInterface, "Zoom scroll wheel sensitivity", "Determines how fast zooming using the mouse wheel is. Default is 1x.")]
	[Range(0f, 2f)]
	[Format("{0}x", 1f, 2)]
	[Step(0.1f)]
	public float ZoomSensitivity = 1f;

	[Setting(SettingCategory.UserInterface, "Thermal vision update rate", "How often should the thermal vision be updated?")]
	[Range(10f, 60f)]
	[Format("{0} Hz", 1f, 0)]
	[Step(5f)]
	public int ThermalVisionUpdateRate = 25;

	[Setting(SettingCategory.UserInterface, "Wires with context menu", "Toggle whether wires block the context menu for the underlying object.")]
	public bool DeleteWireByContextMenu = true;

	[Setting(SettingCategory.UserInterface, "Temperature unit", "Determines the temperature unit used throughout the game. Celsius by default.")]
	[Sort(5)]
	public TemperatureUnit TemperatureUnit;

	[Setting(SettingCategory.Video, "Vsync", "With Vsync turned on, the framerate will be capped to the refresh rate of your monitor. This minimises screen tearing.")]
	[Sort(1)]
	public bool VSync;

	[Setting(SettingCategory.Video, "Anti-aliasing (SMAA)", "Anti-aliasing attempts to get rid of jagged edges.")]
	public bool SMAA;

	[Setting(SettingCategory.Video, "Tonemapping", "Change the way HDR colours are displayed.")]
	[Sort(5)]
	public TonemappingMode TonemappingMode;

	[Setting(SettingCategory.Video, "Brightness", "Modify the final exposure value. Default is 100%")]
	[Range(10f, 400f)]
	[Format("{0}%", 1f, 0)]
	[Sort(4)]
	[Step(5f)]
	public float Brightness = 100f;

	[Setting(SettingCategory.Video, "Render scale", "Downscale the resolution that the game is being rendered at.\n<b><color=#ff9>This only applies when the window mode is set to Borderless or Fullscreen.")]
	[Range(0.25f, 1f)]
	[Option(new float[] { 0.25f, 0.5f, 0.75f, 1f })]
	[Sort(1)]
	[Format("{0}%", 100f, 0)]
	public float RenderScale = 1f;

	[Setting(SettingCategory.Video, "Framerate limit", "Determine the upper bound of the framerate.\n<color=#ff9><b>This will only take effect if Vsync is disabled.")]
	[Range(30f, 1000f)]
	[Sort(3)]
	[Format("{0} fps", 1f, 0)]
	public int FramerateLimit = 144;

	[Setting(SettingCategory.Video, "Simplified background tiles", "A simplified background may help decrease compression artifacts in screen recordings. Simplified rendering does not increase performance.")]
	public bool SimplifiedBackground;

	[Setting(SettingCategory.Video, "Window mode", "Determines the way the window is presented.")]
	[Sort(0)]
	public WindowMode WindowMode = WindowMode.Borderless;

	[HideInSettingsMenu]
	public Vector2Int? Resolution;

	[HideInSettingsMenu]
	public bool ShowTutorial = true;

	[HideInSettingsMenu]
	public float ToyboxSizeOffset = 483.6f;
}
