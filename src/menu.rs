use bevy::prelude::*;

#[derive(Clone, Copy, Default, Eq, PartialEq, Debug, Hash, States)]
pub enum AppState {
    #[default]
    MainMenu,
    MapSelection,
    InGame,
}

#[derive(Resource)]
pub struct SelectedMap {
    pub name: String,
    pub description: String,
}

impl Default for SelectedMap {
    fn default() -> Self {
        Self {
            name: "Default".to_string(),
            description: "A large empty map with a flat floor.".to_string(),
        }
    }
}

pub struct MenuPlugin;

impl Plugin for MenuPlugin {
    fn build(&self, app: &mut App) {
        app.init_state::<AppState>()
            .init_resource::<SelectedMap>()
            .add_systems(OnEnter(AppState::MainMenu), setup_main_menu)
            .add_systems(OnExit(AppState::MainMenu), cleanup_main_menu)
            .add_systems(OnEnter(AppState::MapSelection), setup_map_selection)
            .add_systems(OnExit(AppState::MapSelection), cleanup_map_selection)
            .add_systems(Update, menu_button_system.run_if(in_state(AppState::MainMenu)))
            .add_systems(Update, map_selection_system.run_if(in_state(AppState::MapSelection)));
    }
}

#[derive(Component)]
struct MainMenuUI;

#[derive(Component)]
struct MapSelectionUI;

#[derive(Component)]
enum MenuButtonAction {
    Play,
    Mods,
    Settings,
    Exit,
}

#[derive(Component)]
struct MapSelectButton {
    name: String,
    description: String,
}

fn setup_main_menu(mut commands: Commands) {
    commands
        .spawn((
            NodeBundle {
                style: Style {
                    width: Val::Percent(100.0),
                    height: Val::Percent(100.0),
                    flex_direction: FlexDirection::Column,
                    justify_content: JustifyContent::Center,
                    align_items: AlignItems::Center,
                    ..default()
                },
                background_color: Color::rgb(0.05, 0.05, 0.05).into(),
                ..default()
            },
            MainMenuUI,
        ))
        .with_children(|parent| {
            parent.spawn(TextBundle::from_section(
                "PEOPLE PLAYGROUND",
                TextStyle {
                    font_size: 48.0,
                    color: Color::WHITE,
                    ..default()
                },
            ).with_style(Style {
                margin: UiRect::bottom(Val::Px(40.0)),
                ..default()
            }));

            let buttons = [
                ("PLAY", MenuButtonAction::Play),
                ("MODS", MenuButtonAction::Mods),
                ("SETTINGS", MenuButtonAction::Settings),
                ("EXIT", MenuButtonAction::Exit),
            ];

            for (text, action) in buttons {
                parent
                    .spawn((
                        ButtonBundle {
                            style: Style {
                                width: Val::Px(220.0),
                                height: Val::Px(50.0),
                                margin: UiRect::all(Val::Px(10.0)),
                                justify_content: JustifyContent::Center,
                                align_items: AlignItems::Center,
                                ..default()
                            },
                            background_color: Color::rgb(0.15, 0.15, 0.15).into(),
                            ..default()
                        },
                        action,
                    ))
                    .with_children(|btn| {
                        btn.spawn(TextBundle::from_section(
                            text,
                            TextStyle {
                                font_size: 24.0,
                                color: Color::WHITE,
                                ..default()
                            },
                        ));
                    });
            }
        });
}

fn cleanup_main_menu(mut commands: Commands, query: Query<Entity, With<MainMenuUI>>) {
    for entity in &query {
        commands.entity(entity).despawn_recursive();
    }
}

fn menu_button_system(
    mut interaction_query: Query<
        (&Interaction, &MenuButtonAction, &mut BackgroundColor),
        (Changed<Interaction>, With<Button>),
    >,
    mut next_state: ResMut<NextState<AppState>>,
) {
    for (interaction, action, mut bg_color) in &mut interaction_query {
        match *interaction {
            Interaction::Pressed => {
                *bg_color = Color::rgb(0.3, 0.3, 0.3).into();
                match action {
                    MenuButtonAction::Play => next_state.set(AppState::MapSelection),
                    _ => {}
                }
            }
            Interaction::Hovered => {
                *bg_color = Color::rgb(0.25, 0.25, 0.25).into();
            }
            Interaction::None => {
                *bg_color = Color::rgb(0.15, 0.15, 0.15).into();
            }
        }
    }
}

