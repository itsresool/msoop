using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Msoop.Data;
using Msoop.ViewModels;

namespace Msoop.Features.Sheets
{
    public class EditSheet
    {
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

        public class Handler : IRequestHandler<Command>
        {
            private readonly MsoopContext _db;

            public Handler(MsoopContext db)
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
