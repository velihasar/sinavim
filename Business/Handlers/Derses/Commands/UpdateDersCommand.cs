
using Business.Constants;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.Derses.ValidationRules;
using Core.Entities.Dtos.Project.DersDtos;
using Core.Extensions;

namespace Business.Handlers.Derses.Commands
{


    public class UpdateDersCommand : IRequest<IDataResult<UpdateDersDto>>
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public int SinavId { get; set; }

        public class UpdateDersCommandHandler : IRequestHandler<UpdateDersCommand, IDataResult<UpdateDersDto>>
        {
            private readonly IDersRepository _dersRepository;
            private readonly IMediator _mediator;

            public UpdateDersCommandHandler(IDersRepository dersRepository, IMediator mediator)
            {
                _dersRepository = dersRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateDersValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UpdateDersDto>> Handle(UpdateDersCommand request, CancellationToken cancellationToken)
            {
                var isThereDersRecord = await _dersRepository.GetAsync(u => u.Id == request.Id);

                if(isThereDersRecord == null)
                {
                    return new ErrorDataResult<UpdateDersDto>("Kayıt bulunamadı");
                }

                isThereDersRecord.Ad = request.Ad;
                isThereDersRecord.SinavId = request.SinavId;
                isThereDersRecord.UpdatedBy = UserInfoExtensions.GetUserId();
                isThereDersRecord.UpdatedDate = System.DateTime.Now;


                _dersRepository.Update(isThereDersRecord);
                await _dersRepository.SaveChangesAsync();

                var dto = new UpdateDersDto
                {
                    Id = isThereDersRecord.Id,
                    Ad = isThereDersRecord.Ad,
                    SinavId = isThereDersRecord.SinavId
                };

                return new SuccessDataResult<UpdateDersDto>(dto, Messages.Updated);
            }
        }
    }
}

