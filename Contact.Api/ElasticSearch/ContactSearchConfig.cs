using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace Contact.Api.ElasticSearch
{
	public static class ContactSearchConfig
	{
		private static readonly ConnectionSettings _connectionSettings;

		public static string LiveIndexAlias { get { return "contact"; } }

		public static Uri[] CreateUri()
		{
			return new Uri[] { new Uri("http://1.1.1.1:9200") };//需要改成自己的ip
		}

		static ContactSearchConfig()
		{
            var pool = new StaticConnectionPool(CreateUri());
            _connectionSettings = new ConnectionSettings(pool);
			_connectionSettings.BasicAuthentication("test", "test");//需要改成自己的用户名密码
            _connectionSettings.DefaultIndex(ContactIndexName);
		}

		public static ElasticClient GetClient()
		{
			return new ElasticClient(_connectionSettings);
		}

        public static string ContactIndexName { get; set; } = "contact-20-05-2018-03-05-19";


    }
}
