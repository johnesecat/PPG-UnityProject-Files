mod menu;
mod map;
mod touch;
mod gameplay;

use bevy::prelude::*;
use menu::MenuPlugin;
use map::MapPlugin;
use touch::TouchControlsPlugin;
use gameplay::PhysicsGameplayPlugin;

fn main() {
    #[cfg(target_arch = "wasm32")]
    console_error_panic_hook::set_once();

    App::new()
        .add_plugins(DefaultPlugins.set(WindowPlugin {
            primary_window: Some(Window {
                title: "People Playground Web".into(),
                canvas: Some("#your_canvas_id".into()),
                prevent_default_event_handling: false,
                ..default()
            }),
            ..default()
        }))
        .add_plugins((MenuPlugin, MapPlugin, TouchControlsPlugin, PhysicsGameplayPlugin))
        .add_systems(Startup, setup)
        .run();
}

fn setup(mut commands: Commands) {
    commands.spawn(Camera2dBundle::default());
}
