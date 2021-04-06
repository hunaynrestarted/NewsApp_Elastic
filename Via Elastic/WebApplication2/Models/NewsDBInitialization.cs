using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Nest;

namespace NewsAPP_Elastic.Models
{
    public static class NewsDBInitialization 
    {
        public static void Init()
        {
            CreateIndex();
            CreateMappings();
            CreateSeed();
        }


        public static void CreateIndex()
        {
            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            settings.DefaultIndex("newsstore");
            ElasticClient client = new ElasticClient(settings);
            client.Indices.Delete(Indices.Index("newsstore"));
            var indexSettings = client.Indices.Exists("newsstore");
            if (!indexSettings.Exists)
            {
                var response = client.Indices.Create(Indices.Index("newsstore"));
            }

        }

        public static void CreateSeed()
        {
            int seedValue = 1;
            int limitValue = 20000;

            IList<News> NewsList = new List<News>();

            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            settings.DefaultIndex("newsstore");
            ElasticClient esClient = new ElasticClient(settings);
            while (seedValue <= limitValue)
            {
                var news = new News() { NewsID = seedValue, NewsTitle = "News" + seedValue.ToString(), IsPublished = true };
                var response = esClient.IndexAsync(news, idx => idx.Index("newsstore"));
                seedValue++;
            }

        }
        /// <summary>  
        ///   
        /// </summary>  
        public static void CreateMappings()
        {
            ConnectionSettings settings = new ConnectionSettings(new Uri("http://localhost:9200"));
            settings.DefaultIndex("newsstore");
            ElasticClient esClient = new ElasticClient(settings);
            esClient.Map<News>(m =>
            {
                var putMappingDescriptor = m.Index(Indices.Index("newsstore")).AutoMap();
                return putMappingDescriptor;
            });
        }

    }
}