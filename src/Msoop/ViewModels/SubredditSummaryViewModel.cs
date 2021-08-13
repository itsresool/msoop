using AutoMapper;
using Msoop.Models;

namespace Msoop.ViewModels
{
    public class SubredditSummaryViewModel
    {
        public string Name { get; set; }
        public int MaxPostCount { get; set; }
        public PostOrdering PostOrdering { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Subreddit, SubredditSummaryViewModel>();
            }
        }
    }
}
