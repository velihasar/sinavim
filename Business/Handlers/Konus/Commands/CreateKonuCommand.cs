
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
using Business.Handlers.Konus.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.Project.KonuDtos;
using Core.Extensions;

namespace Business.Handlers.Konus.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateKonuCommand : IRequest<IDataResult<CreateKonuDto>>
    {

        public string Ad { get; set; }
        public int SiraNo { get; set; }
        public int DersId { get; set; }


        public class CreateKonuCommandHandler : IRequestHandler<CreateKonuCommand, IDataResult<CreateKonuDto>>
        {
            private readonly IKonuRepository _konuRepository;
            private readonly IMediator _mediator;
            public CreateKonuCommandHandler(IKonuRepository konuRepository, IMediator mediator)
            {
                _konuRepository = konuRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateKonuValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<CreateKonuDto>> Handle(CreateKonuCommand request, CancellationToken cancellationToken)
            {
                var isThereKonuRecord = _konuRepository.Query().Any(u => u.Ad == request.Ad);

                if (isThereKonuRecord == true)
                    return new ErrorDataResult<CreateKonuDto>(Messages.NameAlreadyExist);

                var addedKonu = new Konu
                {
                    Ad = request.Ad,
                    SiraNo = request.SiraNo,
                    DersId = request.DersId,
                    CreatedBy = UserInfoExtensions.GetUserId(),
                    CreatedDate = DateTimeExtensions.NowForNpgsqlTimestamp(),
                    IsActive = true
                };

                _konuRepository.Add(addedKonu);
                await _konuRepository.SaveChangesAsync();
                
                var dto = new CreateKonuDto
                {
                    Id = addedKonu.Id,
                    Ad = addedKonu.Ad,
                    SiraNo = addedKonu.SiraNo,
                    DersId = addedKonu.DersId
                };

                return new SuccessDataResult<CreateKonuDto>(dto, Messages.Added);
            }
        }
    }
}