use bevy::prelude::*;
use crate::menu::SelectedMap;

pub struct MapPlugin;

impl Plugin for MapPlugin {
    fn build(&self, app: &mut App) {
        app.add_systems(OnEnter(crate::menu::AppState::InGame), load_map);
    }
}

#[derive(Component)]
pub struct MapTile;

fn load_map(mut commands: Commands, selected_map: Res<SelectedMap>) {
    info!("Loading map: {}", selected_map.name);

    match selected_map.name.as_str() {
        "Abyss" => {
            commands.spawn((
                SpriteBundle {
                    sprite: Sprite {
                        color: Color::rgb(0.2, 0.2, 0.2),
                        custom_size: Some(Vec2::new(100.0, 2000.0)),
                        ..default()
                    },
                    transform: Transform::from_xyz(-400.0, -500.0, 0.0),
                    ..default()
                },
                MapTile,
            ));
            commands.spawn((
                SpriteBundle {
                    sprite: Sprite {
                        color: Color::rgb(0.2, 0.2, 0.2),
                        custom_size: Some(Vec2::new(100.0, 2000.0)),
                        ..default()
                    },
                    transform: Transform::from_xyz(400.0, -500.0, 0.0),
                    ..default()
                },
                MapTile,
            ));
        }
        _ => {
            commands.spawn((
                SpriteBundle {
                    sprite: Sprite {
                        color: Color::rgb(0.25, 0.25, 0.25),
                        custom_size: Some(Vec2::new(3000.0, 100.0)),
                        ..default()
                    },
                    transform: Transform::from_xyz(0.0, -300.0, 0.0),
                    ..default()
                },
                MapTile,
            ));
        }
    }
}
