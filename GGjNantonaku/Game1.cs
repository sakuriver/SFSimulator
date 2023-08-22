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
        /// アニメーションカウンター処理
        /// </summary>
        private int animCounter = 0;

        // ゲームとしてビルドするときの画面サイズ

        // 敵が出現するフレーム数
        private int EnemyAppearFrameNum = 100;

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
        /// タイトルbgm
        /// </summary>
        private Song titleBgmSound;

        /// <summary>
        /// メインモードBGM
        /// </summary>
        private Song mainBgmSound;

        /// <summary>
        /// エンディングBGM
        /// </summary>
        private Song endingBgmSound;

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

        // maou_bgm_8bit29


        /// <summary>
        /// ゲーム実行時のコンストラクタ
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

            // MediaPlayerをタイトルbgm再生
            MediaPlayer.IsRepeating = true;

            MediaPlayer.Play(titleBgmSound);
        }

        /// <summary>
        /// ゲームのメインループで使っている時間部分
        /// </summary>
        /// <param name="gameTime">更新時間情報</param>
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
        /// タイトル画面の遷移確認するか
        /// </summary>
        /// <returns>遷移確認をする場合はtrue</returns>
        private bool isGameTitleUpdate()
        {
            return (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter)) &&
                gameNowScene == GameSceneName.Title;
        }

        /// <summary>
        /// ゲーム終了処理
        /// </summary>
        private void gameExit()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
        }

        /// <summary>
        /// ゲームメインアップデート
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
        /// ゲームエンディングアップデート処理
        /// </summary>
        private void gameEndingModeUpdate()
        {
            if (this.gameNowScene == GameSceneName.Ending)
            {
                this.GameEndingUpdate();
            }
        }

        /// <summary>
        /// ゲーム更新処理アップデート
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
            _spriteBatch.Draw(texture, new Rectangle(0, 0, AppDefine.HardWareHeight, AppDefine.HardWareWidth), Color.White);
            _spriteBatch.End();
        }

        /// <summary>
        /// エンディング画面での表示周りを編集する関数
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
        /// メイン画面でのキャラクター表示周りを編集する関数
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
        /// メイン画面内での更新処理
        /// キーボードなどでの入力関連を保持する
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
        /// キャラクターの移動ロジック用関数
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
        /// 自機からの武器発射
        /// </summary>
        private void playerBulletShot()
        {
            decideSoundEffect.Play(0.1f, 0, 0);
            playerBulletDatas.Add(new PlayerActionData(playerData.GetPosition(), 1));
            animCounter = 10;
        }

        /// <summary>
        /// 自弾移動処理
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
        /// エンディングシーン内での遷移処理対応
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
        /// メイン画面内での、更新処理
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
        /// ゲームのエンディング条件を確認する
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
        /// エンディングへ移動する時の基準値を返却する
        /// </summary>
        /// <returns>エンディング移動時のスコア基準値</returns>
        private int getGameClearBorderScore()
        {
            return AppDefine.GameClearScore - 1;
        }

        /// <summary>
        /// 敵出現処理
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
        /// アプリケーション側の当たり判定関数
        /// </summary>
        /// <param name="basePosition">基本座標関連</param>
        /// <param name="baseTexture">当たり判定の範囲で利用する画像情報</param>
        /// <param name="targetPosition">基本座標関連</param>
        /// <param name="targetTexture">当たり判定の範囲で利用する画像情報</param>
        /// <returns>当たっている場合はtrueを返す</returns>
        private bool isHit(Vector2 basePosition, Texture2D baseTexture, Vector2 targetPosition, Texture2D targetTexture)
        {
            return (basePosition.X <= targetPosition.X &&
                targetPosition.X <= basePosition.X + baseTexture.Width &&
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
        /// 自キャラの描画処理
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
        /// 敵キャラ固有の描画表示の改修希望があれば移行して修正
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
        /// ゲーム中のUI情報を表示する
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