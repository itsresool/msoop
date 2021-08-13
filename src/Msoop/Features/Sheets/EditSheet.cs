using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Msoop.Data;
using Msoop.ViewModels;

namespace Msoop.Features.Sheets
{
    public class EditSheet
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

        public class QueryHandler : IRequestHandler<Query, Response>
        {
            private readonly MsoopContext _db;
            private readonly IMapper _mapper;

            public QueryHandler(MsoopContext db, IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Response> Handle(Query msg, CancellationToken cancellationToken)
            {
                var sheet = await _db.Sheets.Include(s => s.Subreddits)
                    .FirstOrDefaultAsync(s => s.Id == msg.Id, cancellationToken);

                if (sheet is null) return null;

                var editSheet = _mapper.Map<EditSheetViewModel>(sheet);
                var subreddits = _mapper.Map<IList<SubredditSummaryViewModel>>(sheet.Subreddits);

                return new(editSheet, subreddits);
            }
        }

        public class Command : IRequest
        {
            public Guid Id { get; }
            public EditSheetViewModel Form { get; }

            public Command(Guid id, EditSheetViewModel form)
            {
                Id = id;
                Form = form;
            }
        }

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly MsoopContext _db;

            public CommandHandler(MsoopContext db)
            {
                _db = db;
            }

            public async Task<Unit> Handle(Command cmd, CancellationToken cancellationToken)
            {
                var sheet = await _db.Sheets.FindAsync(cmd.Id);
                if (sheet is null)
                {
                    throw new InvalidOperationException("There is no sheet to configure.");
                }

                sheet.PostAgeLimitInDays = cmd.Form.PostAgeLimit switch
                {
                    PostAgeLimit.LastDay => 1,
                    PostAgeLimit.LastWeek => 7,
                    PostAgeLimit.LastMonth => 31,
                    PostAgeLimit.LastYear => 365,
                    PostAgeLimit.Custom => cmd.Form.CustomAgeLimit,
                    _ => throw new ArgumentOutOfRangeException(nameof(cmd.Form.PostAgeLimit))
                };
                sheet.AllowOver18 = cmd.Form.AllowOver18;
                sheet.AllowSpoilers = cmd.Form.AllowSpoilers;
                sheet.AllowStickied = cmd.Form.AllowStickied;

                await _db.SaveChangesAsync(cancellationToken);

                return default;
            }
        }
    }
}
