use bevy::prelude::*;

pub struct TouchControlsPlugin;

impl Plugin for TouchControlsPlugin {
    fn build(&self, app: &mut App) {
        app.init_resource::<TouchState>()
            .add_systems(OnEnter(crate::menu::AppState::InGame), setup_touch_ui)
            .add_systems(
                Update,
                (touch_gesture_system, action_button_system)
                    .run_if(in_state(crate::menu::AppState::InGame)),
            );
    }
}

#[derive(Resource, Default)]
pub struct TouchState {
    pub active_touches: usize,
    pub held_entity: Option<Entity>,
}

#[derive(Component)]
pub struct ContextActionButton {
    pub action: ContextAction,
}

#[derive(Clone, Copy, Debug)]
pub enum ContextAction {
    UseFire,
    OptionsMenu,
}

fn setup_touch_ui(mut commands: Commands) {
    commands
        .spawn(NodeBundle {
            style: Style {
                position_type: PositionType::Absolute,
                right: Val::Px(20.0),
                bottom: Val::Px(20.0),
                flex_direction: FlexDirection::Row,
                align_items: AlignItems::Center,
                ..default()
            },
            ..default()
        })
        .with_children(|parent| {
            parent
                .spawn((
                    ButtonBundle {
                        style: Style {
                            width: Val::Px(65.0),
                            height: Val::Px(65.0),
                            margin: UiRect::right(Val::Px(15.0)),
                            justify_content: JustifyContent::Center,
                            align_items: AlignItems::Center,
                            ..default()
                        },
                        background_color: Color::rgba(0.2, 0.2, 0.2, 0.85).into(),
                        ..default()
                    },
                    ContextActionButton {
                        action: ContextAction::OptionsMenu,
                    },
                ))
                .with_children(|btn| {
                    btn.spawn(TextBundle::from_section(
                        "OPT",
                        TextStyle {
                            font_size: 18.0,
                            color: Color::WHITE,
                            ..default()
                        },
                    ));
                });

            parent
                .spawn((
                    ButtonBundle {
                        style: Style {
                            width: Val::Px(75.0),
                            height: Val::Px(75.0),
                            justify_content: JustifyContent::Center,
                            align_items: AlignItems::Center,
                            ..default()
                        },
                        background_color: Color::rgba(0.8, 0.2, 0.2, 0.85).into(),
                        ..default()
                    },
                    ContextActionButton {
                        action: ContextAction::UseFire,
                    },
                ))
                .with_children(|btn| {
                    btn.spawn(TextBundle::from_section(
                        "USE",
                        TextStyle {
                            font_size: 20.0,
                            color: Color::WHITE,
                            ..default()
                        },
                    ));
                });
        });
}

fn touch_gesture_system(
    touches: Res<Touches>,
    mut camera_query: Query<(&mut Transform, &mut OrthographicProjection), With<Camera2d>>,
) {
    let touch_vec: Vec<_> = touches.iter().collect();

    if touch_vec.len() == 1 {
        let touch = touch_vec[0];
        let delta = touch.delta();
        if delta != Vec2::ZERO {
            if let Ok((mut transform, projection)) = camera_query.get_single_mut() {
                transform.translation.x -= delta.x * projection.scale;
                transform.translation.y += delta.y * projection.scale;
            }
        }
    } else if touch_vec.len() == 2 {
        let t1 = touch_vec[0];
        let t2 = touch_vec[1];

        let prev_dist = t1.previous_position().distance(t2.previous_position());
        let current_dist = t1.position().distance(t2.position());

        if prev_dist > 0.0 && current_dist > 0.0 {
            let zoom_factor = prev_dist / current_dist;
            if let Ok((_, mut projection)) = camera_query.get_single_mut() {
                projection.scale = (projection.scale * zoom_factor).clamp(0.2, 5.0);
            }
        }
    }
}

fn action_button_system(
    mut interaction_query: Query<
        (&Interaction, &ContextActionButton, &mut BackgroundColor),
        (Changed<Interaction>, With<Button>),
    >,
) {
    for (interaction, btn, mut bg_color) in &mut interaction_query {
        match *interaction {
            Interaction::Pressed => match btn.action {
                ContextAction::UseFire => {
                    *bg_color = Color::rgba(1.0, 0.3, 0.3, 0.95).into();
                    info!("Touch Action: Fire / Use Triggered");
                }
                ContextAction::OptionsMenu => {
                    *bg_color = Color::rgba(0.4, 0.4, 0.4, 0.95).into();
                    info!("Touch Action: Options Menu Triggered");
                }
            },
            Interaction::Hovered => {
                *bg_color = Color::rgba(0.3, 0.3, 0.3, 0.9).into();
            }
            Interaction::None => match btn.action {
                ContextAction::UseFire => {
                    *bg_color = Color::rgba(0.8, 0.2, 0.2, 0.85).into();
                }
                ContextAction::OptionsMenu => {
                    *bg_color = Color::rgba(0.2, 0.2, 0.2, 0.85).into();
                }
            },
        }
    }
}
