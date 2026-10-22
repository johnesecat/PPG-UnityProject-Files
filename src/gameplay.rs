use bevy::prelude::*;
use crate::catalog::get_full_item_catalog;
use crate::physics::RigidBody2D;
use crate::ragdoll::spawn_human_ragdoll;

pub struct PhysicsGameplayPlugin;

impl Plugin for PhysicsGameplayPlugin {
    fn build(&self, app: &mut App) {
        app.add_systems(OnEnter(crate::menu::AppState::InGame), setup_spawn_menu)
            .add_systems(
                Update,
                spawn_menu_system.run_if(in_state(crate::menu::AppState::InGame)),
            );
    }
}

#[derive(Component)]
pub struct SpawnCatalogButton {
    pub item_id: &'static str,
}

fn setup_spawn_menu(mut commands: Commands) {
    let catalog = get_full_item_catalog();

    commands
        .spawn(NodeBundle {
            style: Style {
                position_type: PositionType::Absolute,
                left: Val::Px(10.0),
                top: Val::Px(10.0),
                width: Val::Px(200.0),
                height: Val::Percent(90.0),
                flex_direction: FlexDirection::Column,
                padding: UiRect::all(Val::Px(10.0)),
                overflow: Overflow::clip_y(),
                ..default()
            },
            background_color: Color::rgba(0.1, 0.1, 0.1, 0.85).into(),
            ..default()
        })
        .with_children(|parent| {
            parent.spawn(TextBundle::from_section(
                "SPAWN MENU",
                TextStyle {
                    font_size: 18.0,
                    color: Color::WHITE,
                    ..default()
                },
            ).with_style(Style {
                margin: UiRect::bottom(Val::Px(10.0)),
                ..default()
            }));

            for item in catalog {
                parent
                    .spawn((
                        ButtonBundle {
                            style: Style {
                                width: Val::Percent(100.0),
                                height: Val::Px(32.0),
                                margin: UiRect::bottom(Val::Px(6.0)),
                                justify_content: JustifyContent::Center,
                                align_items: AlignItems::Center,
                                ..default()
                            },
                            background_color: Color::rgb(0.2, 0.2, 0.2).into(),
                            ..default()
                        },
                        SpawnCatalogButton {
                            item_id: item.id,
                        },
                    ))
                    .with_children(|btn| {
                        btn.spawn(TextBundle::from_section(
                            item.name,
                            TextStyle {
                                font_size: 13.0,
                                color: Color::WHITE,
                                ..default()
                            },
                        ));
                    });
            }
        });
}

fn spawn_menu_system(
    mut commands: Commands,
    asset_server: Res<AssetServer>,
    mut interaction_query: Query<
        (&Interaction, &SpawnCatalogButton, &mut BackgroundColor),
        (Changed<Interaction>, With<Button>),
    >,
) {
    let catalog = get_full_item_catalog();

    for (interaction, spawn_btn, mut bg_color) in &mut interaction_query {
        match *interaction {
            Interaction::Pressed => {
                *bg_color = Color::rgb(0.4, 0.4, 0.4).into();
                if let Some(item_def) = catalog.iter().find(|i| i.id == spawn_btn.item_id) {
                    info!("Spawning catalog item: {}", item_def.name);

                    if item_def.id == "human" {
                        spawn_human_ragdoll(&mut commands, &asset_server, Vec2::new(0.0, 100.0));
                    } else {
                        let texture_handle = asset_server.load(item_def.sprite_path);
                        commands.spawn((
                            SpriteBundle {
                                texture: texture_handle,
                                sprite: Sprite {
                                    custom_size: Some(item_def.size),
                                    ..default()
                                },
                                transform: Transform::from_xyz(0.0, 100.0, 1.0),
                                ..default()
                            },
                            RigidBody2D {
                                velocity: Vec2::ZERO,
                                angular_velocity: 0.0,
                                mass: item_def.mass,
                                inertia: item_def.mass * 10.0,
                                restitution: 0.2,
                                friction: 0.5,
                                is_static: false,
                                size: item_def.size,
                            },
                        ));
                    }
                }
            }
            Interaction::Hovered => {
                *bg_color = Color::rgb(0.3, 0.3, 0.3).into();
            }
            Interaction::None => {
                *bg_color = Color::rgb(0.2, 0.2, 0.2).into();
            }
        }
    }
}
