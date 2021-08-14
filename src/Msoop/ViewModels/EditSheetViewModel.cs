using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Msoop.Models;

namespace Msoop.ViewModels
{
    public class EditSheetViewModel
    {
        public Guid Id { get; set; }
        public PostAgeLimit PostAgeLimit { get; set; }

        [Range(minimum: 1, maximum: 9999, ErrorMessage = "Value must be between {1} and {2}")]
        public int CustomAgeLimit { get; set; }

        [Display(Name = "Allow posts that contain adult content")]
        public bool AllowOver18 { get; set; }

        [Display(Name = "Allow posts that contain spoilers")]
        public bool AllowSpoilers { get; set; }

        [Display(Name = "Allow stickied posts")]
        public bool AllowStickied { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Sheet, EditSheetViewModel>()
                    .ForMember(dest => dest.PostAgeLimit,
                        opt => opt.MapFrom(src => MapToPostAgeLimit(src.PostAgeLimitInDays)))
                    .ForMember(dest => dest.CustomAgeLimit, opt => opt.MapFrom(src => src.PostAgeLimitInDays));
            }

            private static PostAgeLimit MapToPostAgeLimit(int postAgeInDays)
            {
                return postAgeInDays switch
                {
                    1 => PostAgeLimit.LastDay,
                    7 => PostAgeLimit.LastWeek,
                    31 => PostAgeLimit.LastMonth,
                    365 => PostAgeLimit.LastYear,
                    _ => PostAgeLimit.Custom,
                };
            }
        }
    }

    public enum PostAgeLimit
    {
        [Display(Name = "Last Day")]
        LastDay,

        [Display(Name = "Last Week")]
        LastWeek,

        [Display(Name = "Last Month")]
        LastMonth,

        [Display(Name = "Last Year")]
        LastYear,

        [Display(Name = "Custom Period")]
        Custom,
    }
}
