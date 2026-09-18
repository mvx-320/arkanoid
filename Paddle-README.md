# About Paddle in scene
## Justification on using ...
### `StaticBody2D`
- It provides solid collision detection, while not be influenced by other forces we add later.
### Wrong
### `CharacterBody2D`
- Static is though if it doesn't get moved at all. We want to decide how the Body behaves with a script by user input, therefor use `CharacterBody2D`

### `CollisionShape2D`
- The capsule form is simple and closely resembles the shape of the paddle
- I made the collision shape a little wider than the image so the frustration level decreases.

---
## Problems with the paddle
- A extremely fast Ball could glitch trough the paddle
- Should the paddle stop at the corners or reappear on the other side. Either way, how should it be implemented?
- How should the bounce angle be matched to the original?
## How should the paddle move
- There should be an event xPosition that gets `+=` or `-=` while the right or left button is pressed. The event should then update `position.x`. What is added or subtracted is `speed * delta * direction`. 
## Power Ups
- Spawn more balls
- Wider paddle
- Inverse (Paddle fills the whole lower edge besides where the paddle was)(Could maybe spawn close to "Wider paddle" so getting both power ups is a disadvantage)
- Shoot balls out of the platfrom
- Pinball mode (lower edge is replaced with the pinball paddles)
- Ball bursts straight trough the bricks
- Slower ball
### Chaos Ups
- Button directions are inversed
- Faster ball (Maybe the paddle is a trampolin that speed up the ball at every touch)
- Paddle moves in a curve line (sin or just a bow)
