
namespace GGjNantonaku.SetUp
{
    /// <summary>
    /// アプリケーションビルド設定時の定数処理
    /// </summary>
    public class AppDefine
    {
        /// <summary>
        /// 画面全体の高さ
        /// </summary>
        public const int HardWareHeight = 224;

        /// <summary>
        /// 画面全体の幅
        /// </summary>
        public const int HardWareWidth = 256;

        /// <summary>
        /// 座標の移動速度 
        /// </summary>
        public const int PlayerPositionAddValue = 3;
        public const int PlayerPositionSubValue = -3;

        /// <summary>
        /// 武器の弾速設定値
        /// </summary>
        public const int PlayerBulletSpeed = 5;

        /// <summary>
        /// ゲームクリア基準スコア
        /// </summary>
        public const int GameClearScore = 400;

        // 敵出現ライン座標値
        public const int EnemyAppearYPos = 200;

        // 敵を倒した時の効果音ボリューム
        public const float EnemySoundVolume = 0.8f;

        // 敵を倒した時の耐久力
        public const int EnemyDurability = 1;

        // 敵が１フレームで移動する最大速度
        public const int EnemyMaxSpeedNum = 10;

        // ゲームオブジェクトの座標ボーダーライン(X)
        public const int BorderPosition = 300;

    }
}
