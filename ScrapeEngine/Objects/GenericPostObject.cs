using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapeEngine.Objects
{
    public class GenericPostObject
    {
        public static string BaseUrl { get; set; }
        public List<Post> Posts { get; set; }

        public GenericPostObject()
        {
            Posts = new List<Post>();
        }

        public class Post
        {
            public int Id { get; set; }
            public int Score { get; set; }

            public string Title { get; set; }
            public string SubTitle { get; set; }
            public string Content { get; set; }
            public string Author { get; set; }
            public DateTime Posted { get; set; }

            public string Url { get; set; }
            public List<Reply> Replies { get; set; }

            public Post()
            {
                Replies = new List<Reply>();
            }
        }

        public class Reply
        {
            public string Author { get; set; }
            public string Content { get; set; }
            public int Id { get; set; }
            public List<Reply> Replies { get; set; }
            public int Score { get; set; }
            public string TimePostedAgo { get; set; }

            private string url;
            public string Url
            { get { return url; } set { url = BaseUrl + value; } }

            public Reply()
            {
                Replies = new List<Reply>();
            }
        }
    }
}
