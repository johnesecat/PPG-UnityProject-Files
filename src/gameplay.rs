use bevy::prelude::*;

pub struct PhysicsGameplayPlugin;

impl Plugin for PhysicsGameplayPlugin {
    fn build(&self, app: &mut App) {
        app.add_systems(OnEnter(crate::menu::AppState::InGame), setup_spawn_menu)
            .add_systems(
                Update,
                (physics_simulation_system, spawn_menu_system)
                    .run_if(in_state(crate::menu::AppState::InGame)),
            );
    }
}

#[derive(Component)]
pub struct RigidBody2D {
    pub velocity: Vec2,
    pub angular_velocity: f32,
    pub mass: f32,
    pub is_static: bool,
}

impl Default for RigidBody2D {
    fn default() -> Self {
        Self {
            velocity: Vec2::ZERO,
            angular_velocity: 0.0,
            mass: 1.0,
            is_static: false,
        }
    }
}

#[derive(Component)]
pub struct EntityCatalogItem {
    pub name: String,
    pub category: ItemCategory,
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum ItemCategory {
    Entities,
    Melee,
    Firearms,
    Explosives,
}

#[derive(Component)]
pub struct SpawnItemButton {
    pub category: ItemCategory,
    pub item_name: String,
}

fn setup_spawn_menu(mut commands: Commands) {
    // Left side spawn menu overlay panel
    commands
        .spawn(NodeBundle {
            style: Style {
                position_type: PositionType::Absolute,
                left: Val::Px(10.0),
                top: Val::Px(10.0),
                width: Val::Px(180.0),
                height: Val::Percent(90.0),
                flex_direction: FlexDirection::Column,
                padding: UiRect::all(Val::Px(10.0)),
                ..default()
            },
            background_color: Color::rgba(0.1, 0.1, 0.1, 0.8).into(),
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
                margin: UiRect::bottom(Val::Px(15.0)),
                ..default()
            }));

            let spawn_items = [
                ("Human", ItemCategory::Entities),
                ("Android", ItemCategory::Entities),
                ("Sword", ItemCategory::Melee),
                ("Pistol", ItemCategory::Firearms),
                ("AK-47", ItemCategory::Firearms),
                ("Grenade", ItemCategory::Explosives),
                ("C4", ItemCategory::Explosives),
            ];

            for (name, category) in spawn_items {
                parent
                    .spawn((
                        ButtonBundle {
                            style: Style {
                                width: Val::Percent(100.0),
                                height: Val::Px(35.0),
                                margin: UiRect::bottom(Val::Px(8.0)),
                                justify_content: JustifyContent::Center,
                                align_items: AlignItems::Center,
                                ..default()
                            },
                            background_color: Color::rgb(0.2, 0.2, 0.2).into(),
                            ..default()
                        },
                        SpawnItemButton {
                            category,
                            item_name: name.to_string(),
                        },
                    ))
                    .with_children(|btn| {
                        btn.spawn(TextBundle::from_section(
                            name,
                            TextStyle {
                                font_size: 14.0,
                                color: Color::WHITE,
                                ..default()
                            },
                        ));
                    });
            }
        });
}

fn physics_simulation_system(
    time: Res<Time>,
    mut query: Query<(&mut Transform, &mut RigidBody2D)>,
) {
    let dt = time.delta_seconds();
    let gravity = Vec2::new(0.0, -980.0);

    for (mut transform, mut body) in &mut query {
        if body.is_static {
            continue;
        }

        // Apply gravity
        body.velocity += gravity * dt;

        // Apply translation
        transform.translation.x += body.velocity.x * dt;
        transform.translation.y += body.velocity.y * dt;

        // Apply rotation
        transform.rotate_z(body.angular_velocity * dt);

        // Ground floor collision check at y = -250
        if transform.translation.y < -250.0 {
            transform.translation.y = -250.0;
            body.velocity.y = -body.velocity.y * 0.3; // bounce
            body.velocity.x *= 0.8; // friction
        }
    }
}

fn spawn_menu_system(
    mut commands: Commands,
    mut interaction_query: Query<
        (&Interaction, &SpawnItemButton, &mut BackgroundColor),
        (Changed<Interaction>, With<Button>),
    >,
) {
    for (interaction, spawn_btn, mut bg_color) in &mut interaction_query {
        match *interaction {
            Interaction::Pressed => {
                *bg_color = Color::rgb(0.4, 0.4, 0.4).into();
                info!("Spawning entity: {}", spawn_btn.item_name);

                // Spawn entity/item with 2D sprite and physics component
                commands.spawn((
                    SpriteBundle {
                        sprite: Sprite {
                            color: match spawn_btn.category {
                                ItemCategory::Entities => Color::rgb(0.9, 0.7, 0.6),
                                ItemCategory::Melee => Color::rgb(0.7, 0.7, 0.8),
                                ItemCategory::Firearms => Color::rgb(0.3, 0.3, 0.3),
                                ItemCategory::Explosives => Color::rgb(0.9, 0.2, 0.2),
                            },
                            custom_size: Some(Vec2::new(30.0, 60.0)),
                            ..default()
                        },
                        transform: Transform::from_xyz(0.0, 100.0, 1.0),
                        ..default()
                    },
                    RigidBody2D {
                        velocity: Vec2::new(0.0, 0.0),
                        angular_velocity: 0.0,
                        mass: 1.0,
                        is_static: false,
                    },
                    EntityCatalogItem {
                        name: spawn_btn.item_name.clone(),
                        category: spawn_btn.category,
                    },
                ));
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
