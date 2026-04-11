
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Motivasyons.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.Project.MotivasyonDtos;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Motivasyons.Commands
{
    public class UpdateMotivasyonCommand : IRequest<IDataResult<UpdateMotivasyonDto>>
    {
        public int Id { get; set; }
        public string Kelime { get; set; }

        public class UpdateMotivasyonCommandHandler : IRequestHandler<UpdateMotivasyonCommand, IDataResult<UpdateMotivasyonDto>>
        {
            private readonly IMotivasyonRepository _motivasyonRepository;
            private readonly IMediator _mediator;

            public UpdateMotivasyonCommandHandler(IMotivasyonRepository motivasyonRepository, IMediator mediator)
            {
                _motivasyonRepository = motivasyonRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateMotivasyonValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UpdateMotivasyonDto>> Handle(
                UpdateMotivasyonCommand request,
                CancellationToken cancellationToken)
            {
                var isThereMotivasyonRecord = await _motivasyonRepository.GetAsync(u => u.Id == request.Id);

                if (isThereMotivasyonRecord == null)
                {
                    return new ErrorDataResult<UpdateMotivasyonDto>("Kayıt bulunamadı");
                }

                isThereMotivasyonRecord.Kelime = request.Kelime;
                isThereMotivasyonRecord.UpdatedBy = UserInfoExtensions.GetUserId();
                isThereMotivasyonRecord.UpdatedDate = DateTime.Now;

                _motivasyonRepository.Update(isThereMotivasyonRecord);
                await _motivasyonRepository.SaveChangesAsync();

                var dto = new UpdateMotivasyonDto
                {
                    Id = isThereMotivasyonRecord.Id,
                    Kelime = isThereMotivasyonRecord.Kelime,
                };

                return new SuccessDataResult<UpdateMotivasyonDto>(dto, Messages.Updated);
            }
        }
    }
}
