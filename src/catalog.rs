use bevy::prelude::*;

#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub enum ItemCategory {
    Entities,
    Melee,
    Firearms,
    Explosives,
    Syringes,
    Machinery,
    Vehicles,
    Tools,
}

#[derive(Clone, Debug)]
pub struct ItemDefinition {
    pub id: &'static str,
    pub name: &'static str,
    pub category: ItemCategory,
    pub description: &'static str,
    pub sprite_path: &'static str,
    pub size: Vec2,
    pub mass: f32,
}

pub fn get_full_item_catalog() -> Vec<ItemDefinition> {
    vec![
        // Entities
        ItemDefinition { id: "human", name: "Human", category: ItemCategory::Entities, description: "Standard human ragdoll.", sprite_path: "textures/Human.png", size: Vec2::new(25.0, 60.0), mass: 70.0 },
        ItemDefinition { id: "android", name: "Android", category: ItemCategory::Entities, description: "Robotic android ragdoll.", sprite_path: "textures/Android.png", size: Vec2::new(25.0, 60.0), mass: 90.0 },
        ItemDefinition { id: "gorse", name: "Gorse", category: ItemCategory::Entities, description: "Acidic extraterrestrial creature.", sprite_path: "textures/Gorse.png", size: Vec2::new(40.0, 40.0), mass: 50.0 },

        // Melee
        ItemDefinition { id: "sword", name: "Sword", category: ItemCategory::Melee, description: "Sharp metal blade.", sprite_path: "textures/Sword.png", size: Vec2::new(10.0, 50.0), mass: 3.0 },
        ItemDefinition { id: "dagger", name: "Dagger", category: ItemCategory::Melee, description: "Small stabbing knife.", sprite_path: "textures/Dagger.png", size: Vec2::new(8.0, 25.0), mass: 1.0 },
        ItemDefinition { id: "spear", name: "Spear", category: ItemCategory::Melee, description: "Long wooden pole with sharp tip.", sprite_path: "textures/Spear.png", size: Vec2::new(6.0, 80.0), mass: 4.0 },
        ItemDefinition { id: "axe", name: "Axe", category: ItemCategory::Melee, description: "Heavy chopping axe.", sprite_path: "textures/Axe.png", size: Vec2::new(20.0, 45.0), mass: 5.0 },
        ItemDefinition { id: "mace", name: "Mace", category: ItemCategory::Melee, description: "Heavy spiked bludgeon.", sprite_path: "textures/Mace.png", size: Vec2::new(18.0, 45.0), mass: 6.0 },

        // Firearms
        ItemDefinition { id: "pistol", name: "Pistol", category: ItemCategory::Firearms, description: "Semi-automatic handgun.", sprite_path: "textures/Pistol.png", size: Vec2::new(20.0, 12.0), mass: 1.2 },
        ItemDefinition { id: "revolver", name: "Revolver", category: ItemCategory::Firearms, description: "Six-shot revolver.", sprite_path: "textures/Revolver.png", size: Vec2::new(22.0, 14.0), mass: 1.5 },
        ItemDefinition { id: "ak47", name: "AK-47", category: ItemCategory::Firearms, description: "Assault rifle.", sprite_path: "textures/AK-47.png", size: Vec2::new(50.0, 18.0), mass: 4.0 },
        ItemDefinition { id: "shotgun", name: "Shotgun", category: ItemCategory::Firearms, description: "Pump-action shotgun.", sprite_path: "textures/Shotgun.png", size: Vec2::new(55.0, 16.0), mass: 3.5 },
        ItemDefinition { id: "sniper", name: "Sniper Rifle", category: ItemCategory::Firearms, description: "High caliber sniper rifle.", sprite_path: "textures/Sniper Rifle.png", size: Vec2::new(70.0, 18.0), mass: 7.0 },
        ItemDefinition { id: "rocket_launcher", name: "Rocket Launcher", category: ItemCategory::Firearms, description: "Fires explosive rockets.", sprite_path: "textures/Rocket Launcher.png", size: Vec2::new(65.0, 22.0), mass: 8.0 },

        // Explosives
        ItemDefinition { id: "grenade", name: "Grenade", category: ItemCategory::Explosives, description: "Fragmentation grenade.", sprite_path: "textures/Grenade.png", size: Vec2::new(12.0, 16.0), mass: 0.5 },
        ItemDefinition { id: "c4", name: "C4", category: ItemCategory::Explosives, description: "Plastic explosive block.", sprite_path: "textures/C4.png", size: Vec2::new(20.0, 15.0), mass: 1.0 },
        ItemDefinition { id: "dynamite", name: "Dynamite", category: ItemCategory::Explosives, description: "High explosive dynamite stick.", sprite_path: "textures/Dynamite.png", size: Vec2::new(8.0, 30.0), mass: 0.8 },
        ItemDefinition { id: "landmine", name: "Landmine", category: ItemCategory::Explosives, description: "Pressure-triggered landmine.", sprite_path: "textures/Landmine.png", size: Vec2::new(30.0, 10.0), mass: 2.5 },
        ItemDefinition { id: "atom_bomb", name: "Nuke", category: ItemCategory::Explosives, description: "Massive tactical warhead.", sprite_path: "textures/Nuke.png", size: Vec2::new(60.0, 120.0), mass: 500.0 },

        // Syringes
        ItemDefinition { id: "life_syringe", name: "Life Syringe", category: ItemCategory::Syringes, description: "Heals wounds and revives.", sprite_path: "textures/Life Syringe.png", size: Vec2::new(8.0, 25.0), mass: 0.2 },
        ItemDefinition { id: "acid_syringe", name: "Acid Syringe", category: ItemCategory::Syringes, description: "Dissolves organic tissues.", sprite_path: "textures/Acid Syringe.png", size: Vec2::new(8.0, 25.0), mass: 0.2 },
        ItemDefinition { id: "death_syringe", name: "Death Syringe", category: ItemCategory::Syringes, description: "Instant lethal poison.", sprite_path: "textures/Death Syringe.png", size: Vec2::new(8.0, 25.0), mass: 0.2 },
        ItemDefinition { id: "freeze_syringe", name: "Freeze Syringe", category: ItemCategory::Syringes, description: "Freezes flesh instantly.", sprite_path: "textures/Freeze Syringe.png", size: Vec2::new(8.0, 25.0), mass: 0.2 },

        // Machinery
        ItemDefinition { id: "generator", name: "Generator", category: ItemCategory::Machinery, description: "Produces high electrical current.", sprite_path: "textures/Generator.png", size: Vec2::new(40.0, 40.0), mass: 30.0 },
        ItemDefinition { id: "laser", name: "Laser Grid", category: ItemCategory::Machinery, description: "Emits burning laser beam.", sprite_path: "textures/Laser.png", size: Vec2::new(30.0, 30.0), mass: 15.0 },
        ItemDefinition { id: "rotor", name: "Motor Rotor", category: ItemCategory::Machinery, description: "Powered rotating joint motor.", sprite_path: "textures/Rotor.png", size: Vec2::new(35.0, 35.0), mass: 20.0 },
        ItemDefinition { id: "piston", name: "Piston", category: ItemCategory::Machinery, description: "Extending hydraulic shaft.", sprite_path: "textures/Piston.png", size: Vec2::new(20.0, 60.0), mass: 25.0 },

        // Vehicles
        ItemDefinition { id: "car", name: "Car", category: ItemCategory::Vehicles, description: "Drivable four-wheeled automobile.", sprite_path: "textures/Car.png", size: Vec2::new(140.0, 60.0), mass: 800.0 },
        ItemDefinition { id: "truck", name: "Truck", category: ItemCategory::Vehicles, description: "Heavy industrial transport truck.", sprite_path: "textures/Truck.png", size: Vec2::new(200.0, 90.0), mass: 2000.0 },

        // Tools
        ItemDefinition { id: "blood_wire", name: "Blood Wire", category: ItemCategory::Tools, description: "Transfers fluids between entities.", sprite_path: "textures/Blood Wire.png", size: Vec2::new(15.0, 15.0), mass: 0.1 },
        ItemDefinition { id: "rigid_wire", name: "Rigid Cable", category: ItemCategory::Tools, description: "Connects objects rigidly.", sprite_path: "textures/Rigid Cable.png", size: Vec2::new(15.0, 15.0), mass: 0.1 },
    ]
}
