using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Cognitive.LUIS;

namespace LineChatBot.Controllers
{
    public class LineChatBotController : isRock.LineBot.LineWebHookControllerBase
    {

            [Route("api/LineBot")]

            [HttpPost]

            public IHttpActionResult POST()

            {
            

                try

                {

                    //設定ChannelAccessToken(或抓取Web.Config)

                    this.ChannelAccessToken = "qw6hghZir3I1lTq2PQCZ5Ap30I3eDEUju5UXCeS1E1vi8ja2QftluqLEf+TyAfmaS6CDLava+i1jzHEpy9rVVfxXz00kL615WL0mNasRf+Ge9pnxLv2sEmhg9Ml7AGT5MsqM7TktOmovad2CB8H4bgdB04t89/1O/w1cDnyilFU=";
                    const string LuisAppId = "c8a36302-0cc4-426d-b4ba-cac8234a0f09";
                    const string LuisAppKey = "7d43e486a4fd49fe8e743470b8e5e8c6";
                    const string Luisdomain = "westus";
                    Microsoft.Cognitive.LUIS.LuisClient lc =
                    new Microsoft.Cognitive.LUIS.LuisClient(LuisAppId, LuisAppKey, true, Luisdomain);
                //取得Line Event

                var item = this.ReceivedMessage.events.FirstOrDefault();
                    string Message = "";

                    //回覆訊息
                    switch (item.type)

                    {

                        case "join":

                            Message = $"有人把我加入{item.source.type}中了，大家好啊~";



                            //回覆用戶

                            isRock.LineBot.Utility.ReplyMessage(ReceivedMessage.events[0].replyToken, Message, ChannelAccessToken);

                            break;

                        case "message":

                
                            if (item.message.text == "bye")

                            {

                            //回覆用戶

                            isRock.LineBot.Utility.ReplyMessage(item.replyToken, "bye-bye", ChannelAccessToken);

                                //離開

                                if (item.source.type.ToLower() == "room")

                                    isRock.LineBot.Utility.LeaveRoom(item.source.roomId, ChannelAccessToken);

                                if (item.source.type.ToLower() == "group")

                                    isRock.LineBot.Utility.LeaveGroup(item.source.roomId, ChannelAccessToken);





                            }
                            else if (item.message.text == "ID" || item.message.text == "id")
                                Message = "你的user id: " + Convert.ToString(item.source.userId);
                            else
                            {
                             var ret = lc.Predict(item.message.text).Result;
                            if (ret.TopScoringIntent.Name == "打招呼")
                                Message = "你好啊!!!";
                            else if (ret.TopScoringIntent.Name == "時間")
                                Message = "現在時間 : " + System.DateTime.Now;
                            else
                                Message = "??";

                            }
                            //回覆用戶
                            isRock.LineBot.Utility.ReplyMessage(item.replyToken, Message, ChannelAccessToken);

                            break;

                        default:

                            break;

                    }



                    //response OK

                    return Ok();

                }

                catch (Exception ex)

                {

                    //回覆訊息

                    this.PushMessage("Ued3ad06f07d7d541a9cc742c7f2dcb5c", "發生錯誤:\n" + ex.Message);

                    //response OK

                    return Ok();

                }

            }

        
    }
}
