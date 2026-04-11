
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Motivasyons.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.MotivasyonDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Motivasyons.Commands
{
    public class CreateMotivasyonCommand : IRequest<IDataResult<CreateMotivasyonDto>>
    {
        public string Kelime { get; set; }

        public class CreateMotivasyonCommandHandler : IRequestHandler<CreateMotivasyonCommand, IDataResult<CreateMotivasyonDto>>
        {
            private readonly IMotivasyonRepository _motivasyonRepository;
            private readonly IMediator _mediator;

            public CreateMotivasyonCommandHandler(IMotivasyonRepository motivasyonRepository, IMediator mediator)
            {
                _motivasyonRepository = motivasyonRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateMotivasyonValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CreateMotivasyonDto>> Handle(
                CreateMotivasyonCommand request,
                CancellationToken cancellationToken)
            {
                var isThereMotivasyonRecord = _motivasyonRepository.Query().Any(u => u.Kelime == request.Kelime);

                if (isThereMotivasyonRecord)
                {
                    return new ErrorDataResult<CreateMotivasyonDto>(Messages.NameAlreadyExist);
                }

                var addedMotivasyon = new Motivasyon
                {
                    Kelime = request.Kelime,
                    CreatedBy = UserInfoExtensions.GetUserId(),
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                };

                _motivasyonRepository.Add(addedMotivasyon);
                await _motivasyonRepository.SaveChangesAsync();

                var dto = new CreateMotivasyonDto
                {
                    Id = addedMotivasyon.Id,
                    Kelime = addedMotivasyon.Kelime,
                };

                return new SuccessDataResult<CreateMotivasyonDto>(dto, Messages.Added);
            }
        }
    }
}
