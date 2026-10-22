use bevy::prelude::*;
use crate::ragdoll::Limb;
use crate::physics::RigidBody2D;

pub struct GorePlugin;

impl Plugin for GorePlugin {
    fn build(&self, app: &mut App) {
        app.add_systems(
            Update,
            (
                blood_particle_system,
                dismemberment_system,
            )
                .run_if(in_state(crate::menu::AppState::InGame)),
        );
    }
}

#[derive(Component, Debug, Clone)]
pub struct BloodParticle {
    pub lifetime: f32,
    pub velocity: Vec2,
}

#[derive(Component, Debug, Clone)]
pub struct Wound {
    pub bleeding_rate: f32,
    pub severity: f32,
}

fn blood_particle_system(
    mut commands: Commands,
    time: Res<Time>,
    mut query: Query<(Entity, &mut Transform, &mut BloodParticle)>,
) {
    let dt = time.delta_seconds();
    let gravity = Vec2::new(0.0, -980.0);

    for (entity, mut transform, mut particle) in &mut query {
        particle.lifetime -= dt;
        if particle.lifetime <= 0.0 {
            commands.entity(entity).despawn();
            continue;
        }

        particle.velocity += gravity * dt;
        transform.translation.x += particle.velocity.x * dt;
        transform.translation.y += particle.velocity.y * dt;

        if transform.translation.y <= -250.0 {
            transform.translation.y = -250.0;
            particle.velocity = Vec2::ZERO;
        }
    }
}

fn dismemberment_system(
    mut commands: Commands,
    mut query: Query<(Entity, &Transform, &mut Limb, Option<&RigidBody2D>)>,
) {
    for (entity, transform, mut limb, _body) in &mut query {
        if limb.health <= 0.0 && !limb.is_severed {
            limb.is_severed = true;
            info!("Limb severed/dismembered!");

            for _ in 0..12 {
                let angle = rand_simple() * std::f32::consts::TAU;
                let speed = 100.0 + rand_simple() * 200.0;
                let vel = Vec2::new(angle.cos() * speed, angle.sin() * speed);

                commands.spawn((
                    SpriteBundle {
                        sprite: Sprite {
                            color: Color::rgb(0.7, 0.0, 0.0),
                            custom_size: Some(Vec2::new(4.0, 4.0)),
                            ..default()
                        },
                        transform: Transform::from_translation(transform.translation),
                        ..default()
                    },
                    BloodParticle {
                        lifetime: 2.0 + rand_simple() * 1.5,
                        velocity: vel,
                    },
                ));
            }

            commands.entity(entity).insert(Wound {
                bleeding_rate: 1.0,
                severity: 0.8,
            });
        }
    }
}

fn rand_simple() -> f32 {
    0.5
}
