using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Msoop.Data;
using Msoop.ViewModels;

namespace Msoop.Features.Sheets
{
    public class GetSheet
    {
        public class Response
        {
            public EditSheetViewModel Sheet { get; }
            public IList<SubredditSummaryViewModel> OwnedSubreddits { get; }

            public Response(EditSheetViewModel sheet, IList<SubredditSummaryViewModel> ownedSubreddits)
            {
                Sheet = sheet;
                OwnedSubreddits = ownedSubreddits;
            }
        }

        public class Query : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly MsoopContext _db;

            public Handler(MsoopContext db)
            {
                _db = db;
            }

            public async Task<Response> Handle(Query cmd, CancellationToken cancellationToken)
            {
                var sheet = await _db.Sheets.Include(s => s.Subreddits)
                    .FirstOrDefaultAsync(s => s.Id == cmd.Id, cancellationToken);

                if (sheet is null) return null;

                var editSheet = new EditSheetViewModel()
                {
                    PostAgeLimit = sheet.PostAgeLimitInDays switch
                    {
                        1 => PostAgeLimit.LastDay,
                        7 => PostAgeLimit.LastWeek,
                        31 => PostAgeLimit.LastMonth,
                        365 => PostAgeLimit.LastYear,
                        _ => PostAgeLimit.Custom
                    },
                    AllowOver18 = sheet.AllowOver18,
                    AllowSpoilers = sheet.AllowSpoilers,
                    AllowStickied = sheet.AllowStickied,
                    CustomAgeLimit = sheet.PostAgeLimitInDays,
                };
                var subreddits = sheet.Subreddits.Select(sub => new SubredditSummaryViewModel()
                    {
                        Name = sub.Name,
                        MaxPostCount = sub.MaxPostCount,
                        PostOrdering = sub.PostOrdering,
                    })
                    .ToList();

                return new(editSheet, subreddits);
            }
        }
    }
}
