using System.Collections.Generic;
using System.Linq;
using Bolt.Common.Extensions;
using Bolt.RequestBus;
using Bolt.RequestBus.Validators;
using Bolt.Validation.Extensions;

namespace Sample.CreateBook
{
    public class CreateBookValidator : ValidatorBase<CreateBookRequest>
    {
        public override IEnumerable<IError> Validate(CreateBookRequest request)
        {
            return Bolt.Validation.RuleChecker<CreateBookRequest>
                .For(request)
                .Check(p => p.Title).NotEmpty()
                .Check(p => p.Author).NotEmpty()
                .Check(p => p.Price).GreaterThan(0)
                .Result()
                .Errors.NullSafe().Select(x => new Error(x.ErrorCode, x.PropertyName, x.ErrorMessage));
        }
    }
}