fn setup_map_selection(mut commands: Commands) {
    let maps = [
        ("Default", "Standard flat map with infinite bounds."),
        ("Abyss", "Deep vertical pit map."),
        ("Blocks", "Map with various geometric obstacles."),
        ("Humongous", "Extremely massive testing environment."),
        ("Sea", "Submerged underwater testing area."),
        ("Snow", "Snowy environment with slippery terrain."),
        ("Tower", "Tall vertical structure."),
    ];

    commands
        .spawn((
            NodeBundle {
                style: Style {
                    width: Val::Percent(100.0),
                    height: Val::Percent(100.0),
                    flex_direction: FlexDirection::Column,
                    justify_content: JustifyContent::FlexStart,
                    align_items: AlignItems::Center,
                    padding: UiRect::all(Val::Px(20.0)),
                    ..default()
                },
                background_color: Color::rgb(0.08, 0.08, 0.08).into(),
                ..default()
            },
            MapSelectionUI,
        ))
        .with_children(|parent| {
            parent.spawn(TextBundle::from_section(
                "SELECT MAP",
                TextStyle {
                    font_size: 36.0,
                    color: Color::WHITE,
                    ..default()
                },
            ).with_style(Style {
                margin: UiRect::bottom(Val::Px(20.0)),
                ..default()
            }));

            parent
                .spawn(NodeBundle {
                    style: Style {
                        flex_direction: FlexDirection::Row,
                        flex_wrap: FlexWrap::Wrap,
                        justify_content: JustifyContent::Center,
                        align_items: AlignItems::Center,
                        max_width: Val::Px(800.0),
                        ..default()
                    },
                    ..default()
                })
                .with_children(|grid| {
                    for (name, desc) in maps {
                        grid.spawn((
                            ButtonBundle {
                                style: Style {
                                    width: Val::Px(160.0),
                                    height: Val::Px(100.0),
                                    margin: UiRect::all(Val::Px(10.0)),
                                    flex_direction: FlexDirection::Column,
                                    justify_content: JustifyContent::Center,
                                    align_items: AlignItems::Center,
                                    ..default()
                                },
                                background_color: Color::rgb(0.2, 0.2, 0.2).into(),
                                ..default()
                            },
                            MapSelectButton {
                                name: name.to_string(),
                                description: desc.to_string(),
                            },
                        ))
                        .with_children(|btn| {
                            btn.spawn(TextBundle::from_section(
                                name,
                                TextStyle {
                                    font_size: 20.0,
                                    color: Color::WHITE,
                                    ..default()
                                },
                            ));
                        });
                    }
                });
        });
}

fn cleanup_map_selection(mut commands: Commands, query: Query<Entity, With<MapSelectionUI>>) {
    for entity in &query {
        commands.entity(entity).despawn_recursive();
    }
}

fn map_selection_system(
    mut interaction_query: Query<
        (&Interaction, &MapSelectButton, &mut BackgroundColor),
        (Changed<Interaction>, With<Button>),
    >,
    mut selected_map: ResMut<SelectedMap>,
    mut next_state: ResMut<NextState<AppState>>,
) {
    for (interaction, map_btn, mut bg_color) in &mut interaction_query {
        match *interaction {
            Interaction::Pressed => {
                selected_map.name = map_btn.name.clone();
                selected_map.description = map_btn.description.clone();
                next_state.set(AppState::InGame);
            }
            Interaction::Hovered => {
                *bg_color = Color::rgb(0.35, 0.35, 0.35).into();
            }
            Interaction::None => {
                *bg_color = Color::rgb(0.2, 0.2, 0.2).into();
            }
        }
    }
}
