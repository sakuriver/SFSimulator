using GGjNantonaku.SetUp;
using GGjNantonaku.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SFSimulatorLibrary;
using System;
using System.Collections.Generic;
using GameSceneName = SimulatorLibrary.GameSceneBase.GameSceneName;

namespace GGjNantonaku
{
    public class Game1 : Game
    {
        /// <summary>
        /// Contents Display Object Context
        /// </summary>
        private GraphicsDeviceManager _graphics;

        /// <summary>
        /// Game Display Font Data
        /// </summary>
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// Game Title Texture
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// Acrtion Game Player Character Texture
        /// </summary>
        private Texture2D characterTexture;

        /// <summary>
        /// Action Game Enemy Texture
        /// </summary>
        private Texture2D enemyTexture;

        /// <summary>
        /// OutGame Header Info Parameter Object
        /// </summary>
        private AppOutParam outParam;

        /// <summary>
        /// Player Action Bullets List
        /// </summary>
        private List<PlayerActionData> playerBulletDatas = new List<PlayerActionData>();

        /// <summary>
        /// Animation Counter
        /// </summary>
        private int animCounter = 0;

        /// <summary>
        /// Contents Action Scnene EnemyAppearFrame
        /// </summary>
        private int EnemyAppearFrameNum = 100;

        /// <summary>
        /// Player Action Scene Enemy Appear Counter
        /// </summary>
        private int enemyAppearFrameCount = 0;

        /// <summary>
        /// game header display score value
        /// example enemy action calculate bonus enemy complete 
        /// </summary>
        private int gameScore;

        /// <summary>
        /// Contents Playing Now Scene.
        /// </summary>
        private GameSceneName gameNowScene;

        /// <summary>
        /// GamePlayerActionData.
        /// </summary>
        private PlayerActionData playerData;

        /// <summary>
        /// Enermy Action Data.
        /// </summary>
        private List<PlayerActionData> enemyShipDatas = new List<PlayerActionData>();

        /// <summary>
        /// Game Title Scene Play BackGroundMusic.
        /// </summary>
        private Song titleBgmSound;

        /// <summary>
        /// Game Action Scene BackGroundMusic.
        /// </summary>
        private Song mainBgmSound;

        /// <summary>
        /// Game Ending Scene BackGroundMusic.
        /// </summary>
        private Song endingBgmSound;

        /// <summary>
        /// KeyBoard Decide Touch SoundEffect
        /// </summary>
        private SoundEffect decideSoundEffect;

        /// <summary>
        /// Enemy Bom Sound Effect
        /// </summary>
        private SoundEffect bomSoundEffect;

        /// <summary>
        /// Game Clear Sound Effect
        /// </summary>
        private SoundEffect clearEffect;

        /// <summary>
        /// Game Setup Construct
        /// </summary>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = AppDefine.HardWareHeight;
            _graphics.PreferredBackBufferWidth = AppDefine.HardWareWidth;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            gameNowScene = GameSceneName.Title;
            var libPos = new System.Numerics.Vector2(100.0f, 150.0f);
            playerData = new PlayerActionData(libPos, 3);
        }


        /// <summary>
        /// Game Initialize
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// Contents Asset Load Functioin
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            texture = Content.Load<Texture2D>("tp_title_ggj");
            characterTexture = Content.Load<Texture2D>("tp_player_ship");
            enemyTexture = Content.Load<Texture2D>("tp_enemy_ship");
            outParam = new AppOutParam(Content.Load<SpriteFont>("File"));
            titleBgmSound = Content.Load<Song>(".\\maou_bgm_8bit29");
            mainBgmSound = Content.Load<Song>(".\\maou_bgm_8bit27");
            endingBgmSound = Content.Load<Song>(".\\maou_bgm_healing13");
            decideSoundEffect = Content.Load<SoundEffect>(".\\Sound\\maou_se_system49");
            bomSoundEffect = Content.Load<SoundEffect>(".\\Sound\\maou_se_battle18");
            clearEffect = Content.Load<SoundEffect>(".\\Sound\\maou_se_magical25");
            base.LoadContent();

