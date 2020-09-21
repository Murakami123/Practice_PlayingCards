using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class WarGameController : MonoBehaviour
{
    [SerializeField] private DeckController deckController;
    [SerializeField] private PlayerController[] playerController = new PlayerController[2]; // 2人対戦
    [SerializeField] private RectTransform HikiwakeCardPatent; 
    private PlayerController player1 => playerController[0];
    private PlayerController player2 => playerController[1];
    private int useJokerCount = 2; // 1にしたいけど奇数だと最後1枚余る

    private void Start()
    {
        MainFlow().Forget();
    }

    private async UniTask MainFlow()
    {
        // 山札リセット
        deckController.ResetDeck(useJokerCount);

        // 山札なくなるまで対戦繰り返し
        Debug.Log("deckController.GetDeckCardCount:" + deckController.GetDeckCardCount);
        while (deckController.GetDeckCardCount >=0)
        {
            // プレイヤーにカードを配る
            var task1 = DistributeCard( player1 );
            var task2 = DistributeCard( player2 );
            await UniTask.WhenAll(task1, task2);

            // 両方のプレイヤーが選択するの待ち
            player1.ReleaseChoiceCard();
            player2.ReleaseChoiceCard();
            var task3 = player1.PlayerChoiceCard(isRemovePlayerCard:true);
            var task4 = player2.PlayerChoiceCard(isRemovePlayerCard:true);
            var choiceCards = await UniTask.WhenAll(task3, task4);

            // 両方のカードを表向きにする
            var player1Card = choiceCards.Item1;
            var player2Card = choiceCards.Item2;
            var task9  = player1Card.ReturnCard(isShowFront:true);
            var task10 = player2Card.ReturnCard(isShowFront:true);
            await UniTask.WhenAll(task9, task10);
            
            // 1回の対戦;
            if (player1Card.cardNo == player2Card.cardNo)
            {
                Debug.Log("引き分け");

                // 引き分け
                var hikwakeCardPos = HikiwakeCardPatent.localPosition;
                var task7 = player1Card.MoveCard(HikiwakeCardPatent, hikwakeCardPos, isLittleShit:true);
                var task8 = player2Card.MoveCard(HikiwakeCardPatent, hikwakeCardPos, isLittleShit:true);
                await UniTask.WhenAll(task7, task8);
            }
            else
            {
                // どっちか勝ち_カード移動
                var winner = (player1Card.cardNo > player2Card.cardNo) ? player1 : player2;
                Debug.Log("勝者:" + winner);
                var task5 = winner.SetWinCard(player1Card);
                var task6 = winner.SetWinCard(player2Card);
                await UniTask.WhenAll(task5, task6);
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
    private void GameEnd_Player1Win(){ player1WinObj.SetActive(true);}
    private void GameEnd_Player2Win(){ player2WinObj.SetActive(true);}
    private void GameEnd_Hikiwake(){ gameHikiwakeObj.SetActive(true);}
   
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
