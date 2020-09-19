using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarGameController : MonoBehaviour
{


    // ◆トランプゲーム共通であったらよさそうな概念
    // ○カードbase
    //      ・片面が数字と絵柄、片面がトランプの裏、ひっくり返すことができる

    // ◆「戦争」に出てくる概念の洗い出し
    // ○プレイヤー（PlayerController）
    //      ・手元のカードを、場に出すことができる。
    // ○山札（DeckController）
    //      ・シャッフルすることができる。
    //      ・タイミングで、2枚を両プレイヤーの手元のカードにする。
    //      ・残りの枚数が0枚の状態で、タイミングになったらゲーム終了
    // ○カード（Card）
    //      ・手元にあるとき、タイミングで出すことができる
    // ○入手したカード
    //      ・枚数がわかる
    // ○勝敗引き分け

    // ◆トランプゲームはウォーターフォール型が多いから、シーケンス図が向いてる気がする。
    // 






}
