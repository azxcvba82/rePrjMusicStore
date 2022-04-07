using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeetleX.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Music_store_backend.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Music_store_backend.Controllers.HomePage
{
    [Route("api/Home")]
    [ApiController]
    public class MainController : MBController
    {
        private readonly IDistributedCache _distributedCache;
        public MainController(IOptions<AppSettings> appSettings, IDistributedCache distributedCache) : base(appSettings) {
            _distributedCache = distributedCache;
        }

        [Route("getAlbum")]
        [HttpGet()]
        public ActionResult<JArray> albumQuery()
        {
            try
            {
                //DB + Cache method
                string key = "homepageListCache";
                byte[] valueByte = _distributedCache.Get(key);
                if (valueByte == null)
                {
                    JArray tA = JArray.FromObject(CSearch.ConstructAllAlbum(Util).Take(20));
                    TimeSpan ts = DateTime.Now.AddMinutes(60) - DateTime.Now;
                    _distributedCache.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(tA)), new DistributedCacheEntryOptions().SetSlidingExpiration(ts));
                    valueByte = _distributedCache.Get(key);
                    return tA;
                }
                string valueString = Encoding.UTF8.GetString(valueByte);
                return JArray.Parse(valueString);
                //only DB method
                //Array tA = CSearch.ConstructAllAlbum(Util).Take(20).ToArray();
                //return tA;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("eventQuery")]
        [HttpGet()]
        public ActionResult<Array> eventQuery()
        {
            string account = "";
            try
            {
                Array tA = CEvent.ConstructEventQuery(Util, account).ToArray();
                return tA;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("getEventTitleById/{eventId}")]
        [HttpGet()]
        public ActionResult<JObject> getEventTitleById([FromRoute]int eventId)
        {
            try
            {
                JObject tA = JObject.FromObject(CgetActivity.getActivityById(Util, eventId));
                return tA;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("test")]
        [HttpGet()]
        public ActionResult<string> test()
        {
            try
            {
                Redis.Default.DataFormater = new JsonFormater();
                Redis.Default.Host.AddWriteHost("52.137.103.57", 6379).Password = "henry";
                Redis.Default.Host.AddReadHost("52.137.103.57", 6379).Password = "henry";
                //RedisDB DB = new RedisDB(0, new JsonFormater());
                //string a = Redis.Set("test", "20210105!!!!!!!!!!!!!!!!").Result;
                //DB.Host.AddWriteHost("52.149.25.251", 6379).Password = "henrya";
                string b = Redis.Get<string>("test").Result;
                return b;
                //MemoryStream csvStream = new MemoryStream();
                //StreamWriter writer = new StreamWriter(csvStream);
                //List<SourceUsage> raw = new List<SourceUsage>(){ };
                //Dictionary<string, List<Usage>> myDic = new Dictionary<string, List<Usage>>() { };
                //foreach (SourceUsage aa in raw)
                //{
                //    if (!myDic.ContainsKey(aa.ScustomerId))
                //    {
                //        List<Usage> customer = new List<Usage>() { };
                //        myDic.Add(aa.ScustomerId, customer);
                //        Usage item1 = new Usage() { };
                //        item1.customerId = aa.ScustomerId;
                //        item1.period = aa.SproductId;
                //        item1.productId = aa.SproductId;
                //        item1.cost = aa.Scost;
                //        myDic[aa.ScustomerId].Add(item1);
                //    }
                //    else
                //    {
                //        Usage item1 = new Usage() { };
                //        item1.customerId = aa.ScustomerId;
                //        item1.period = aa.SproductId;
                //        item1.productId = aa.SproductId;
                //        item1.cost = aa.Scost;
                //        myDic[aa.ScustomerId].Add(item1);
                //    }
                //}
                //foreach (KeyValuePair<string, List<Usage>> kvp in myDic)
                //{
                //    String outputFileName = String.Format("{0}-{1}.csv", kvp.Key, "billingPeriodId");
                //    CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                //    csv.WriteRecords(kvp.Value);
                //}

                /*int i = 0;

                if (i % 5 == 0)
                {
                    string asa = "1";
                }

                List<KeyValuePair<string, string>> tA = new List<KeyValuePair<string, string>> { };
                KeyValuePair<string, string> a = new KeyValuePair<string, string>("os", "linux");
                KeyValuePair<string, string> b = new KeyValuePair<string, string>("env", "dev");
                KeyValuePair<string, string> c = new KeyValuePair<string, string>("creator", "123");
                tA.Add(a); tA.Add(b); tA.Add(c);
                string source = JsonConvert.SerializeObject(tA);

                JObject ja = new JObject() { };
                foreach (KeyValuePair<string, string> obj in JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(source))
                {
                    ja.Add(obj.Key, obj.Value);
                }
                return JsonConvert.SerializeObject(ja);

                string dash = "a/b/c/d/e/f/g/h/i/j";
                
                string[] column = dash.Split("/", StringSplitOptions.RemoveEmptyEntries);
                return String.Format("s{0}-{1}-{2}-{3}-{4}-e",column[0],column[1],column[2],column[3],column[4]);*/
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        public class Usage
        {
            public string customerId { get; set; }
            public string period { get; set; }
            public string productId { get; set; }
            public string cost { get; set; }
        }

        public class SourceUsage
        {
            public string ScustomerId { get; set; }
            public string Speriod { get; set; }
            public string SproductId { get; set; }
            public string Scost { get; set; }
        }
    }
}