using System.Collections.Generic;
using Bolt.RequestBus;
using Bolt.RequestBus.Validators;
using Bolt.Validation.Extensions;
using Sample.Api.Infrastructure.Extensions;
using Sample.Api.Infrastructure.PersistentStores;

namespace Sample.Api.Features.CreateBook
{
    public class InputValidator : ValidatorBase<CreateBookRequest>
    {
        public override IEnumerable<IError> Validate(CreateBookRequest request)
        {
            return Bolt.Validation.RuleChecker<CreateBookRequest>
                .For(request)
                .Check(p => p.Title).NotEmpty()
                .Check(p => p.Author).NotEmpty()
                .Check(p => p.Price).GreaterThan(0)
                .ToRequestValidationErrors();
        }
    }
}
