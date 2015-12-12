using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD34
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public const int BYTES_PER_PIXEL = 4;

        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        private IRenderer[] _renderers;
        private IUpdater[] _updaters;
        private IGesturer[] _gesturers;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferHeight = 800,
                PreferredBackBufferWidth = 500
            };
            IsMouseVisible = true;

            Window.Position = new Point(Window.Position.X, 100);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            var beatTrigger = new BeatTrigger(1000);

            var frameRateCounter = new FrameRateCounter();
            var mouseGesturer = new MouseGesturer();
            var clickEffects = new GestureEffects();
            var levelManager = new LevelManager(beatTrigger);
            var keyboardGesturer = new KeyboardGesturer();
            var menuManager = new MenuManager();

            // order ==> layer. last is on top.
            _gesturers = new IGesturer[]
            {
                mouseGesturer,
                keyboardGesturer
            };

            _updaters = new IUpdater[]
            {
                beatTrigger,
                menuManager,
                levelManager,
                clickEffects
            };

            _renderers = new IRenderer[]
            {
                levelManager,
                menuManager,
                clickEffects,
                frameRateCounter,
            };

            var gameComponents = _gesturers
                .Cast<IGameComponent>()
                .Concat(_updaters)
                .Concat(_renderers)
                .Distinct();
            foreach (var gameComponent in gameComponents)
            {
                gameComponent.Initialise();
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (var renderer in _renderers)
            {
                renderer.LoadContent(Content, GraphicsDevice, GraphicsDevice.Viewport.Bounds);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var completedGestures = _gesturers.SelectMany(g => g.DetectGestures().Where(e => e != null)).ToList();

            foreach (var updater in _updaters)
            {
                updater.Update(gameTime, completedGestures);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(30, 30, 30));

            foreach (var renderer in _renderers)
            {
                renderer.Render(_spriteBatch, gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
