
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
using Business.Handlers.Konus.ValidationRules;
using Core.Entities.Dtos.Project.KonuDtos;
using Core.Extensions;

namespace Business.Handlers.Konus.Commands
{


    public class UpdateKonuCommand : IRequest<IDataResult<UpdateKonuDto>>
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public int SiraNo { get; set; }
        public int DersId { get; set; }

        public class UpdateKonuCommandHandler : IRequestHandler<UpdateKonuCommand, IDataResult<UpdateKonuDto>>
        {
            private readonly IKonuRepository _konuRepository;
            private readonly IMediator _mediator;

            public UpdateKonuCommandHandler(IKonuRepository konuRepository, IMediator mediator)
            {
                _konuRepository = konuRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateKonuValidator), Priority = 1)]
            //[CacheRemoveAspect("Get")]
            //[LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<UpdateKonuDto>> Handle(UpdateKonuCommand request, CancellationToken cancellationToken)
            {
                var isThereKonuRecord = await _konuRepository.GetAsync(u => u.Id == request.Id);

                if(isThereKonuRecord == null)
                {
                    return new ErrorDataResult<UpdateKonuDto>("Kayıt bulunamadı");
                }

                isThereKonuRecord.Ad = request.Ad;
                isThereKonuRecord.SiraNo = request.SiraNo;
                isThereKonuRecord.DersId = request.DersId;
                isThereKonuRecord.UpdatedBy = UserInfoExtensions.GetUserId();
                isThereKonuRecord.UpdatedDate = DateTimeExtensions.NowForNpgsqlTimestamp();


                _konuRepository.Update(isThereKonuRecord);
                await _konuRepository.SaveChangesAsync();
                
                var dto = new UpdateKonuDto
                {
                    Id = isThereKonuRecord.Id,
                    Ad = isThereKonuRecord.Ad,
                    SiraNo = isThereKonuRecord.SiraNo,
                    DersId = isThereKonuRecord.DersId
                };

                return new SuccessDataResult<UpdateKonuDto>(dto, Messages.Updated);
            }
        }
    }
}

