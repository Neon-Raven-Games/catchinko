Setup:

1. drag & drop the Quantity prefab (Minimalist/Quantity System/Prefabs) onto your scene. A Minimalist System Manager game object will be instantiated for you (if one doesn't exist already);

2. drag & drop the Bar prefab (Minimalist/Bar System/Prefabs) onto your scene. A parent canvas game object will be automatically created for you;

3. assign the created Quantity to the created Bar's "Subscription" field (in the BarBhv component);

4. customize the BarBhv component to your liking (change colors, animation parameters, etc.);

5. assign a transform to the "Anchor" field of the QuantitySubscriberCanvasBhv component of the canvas object;

6. change the "Render Mode" field of the Canvas component in the canvas parent object to switch between world, screen & camera spaces.