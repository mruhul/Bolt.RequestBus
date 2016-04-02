using System.Collections.Generic;
using Bolt.RequestBus;
using Bolt.RequestBus.Validators;
using Bolt.Validation;
using Bolt.Validation.Extensions;
using Sample.Api.Infrastructure.Extensions;

namespace Sample.Api.Features.CreateBook
{
    public class UniqueBookValidator : ValidatorBase<CreateBookRequest>
    {
        private readonly IBookRepository repository;

        public UniqueBookValidator(IBookRepository repository)
        {
            this.repository = repository;
        }

        public override IEnumerable<IError> Validate(CreateBookRequest request)
        {
            return RuleChecker<CreateBookRequest>.For(request)
                .Check(p => p.Title)
                    .ErrorMessage("Book with same title already exists")
                    .That(IsTitleUniqueInStore)
                .ToRequestValidationErrors();
        }

        private bool IsTitleUniqueInStore(string title)
        {
            return repository.GetByTitle(title) == null;
        }

        public override int Order => 1;
    }
}