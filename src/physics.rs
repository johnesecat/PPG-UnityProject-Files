use bevy::prelude::*;

pub struct PhysicsPlugin;

impl Plugin for PhysicsPlugin {
    fn build(&self, app: &mut App) {
        app.insert_resource(PhysicsWorldSettings::default())
            .add_systems(
                Update,
                (
                    integrate_bodies,
                    resolve_box_collisions,
                    solve_joints,
                )
                    .chain()
                    .run_if(in_state(crate::menu::AppState::InGame)),
            );
    }
}

#[derive(Resource)]
pub struct PhysicsWorldSettings {
    pub gravity: Vec2,
    pub sub_steps: usize,
}

impl Default for PhysicsWorldSettings {
    fn default() -> Self {
        Self {
            gravity: Vec2::new(0.0, -980.0),
            sub_steps: 8,
        }
    }
}

#[derive(Component, Debug, Clone)]
pub struct RigidBody2D {
    pub velocity: Vec2,
    pub angular_velocity: f32,
    pub mass: f32,
    pub inertia: f32,
    pub restitution: f32,
    pub friction: f32,
    pub is_static: bool,
    pub size: Vec2,
}

impl Default for RigidBody2D {
    fn default() -> Self {
        Self {
            velocity: Vec2::ZERO,
            angular_velocity: 0.0,
            mass: 1.0,
            inertia: 100.0,
            restitution: 0.2,
            friction: 0.5,
            is_static: false,
            size: Vec2::new(30.0, 30.0),
        }
    }
}

#[derive(Component, Debug, Clone)]
pub struct DistanceJoint2D {
    pub entity_a: Entity,
    pub entity_b: Entity,
    pub local_anchor_a: Vec2,
    pub local_anchor_b: Vec2,
    pub target_distance: f32,
    pub stiffness: f32,
    pub damping: f32,
    pub break_force: f32,
}

fn integrate_bodies(
    time: Res<Time>,
    settings: Res<PhysicsWorldSettings>,
    mut query: Query<(&mut Transform, &mut RigidBody2D)>,
) {
    let dt = time.delta_seconds();
    if dt <= 0.0 {
        return;
    }

    let sub_dt = dt / settings.sub_steps as f32;

    for _ in 0..settings.sub_steps {
        for (mut transform, mut body) in &mut query {
            if body.is_static {
                continue;
            }

            body.velocity += settings.gravity * sub_dt;
            transform.translation.x += body.velocity.x * sub_dt;
            transform.translation.y += body.velocity.y * sub_dt;
            transform.rotate_z(body.angular_velocity * sub_dt);
        }
    }
}

fn resolve_box_collisions(
    mut query: Query<(Entity, &mut Transform, &mut RigidBody2D)>,
) {
    let mut combinations = query.iter_combinations_mut();

    while let Some([(_e1, mut t1, mut b1), (_e2, mut t2, mut b2)]) = combinations.fetch_next() {
        if b1.is_static && b2.is_static {
            continue;
        }

        let pos1 = t1.translation.truncate();
        let pos2 = t2.translation.truncate();

        let half_s1 = b1.size * 0.5;
        let half_s2 = b2.size * 0.5;

        let delta = pos2 - pos1;
        let overlap_x = half_s1.x + half_s2.x - delta.x.abs();
        let overlap_y = half_s1.y + half_s2.y - delta.y.abs();

        if overlap_x > 0.0 && overlap_y > 0.0 {
            let (normal, penetration) = if overlap_x < overlap_y {
                let normal = if delta.x < 0.0 { Vec2::new(-1.0, 0.0) } else { Vec2::new(1.0, 0.0) };
                (normal, overlap_x)
            } else {
                let normal = if delta.y < 0.0 { Vec2::new(0.0, -1.0) } else { Vec2::new(0.0, 1.0) };
                (normal, overlap_y)
            };

            let total_mass = if b1.is_static { b2.mass } else if b2.is_static { b1.mass } else { b1.mass + b2.mass };
            if total_mass > 0.0 {
                if !b1.is_static {
                    let ratio = if b2.is_static { 1.0 } else { b2.mass / total_mass };
                    t1.translation.x -= normal.x * penetration * ratio;
                    t1.translation.y -= normal.y * penetration * ratio;
                }
                if !b2.is_static {
                    let ratio = if b1.is_static { 1.0 } else { b1.mass / total_mass };
                    t2.translation.x += normal.x * penetration * ratio;
                    t2.translation.y += normal.y * penetration * ratio;
                }
            }

            let relative_velocity = b2.velocity - b1.velocity;
            let vel_along_normal = relative_velocity.dot(normal);

            if vel_along_normal < 0.0 {
                let restitution = b1.restitution.min(b2.restitution);
                let impulse_scalar = -(1.0 + restitution) * vel_along_normal;
                let inv_mass1 = if b1.is_static { 0.0 } else { 1.0 / b1.mass };
                let inv_mass2 = if b2.is_static { 0.0 } else { 1.0 / b2.mass };
                let impulse = impulse_scalar / (inv_mass1 + inv_mass2);

                let impulse_vec = normal * impulse;

                if !b1.is_static {
                    b1.velocity -= impulse_vec * inv_mass1;
                }
                if !b2.is_static {
                    b2.velocity += impulse_vec * inv_mass2;
                }
            }
        }
    }
}

fn solve_joints(
    mut commands: Commands,
    joint_query: Query<(Entity, &DistanceJoint2D)>,
    mut body_query: Query<(&mut Transform, &mut RigidBody2D)>,
) {
    for (joint_entity, joint) in &joint_query {
        if let Ok([(mut t1, mut b1), (mut t2, mut b2)]) = body_query.get_many_mut([joint.entity_a, joint.entity_b]) {
            let world_anchor_a = t1.transform_point(joint.local_anchor_a.extend(0.0)).truncate();
            let world_anchor_b = t2.transform_point(joint.local_anchor_b.extend(0.0)).truncate();

            let delta = world_anchor_b - world_anchor_a;
            let current_dist = delta.length();

            if current_dist == 0.0 {
                continue;
            }

            let error = current_dist - joint.target_distance;

            if error.abs() * joint.stiffness > joint.break_force {
                commands.entity(joint_entity).despawn();
                info!("Joint broke due to force!");
                continue;
            }

            let dir = delta / current_dist;
            let correction = dir * error * joint.stiffness * 0.5;

            if !b1.is_static {
                t1.translation.x += correction.x;
                t1.translation.y += correction.y;
                b1.velocity += correction * 10.0;
            }
            if !b2.is_static {
                t2.translation.x -= correction.x;
                t2.translation.y -= correction.y;
                b2.velocity -= correction * 10.0;
            }
        }
    }
}
