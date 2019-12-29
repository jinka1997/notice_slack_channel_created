using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace pre_verification.Controllers
{
    [ApiController]
    [Route("slack/[controller]")]
    public class channel_createdController : ControllerBase
    {
        private IConfiguration _configuration;

        public channel_createdController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST: Notifier
        [HttpPost]
        public async Task<ActionResult> PostAsync(JToken token)
        {
            //var type = token["type"].Value<string>();
            //var challenge = token["challenge"].Value<string>();
            //if (type != "url_verification")
            //{
            //    return NotFound();
            //}
            //return Ok(challenge);

            try
            {
                var type = token["type"].Value<string>();
                if (type != "channel_created")
                {
                    return NotFound();
                }

                var channel = token["channel"];
                var channelName = channel["name"].Value<string>();

                var targetChannelId = _configuration["TARGET_CHANNEL_ID"];
                //var targetChannelId = Environment.GetEnvironmentVariable("TARGET_CHANNEL_ID");
                var parameters = new Dictionary<string, string>()
                {
                    { "channel", targetChannelId },
                    { "text", $"チャンネルが作成されました！ {Environment.NewLine}#{channelName}" },
                    { "link_names", "1" },
                };
                var json = JsonConvert.SerializeObject(parameters);
                var content = new StringContent(json, Encoding.UTF8, @"application/json");

                var botToken = _configuration["BOT_TOKEN"];
                //var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN");
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, $"https://slack.com/api/chat.postMessage");
                    request.Headers.Add("Authorization", $"Bearer {botToken}");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                }
                return Ok($"targetChannelId={targetChannelId}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.StackTrace);
            }
        }
    }
}