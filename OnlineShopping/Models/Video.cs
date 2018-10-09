using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopping.Models
{
    public class Video
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public virtual List<User> Users { get; set; }
    }

    public static class VideoList
    {
        public static List<Video> Videos()
        {
            var videoList = new List<Video> {
                new Video { Id = 1, Title = "How To Become A Millionaire, Step-By-Step", Url = "jfV2CnNQJCg",Image="",Price=3},
                new Video { Id = 2, Title = "Jack Ma - How To Become A Billionaire (MUST WATCH!)", Url = "-JM_bA3EoMo",Image="",Price=2},
                new Video { Id = 3, Title = "TOP SECRET Abandoned Projects You're Not Supposed To Know About!", Url = "HO7mE8Ovju0",Image="",Price=5},
                new Video { Id = 4, Title = "5 Most Top Secret Military Locations", Url = "RqQ0zqyp6_g",Image="",Price=1},
                new Video { Id = 5, Title = "15 Insane Guinness World Records of ALL TIME", Url = "iHSKFgPYySM",Image="",Price=6},
                new Video { Id = 6, Title = "Top 10 Most Dangerous World Records", Url = "QSEwDuKW0Qo",Image="",Price=6}
            };

            return videoList;
        }
    }
}
