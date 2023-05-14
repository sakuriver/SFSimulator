using Microsoft.Xna.Framework.Graphics;

namespace SFSimulator.UI
{
    /// <summary>
    /// アプリの外回り用の情報管理
    /// 
    /// </summary>
    public class AppOutParam
    {
        /// <summary>
        /// 指定がない場合に利用する文字列
        /// </summary>
        private SpriteFont defaultFont;

        /// <summary>
        /// ヘッダー情報のUI定義
        /// </summary>
        public const int OutPutUiHeaderYpos = 10;

        /// <summary>
        /// 会話ダイアログ用のUI定義
        /// </summary>
        public const int OutputTalkYpos = 180;

        /// <summary>
        /// コンストラクタ
        /// デフォルトのフォント情報を定義する
        /// </summary>
        /// <param name="spriteFont">文字情報ファイル</param>
        public AppOutParam(SpriteFont _defaultFont)
        {
            defaultFont = _defaultFont;
        }

        /// <summary>
        /// ゲーム内で利用するフォント情報を取得する
        /// </summary>
        /// <returns>デフォルトのフォント情報</returns>
        public SpriteFont GetSpriteFont()
        {
            return defaultFont;
        }


    }
}
