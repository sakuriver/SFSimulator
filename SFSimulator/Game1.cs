using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SFSimulator.UI;
using SFSimulatorLibrary;
using System;
using System.Collections.Generic;
using GameSceneName = SimulatorLibrary.GameSceneBase.GameSceneName;

namespace SFWorldSimulator
{
    public class Game1 : Game
    {
        /// <summary>
        /// ゲーム内で使う描画コンテキスト
        /// </summary>
        private GraphicsDeviceManager _graphics;

        /// <summary>
        /// ゲームで表示するフォントオブジェクト
        /// </summary>
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// タイトル画像
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// キャラクター画像
        /// </summary>
        private Texture2D characterTexture;

        /// <summary>
        /// 敵画像
        /// </summary>
        private Texture2D enemyTexture;

        /// <summary>
        /// 各種UI関連情報として利用する部分
        /// </summary>
        private AppOutParam outParam;

        /// <summary>
        /// プレイヤーの発射したときの弾情報
        /// </summary>
        private List<PlayerActionData> playerBulletDatas = new List<PlayerActionData>();

        /// <summary>
        /// 
        /// </summary>
        private int animCounter = 0;

        // ゲームとしてビルドするときの画面サイズ

        /// <summary>
        /// 画面全体の高さ
        /// </summary>
        private int hardWareHeight = 224;

        /// <summary>
        /// 画面全体の幅
        /// </summary>
        private int hardWareWidth = 256;

        /// <summary>
        /// 座標の移動速度 
        /// </summary>
        private int playerPositionAddValue = 3;
        private int playerPositionSubValue = -3;

        // 敵が出現するフレーム数
        private int EnemyAppearFrameNum = 100;

        // 敵が１フレームで移動する最大速度
        private int EnemyMaxSpeedNum = 10;

        // 敵出現用フレームカウンター
        private int enemyAppearFrameCount = 0;

        // ゲーム内の上部に表示するスコア情報
        // 敵を倒したりボーナスを計算したいときはこの変数を書き換える
        private int gameScore;

        /// <summary>
        /// ゲームの進行上のシーン情報
        /// </summary>
        private GameSceneName gameNowScene;

        /// <summary>
        /// プレイヤーのアクション用データ
        /// </summary>
        private PlayerActionData playerData;

        /// <summary>
        /// 敵情報
        /// </summary>
        private List<PlayerActionData> enemyShipDatas = new List<PlayerActionData>();


        /// <summary>
        /// 決定効果音
        /// </summary>
        private SoundEffect decideSoundEffect;

        /// <summary>
        /// 撃破時の効果音
        /// </summary>
        private SoundEffect bomSoundEffect;

        /// <summary>
        /// ゲームクリア時の効果音
        /// </summary>
        private SoundEffect clearEffect;

        /// <summary>
        /// 
        /// </summary>
        private int GameClearScore = 300;

        /// <summary>
        /// ゲーム実行時のコンストラクタ
        /// </summary>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = hardWareHeight;
            _graphics.PreferredBackBufferWidth = hardWareWidth;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            gameNowScene = GameSceneName.Title;
            var libPos = new System.Numerics.Vector2(100.0f, 150.0f);
            playerData = new PlayerActionData(libPos, 3);
        }


