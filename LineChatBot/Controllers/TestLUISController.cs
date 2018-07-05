using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LineChatBot.Controllers
{
    public class TestLUISController : isRock.LineBot.LineWebHookControllerBase
    {
        const string channelAccessToken = "qw6hghZir3I1lTq2PQCZ5Ap30I3eDEUju5UXCeS1E1vi8ja2QftluqLEf+TyAfmaS6CDLava+i1jzHEpy9rVVfxXz00kL615WL0mNasRf+Ge9pnxLv2sEmhg9Ml7AGT5MsqM7TktOmovad2CB8H4bgdB04t89/1O/w1cDnyilFU=";
        const string AdminUserId = "Ued3ad06f07d7d541a9cc742c7f2dcb5c";
        const string LuisAppId = " 	https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/c8a36302-0cc4-426d-b4ba-cac8234a0f09?subscription-key=7d43e486a4fd49fe8e743470b8e5e8c6&verbose=true&timezoneOffset=0&q= ";
        const string LuisAppKey = "7d43e486a4fd49fe8e743470b8e5e8c6";
        const string Luisdomain = "westus";

        [Route("api/testLUIS")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            try
            {
                //設定ChannelAccessToken(或抓取Web.Config)
                this.ChannelAccessToken = channelAccessToken;
                //取得Line Event(範例，只取第一個)
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();
                //回覆訊息
                if (LineEvent.type == "message")
                {
                    var repmsg = "";
                    if (LineEvent.message.type == "text") //收到文字
                    {
                        //建立LuisClient
                        Microsoft.Cognitive.LUIS.LuisClient lc =
                          new Microsoft.Cognitive.LUIS.LuisClient(LuisAppId, LuisAppKey, true, Luisdomain);

                        //Call Luis API 查詢
                        var ret = lc.Predict(LineEvent.message.text).Result;
                        if (ret.Intents.Count() <= 0)
                            repmsg = $"你說了 '{LineEvent.message.text}' ，但我看不懂喔!";
                        else if (ret.TopScoringIntent.Name == "None")
                            repmsg = $"你說了 '{LineEvent.message.text}' ，但不在我的服務範圍內喔!";
                        else
                        {
                            if (ret.TopScoringIntent.Name == "打招呼")
                                repmsg = "你好啊!!!";
                            else if (ret.TopScoringIntent.Name == "時間")
                                repmsg = "現在時間 : " + System.DateTime.Now;
                            else
                                repmsg = "??";
                        }
                        this.ReplyMessage(LineEvent.replyToken, repmsg);
                    }
                    if (LineEvent.message.type == "sticker") //收到貼圖
                        this.ReplyMessage(LineEvent.replyToken, 1, 2);
                }
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //如果發生錯誤，傳訊息給Admin
                this.PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }
    }
}
