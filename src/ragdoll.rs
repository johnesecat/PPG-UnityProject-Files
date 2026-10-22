use bevy::prelude::*;
use crate::physics::{RigidBody2D, DistanceJoint2D};

pub struct RagdollPlugin;

impl Plugin for RagdollPlugin {
    fn build(&self, app: &mut App) {
        app.add_systems(
            Update,
            ragdoll_balance_system.run_if(in_state(crate::menu::AppState::InGame)),
        );
    }
}

#[derive(Component, Debug, Clone)]
pub struct HumanRagdoll {
    pub head: Entity,
    pub torso: Entity,
    pub upper_left_arm: Entity,
    pub lower_left_arm: Entity,
    pub upper_right_arm: Entity,
    pub lower_right_arm: Entity,
    pub upper_left_leg: Entity,
    pub lower_left_leg: Entity,
    pub upper_right_leg: Entity,
    pub lower_right_leg: Entity,
    pub is_alive: bool,
    pub consciousness: f32,
}

#[derive(Component, Debug, Clone)]
pub struct Limb {
    pub limb_type: LimbType,
    pub health: f32,
    pub is_broken: bool,
    pub is_severed: bool,
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum LimbType {
    Head,
    Torso,
    UpperArm,
    LowerArm,
    UpperLeg,
    LowerLeg,
}

pub fn spawn_human_ragdoll(
    commands: &mut Commands,
    asset_server: &Res<AssetServer>,
    spawn_pos: Vec2,
) -> Entity {
    let head_texture = asset_server.load("textures/Head.png");
    let torso_texture = asset_server.load("textures/Torso.png");
    let limb_texture = asset_server.load("textures/Limb.png");

    let torso = commands.spawn((
        SpriteBundle {
            texture: torso_texture,
            sprite: Sprite {
                custom_size: Some(Vec2::new(25.0, 45.0)),
                ..default()
            },
            transform: Transform::from_translation(spawn_pos.extend(1.0)),
            ..default()
        },
        RigidBody2D {
            velocity: Vec2::ZERO,
            angular_velocity: 0.0,
            mass: 20.0,
            inertia: 500.0,
            restitution: 0.1,
            friction: 0.5,
            is_static: false,
            size: Vec2::new(25.0, 45.0),
        },
        Limb {
            limb_type: LimbType::Torso,
            health: 100.0,
            is_broken: false,
            is_severed: false,
        },
    )).id();

    let head = commands.spawn((
        SpriteBundle {
            texture: head_texture,
            sprite: Sprite {
                custom_size: Some(Vec2::new(20.0, 20.0)),
                ..default()
            },
            transform: Transform::from_translation((spawn_pos + Vec2::new(0.0, 35.0)).extend(1.0)),
            ..default()
        },
        RigidBody2D {
            velocity: Vec2::ZERO,
            angular_velocity: 0.0,
            mass: 5.0,
            inertia: 100.0,
            restitution: 0.1,
            friction: 0.5,
            is_static: false,
            size: Vec2::new(20.0, 20.0),
        },
        Limb {
            limb_type: LimbType::Head,
            health: 100.0,
            is_broken: false,
            is_severed: false,
        },
    )).id();

    commands.spawn(DistanceJoint2D {
        entity_a: torso,
        entity_b: head,
        local_anchor_a: Vec2::new(0.0, 22.5),
        local_anchor_b: Vec2::new(0.0, -10.0),
        target_distance: 5.0,
        stiffness: 0.8,
        damping: 0.1,
        break_force: 1500.0,
    });

    let upper_left_leg = commands.spawn((
        SpriteBundle {
            texture: limb_texture,
            sprite: Sprite {
                custom_size: Some(Vec2::new(12.0, 30.0)),
                ..default()
            },
            transform: Transform::from_translation((spawn_pos + Vec2::new(-8.0, -35.0)).extend(1.0)),
            ..default()
        },
        RigidBody2D {
            velocity: Vec2::ZERO,
            angular_velocity: 0.0,
            mass: 8.0,
            inertia: 200.0,
            restitution: 0.1,
            friction: 0.6,
            is_static: false,
            size: Vec2::new(12.0, 30.0),
        },
        Limb {
            limb_type: LimbType::UpperLeg,
            health: 100.0,
            is_broken: false,
            is_severed: false,
        },
    )).id();

    commands.spawn(DistanceJoint2D {
        entity_a: torso,
        entity_b: upper_left_leg,
        local_anchor_a: Vec2::new(-8.0, -22.5),
        local_anchor_b: Vec2::new(0.0, 15.0),
        target_distance: 3.0,
        stiffness: 0.8,
        damping: 0.1,
        break_force: 1200.0,
    });

    commands.spawn(HumanRagdoll {
        head,
        torso,
        upper_left_arm: torso,
        lower_left_arm: torso,
        upper_right_arm: torso,
        lower_right_arm: torso,
        upper_left_leg,
        lower_left_leg: upper_left_leg,
        upper_right_leg: upper_left_leg,
        lower_right_leg: upper_left_leg,
        is_alive: true,
        consciousness: 1.0,
    }).id()
}

fn ragdoll_balance_system(
    mut query: Query<(&HumanRagdoll, &mut Transform)>,
    mut body_query: Query<&mut RigidBody2D>,
) {
    for (ragdoll, _transform) in &mut query {
        if ragdoll.is_alive && ragdoll.consciousness > 0.5 {
            if let Ok(mut torso_body) = body_query.get_mut(ragdoll.torso) {
                torso_body.angular_velocity *= 0.9;
            }
        }
    }
}
