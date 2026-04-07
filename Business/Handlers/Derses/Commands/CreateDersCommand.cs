
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.Derses.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.DersDtos;
using Core.Extensions;

namespace Business.Handlers.Derses.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateDersCommand : IRequest<IDataResult<CreateDersDto>>
    {

        public string Ad { get; set; }
        public int SinavId { get; set; }


        public class CreateDersCommandHandler : IRequestHandler<CreateDersCommand, IDataResult<CreateDersDto>>
        {
            private readonly IDersRepository _dersRepository;
            private readonly IMediator _mediator;
            public CreateDersCommandHandler(IDersRepository dersRepository, IMediator mediator)
            {
                _dersRepository = dersRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateDersValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CreateDersDto>> Handle(CreateDersCommand request, CancellationToken cancellationToken)
            {
                var isThereDersRecord = _dersRepository.Query().Any(u => u.Ad == request.Ad);

                if (isThereDersRecord == true)
                    return new ErrorDataResult<CreateDersDto>(Messages.NameAlreadyExist);

                var addedDers = new Ders
                {
                    Ad = request.Ad,
                    SinavId = request.SinavId,
                    CreatedBy = UserInfoExtensions.GetUserId(),
                    CreatedDate = System.DateTime.Now,
                    IsActive = true
                };

                _dersRepository.Add(addedDers);
                await _dersRepository.SaveChangesAsync();

                var dto = new CreateDersDto
                {
                    Id = addedDers.Id,
                    Ad = addedDers.Ad,
                    SinavId = addedDers.SinavId
                };

                return new SuccessDataResult<CreateDersDto>(dto, Messages.Added);
            }
        }
    }
}