            // MediaPlayer is Title BackGroundMusic Play
            MediaPlayer.IsRepeating = true;

            MediaPlayer.Play(titleBgmSound);
        }

        /// <summary>
        /// Game MainLoop Update Function
        /// </summary>
        /// <param name="gameTime">UpdateTimeInformation</param>
        protected override void Update(GameTime gameTime)
        {
            // アプリの終了判定確認を流す
            gameExit();
            if (isGameTitleUpdate())
            {
                gameTitleUpdate();
            }
            gameMainUpdate();
            gameEndingModeUpdate();
            base.Update(gameTime);
        }

        /// <summary>
        /// TitleScneMove ConditionCheck
        /// </summary>
        /// <returns>isGameTitleUpdate is True</returns>
        private bool isGameTitleUpdate()
        {
            return (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter)) &&
                gameNowScene == GameSceneName.Title;
        }

        /// <summary>
        /// Application Exit Check Function
        /// </summary>
        private void gameExit()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
        }

        /// <summary>
        /// Game Main Update Function
        /// </summary>
        private void gameMainUpdate()
        {
            if (this.gameNowScene == GameSceneName.Main)
            {
                this.GameMainUpdate();
                this.UpdateFieldBattle();
            }
        }

        /// <summary>
        /// Game Ending Scene Update Function
        /// </summary>
        private void gameEndingModeUpdate()
        {
            if (this.gameNowScene == GameSceneName.Ending)
            {
                this.GameEndingUpdate();
            }
        }

        /// <summary>
        /// Game Title Scene Update Function
        /// </summary>
        private void gameTitleUpdate()
        {
            gameScore = 0;
            animCounter = 0;
            MediaPlayer.Play(mainBgmSound);
            gameNowScene = GameSceneName.Main;
            playerData.resetInitial();
            decideSoundEffect.Play();
        }

        /// <summary>
        /// Display Draw Function
        /// </summary>
        /// <param name="gameTime">GameTimeInformation</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            switch (gameNowScene)
            {
                case GameSceneName.Title:
                    this.GameTitleDraw();
                    break;
                case GameSceneName.Main:
                    this.GameMainDraw();
                    break;
                case GameSceneName.Ending:
                    this.GameEndingDraw();
                    break;
            }
            base.Draw(gameTime);
        }

        /// <summary>
        /// Game Scene Title Draw
        /// </summary>
        private void GameTitleDraw()
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(texture, new Rectangle(0, 0, AppDefine.HardWareHeight, AppDefine.HardWareWidth), Color.White);
            _spriteBatch.End();
        }

        /// <summary>
        /// Ending Scene Display Draw Function
        /// </summary>
        private void GameEndingDraw()
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(outParam.GetSpriteFont(), "GameClear", new Vector2(0, AppOutParam.OutPutUiHeaderYpos), Color.White);
            _spriteBatch.DrawString(outParam.GetSpriteFont(), "Score", new Vector2(0, AppOutParam.OutPutUiHeaderYpos), Color.White);
            _spriteBatch.DrawString(outParam.GetSpriteFont(), "Press Z Button To Title", new Vector2(0, AppOutParam.OutPutUiHeaderYpos + 20), Color.White);
            _spriteBatch.Draw(characterTexture, playerData.GetPosition(), Color.White);
            _spriteBatch.End();
        }

        /// <summary>
        /// Game Main Draw
        /// </summary>
        private void GameMainDraw()
        {
            _spriteBatch.Begin();


            // 描画専用処理が増えたらコメント解除して実装
            this.MyCharacterDraw();
            this.EnemyDraw();

            // 画面の一番奥に表示されるUIを設定
            this.UIHeaderDraw();
            _spriteBatch.End();
        }

        /// <summary>
        /// Game Main Scene Update Function
        /// KeyBoard Input Check And Action
        /// </summary>
        private void GameMainUpdate()
        {
            playerMoveCharacter();

            // チャタリングのバランス調整
            if (animCounter > 0)
            {
                animCounter--;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && animCounter == 0)
            {
                playerBulletShot();
            }
            playerWeaponMove();

        }

        /// <summary>
        /// Player Move Character
        /// </summary>
        private void playerMoveCharacter()
        {
            if (gameNowScene != GameSceneName.Main)
            {
                return;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                playerData.AddPosition(0, AppDefine.PlayerPositionSubValue);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                playerData.AddPosition(0, AppDefine.PlayerPositionAddValue);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                playerData.AddPosition(AppDefine.PlayerPositionSubValue, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                playerData.AddPosition(AppDefine.PlayerPositionAddValue, 0);
            }

        }

        /// <summary>
        /// Action Scene Player Game Character Shoot
        /// </summary>
        private void playerBulletShot()
        {
            decideSoundEffect.Play(0.1f, 0, 0);
            playerBulletDatas.Add(new PlayerActionData(playerData.GetPosition(), 1));
            animCounter = 10;
        }

        /// <summary>
        /// Player Weapon Use After Action Function
        /// </summary>
        private void playerWeaponMove()
        {
            for (int i = 0; i < playerBulletDatas.Count; i++)
            {
                playerBulletDatas[i].AddPosition(AppDefine.PlayerBulletSpeed, 0);

                if (playerBulletDatas[i].GetPosition().X > AppDefine.BorderPosition)
                {
                    playerBulletDatas.Remove(playerBulletDatas[i]);
                }
            }
        }

        /// <summary>
        /// Game Ending Scene Update
        /// </summary>
        private void GameEndingUpdate()
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                MediaPlayer.Play(titleBgmSound);
                decideSoundEffect.Play();
                gameNowScene = GameSceneName.Title;
            }
        }


        /// <summary>
        /// Field Information Update Function
        /// </summary>
        private void UpdateFieldBattle()
        {

            // 敵出現処理を実装
            enemyFieldAppear();

            // 敵キャラ関係
            for (int i = 0; i < enemyShipDatas.Count; i++)
            {
                if (isHit(playerData.GetPosition(), characterTexture,
                    enemyShipDatas[i].GetPosition(), enemyTexture))
                {
                    enemyShipDatas[i].SubDurability(1);
                    if (!enemyShipDatas[i].IsRunning())
                    {
                        enemyShipDatas.Remove(enemyShipDatas[i]);
                        bomSoundEffect.Play(AppDefine.EnemySoundVolume, 0, 0);
                    }
                    playerData.SubDurability(1);
                    if (!playerData.IsRunning())
                    {
                        gameNowScene = GameSceneName.Title;
                        MediaPlayer.Play(titleBgmSound);
                    }
                }
            }

            // エンディング確認処理の呼び出し
            updateCheckGameEnding();

        }

        /// <summary>
        /// Update Game Scene Ending Timer Check
        /// </summary>
        private void updateCheckGameEnding()
        {
            // エンディングシーンへの条件を設定
            if (gameScore > getGameClearBorderScore())
            {
                clearEffect.Play();
                gameNowScene = GameSceneName.Ending;
                MediaPlayer.Play(endingBgmSound);
            }

        }

        /// <summary>
        /// Getter Ending Scene Border Score Line
        /// </summary>
        /// <returns>Clear Ending Score</returns>
        private int getGameClearBorderScore()
        {
            return AppDefine.GameClearScore - 1;
        }

        /// <summary>
        /// Action Scene Enemy Appear
        /// </summary>
        private void enemyFieldAppear()
        {
            Random random = new Random();
            MoveEnemy(random);

            // 出現処理
            if (enemyAppearFrameCount >= EnemyAppearFrameNum)
            {
                int yValue = random.Next(AppDefine.HardWareHeight);
                var enemyShipPos = new System.Numerics.Vector2(AppDefine.EnemyAppearYPos, yValue);
                enemyShipDatas.Add(new PlayerActionData(enemyShipPos, 1));
                enemyAppearFrameCount = 0;
            }

            enemyAppearFrameCount++;
        }

        /// <summary>
        /// Application Object Rect Hit Function
        /// </summary>
        /// <param name="basePosition">基本座標関連</param>
        /// <param name="baseTexture">当たり判定の範囲で利用する画像情報</param>
        /// <param name="targetPosition">基本座標関連</param>
        /// <param name="targetTexture">当たり判定の範囲で利用する画像情報</param>
        /// <returns>is target and base hit true</returns>
        private bool isHit(Vector2 basePosition, Texture2D baseTexture, Vector2 targetPosition, Texture2D targetTexture)
        {
            return (basePosition.X <= targetPosition.X &&
                targetPosition.X <= basePosition.X + baseTexture.Width &&
                basePosition.Y <= targetPosition.Y &&
                targetPosition.Y <= basePosition.Y + baseTexture.Width);
        }

        /// <summary>
        /// Action Scene Enemy Move
        /// </summary>
        /// <param name="random">移動速度算出用オブジェクト</param>
        private void MoveEnemy(Random random)
        {
            // 敵キャラ関係
            for (int i = 0; i < enemyShipDatas.Count; i++)
            {
                // 敵を移動する
                int yAddValue = calculateEnemyMoveSpeed(random);
                int ySubValue = calculateEnemyMoveSpeed(random);
                enemyShipDatas[i].AddPosition(0, yAddValue - ySubValue);
                // 移動後に弾に接触しているかを確認する
                for (int j = 0; j < playerBulletDatas.Count; j++)
                {
                    if (isHit(playerBulletDatas[j].GetPosition(), characterTexture,
                        enemyShipDatas[i].GetPosition(), enemyTexture))
                    {
                        enemyShipDatas[i].SubDurability(1);
                        playerBulletDatas[j].SubDurability(1);
                        if (!enemyShipDatas[i].IsRunning())
                        {
                            enemyShipDatas.Remove(enemyShipDatas[i]);
                            bomSoundEffect.Play(AppDefine.EnemySoundVolume, 0, 0);
                        }
                        gameScore += 10;
                        // プレイヤー側の方がなくなった場合は、ループから抜ける
                        if (!playerBulletDatas[j].IsRunning())
                        {
                            playerBulletDatas.Remove(playerBulletDatas[j]);
                            break;
                        }

                    }
                }
            }

        }

        /// <summary>
        /// 敵の移動速度を算出
        /// </summary>
        /// <param name="random">移動速度で利用する乱数生成器</param>
        /// <returns>乱数移動速度結果</returns>
        private int calculateEnemyMoveSpeed(Random random)
        {
            return random.Next(AppDefine.EnemyMaxSpeedNum);
        }

        /// <summary>
        /// Player Character Draw Action Function
        /// </summary>
        private void MyCharacterDraw()
        {
            // 自キャラ関係の描画
            _spriteBatch.Draw(characterTexture, playerData.GetPosition(), Color.White);

            for (int i = 0; i < playerBulletDatas.Count; i++)
            {
                _spriteBatch.Draw(characterTexture, playerBulletDatas[i].GetPosition(), Color.White);
            }

        }

        /// <summary>
        /// PlayerAction Scene For Enemy Object Draw
        /// </summary>
        private void EnemyDraw()
        {
            // 敵キャラ関係
            for (int i = 0; i < enemyShipDatas.Count; i++)
            {
                _spriteBatch.Draw(enemyTexture, enemyShipDatas[i].GetPosition(), Color.White);
            }
        }


        /// <summary>
        /// UI Header Display Draw Scene
        /// </summary>
        private void UIHeaderDraw()
        {
            string scoreStr = $"score {gameScore} ";
            string lifeStr = $"life {playerData.GetDurability()}";
            _spriteBatch.DrawString(outParam.GetSpriteFont(), "stage 1", new Vector2(0, AppOutParam.OutPutUiHeaderYpos), Color.White);
            _spriteBatch.DrawString(outParam.GetSpriteFont(), scoreStr, new Vector2(100, AppOutParam.OutPutUiHeaderYpos), Color.White);
            _spriteBatch.DrawString(outParam.GetSpriteFont(), lifeStr, new Vector2(180, AppOutParam.OutPutUiHeaderYpos), Color.White);
        }

    }
}