        /// <summary>
        /// 初期化処理
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// Contentからの各種画像情報を読み込み
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            texture = Content.Load<Texture2D>("tp_title");
            characterTexture = Content.Load<Texture2D>("tp_player_ship");
            enemyTexture = Content.Load<Texture2D>("tp_enemy_ship");
            outParam = new AppOutParam(Content.Load<SpriteFont>("File"));
            decideSoundEffect = Content.Load<SoundEffect>(".\\Sound\\maou_se_system49");
            bomSoundEffect = Content.Load<SoundEffect>(".\\Sound\\maou_se_battle18");
            clearEffect = Content.Load<SoundEffect>(".\\Sound\\maou_se_magical25");
        }

        /// <summary>
        /// ゲームのメインループで使っている時間部分
        /// </summary>
        /// <param name="gameTime">更新時間情報</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (gameNowScene == GameSceneName.Title)
                {
                    gameScore = 0;
                    animCounter = 0;
                    gameNowScene = GameSceneName.Main;
                    decideSoundEffect.Play();
                }
            }
            if (this.gameNowScene == GameSceneName.Main) {
                this.GameMainUpdate();
                this.UpdateFieldBattle();
            }

            if (this.gameNowScene == GameSceneName.Ending) {
                this.GameEndingUpdate();
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// 描画処理
        /// </summary>
        /// <param name="gameTime">更新時間情報</param>
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
        /// タイトル画面での表示周りを編集する関数
        /// タイトルでの反転なども記述
        /// </summary>
        private void GameTitleDraw()
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(texture, new Rectangle(0, 0, hardWareWidth, hardWareHeight), Color.White);
            _spriteBatch.End();
        }

        /// <summary>
        /// エンディング画面での表示周りを編集する関数
        /// </summary>
        private void GameEndingDraw() {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(outParam.GetSpriteFont(), "GameClear", new Vector2(0, AppOutParam.OutPutUiHeaderYpos), Color.White);
            _spriteBatch.DrawString(outParam.GetSpriteFont(), "Score", new Vector2(0, AppOutParam.OutPutUiHeaderYpos), Color.White);
            _spriteBatch.DrawString(outParam.GetSpriteFont(), "Press Z Button To Title", new Vector2(0, AppOutParam.OutPutUiHeaderYpos + 20), Color.White);
            _spriteBatch.Draw(characterTexture, playerData.GetPosition(), Color.White);
            _spriteBatch.End();
        }


        /// <summary>
        /// メイン画面でのキャラクター表示周りを編集する関数
        /// </summary>
        private void GameMainDraw()
        {
            _spriteBatch.Begin();
            // 自キャラ関係の描画
            _spriteBatch.Draw(characterTexture, playerData.GetPosition(), Color.White);

            for (int i = 0; i < playerBulletDatas.Count; i++)
            {
                _spriteBatch.Draw(characterTexture, playerBulletDatas[i].GetPosition(), Color.White);
            }

            // 敵キャラ関係
            for (int i = 0; i < enemyShipDatas.Count; i++)
            {
                _spriteBatch.Draw(enemyTexture, enemyShipDatas[i].GetPosition(), Color.White);
            }

            // 描画専用処理が増えたらコメント解除して実装
            this.MyCharacterDraw();
            this.EnemyDraw();

            // 画面の一番奥に表示されるUIを設定
            this.UIHeaderDraw();
            _spriteBatch.End();
        }

        /// <summary>
        /// メイン画面内での更新処理
        /// キーボードなどでの入力関連を保持する
        /// </summary>
        private void GameMainUpdate()
        {
            if (gameNowScene != GameSceneName.Main)
            {
                return;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                playerData.AddPosition(0, playerPositionSubValue);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                playerData.AddPosition(0, playerPositionAddValue);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                playerData.AddPosition(playerPositionSubValue, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                playerData.AddPosition(playerPositionAddValue, 0);
            }

            // チャタリングのバランス調整
            if (animCounter > 0) {
                animCounter--;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && animCounter == 0)
            {
                decideSoundEffect.Play(0.1f,0,0);
                playerBulletDatas.Add(new PlayerActionData(playerData.GetPosition(), 1));
                animCounter = 10;
            }

            for (int i = 0; i < playerBulletDatas.Count; i++)
            {
                playerBulletDatas[i].AddPosition(playerPositionAddValue, 0);

                if (playerBulletDatas[i].GetPosition().X > 300)
                {
                    playerBulletDatas.Remove(playerBulletDatas[i]);
                }
            }

        }

        /// <summary>
        /// エンディングシーン内での遷移処理対応
        /// </summary>
        private void GameEndingUpdate()
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                decideSoundEffect.Play();
                gameNowScene = GameSceneName.Title;
            }
        }


        /// <summary>
        /// メイン画面内での、更新処理
        /// </summary>
        private void UpdateFieldBattle()
        {
            // 敵機体の移動処理
            Random random = new Random();
            MoveEnemy(random);

            // 出現処理
            if (enemyAppearFrameCount >= EnemyAppearFrameNum)
            {
                int yValue = random.Next(hardWareHeight);
                var enemyShipPos = new System.Numerics.Vector2(200, yValue);
                enemyShipDatas.Add(new PlayerActionData(enemyShipPos, 1));
                enemyAppearFrameCount = 0;
            }

            enemyAppearFrameCount++;

            // 敵キャラ関係
            for (int i = 0; i < enemyShipDatas.Count; i++)
            {
                if (isHit(playerData.GetPosition(), characterTexture,
                    enemyShipDatas[i].GetPosition(), enemyTexture)) {
                    enemyShipDatas[i].SubDurability(1);
                    if (!enemyShipDatas[i].IsRunning()) { 
                         enemyShipDatas.Remove(enemyShipDatas[i]);
                         bomSoundEffect.Play(0.8f,0,0);
                    }
                    playerData.SubDurability(1);
                    if (playerData.IsRunning()) {
                        gameNowScene = GameSceneName.Title;
                    }
                }
            }

            // エンディングシーンへの仮条件を設定
            if (gameScore > GameClearScore-1) {
                clearEffect.Play();
                gameNowScene = GameSceneName.Ending;
            }

        }


        /// <summary>
        /// アプリケーション側の当たり判定関数
        /// </summary>
        /// <param name="basePosition">基本座標関連</param>
        /// <param name="baseTexture">当たり判定の範囲で利用する画像情報</param>
        /// <param name="targetPosition">基本座標関連</param>
        /// <param name="targetTexture">当たり判定の範囲で利用する画像情報</param>
        /// <returns>当たっている場合はtrueを返す</returns>
        private bool isHit(Vector2 basePosition, Texture2D baseTexture,  Vector2 targetPosition, Texture2D targetTexture ) {
            return (basePosition.X <= targetPosition.X &&
                targetPosition.X <=  basePosition.X + baseTexture.Width &&
                basePosition.Y <= targetPosition.Y &&
                targetPosition.Y <= basePosition.Y + baseTexture.Width);
        }

        /// <summary>
        /// 敵移動処理
        /// </summary>
        /// <param name="random">移動速度算出用オブジェクト</param>
        private void MoveEnemy(Random random)
        {
            // 敵キャラ関係
            for (int i = 0; i < enemyShipDatas.Count; i++)
            {
                // 敵を移動する
                int yAddValue = random.Next(EnemyMaxSpeedNum);
                int ySubValue = random.Next(EnemyMaxSpeedNum);
                enemyShipDatas[i].AddPosition(0, yAddValue - ySubValue);
                // 移動後に弾に接触しているかを確認する
                for (int j = 0; j < playerBulletDatas.Count;  j++) {
                    if (isHit(playerBulletDatas[j].GetPosition(), characterTexture,
                        enemyShipDatas[i].GetPosition(), enemyTexture)) {
                        enemyShipDatas[i].SubDurability(1);
                        playerBulletDatas[j].SubDurability(1);
                        if (!enemyShipDatas[i].IsRunning()) {
                            enemyShipDatas.Remove(enemyShipDatas[i]);
                            bomSoundEffect.Play(0.8f, 0, 0);
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
        /// Todo 自キャラ関係の描画処理
        /// 処理行数が多そうであれば、専用関数で呼び出し
        /// </summary>
        private void MyCharacterDraw()
        {
        }

        /// <summary>
        /// Todo 敵キャラ固有の描画表示の改修希望があれば移行して修正
        /// </summary>
        private void EnemyDraw()
        {
        }


        /// <summary>
        /// ゲーム中のUI情報を表示する
        /// </summary>
        private void UIHeaderDraw()
        {
            string scoreStr = $"score {gameScore}";
            _spriteBatch.DrawString(outParam.GetSpriteFont(), "stage 1", new Vector2(0, AppOutParam.OutPutUiHeaderYpos), Color.White);
            _spriteBatch.DrawString(outParam.GetSpriteFont(), scoreStr, new Vector2(100, AppOutParam.OutPutUiHeaderYpos), Color.White);
        }

    }
}