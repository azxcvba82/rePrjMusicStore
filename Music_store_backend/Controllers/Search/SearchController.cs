using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Music_store_backend.Model;
using Newtonsoft.Json.Linq;

namespace Music_store_backend.Controllers.Search
{
    [Route("api/Search")]
    [ApiController]
    public class SearchController : MBController
    {
        public SearchController(IOptions<AppSettings> appSettings) : base(appSettings) { }

        [Route("Kinds")]
        [HttpGet()]
        public ActionResult<Array> Kinds()
        {
            try
            {
                Array tA = CSearch.takeAllKind(Util).ToArray();
                return tA;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("KindResult/{kindId}")]
        [HttpGet()]
        public ActionResult<Array> KindResult([FromRoute]int kindId)
        {
            try
            {
                Array tA = CSearch.byKindPage(Util, kindId).ToArray();
                return tA;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("EventResult/{eventId}")]
        [HttpGet()]
        public ActionResult<Array> EventResult([FromRoute]int eventId)
        {
            try
            {
                Array tA = CSearch.byEvent(Util, eventId).ToArray();
                return tA;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("Result/{keywords}")]
        [HttpGet()]
        public ActionResult<Array> Result([FromRoute]string keywords)
        {
            try
            {
                Array tA = CSearch.byKeyword(Util, keywords).ToArray();
                return tA;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("AdvancedResult")]
        [HttpPost()]
        public ActionResult<Array> AdvancedResult([FromBody]CAdvancedSearchObject keyObj)
        {
            if(keyObj == null ||(
                string.IsNullOrEmpty(keyObj.adAlbum) &&
                string.IsNullOrEmpty(keyObj.adComposer) &&
                string.IsNullOrEmpty(keyObj.adGroup) &&
                string.IsNullOrEmpty(keyObj.adKinds) &&
                string.IsNullOrEmpty(keyObj.adSinger) &&
                string.IsNullOrEmpty(keyObj.adSong) &&
                keyObj.adType == "1"))
            {
                return null;
            }
            try
            {
                Array tA = CSearch.byAdvanced(Util, keyObj).ToArray();
                return tA;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("SetTypes")]
        [HttpGet()]
        //進階搜尋內容的初始化
        public ActionResult<JArray> SetTypes()
        {
            List<tAlbumType> data = CSearch.takeAllType(Util);
            return JArray.FromObject(data);
        }

        [Route("AutoCompleteSong/{term}")]
        [HttpGet()]
        public ActionResult<Array> AutoCompleteSong([FromRoute]string term)
        {
            return CAutoComplete.SearchSong(Util, term).ToArray();
        }

        [Route("AutoCompleteAlbum/{term}")]
        [HttpGet()]
        public ActionResult<Array> AutoCompleteAlbum([FromRoute]string term)
        {
            return CAutoComplete.SearchAlbum(Util, term).ToArray();
        }

        [Route("AutoCompleteSinger/{term}")]
        [HttpGet()]
        public ActionResult<Array> AutoCompleteSinger([FromRoute]string term)
        {
            return CAutoComplete.SearchSinger(Util, term).ToArray();
        }

        [Route("AutoCompleteGroup/{term}")]
        [HttpGet()]
        public ActionResult<Array> AutoCompleteGroup([FromRoute]string term)
        {
            return CAutoComplete.SearchGroup(Util, term).ToArray();
        }

        [Route("AutoCompleteComposer/{term}")]
        [HttpGet()]
        public ActionResult<Array> AutoCompleteComposer([FromRoute]string term)
        {
            return CAutoComplete.SearchComposer(Util, term).ToArray();
        }
    }
}