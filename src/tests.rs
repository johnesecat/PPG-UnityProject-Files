#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_catalog_length() {
        let catalog = crate::catalog::get_full_item_catalog();
        assert!(catalog.len() >= 25, "Catalog should contain at least 25 items");
    }

    #[test]
    fn test_ragdoll_human_structure() {
        // Simple sanity check for catalog items
        let catalog = crate::catalog::get_full_item_catalog();
        let human = catalog.iter().find(|i| i.id == "human");
        assert!(human.is_some(), "Human item should exist in catalog");
    }
}
