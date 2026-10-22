use bevy::prelude::*;
use crate::physics::RigidBody2D;

pub struct ItemsPlugin;

impl Plugin for ItemsPlugin {
    fn build(&self, app: &mut App) {
        app.add_systems(
            Update,
            (
                firearm_system,
                explosive_system,
                syringe_system,
            )
                .run_if(in_state(crate::menu::AppState::InGame)),
        );
    }
}

#[derive(Component, Debug, Clone)]
pub struct Firearm {
    pub muzzle_offset: Vec2,
    pub bullet_damage: f32,
    pub recoil_force: f32,
    pub fire_rate: f32,
    pub last_shot_time: f32,
}

#[derive(Component, Debug, Clone)]
pub struct Explosive {
    pub blast_radius: f32,
    pub blast_force: f32,
    pub damage: f32,
    pub is_primed: bool,
}

#[derive(Component, Debug, Clone)]
pub struct Syringe {
    pub liquid_type: SyringeLiquid,
    pub capacity: f32,
    pub amount: f32,
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum SyringeLiquid {
    LifeSyringe,
    AcidSyringe,
    DeathSyringe,
    FreezeSyringe,
}

fn firearm_system(
    mut commands: Commands,
    time: Res<Time>,
    keyboard_input: Res<ButtonInput<KeyCode>>,
    mut query: Query<(&Transform, &mut RigidBody2D, &mut Firearm)>,
) {
    if keyboard_input.just_pressed(KeyCode::KeyF) {
        for (transform, mut body, mut firearm) in &mut query {
            let current_time = time.elapsed_seconds();
            if current_time - firearm.last_shot_time < firearm.fire_rate {
                continue;
            }
            firearm.last_shot_time = current_time;

            let dir = transform.right().truncate();
            body.velocity -= dir * firearm.recoil_force;

            let spawn_pos = transform.transform_point(firearm.muzzle_offset.extend(0.0));
            commands.spawn((
                SpriteBundle {
                    sprite: Sprite {
                        color: Color::rgb(1.0, 0.9, 0.2),
                        custom_size: Some(Vec2::new(10.0, 4.0)),
                        ..default()
                    },
                    transform: Transform::from_translation(spawn_pos).with_rotation(transform.rotation),
                    ..default()
                },
                RigidBody2D {
                    velocity: dir * 2500.0,
                    angular_velocity: 0.0,
                    mass: 0.1,
                    inertia: 1.0,
                    restitution: 0.0,
                    friction: 0.1,
                    is_static: false,
                    size: Vec2::new(10.0, 4.0),
                },
            ));
        }
    }
}

fn explosive_system(
    mut commands: Commands,
    keyboard_input: Res<ButtonInput<KeyCode>>,
    query: Query<(Entity, &Transform, &Explosive)>,
    mut body_query: Query<(&Transform, &mut RigidBody2D), Without<Explosive>>,
) {
    if keyboard_input.just_pressed(KeyCode::KeyE) {
        for (explosive_entity, exp_transform, explosive) in &query {
            let exp_pos = exp_transform.translation.truncate();

            for (transform, mut body) in &mut body_query {
                let pos = transform.translation.truncate();
                let delta = pos - exp_pos;
                let dist = delta.length();

                if dist > 0.0 && dist <= explosive.blast_radius {
                    let force_scalar = (1.0 - dist / explosive.blast_radius) * explosive.blast_force;
                    let dir = delta / dist;
                    body.velocity += dir * force_scalar;
                }
            }

            commands.entity(explosive_entity).despawn();
            info!("Explosive detonated at {:?}", exp_pos);
        }
    }
}

fn syringe_system(
    mut query: Query<(&Transform, &mut Syringe)>,
) {
    for (_transform, mut syringe) in &mut query {
        if syringe.amount > 0.0 {
            syringe.amount -= 0.01;
        }
    }
}
