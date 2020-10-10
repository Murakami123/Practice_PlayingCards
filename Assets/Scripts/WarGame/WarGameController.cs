using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Photon.Realtime;

public class WarGameController : BaseGameScene
{
    /////////////////////////////////////////////////////
    /// 基本形
    /////////////////////////////////////////////////////        
    protected override string randomRoomName
    {
        get { return "randomRoom_War"; }
    }

    private async UniTask Start() => MainFlow().Forget();

    /////////////////////////////////////////////////////
    /// シーンのごとの処理
    /////////////////////////////////////////////////////
    [SerializeField] private DeckController deckController;

    [SerializeField] private PlayerController[] playerController = new PlayerController[2]; // 2人対戦
    [SerializeField] private RectTransform HikiwakeCardPatent;
    private PlayerController player1 => playerController[0];
    private PlayerController player2 => playerController[1];
    private int useJokerCount = 2; // 1にしたいけど奇数だと最後1枚余る


    protected async UniTask MainFlow()
    {
        // 部屋がなければ作って入る
        Debug.Log("WarGameController.Start_1");
        await base.JoinOrCreateRoom();
        Debug.Log("WarGameController.Start_2");

        // 山札リセット
        Debug.Log("WarGameController.MainFlow");
        deckController.ResetDeck(useJokerCount);

        // 山札なくなるまで対戦繰り返し
        Debug.Log("deckController.GetDeckCardCount:" + deckController.GetDeckCardCount);
        while (deckController.GetDeckCardCount > 0)
        {
            Debug.Log("残り山札枚数；" + deckController.GetDeckCardCount);

            // プレイヤーにカードを配る
            await (
                DistributeCard(player1),
                DistributeCard(player2)
            );

            // 両方のプレイヤーが選択するの待ち
            player1.ReleaseChoiceCard();
            player2.ReleaseChoiceCard();

            var choiceCards = await (
                player1.PlayerChoiceCard(isRemovePlayerCard: true),
                player2.PlayerChoiceCard(isRemovePlayerCard: true)
            );

            // 両方のカードを表向きにする
            var player1Card = choiceCards.Item1;
            var player2Card = choiceCards.Item2;
            player1Card.SetChoice(false); // 選択中表示終了
            player2Card.SetChoice(false);
            await (
                player1Card.ReturnCard(isShowFront: true),
                player2Card.ReturnCard(isShowFront: true)
            );

            // 1回の対戦;
            if (player1Card.cardNo == player2Card.cardNo)
            {
                Debug.Log("引き分け");

                // 引き分け
                var hikwakeCardPos = HikiwakeCardPatent.localPosition;
                await (
                    player1Card.MoveCard(HikiwakeCardPatent, hikwakeCardPos, isLittleShit: true),
                    player2Card.MoveCard(HikiwakeCardPatent, hikwakeCardPos, isLittleShit: true)
                );
            }
            else
            {
                // どっちか勝ち_カード移動
                var winner = (player1Card.cardNo > player2Card.cardNo) ? player1 : player2;
                Debug.Log("勝者:" + winner);
                await (
                    winner.SetWinCard(player1Card),
                    winner.SetWinCard(player2Card)
                );
            }
        }

        // 勝敗引き分けチェック
        if (player1.winGetCardList.Count == player2.winGetCardList.Count)
        {
            // 引き分け
            GameEnd_Hikiwake();
        }
        else
        {
            var isPlayer1win = (player1.winGetCardList.Count > player2.winGetCardList.Count);
            if (isPlayer1win)
                GameEnd_Player1Win();
            else
                GameEnd_Player2Win();
        }
    }

    private async UniTask DistributeCard(PlayerController player)
    {
        var drawCard = deckController.GetCard();
        Debug.Log("drawCard:" + drawCard);
        player.AddCard(drawCard);
    }

    /////////////////////////////////////////////////////
    // 勝敗表示
    /////////////////////////////////////////////////////    
    [SerializeField] private GameObject player1WinObj, player2WinObj, gameHikiwakeObj;

    private void GameEnd_Player1Win()
    {
        player1WinObj.SetActive(true);
    }

    private void GameEnd_Player2Win()
    {
        player2WinObj.SetActive(true);
    }

    private void GameEnd_Hikiwake()
    {
        gameHikiwakeObj.SetActive(true);
    }

    // ◆トランプゲーム共通であったらよさそうな概念
    // ○GameControllerBase
    //      ・（「実際のトランプの場合、ユーザが行うが自動で行った方がよいもの」等を実行する）
    //      ・山札インスタンスからプレイヤーにカードを配る(Distributeメソッド)
    //      ・勝敗引き分け表示。
    //      ・リトライ(GameRetryメソッド)
    // ○カードbase
    //      ・片面が数字と絵柄、片面がトランプの裏、ひっくり返すことができる

    // ◆「戦争:GameControllerBase」に出てくる概念の洗い出し
    // ○WarGameController
    //      ・ゲームで使うジョーカーの枚数指定
    //      ・プレイできる人数の指定（実装未定）
    // ○プレイヤー（PlayerController）
    //      ・手元のカードを、場に出すことができる。
    // 
    // ○山札（DeckController）
    //      ・シャッフルすることができる。
    //      ・カードを山札に戻すことができる。
    //      ・全てのカードを山札に戻すことができる（ゲームリトライ）。
    //      ・タイミングで、2枚を両プレイヤーの手元のカードにする。
    //      ・残りの枚数が0枚の状態で、タイミングになったらゲーム終了
    //      ・カードを引くことできる。
    //      ・残り枚数がだいたいわかるぐらいの見た目。
    //      ・＜カードの生成を管理＞（Destoryはそれぞれのカード？）
    //      ・＜カードを山札の上に生成する＞
    // 
    // ○カード（Card）
    //      ・手元にあるとき、タイミングで出すことができる
    //      ・山札に、手もとに、入手カード枠に移動できる。
    //      ・＜タップした時＞ タップしたカードのインスタンスを返す。
    //      ・＜長押しした時＞
    //      ・＜ドラッグした時＞（スピードとか）
    // 
    // ○入手カード枠
    //      ・枚数がわかる
    // 
    // ○勝敗引き分け

    // ◆トランプゲームはウォーターフォール型が多いから、シーケンス図が向いてる気がする。
    // 
}