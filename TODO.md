# Todo

## Planned


## Pending

- [ ] Dancer's "Curing Waltz" (#16015), and PvP ver (#29429): 
  Not showing effect range for additional effect (AoE heal around partner)
    - `ReceiveActionEffect` is triggered only once on action use;
      cannot know whether player has a partner without checking status effects; 
    - Also cannot know the partner's position directly

- [ ] Ninja's "Hollow Nozuchi" (#25776): 
  Not showing effect range on Doton area
    - `ReceiveActionEffect` is triggered for #25776 but cannot get position information 
      as it is not implemented as "pet".

- [ ] Reaper's "Arcane Crest" (#24404):
  Not showing effect range when the barrier effect is triggered
    - `ReceiveActionEffect` is not triggered for the barrier effect

- [ ] Configuration option to draw (or not draw) for auto-triggered action effects
