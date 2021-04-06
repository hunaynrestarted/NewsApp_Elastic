using NewsAPP_Elastic.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NewsAPP_Elastic.Controllers
{
    public class NewsController : Controller
    {

        public async Task<ActionResult> GetNews(int? page=1, int? pageSize=10,  string newsTitle="")
        {
            List<News> cricketersList = new List<News>();

            News newsObj = null;
            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            settings.DefaultIndex("newsstore");
            ElasticClient esClient = new ElasticClient(settings);
             
            var response = await esClient.SearchAsync<News>(s => s.From((page.Value - 1) * pageSize.Value).Size(pageSize)
                            .Query(q => q
                            .Bool(b => b
                                .Should(m => m
                                    .MultiMatch(mm => mm
                                        .Fields(f => f.Field("NewsID"))
                                        .Query(newsTitle)

                                    ), wc => wc
                                    .Wildcard(c => c
                                        .Field("newsTitle")
                                        .Value("*" + newsTitle + "*")
                                    )
                                )
                            )
                        ).Sort(ss => ss
                    .Ascending(f => f.NewsID)));

            var newsResponse = response.Hits.ToList();
            List<News> newsListReturned = new List<News>();
            foreach (var item in newsResponse)
            {
                newsObj = new News
                {
                    NewsID = item.Source.NewsID,
                    NewsTitle = item.Source.NewsTitle,
                    IsPublished = item.Source.IsPublished
                };
                newsListReturned.Add(newsObj);
            }
            List<News> userList = new List<News>();
            foreach (var item in newsListReturned)
            {
                News um = new News();
                um.NewsID = item.NewsID;
                um.NewsTitle = item.NewsTitle;
                um.IsPublished = item.IsPublished;
                userList.Add(um);
            }

            ViewBag.PageNumber = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchParam = newsTitle;

            var countResponse = await esClient.CountAsync<News>(s => s.Query(q => q
                            .Bool(b => b
                                .Should(m => m
                                    .MultiMatch(mm => mm
                                        .Fields(f => f.Field("NewsID"))
                                        .Query(newsTitle)

                                    ), wc => wc
                                    .Wildcard(c => c
                                        .Field("newsTitle")
                                        .Value("*" + newsTitle + "*")
                                    )
                                ))));

            ViewBag.TotalPages = countResponse.Count % pageSize == 0 ? countResponse.Count / pageSize : (countResponse.Count / pageSize) + 1;
            
            return View(userList);
        }

        //Used for server side pagination

        [HttpGet, ActionName("GetNewsData")]
        public async Task<ActionResult> GetNewsData(int? page=1, int? pageSize=10, string newsTitle="")
        {
            List<News> cricketersList = new List<News>();

            News newsObj = null;
            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            settings.DefaultIndex("newsstore");
            ElasticClient esClient = new ElasticClient(settings);

            var response = await esClient.SearchAsync<News>(s => s.From((page.Value - 1) * pageSize.Value).Size(pageSize)
                            .Query(q => q
                            .Bool(b => b
                                .Should(m => m
                                    .MultiMatch(mm => mm
                                        .Fields(f => f.Field("NewsID"))
                                        .Query(newsTitle)

                                    ), wc => wc
                                    .Wildcard(c => c
                                        .Field("newsTitle")
                                        .Value("*" + newsTitle + "*")
                                    )
                                )
                            )
                        ).Sort(ss => ss
                    .Ascending(f => f.NewsID)));

            var newsResponse = response.Hits.ToList();
            List<News> newsListReturned = new List<News>();
            foreach (var item in newsResponse)
            {
                newsObj = new News
                {
                    NewsID = item.Source.NewsID,
                    NewsTitle = item.Source.NewsTitle,
                    IsPublished = item.Source.IsPublished
                };
                newsListReturned.Add(newsObj);
            }
            List<News> userList = new List<News>();
            foreach (var item in newsListReturned)
            {
                News um = new News();
                um.NewsID = item.NewsID;
                um.NewsTitle = item.NewsTitle;
                um.IsPublished = item.IsPublished;
                userList.Add(um);
            }
             
            ViewBag.PageNumber = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchParam = newsTitle;

            var countResponse = await esClient.CountAsync<News>(s => s.Query(q => q
                            .Bool(b => b
                                .Should(m => m
                                    .MultiMatch(mm => mm
                                        .Fields(f => f.Field("NewsID"))
                                        .Query(newsTitle)

                                    ), wc => wc
                                    .Wildcard(c => c
                                        .Field("newsTitle")
                                        .Value("*" + newsTitle + "*")
                                    )
                                ))));

            ViewBag.TotalPages = countResponse.Count % pageSize == 0 ? countResponse.Count / pageSize : (countResponse.Count / pageSize) + 1;

            return PartialView("_PartialNews", userList);
        }

        public ActionResult Edit(int id)
        {
           
            return View();
        }

    }
}