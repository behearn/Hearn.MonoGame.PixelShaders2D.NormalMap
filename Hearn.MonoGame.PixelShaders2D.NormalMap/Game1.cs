using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hearn.MonoGame.PixelShaders2D.NormalMap
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D _stone;
        Texture2D _stoneNormalMap;
        Effect _normalMapEffect;
        float _lightZ = 0.075f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 1024;
            graphics.ApplyChanges();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //https://3dtextures.me/2017/03/23/stone-wall-004/
            _stone = Content.Load<Texture2D>("Wall Stone 004_COLOR");
            _stoneNormalMap = Content.Load<Texture2D>("Wall Stone 004_NRM");           

            _normalMapEffect = Content.Load<Effect>("NormalMap");            
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            var mouseState = Mouse.GetState();

            var x = mouseState.X / (float)_stone.Width;
            var y = mouseState.Y / (float)_stone.Height;

            var lightPos = new Vector3(x, y, _lightZ);

            _normalMapEffect.Parameters["NormalMapTexture"].SetValue(_stoneNormalMap);
            _normalMapEffect.Parameters["LightPos"].SetValue(lightPos);
            _normalMapEffect.Parameters["LightColor"].SetValue(new Vector4(1f, 0.8f, 0.6f, 1f));
            _normalMapEffect.Parameters["AmbientColor"].SetValue(new Vector4(0.6f, 0.6f, 1f, 0.8f));
            _normalMapEffect.Parameters["Falloff"].SetValue(new Vector3(0.4f, 3f, 20f));
            _normalMapEffect.Parameters["Resolution"].SetValue(new Vector2(_stone.Width, _stone.Height));
            _normalMapEffect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(_stone, new Vector2(0, 0), Color.White);

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
